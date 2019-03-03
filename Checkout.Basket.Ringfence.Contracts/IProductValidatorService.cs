using Checkout.Core.Contracts;
using System;

using System.Threading.Tasks;

namespace Checkout.Basket.Ringfence.Contracts
{
    public interface IProductValidatorService
    {
        Task<Result> IsProductAvailable(Guid productId, int quantity);
        Task<Result> IsProductValid(Guid productId);
    }
}
