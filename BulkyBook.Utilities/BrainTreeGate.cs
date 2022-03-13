using Braintree;
using BulkyBook.Utilities.Options;
using Microsoft.Extensions.Options;

namespace BulkyBook.Utilities
{
    public class BrainTreeGate : IBrainTreeGate
    {
        private readonly BrainTreeOptions _options;
        private IBraintreeGateway BraintreeGateway { get; set; }

        public BrainTreeGate(IOptions<BrainTreeOptions> options)
        {
            _options = options.Value;
        }

        public IBraintreeGateway CreateGateway()
        {
            return new BraintreeGateway(
                _options.Environment,
                _options.MerchantId,
                _options.PublicKey,
                _options.PrivateKey);
        }

        public IBraintreeGateway GetGateway()
        {
            return BraintreeGateway ??= CreateGateway();
        }
    }
}