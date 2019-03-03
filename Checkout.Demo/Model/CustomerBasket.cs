using System.Collections.Generic;
using System.Linq;

namespace Checkout.Demo
{
    public class CustomerBasket
    {
        public List<BasketItem> BasketItemList { get; set; }

        public double Total { get; set; }
    }
}
