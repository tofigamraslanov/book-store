namespace BulkyBook.Utilities
{
    public class StripeOptions
    {
        public const string Stripe = "Stripe";

        public string SecretKey { get; set; }
        public string PublishableKey { get; set; }
    }
}