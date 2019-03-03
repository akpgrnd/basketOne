using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Checkout.Basket.API.ExtensionMethods
{
    public static class SecurityExtensions
    {
        public static Guid? GetBasketToken(this ClaimsPrincipal principal)
        {
            foreach (ClaimsIdentity ident in principal.Identities)
            {
                foreach (Claim claim in ident.Claims)
                {
                    if (claim.Type == Constants.Claims.BasketToken)
                    {
                        if (Guid.TryParse(claim.Value, out Guid res))
                        {
                            return res;
                        }
                        return null;
                    }
                }
            }

            return null;
        }
    }
}
