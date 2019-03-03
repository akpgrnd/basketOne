using Checkout.Basket.Ringfence.Contracts;
using System;
using Checkout.Core.Contracts;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Checkout.Data.Contracts;
using Checkout.Core.Contracts.Constants;
using Checkout.Data.Model;

namespace Checkout.Basket.RingfenceService
{
    public class ProductValidatorService : IProductValidatorService
    {
        readonly IProductReader productReader;
        readonly IRingfenceReader ringfenceReader;
        readonly ILogger<ProductValidatorService> logger;

        public ProductValidatorService(IProductReader productReader, IRingfenceReader ringfenceReader, ILogger<ProductValidatorService> logger)
        {
            this.productReader = productReader ?? throw new ArgumentNullException(nameof(productReader));
            this.ringfenceReader = ringfenceReader ?? throw new ArgumentNullException(nameof(ringfenceReader));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result> IsProductAvailable(Guid productId, int quantity)
        {
            if (productId == Guid.Empty) throw new ArgumentException(nameof(productId));
            if (quantity < 0) throw new ArgumentException(nameof(quantity));
            if (quantity == 0) return new Result { Code = ResultCode.Ok };

            Product product = await productReader.GetProduct(productId);
            if (product == null)
            {
                logger.LogInformation($"Invalid product requested {productId}");
                return new Result { Code = ResultCode.InvalidProduct };
            }

            int availableItems = product.Quantity - await ringfenceReader.CountRingfencedProduct(productId);
            if (availableItems < 0 || checked(availableItems - quantity) < 0)
            {
                logger.LogInformation($"Insufficient inventory for product {productId}");
                return new Result { Code = ResultCode.InsufficientInventory };
            }

            return new Result { Code = ResultCode.Ok };
        }

        public async Task<Result> IsProductValid(Guid productId)
        {
            if (productId == Guid.Empty) throw new ArgumentException(nameof(productId));

            Product product = await productReader.GetProduct(productId);
            if (product == null)
            {
                logger.LogInformation($"Invalid product requested {productId}");
                return new Result { Code = ResultCode.InvalidProduct };
            }
            
            return new Result { Code = ResultCode.Ok };
        }
    }
}
