using Checkout.Data.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace Checkout.Basket.TokenService.Tests
{
    [TestClass]
    public class BasketTokenServiceTests
    {
        ILogger<BasketTokenService> logger;
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_Throws_Exception_If_Writer_Dependency_Is_Null()
        {
            var reader = new Mock<IBasketReader>().Object;
            new BasketTokenService(null, reader, logger);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_Throws_Exception_If_Reader_Dependency_Is_Null()
        {
            var wrtrMock = new Mock<IBasketWriter>().Object;
            new BasketTokenService(wrtrMock, null, logger);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_Throws_Exception_If_Logger_Dependency_Is_Null()
        {
            var wrtrMock = new Mock<IBasketWriter>().Object;
            var reader = new Mock<IBasketReader>().Object;
            new BasketTokenService(wrtrMock, reader, null);
        }

        [TestMethod]
        public async Task BasketExists_Returns_False_If_BasketId_Is_Empty()
        {
            var wrtrMock = new Mock<IBasketWriter>().Object;
            var readerMock = new Mock<IBasketReader>();
            readerMock
                .Setup(x => x.GetBasketAsync(Guid.Empty))
                .ReturnsAsync(default(Data.Model.Basket));

            var service = new BasketTokenService(wrtrMock, readerMock.Object, logger);
            Assert.IsFalse(await service.BasketExistAsync(Guid.Empty));
        }

        [DataTestMethod]
        [DataRow(true, DisplayName = "basket exists")]
        [DataRow(false, DisplayName = "basket does not exist")]
        public async Task BasketExist_Returns_Correct_Result(bool basketExist)
        {
            var guid = new Guid("0f73d0f4-8745-4247-bd72-f0c1fdc614bb");
            var wrtrMock = new Mock<IBasketWriter>().Object;
            var readerMock = new Mock<IBasketReader>();
            readerMock.Setup(x => x.GetBasketAsync(guid))
                .ReturnsAsync(basketExist ? new Data.Model.Basket { Id = guid } : null);

            var service = new BasketTokenService(wrtrMock, readerMock.Object, logger);
            bool res = await service.BasketExistAsync(guid);
            Assert.AreEqual(basketExist, res);
        }

        [TestMethod]
        public async Task InitBasket_Creates_Basket()
        {
            var expected = new Data.Model.Basket { Id = Guid.NewGuid() };
            var writer = new Mock<IBasketWriter>();
            writer.Setup(x => x.InitBasketAsync())
                .ReturnsAsync(expected)
                .Verifiable();
            var readerMock = new Mock<IBasketReader>();
            var service = new BasketTokenService(writer.Object, readerMock.Object, logger);
            Guid res = await service.InitBasket();

            Assert.AreEqual(expected.Id, res);
            writer.Verify(x => x.InitBasketAsync(), Times.Once);
        }

        [TestMethod]
        public async Task InitBasket_Throws_Exception_If_Basket_Is_Null()
        {
            var writer = new Mock<IBasketWriter>();
            writer.Setup(x => x.InitBasketAsync()).ReturnsAsync(default(Data.Model.Basket))
                .Verifiable();
            var readerMock = new Mock<IBasketReader>();
            var service = new BasketTokenService(writer.Object, readerMock.Object, logger);

            bool gotException = false; 
            try
            {
                Guid res = await service.InitBasket();
            }
            catch (ApplicationException)
            {
                gotException = true;
            }
            
            Assert.IsTrue(gotException);
            writer.Verify(x => x.InitBasketAsync(), Times.Once);
        }

        [TestInitialize]
        public void Init()
        {
            this.logger = new NullLoggerFactory().CreateLogger<BasketTokenService>();
        }
    }
}
