using E_Commerce.Models;
using E_Commerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicarionDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly decimal shippingFee;
        public CartController(ApplicarionDbContext context , IConfiguration configuration,UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
            shippingFee = configuration.GetValue<decimal>("CartSetting:ShippingFee");

        }
        public IActionResult Index()
        {
            List<OrderItem> cartItems = CartHelper.GetCartItems(Request,Response, context);
            decimal subTotal = CartHelper.GetSubTotal(cartItems);
            ViewBag.CartItems = cartItems;
            ViewBag.ShippingFee = shippingFee;
            ViewBag.SubTotal = subTotal;
            ViewBag.Total = subTotal + shippingFee;
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Index(CheckOutDto model)
        {
            List<OrderItem> cartItems = CartHelper.GetCartItems(Request, Response, context);
            decimal subTotal = CartHelper.GetSubTotal(cartItems);
            ViewBag.CartItems = cartItems;
            ViewBag.ShippingFee = shippingFee;
            ViewBag.SubTotal = subTotal;
            ViewBag.Total = subTotal + shippingFee;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (cartItems.Count == 0)
            {
                ViewBag.ErrorMessage = "Your Cart is Empty";
                return View(model);
            }
            TempData["DeliveryAddress"] = model.DeliveryAddree;
            TempData["PaymentMethod"] = model.PaymentMethod;
            return RedirectToAction("Confirm");

        }
    }
}
