using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace BulkyBook.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;

        public ShoppingCartViewModel ShoppingCartViewModel { get; set; }

        public CartController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

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
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
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
    }
}