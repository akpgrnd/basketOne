using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Checkout.Data.Contracts
{
    public interface IRingfenceReader
    {
        Task<int> CountRingfencedProduct(Guid productId);
        Task<int> CountRingfencedProduct(Guid basketId, Guid productId);
    }
}
