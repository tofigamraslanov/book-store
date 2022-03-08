using BulkyBook.DataAccess.Repositories.Abstract;
using BulkyBook.Entities;
using BulkyBook.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Stripe;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BulkyBookWeb.ViewModels;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;

        [BindProperty] 
        public ShoppingCartViewModel ShoppingCartViewModel { get; set; }

        public CartController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            var claim = GetClaim();

            ShoppingCartViewModel = new ShoppingCartViewModel()
            {
                OrderHeader = new OrderHeader
                {
                    OrderTotal = 0,
                    ApplicationUser =
                        _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value,
                            includeProperties: "Company")
                },
                ShoppingCarts = _unitOfWork.ShoppingCart.GetAll(s => s.ApplicationUserId == claim.Value,
                    includeProperties: "Product")
            };

            foreach (var shoppingCart in ShoppingCartViewModel.ShoppingCarts)
            {
                shoppingCart.Price = StaticDetails.GetPriceBasedOnQuantity(shoppingCart.Count,
                    shoppingCart.Product.Price, shoppingCart.Product.Price50, shoppingCart.Product.Price100);
                ShoppingCartViewModel.OrderHeader.OrderTotal += (shoppingCart.Price * shoppingCart.Count);
                shoppingCart.Product.Description = StaticDetails.ConvertToRawHtml(shoppingCart.Product.Description);

                if (shoppingCart.Product.Description.Length > 100)
                    shoppingCart.Product.Description = shoppingCart.Product.Description.Substring(0, 99) + "...";
            }

            return View(ShoppingCartViewModel);
        }

        [HttpPost]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPost()
        {
            var claim = GetClaim();
            var user = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value);

            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Verification email is empty!");
            }

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = user?.Id, code },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(user?.Email, "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
            return RedirectToAction("Index");
        }

        public IActionResult Plus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault
                (c => c.Id == cartId, includeProperties: "Product");
            cart.Count += 1;

            cart.Price = StaticDetails.GetPriceBasedOnQuantity
                (cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault
                (c => c.Id == cartId, includeProperties: "Product");

            if (cart.Count == 1)
            {
                var count = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == cart.ApplicationUserId).ToList()
                    .Count;
                _unitOfWork.ShoppingCart.Remove(cart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(StaticDetails.SessionShoppingCart, count - 1);
            }
            else
            {
                cart.Count -= 1;

                cart.Price = StaticDetails.GetPriceBasedOnQuantity
                    (cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
                _unitOfWork.Save();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault
                (c => c.Id == cartId, includeProperties: "Product");

            var count = _unitOfWork.ShoppingCart
                .GetAll(c => c.ApplicationUserId == cart.ApplicationUserId)
                .ToList().Count;

            _unitOfWork.ShoppingCart.Remove(cart);
            _unitOfWork.Save();

            HttpContext.Session.SetInt32(StaticDetails.SessionShoppingCart, count - 1);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Summary()
        {
            var claim = GetClaim();

            ShoppingCartViewModel = new ShoppingCartViewModel()
            {
                OrderHeader = new OrderHeader()
                {
                    ApplicationUser = _unitOfWork.ApplicationUser
                        .GetFirstOrDefault(u => u.Id == claim.Value, includeProperties: "Company"),
                },
                ShoppingCarts = _unitOfWork.ShoppingCart
                    .GetAll(s => s.ApplicationUserId == claim.Value, includeProperties: "Product")
            };

            foreach (var shoppingCart in ShoppingCartViewModel.ShoppingCarts)
            {
                shoppingCart.Price = StaticDetails.GetPriceBasedOnQuantity(shoppingCart.Count,
                    shoppingCart.Product.Price, shoppingCart.Product.Price50, shoppingCart.Product.Price100);
                ShoppingCartViewModel.OrderHeader.OrderTotal += (shoppingCart.Price * shoppingCart.Count);
            }

            ShoppingCartViewModel.OrderHeader.Name = ShoppingCartViewModel.OrderHeader.ApplicationUser.Name;
            ShoppingCartViewModel.OrderHeader.PhoneNumber =
                ShoppingCartViewModel.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartViewModel.OrderHeader.StreetAddress =
                ShoppingCartViewModel.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartViewModel.OrderHeader.City = ShoppingCartViewModel.OrderHeader.ApplicationUser.City;
            ShoppingCartViewModel.OrderHeader.State = ShoppingCartViewModel.OrderHeader.ApplicationUser.State;
            ShoppingCartViewModel.OrderHeader.PostalCode = ShoppingCartViewModel.OrderHeader.ApplicationUser.PostalCode;

            return View(ShoppingCartViewModel);
        }

        [HttpPost]
        [ActionName("Summary")]
        [ValidateAntiForgeryToken]
        public IActionResult SummaryPost(string stripeToken)
        {
            var claim = GetClaim();

            ShoppingCartViewModel.OrderHeader.ApplicationUser =
                _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value, includeProperties: "Company");

            ShoppingCartViewModel.ShoppingCarts =
                _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == claim.Value, includeProperties: "Product");

            ShoppingCartViewModel.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusPending;
            ShoppingCartViewModel.OrderHeader.OrderStatus = StaticDetails.StatusPending;
            ShoppingCartViewModel.OrderHeader.ApplicationUserId = claim?.Value;
            ShoppingCartViewModel.OrderHeader.OrderDate = DateTime.Now;

            _unitOfWork.OrderHeader.Add(ShoppingCartViewModel.OrderHeader);
            _unitOfWork.Save();

            var shoppingCarts = ShoppingCartViewModel.ShoppingCarts.ToList();

            foreach (var shoppingCart in shoppingCarts)
            {
                shoppingCart.Price = StaticDetails.GetPriceBasedOnQuantity(shoppingCart.Count,
                    shoppingCart.Product.Price, shoppingCart.Product.Price50, shoppingCart.Product.Price100);
                OrderDetails orderDetails = new()
                {
                    ProductId = shoppingCart.ProductId,
                    OrderId = ShoppingCartViewModel.OrderHeader.Id,
                    Price = shoppingCart.Price,
                    Count = shoppingCart.Count
                };
                ShoppingCartViewModel.OrderHeader.OrderTotal += orderDetails.Count * orderDetails.Price;
                _unitOfWork.OrderDetails.Add(orderDetails);
            }

            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.Save();
            HttpContext.Session.SetInt32(StaticDetails.SessionShoppingCart, 0);

            if (stripeToken == null)
            {
                // Order will be created for delayed payment for authorized company
                ShoppingCartViewModel.OrderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
                ShoppingCartViewModel.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusDelayedPayment;
                ShoppingCartViewModel.OrderHeader.OrderStatus = StaticDetails.StatusApproved;
            }   
            else
            {
                // Process the payment
                var options = new ChargeCreateOptions()
                {
                    Amount = Convert.ToInt32(ShoppingCartViewModel.OrderHeader.OrderTotal * 100),
                    Currency = "usd",
                    Description = $"Order ID: {ShoppingCartViewModel.OrderHeader.Id}",
                    Source = stripeToken
                };

                var service = new ChargeService();
                var charge = service.Create(options);

                if (charge.Id == null)
                    ShoppingCartViewModel.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusRejected;
                else
                    ShoppingCartViewModel.OrderHeader.TransactionId = charge.Id;

                if (charge.Status.ToLower() == "succeeded")
                {
                    ShoppingCartViewModel.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusApproved;
                    ShoppingCartViewModel.OrderHeader.OrderStatus = StaticDetails.StatusApproved;
                    ShoppingCartViewModel.OrderHeader.PaymentDate = DateTime.Now;
                }
            }

            _unitOfWork.Save();

            return RedirectToAction("OrderConfirmation", "Cart", new { id = ShoppingCartViewModel.OrderHeader.Id });
        }

        public IActionResult OrderConfirmation(int id)
        {
            return View(id);
        }

        private Claim GetClaim()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            return claim;
        }
    }
}