using System.Collections.Generic;
using System.Linq;

namespace Checkout.Basket.Business.Contracts.Model
{
    public class CustomerBasket
    {
        public CustomerBasket()
        {
            BasketItemList = new List<BasketItem>();
        }

        public List<BasketItem> BasketItemList { get; private set; }

        public double Total => BasketItemList.Sum(x => x.Quantity * x.Price);
    }
}
