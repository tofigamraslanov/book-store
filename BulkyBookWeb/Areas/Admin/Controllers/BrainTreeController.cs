using System;
using Braintree;
using BulkyBook.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrainTreeController : Controller
    {
        private readonly IBrainTreeGate _gate;

        public BrainTreeController(IBrainTreeGate gate)
        {
            _gate = gate;
        }

        public IActionResult Index()
        {
            var gateWay = _gate.GetGateway();
            var clientToken = gateWay.ClientToken.Generate();
            ViewBag.ClientToken = clientToken;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(IFormCollection collection)
        {
            var random = new Random();
            var nonce = collection["payment_method_nonce"];
            var request = new TransactionRequest
            {
                Amount = random.Next(1, 100),
                PaymentMethodNonce = nonce,
                OrderId = "55501",
                Options = new TransactionOptionsRequest()
                {
                    SubmitForSettlement = true,
                }
            };

            var gateway = _gate.GetGateway();

            var result = gateway.Transaction.Sale(request);

            if (result.Target.ProcessorResponseText == "Approved")
            {
                TempData["Success"] =
                    $"Transaction was successful. Transaction Id: {result.Target.Id}, Amount charged: ${result.Target.Amount}";
            }

            return RedirectToAction("Index");
        }
    }
}