using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Checkout.Basket.Business.Contracts.Model;
using Checkout.Basket.Business.Contracts;
using Checkout.Basket.API.ExtensionMethods;
using Microsoft.AspNetCore.Http;
using Checkout.Core.Contracts.Constants;
using Checkout.Basket.API.Filters;

namespace Checkout.Basket.API.Controllers
{
    [Route("api/[controller]")]
    [ServiceFilter(typeof(BasketTokenFilter))]
    public class BasketController : Controller
    {
        readonly IBasketManager basketManager;
        public BasketController(IBasketManager bmanager, ILogger<BasketController> log)
        {   
            this.basketManager = bmanager;
        }

        [HttpGet]
        public async Task<CustomerBasket> Get()
        {
            Guid? basketId = this.HttpContext.User.GetBasketToken();

            return await basketManager.GetBasketAsync(basketId.Value);
        }
        
        [HttpPost("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddItem(Guid id, [FromBody]int quantity)
        {
            Guid? basketId = this.HttpContext.User.GetBasketToken();
            var result = await basketManager.UpdateBasketAsync(basketId.Value, id, quantity);

            if (result.Code != ResultCode.Ok) return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateQuantity(Guid id, [FromBody]int quantity)
        {
            Guid? basketId = this.HttpContext.User.GetBasketToken();

            var result = await basketManager.UpdateBasketAsync(basketId.Value, id, quantity);

            if (result.Code != ResultCode.Ok) return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoveItem(Guid id)
        {
            Guid? basketId = this.HttpContext.User.GetBasketToken();

            var result = await basketManager.RemoveItemAsync(basketId.Value, id);

            if (result.Code != ResultCode.Ok) return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Clear()
        {
            Guid? basketId = this.HttpContext.User.GetBasketToken();

            var result = await basketManager.ClearBasketAsync(basketId.Value);

            if (result.Code != ResultCode.Ok) return BadRequest(result);

            return Ok(result);
        }
    }
}
