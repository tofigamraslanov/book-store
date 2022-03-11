using BulkyBook.DataAccess.Repositories.Abstract;
using BulkyBook.Entities;
using BulkyBook.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using BulkyBookWeb.ViewModels;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var products = _unitOfWork.ProductRepository.GetAll(includeProperties: "Category,CoverType");

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                var count = _unitOfWork.ShoppingCartRepository
                    .GetAll(c => c.ApplicationUserId == claim.Value)
                    .ToList().Count;

                HttpContext.Session.SetInt32(StaticDetails.SessionShoppingCart, count);
            }

            return View(products);
        }

        public IActionResult Details(int id)
        {
            var product =
                _unitOfWork.ProductRepository.GetFirstOrDefault(p => p.Id == id, includeProperties: "Category,CoverType");
            var shoppingCart = new ShoppingCart()
            {
                Product = product,
                ProductId = product.Id
            };

            return View(shoppingCart);
        }


        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Details(ShoppingCart cart)
        {
            cart.Id = 0;
            if (ModelState.IsValid)
            {
                //then we will add to cart
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
                cart.ApplicationUserId = claim?.Value;

                var cartFromDb =
                    _unitOfWork.ShoppingCartRepository.GetFirstOrDefault(
                        c => c.ApplicationUserId == cart.ApplicationUserId &&
                             c.ProductId == cart.ProductId, includeProperties: "Product");

                if (cartFromDb == null)
                    _unitOfWork.ShoppingCartRepository.Add(cart);
                else
                {
                    cartFromDb.Count += cart.Count;
                    _unitOfWork.ShoppingCartRepository.Update(cartFromDb);
                }

                _unitOfWork.Save();

                var count = _unitOfWork.ShoppingCartRepository
                    .GetAll(c => c.ApplicationUserId == cart.ApplicationUserId)
                    .ToList().Count;

                //HttpContext.Session.SetObject(StaticDetails.SessionShoppingCart, cart);
                HttpContext.Session.SetInt32(StaticDetails.SessionShoppingCart, count);

                //var obj = HttpContext.Session.GetObject<ShoppingCartRepository>(StaticDetails.SessionShoppingCart);

                return RedirectToAction("Index");
            }

            var product = _unitOfWork.ProductRepository.GetFirstOrDefault(p => p.Id == cart.ProductId,
                includeProperties: "Category,CoverTypeRepository");

            var shoppingCart = new ShoppingCart()
            {
                Product = product,
                ProductId = product.Id
            };

            return View(shoppingCart);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}