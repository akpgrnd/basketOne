using Checkout.Basket.Ringfence.Contracts;
using Checkout.Core.Contracts;
using Checkout.Core.Contracts.Constants;
using Checkout.Data.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Checkout.Basket.Business.Test
{
    [TestClass]
    public class BasketManagerTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task ClearBasket_Throws_Exception_If_BasketId_Is_Empty()
        {
            var pvs = new Mock<IProductValidatorService>().Object;
            var rfs = new Mock<IRingfenceService>().Object;
            var writer = new Mock<IBasketWriter>().Object;
            var reader = new Mock<IBasketReader>().Object;

            var service = new BasketManager(pvs, rfs, writer, reader);
            await service.ClearBasketAsync(Guid.Empty);
        }

        [TestMethod]
        public async Task ClearBasket_Clears_Basket()
        {
            var pvs = new Mock<IProductValidatorService>().Object;
            var rfs = new Mock<IRingfenceService>().Object;            
            var reader = new Mock<IBasketReader>().Object;            
            var writer = new Mock<IBasketWriter>();

            var guid = Guid.NewGuid();
            var expected = new Result { Code = ResultCode.Ok };
            writer.Setup(x => x.ClearBasketAsync(guid))
                .ReturnsAsync(expected).Verifiable();

            var service = new BasketManager(pvs, rfs, writer.Object, reader);
            var result = await service.ClearBasketAsync(guid);

            Assert.AreEqual(expected, result);
            writer.Verify(x => x.ClearBasketAsync(guid), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task UpdateBasketAsync_Throws_Exception_If_NewQuantity_Is_Negative()
        {
            var pvs = new Mock<IProductValidatorService>().Object;
            var rfs = new Mock<IRingfenceService>().Object;
            var reader = new Mock<IBasketReader>().Object;
            var writer = new Mock<IBasketWriter>().Object;

            var service = new BasketManager(pvs, rfs, writer, reader);
            var res = await service.UpdateBasketAsync(Guid.NewGuid(), Guid.NewGuid(), -1);
        }

        [TestMethod]
        public async Task UpdateBasketAsync_Returns_InvalidBasket_Result_If_Basket_Is_Not_Found()
        {
            var pvs = new Mock<IProductValidatorService>().Object;
            var rfs = new Mock<IRingfenceService>().Object;
            var reader = new Mock<IBasketReader>();
            var writer = new Mock<IBasketWriter>().Object;

            var guid = Guid.NewGuid();            
            reader.Setup(x => x.GetBasketAsync(guid))
                .ReturnsAsync(default(Data.Model.Basket))
                .Verifiable();

            var service = new BasketManager(pvs, rfs, writer, reader.Object);
            var res = await service.UpdateBasketAsync(guid, Guid.NewGuid(), 1);

            Assert.AreEqual(ResultCode.BasketInvalid, res.Code);
            reader.Verify(x => x.GetBasketAsync(guid), Times.Once);
        }

        [TestMethod]
        public async Task UpdateBasketAsync_AddsItems_If_NewQuantity_Is_Larger_Than_Existing()
        {
            var basketId = Guid.NewGuid();
            var itemId = Guid.NewGuid();
            int newQuantity = 10;
            int curQuantity = 9;

            var pvs = new Mock<IProductValidatorService>();
            pvs.Setup(x => x.IsProductAvailable(itemId, newQuantity - curQuantity))
                .ReturnsAsync(new Result { Code = ResultCode.Ok });
            var rfs = new Mock<IRingfenceService>();
            rfs.Setup(x => x.TryReserveAsync(basketId, itemId, newQuantity))
                .ReturnsAsync(new Result { Code = ResultCode.Ok });
            var reader = new Mock<IBasketReader>();
            reader.Setup(x => x.GetBasketAsync(basketId))
                .ReturnsAsync(() => {
                    var basket = new Data.Model.Basket
                    {
                        Id = basketId
                    };
                    basket.ItemList.Add(new Data.Model.BasketItem { ProductId = itemId, Quantity = curQuantity });
                    return basket;
                });
            var writer = new Mock<IBasketWriter>();
            writer.Setup(x => x.AddAsync(basketId, itemId, newQuantity))
                .ReturnsAsync(new Result { Code = ResultCode.Ok });
            
            var service = new BasketManager(pvs.Object, rfs.Object, writer.Object, reader.Object);
            var res = await service.UpdateBasketAsync(basketId, itemId, newQuantity);

            Assert.AreEqual(ResultCode.Ok, res.Code);
            writer.Verify(x => x.AddAsync(basketId, itemId, newQuantity), Times.Once);
        }

        [TestMethod]
        public async Task RemoveBasketAsync_RemovesItems_If_NewQuantity_Is_Smaller_Than_Existing()
        {
            var basketId = Guid.NewGuid();
            var itemId = Guid.NewGuid();
            int newQuantity = 9;
            int curQuantity = 10;

            var pvs = new Mock<IProductValidatorService>();
            pvs.Setup(x => x.IsProductValid(itemId))
                .ReturnsAsync(new Result { Code = ResultCode.Ok });
            var rfs = new Mock<IRingfenceService>();
            rfs.Setup(x => x.ReleaseAsync(basketId, itemId, newQuantity))
                .ReturnsAsync(new Result { Code = ResultCode.Ok });
            var reader = new Mock<IBasketReader>();
            reader.Setup(x => x.GetBasketAsync(basketId))
                .ReturnsAsync(() => {
                    var basket = new Data.Model.Basket
                    {
                        Id = basketId,                    
                    };

                    basket.ItemList.Add(new Data.Model.BasketItem { ProductId = itemId, Quantity = curQuantity });

                    return basket;
                });
            var writer = new Mock<IBasketWriter>();
            writer.Setup(x => x.RemoveAsync(basketId, itemId, newQuantity))
                .ReturnsAsync(new Result { Code = ResultCode.Ok });

            var service = new BasketManager(pvs.Object, rfs.Object, writer.Object, reader.Object);
            var res = await service.UpdateBasketAsync(basketId, itemId, newQuantity);

            Assert.AreEqual(ResultCode.Ok, res.Code);
            writer.Verify(x => x.RemoveAsync(basketId, itemId, newQuantity), Times.Once);
        }
    }
}
