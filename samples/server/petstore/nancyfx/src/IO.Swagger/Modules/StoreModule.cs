using System;
using Nancy;
using Nancy.ModelBinding;
using System.Collections.Generic;
using Sharpility.Base;
using IO.Swagger.v2.Models;
using IO.Swagger.v2.Utils;
using NodaTime;

namespace IO.Swagger.v2.Modules
{ 

    /// <summary>
    /// Module processing requests of Store domain.
    /// </summary>
    public sealed class StoreModule : NancyModule
    {
        /// <summary>
        /// Sets up HTTP methods mappings.
        /// </summary>
        /// <param name="service">Service handling requests</param>
        public StoreModule(StoreService service) : base("/v2")
        { 
            Delete["/store/order/{orderId}"] = parameters =>
            {
                var orderId = Parameters.ValueOf<long?>(parameters, Context.Request, "orderId", ParameterType.Path);
                Preconditions.IsNotNull(orderId, "Required parameter: 'orderId' is missing at 'DeleteOrder'");
                
                return service.DeleteOrder(Context, orderId);
            };

            Get["/store/inventory"] = parameters =>
            {
                
                return service.GetInventory(Context);
            };

            Get["/store/order/{orderId}"] = parameters =>
            {
                var orderId = Parameters.ValueOf<long?>(parameters, Context.Request, "orderId", ParameterType.Path);
                Preconditions.IsNotNull(orderId, "Required parameter: 'orderId' is missing at 'GetOrderById'");
                
                return service.GetOrderById(Context, orderId);
            };

            Post["/store/order"] = parameters =>
            {
                var body = this.Bind<Order>();
                Preconditions.IsNotNull(body, "Required parameter: 'body' is missing at 'PlaceOrder'");
                
                return service.PlaceOrder(Context, body);
            };
        }
    }

    /// <summary>
    /// Service handling Store requests.
    /// </summary>
    public interface StoreService
    {
        /// <summary>
        /// For valid response try integer IDs with positive integer value. Negative or non-integer values will generate API errors
        /// </summary>
        /// <param name="context">Context of request</param>
        /// <param name="orderId">ID of the order that needs to be deleted</param>
        /// <returns></returns>
        dynamic DeleteOrder(NancyContext context, long? orderId);

        /// <summary>
        /// Returns a map of status codes to quantities
        /// </summary>
        /// <param name="context">Context of request</param>
        /// <returns>Dictionary&lt;string, int?&gt;</returns>
        dynamic GetInventory(NancyContext context);

        /// <summary>
        /// For valid response try integer IDs with value &gt;&#x3D; 1 and &lt;&#x3D; 10. Other values will generated exceptions
        /// </summary>
        /// <param name="context">Context of request</param>
        /// <param name="orderId">ID of pet that needs to be fetched</param>
        /// <returns>Order</returns>
        dynamic GetOrderById(NancyContext context, long? orderId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context">Context of request</param>
        /// <param name="body">order placed for purchasing the pet</param>
        /// <returns>Order</returns>
        dynamic PlaceOrder(NancyContext context, Order body);
    }

    /// <summary>
    /// Abstraction of StoreService.
    /// </summary>
    public abstract class AbstractStoreService: StoreService
    {
        public virtual dynamic DeleteOrder(NancyContext context, long? orderId)
        {
            return DeleteOrder(orderId);
        }

        public virtual dynamic GetInventory(NancyContext context)
        {
            return GetInventory();
        }

        public virtual dynamic GetOrderById(NancyContext context, long? orderId)
        {
            return GetOrderById(orderId);
        }

        public virtual dynamic PlaceOrder(NancyContext context, Order body)
        {
            return PlaceOrder(body);
        }

        protected abstract dynamic DeleteOrder(long? orderId);

        protected abstract dynamic GetInventory();

        protected abstract dynamic GetOrderById(long? orderId);

        protected abstract dynamic PlaceOrder(Order body);
    }

}
