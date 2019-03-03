using Checkout.Core.Contracts.Constants;

namespace Checkout.Core.Contracts
{
    public class Result
    {
        public ResultCode Code { get; set; }
        public string Description { get; set; }
    }
}
