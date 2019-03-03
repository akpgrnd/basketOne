# basketOne
Basket test API

# Technology stack
- Solution is written .Net Core 2.2 
- Visual Studio 2017

# How to run
- Run from VisualStudio using IIS Express
- Set Checkout.Basket.API as a startup project
- To run demo automatically add Checkout.Demo project to startup (i.e. use option to set two startup projects).

# What API is capable of
- Show basket
- Add items to basket
- Remove items from basket
- Clear items in basket
- Clear whole basket
- Ringfence inventory added to user baskets (incomplete prototype functionality only)
- Store Data in memory only

# Security
- This is a prototype, it only relies on http header X-Token to identify current user basket. Client application is expected to handle token securely. The header is validated and set in Middleware
- Additional security check is added to API controller via ActionFilter
- Services Checkout.Basket.API and Checkout.Basket.TokenService can be improved for additional security 


# Biggest shortcomings due to time constraints
- It would be great to implement pricing service which would verify prices, apply discounts and add notifications just before returning basket. This can be added later to BasketManager class
- Low unit test coverage. Interfaces were written before implementations and solution in general allows easy unit testing, but unit tests are written only for a small amount of classes
- Corner cuttings in data access layer (because everything is in memory and not as in real life)
- Basket expiry is not implemented fully
- Inventory ringfencing is not imlemented fully, but can be easily added
- RingfenceService and TokenService are meant to be external services as opposed to class libraries. However, to make them as microservices further code decoupling improvements will be required
- Config values are hardcoded
- Serilog should write to a DB or file system
- HTTPS, HSTS, etc. is not configured in the API project. Those configs are omitted for simplicity. This API is meant to be available in HTTPS-only mode
- Some namespaces could have better names
- No swagger


# Navigating through Visual Studio Solution

- Checkout.Basket.API - Web API project
- Checkout.Basket.Business - Here leave business/application services which are used in the API project
- Checkout.Data - Data access layer
- Checkout.Basket.RingfenceService - Inventory ringfence service which consists of two business projects (services and contracts) and a unit tests project. In real life this would be an external service
- Checkout.Basket.TokenService Basket - Token/Security service which consists of two business projects (services and contracts) and a unit tests project. In real life this would be an external service
- Checkout.Core.Contracts - A project without dependencies to any other projects in the solution. It is a temporary project and would need to be removed

- Checkout.Demo - Demo console application
