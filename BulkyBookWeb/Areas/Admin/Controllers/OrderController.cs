using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using BulkyBook.DataAccess.Repositories.Abstract;
using BulkyBook.Entities;
using BulkyBook.Utilities;
using BulkyBookWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty] 
        public OrderDetailsViewModel OrderDetailsViewModel { get; set; }

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            OrderDetailsViewModel = new OrderDetailsViewModel()
            {
                OrderHeader =
                    _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(h => h.Id == id, includeProperties: "ApplicationUser"),
                OrderDetails = _unitOfWork.OrderDetailsRepository.GetAll(d => d.OrderId == id, includeProperties: "Product")
            };

            return View(OrderDetailsViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Details")]
        public IActionResult Details(string stripeToken)
        {
            var orderHeader =
                _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(o => o.Id == OrderDetailsViewModel.OrderHeader.Id,
                    includeProperties: "ApplicationUser");

            if (stripeToken != null)
            {
                // Process the payment
                var options = new ChargeCreateOptions()
                {
                    Amount = Convert.ToInt32(orderHeader.OrderTotal * 100),
                    Currency = "usd",
                    Description = $"Order ID: {orderHeader.Id}",
                    Source = stripeToken
                };

                var service = new ChargeService();
                var charge = service.Create(options);

                if (charge.Id == null)
                    orderHeader.PaymentStatus = StaticDetails.PaymentStatusRejected;
                else
                    orderHeader.TransactionId = charge.Id;

                if (charge.Status.ToLower() == "succeeded")
                {
                    orderHeader.PaymentStatus = StaticDetails.PaymentStatusApproved;
                    orderHeader.PaymentDate = DateTime.Now;
                }

                _unitOfWork.Save();
            }

            return RedirectToAction("Details","Order",new {id=orderHeader.Id});
        }

        [Authorize(Roles = StaticDetails.RoleAdmin + "," + StaticDetails.RoleEmployee)]
        public IActionResult StartProcessing(int id)
        {
            var orderHeader = _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(o => o.Id == id);
            orderHeader.OrderStatus = StaticDetails.StatusInProcess;
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = StaticDetails.RoleAdmin + "," + StaticDetails.RoleEmployee)]
        public IActionResult ShipOrder()
        {
            var orderHeader =
                _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(o => o.Id == OrderDetailsViewModel.OrderHeader.Id);

            orderHeader.TrackingNumber = OrderDetailsViewModel.OrderHeader.TrackingNumber;
            orderHeader.Carrier = OrderDetailsViewModel.OrderHeader.Carrier;
            orderHeader.OrderStatus = StaticDetails.StatusShipped;
            orderHeader.OrderDate = DateTime.Now;

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = StaticDetails.RoleAdmin + "," + StaticDetails.RoleEmployee)]
        public IActionResult CancelOrder(int id)
        {
            var orderHeader = _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(o => o.Id == id);

            if (orderHeader.PaymentStatus == StaticDetails.StatusApproved)
            {
                var options = new RefundCreateOptions()
                {
                    Amount = Convert.ToInt32(orderHeader.OrderTotal * 100),
                    Reason = RefundReasons.RequestedByCustomer,
                    Charge = orderHeader.TransactionId
                };
                var service = new RefundService();
                var refund = service.Create(options);

                orderHeader.OrderStatus = StaticDetails.StatusRefunded;
                orderHeader.PaymentStatus = StaticDetails.StatusRefunded;
            }
            else
            {
                orderHeader.OrderStatus = StaticDetails.StatusCancelled;
                orderHeader.PaymentStatus = StaticDetails.StatusCancelled;
            }

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetOrders(string status)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            IEnumerable<OrderHeader> orderHeaders;

            if (User.IsInRole(StaticDetails.RoleAdmin) || User.IsInRole(StaticDetails.RoleEmployee))
                orderHeaders = _unitOfWork.OrderHeaderRepository.GetAll(includeProperties: "ApplicationUser");
            else
                orderHeaders = _unitOfWork.OrderHeaderRepository.GetAll(
                    o => o.ApplicationUserId == claim.Value,
                    includeProperties: "ApplicationUser");

            orderHeaders = status switch
            {
                "pending" => orderHeaders.Where(o => o.PaymentStatus == StaticDetails.PaymentStatusDelayedPayment),
                "inProcess" => orderHeaders.Where(o =>
                    o.OrderStatus is StaticDetails.StatusApproved or StaticDetails.StatusInProcess
                        or StaticDetails.StatusPending),
                "completed" => orderHeaders.Where(o => o.OrderStatus == StaticDetails.StatusShipped),
                "rejected" => orderHeaders.Where(o =>
                    o.OrderStatus is StaticDetails.StatusCancelled or StaticDetails.StatusRefunded
                        or StaticDetails.PaymentStatusRejected),
                _ => orderHeaders
            };

            return Json(new { data = orderHeaders });
        }

        #endregion
    }
}