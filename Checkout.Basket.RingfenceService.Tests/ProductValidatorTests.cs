using Checkout.Core.Contracts;
using Checkout.Core.Contracts.Constants;
using Checkout.Data.Contracts;
using Checkout.Data.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace Checkout.Basket.RingfenceService.Tests
{
    [TestClass]
    public class ProductValidatorTests
    {
        ILogger<ProductValidatorService> logger;

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_Throws_Exception_If_ProductReader_Is_Null()
        {
            var rfReader = new Mock<IRingfenceReader>();
            new ProductValidatorService(null, rfReader.Object, logger);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_Throws_Exception_If_RingfenceReader_Is_Null()
        {
            var prodReader = new Mock<IProductReader>();
            new ProductValidatorService(prodReader.Object, null, logger);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_Throws_Exception_If_Logger_Is_Null()
        {
            var rfReader = new Mock<IRingfenceReader>();
            var prodReader = new Mock<IProductReader>();
            new ProductValidatorService(prodReader.Object, rfReader.Object, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task IsProductValid_Throws_Exception_If_ProductId_Is_Empty()
        {
            var rfReader = new Mock<IRingfenceReader>();
            var prodReader = new Mock<IProductReader>();
            var service = new ProductValidatorService(prodReader.Object, rfReader.Object, logger);

            await service.IsProductAvailable(Guid.Empty, 1);
        }

        [TestMethod]
        public async Task IsProductValid_Returns_InvalidProduct_Status_If_No_Product_Exists()
        {
            var rfReader = new Mock<IRingfenceReader>();
            var prodReader = new Mock<IProductReader>();
            prodReader.Setup(x => x.GetProduct(It.IsAny<Guid>()))
            .ReturnsAsync(default(Product));

            var service = new ProductValidatorService(prodReader.Object, rfReader.Object, logger);

            Result res = await service.IsProductValid(Guid.NewGuid());

            Assert.AreEqual(ResultCode.InvalidProduct, res.Code);
        }

        [TestMethod]
        public async Task IsProductValid_Returns_OK_Status_If_Product_Exists()
        {
            var rfReader = new Mock<IRingfenceReader>();
            var prodReader = new Mock<IProductReader>();
            prodReader.Setup(x => x.GetProduct(It.IsAny<Guid>()))
            .ReturnsAsync(new Product { Id = Guid.NewGuid() });

            var service = new ProductValidatorService(prodReader.Object, rfReader.Object, logger);

            Result res = await service.IsProductValid(Guid.NewGuid());

            Assert.AreEqual(ResultCode.Ok, res.Code);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task IsProductAvailable_Throws_Exception_If_ProductId_Is_Empty()
        {
            var rfReader = new Mock<IRingfenceReader>();
            var prodReader = new Mock<IProductReader>();
            var service = new ProductValidatorService(prodReader.Object, rfReader.Object, logger);

            await service.IsProductAvailable(Guid.Empty, 1);
        }

        [DataTestMethod]
        [DataRow(int.MinValue, DisplayName = "Min integer value")]
        [DataRow(-1, DisplayName = "Negative quantity")]        
        [ExpectedException(typeof(ArgumentException))]
        public async Task IsProductAvailable_Throws_Exception_If_Quantity_Is_Invalid(int quantity)
        {
            var rfReader = new Mock<IRingfenceReader>();
            var prodReader = new Mock<IProductReader>();
            var service = new ProductValidatorService(prodReader.Object, rfReader.Object, logger);

            await service.IsProductAvailable(Guid.NewGuid(), quantity);
        }

        [TestMethod]
        public async Task IsProductAvailable_Returns_InvalidProduct_Status_If_No_Product_Exists()
        {
            var rfReader = new Mock<IRingfenceReader>();
            var prodReader = new Mock<IProductReader>();
            prodReader.Setup(x => x.GetProduct(It.IsAny<Guid>()))
            .ReturnsAsync(default(Product));

            var service = new ProductValidatorService(prodReader.Object, rfReader.Object, logger);

            Result res = await service.IsProductAvailable(Guid.NewGuid(), 1);

            Assert.AreEqual(ResultCode.InvalidProduct, res.Code);
        }
        
        [DataTestMethod]
        [DataRow(10, null, 10, 1, ResultCode.InsufficientInventory, DisplayName = "all avaliable items reserved")]
        [DataRow(10, null, 9, 2, ResultCode.InsufficientInventory, DisplayName = "attempt to reserve more than available")]
        [DataRow(0, null, 0, 1, ResultCode.InsufficientInventory, DisplayName = "no inventory")]
        [DataRow(1, null, 0, 1, ResultCode.Ok, DisplayName = "last item")]
        [DataRow(2, null, 1, 1, ResultCode.Ok, DisplayName = "last unreserved item")]
        [DataRow(13, null, 1, 12, ResultCode.Ok, DisplayName = "many unreserved items")]
        public async Task IsProductAvailable_Returns_Correct_Status(int invetory, int? itemLimit, int reserved, int requested, ResultCode expectedResult)
        {
            var prodReader = new Mock<IProductReader>();
            prodReader.Setup(x => x.GetProduct(It.IsAny<Guid>()))
                      .ReturnsAsync(new Product { Quantity = invetory, LimitPerOrder = itemLimit});

            var rfReader = new Mock<IRingfenceReader>();
            rfReader.Setup(x => x.CountRingfencedProduct(It.IsAny<Guid>())).ReturnsAsync(reserved);
            var service = new ProductValidatorService(prodReader.Object, rfReader.Object, logger);

            Result res = await service.IsProductAvailable(Guid.NewGuid(), requested);

            Assert.AreEqual(expectedResult, res.Code);
        }        

        [TestInitialize]
        public void Init()
        {
            this.logger = new NullLoggerFactory().CreateLogger<ProductValidatorService>();
        }
    }
}
