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
    /// Module processing requests of User domain.
    /// </summary>
    public sealed class UserModule : NancyModule
    {
        /// <summary>
        /// Sets up HTTP methods mappings.
        /// </summary>
        /// <param name="service">Service handling requests</param>
        public UserModule(UserService service) : base("/v2")
        { 
            Post["/user"] = parameters =>
            {
                var body = this.Bind<User>();
                Preconditions.IsNotNull(body, "Required parameter: 'body' is missing at 'CreateUser'");
                
                return service.CreateUser(Context, body);
            };

            Post["/user/createWithArray"] = parameters =>
            {
                var body = this.Bind<List<User>>();
                Preconditions.IsNotNull(body, "Required parameter: 'body' is missing at 'CreateUsersWithArrayInput'");
                
                return service.CreateUsersWithArrayInput(Context, body);
            };

            Post["/user/createWithList"] = parameters =>
            {
                var body = this.Bind<List<User>>();
                Preconditions.IsNotNull(body, "Required parameter: 'body' is missing at 'CreateUsersWithListInput'");
                
                return service.CreateUsersWithListInput(Context, body);
            };

            Delete["/user/{username}"] = parameters =>
            {
                var username = Parameters.ValueOf<string>(parameters, Context.Request, "username", ParameterType.Path);
                Preconditions.IsNotNull(username, "Required parameter: 'username' is missing at 'DeleteUser'");
                
                return service.DeleteUser(Context, username);
            };

            Get["/user/{username}"] = parameters =>
            {
                var username = Parameters.ValueOf<string>(parameters, Context.Request, "username", ParameterType.Path);
                Preconditions.IsNotNull(username, "Required parameter: 'username' is missing at 'GetUserByName'");
                
                return service.GetUserByName(Context, username);
            };

            Get["/user/login"] = parameters =>
            {
                var username = Parameters.ValueOf<string>(parameters, Context.Request, "username", ParameterType.Query);
                var password = Parameters.ValueOf<string>(parameters, Context.Request, "password", ParameterType.Query);
                Preconditions.IsNotNull(username, "Required parameter: 'username' is missing at 'LoginUser'");
                
                Preconditions.IsNotNull(password, "Required parameter: 'password' is missing at 'LoginUser'");
                
                return service.LoginUser(Context, username, password);
            };

            Get["/user/logout"] = parameters =>
            {
                
                return service.LogoutUser(Context);
            };

            Put["/user/{username}"] = parameters =>
            {
                var username = Parameters.ValueOf<string>(parameters, Context.Request, "username", ParameterType.Path);
                var body = this.Bind<User>();
                Preconditions.IsNotNull(username, "Required parameter: 'username' is missing at 'UpdateUser'");
                
                Preconditions.IsNotNull(body, "Required parameter: 'body' is missing at 'UpdateUser'");
                
                return service.UpdateUser(Context, username, body);
            };
        }
    }

    /// <summary>
    /// Service handling User requests.
    /// </summary>
    public interface UserService
    {
        /// <summary>
        /// This can only be done by the logged in user.
        /// </summary>
        /// <param name="context">Context of request</param>
        /// <param name="body">Created user object</param>
        /// <returns></returns>
        dynamic CreateUser(NancyContext context, User body);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context">Context of request</param>
        /// <param name="body">List of user object</param>
        /// <returns></returns>
        dynamic CreateUsersWithArrayInput(NancyContext context, List<User> body);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context">Context of request</param>
        /// <param name="body">List of user object</param>
        /// <returns></returns>
        dynamic CreateUsersWithListInput(NancyContext context, List<User> body);

        /// <summary>
        /// This can only be done by the logged in user.
        /// </summary>
        /// <param name="context">Context of request</param>
        /// <param name="username">The name that needs to be deleted</param>
        /// <returns></returns>
        dynamic DeleteUser(NancyContext context, string username);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context">Context of request</param>
        /// <param name="username">The name that needs to be fetched. Use user1 for testing. </param>
        /// <returns>User</returns>
        dynamic GetUserByName(NancyContext context, string username);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context">Context of request</param>
        /// <param name="username">The user name for login</param>
        /// <param name="password">The password for login in clear text</param>
        /// <returns>string</returns>
        dynamic LoginUser(NancyContext context, string username, string password);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context">Context of request</param>
        /// <returns></returns>
        dynamic LogoutUser(NancyContext context);

        /// <summary>
        /// This can only be done by the logged in user.
        /// </summary>
        /// <param name="context">Context of request</param>
        /// <param name="username">name that need to be updated</param>
        /// <param name="body">Updated user object</param>
        /// <returns></returns>
        dynamic UpdateUser(NancyContext context, string username, User body);
    }

    /// <summary>
    /// Abstraction of UserService.
    /// </summary>
    public abstract class AbstractUserService: UserService
    {
        public virtual dynamic CreateUser(NancyContext context, User body)
        {
            return CreateUser(body);
        }

        public virtual dynamic CreateUsersWithArrayInput(NancyContext context, List<User> body)
        {
            return CreateUsersWithArrayInput(body);
        }

        public virtual dynamic CreateUsersWithListInput(NancyContext context, List<User> body)
        {
            return CreateUsersWithListInput(body);
        }

        public virtual dynamic DeleteUser(NancyContext context, string username)
        {
            return DeleteUser(username);
        }

        public virtual dynamic GetUserByName(NancyContext context, string username)
        {
            return GetUserByName(username);
        }

        public virtual dynamic LoginUser(NancyContext context, string username, string password)
        {
            return LoginUser(username, password);
        }

        public virtual dynamic LogoutUser(NancyContext context)
        {
            return LogoutUser();
        }

        public virtual dynamic UpdateUser(NancyContext context, string username, User body)
        {
            return UpdateUser(username, body);
        }

        protected abstract dynamic CreateUser(User body);

        protected abstract dynamic CreateUsersWithArrayInput(List<User> body);

        protected abstract dynamic CreateUsersWithListInput(List<User> body);

        protected abstract dynamic DeleteUser(string username);

        protected abstract dynamic GetUserByName(string username);

        protected abstract dynamic LoginUser(string username, string password);

        protected abstract dynamic LogoutUser();

        protected abstract dynamic UpdateUser(string username, User body);
    }

}
