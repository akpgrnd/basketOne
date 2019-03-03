using Checkout.Core.Contracts;
using System;
using System.Threading.Tasks;

namespace Checkout.Basket.Ringfence.Contracts
{
    public interface IRingfenceService
    {
        Task<Result> TryReserveAsync(Guid basketId, Guid itemId, int quantity);
        Task<Result> ReleaseAsync(Guid basketId, Guid itemId, int quantity);
    }
}
