using Checkout.Basket.Ringfence.Contracts;
using Checkout.Core.Contracts;
using Checkout.Core.Contracts.Constants;
using Checkout.Data.Contracts;
using Checkout.Data.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Checkout.Basket.RingfenceService
{
    public class RingfenceService : IRingfenceService
    {
        readonly IRingfenceReader ringfenceReader;
        readonly IRingfenceWriter ringfenceWriter;
        readonly IProductReader productReader;

        public RingfenceService(IRingfenceReader ringfenceReader, IRingfenceWriter ringfenceWriter, IProductReader productReader)
        {
            this.ringfenceReader = ringfenceReader ?? throw new ArgumentNullException(nameof(ringfenceReader));
            this.ringfenceWriter = ringfenceWriter ?? throw new ArgumentNullException(nameof(ringfenceWriter));
            this.productReader = productReader ?? throw new ArgumentNullException(nameof(productReader));
        }

        public async Task<Result> ReleaseAsync(Guid basketId, Guid itemId, int quantity)
        {
            if (quantity < 0) throw new ArgumentException(nameof(quantity));
            int current = await ringfenceReader.CountRingfencedProduct(basketId, itemId);

            if (current < quantity) return new Result { Code = ResultCode.InvalidQuantity };

            return await ringfenceWriter.ReleaseItem(basketId, itemId, quantity)
                ? new Result { Code = ResultCode.Ok }
                : new Result { Code = ResultCode.RingfenceError };
        }

        public async Task<Result> TryReserveAsync(Guid basketId, Guid itemId, int quantity)
        {
            if (quantity < 0) throw new ArgumentException(nameof(quantity));
            int totalRingfenced = await ringfenceReader.CountRingfencedProduct(itemId);
            int current = await ringfenceReader.CountRingfencedProduct(basketId, itemId);

            if (current > quantity) return new Result { Code = ResultCode.InvalidQuantity};
            if (current == quantity) return new Result { Code = ResultCode.Ok };

            int toRingfence = quantity - current;
            
            Product product = await productReader.GetProduct(itemId);
            if (product == null) new Result { Code = ResultCode.InvalidProduct };
            if (product.LimitPerOrder > quantity) new Result { Code = ResultCode.ItemOrderLimitExceeded };
            if (product.Quantity < totalRingfenced + toRingfence) { new Result { Code = ResultCode.InsufficientInventory };
            }

            return await ringfenceWriter.ReserveItem(basketId, itemId, quantity)
                ? new Result { Code = ResultCode.Ok }
                : new Result { Code = ResultCode.RingfenceError };
        }
    }
}
