namespace BulkyBook.Utilities
{
    public class TwilioOptions
    {
        public const string Twilio = "Twilio";

        public string PhoneNumber { get; set; }
        public string AuthToken { get; set; }
        public string AccountSid { get; set; }
    }
}