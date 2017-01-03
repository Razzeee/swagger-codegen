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
    /// Status values that need to be considered for filter
    /// </summary>
    public enum FindPetsByStatusStatusEnum
    {
        available, 
        pending, 
        sold
    };


    /// <summary>
    /// Module processing requests of Pet domain.
    /// </summary>
    public sealed class PetModule : NancyModule
    {
        /// <summary>
        /// Sets up HTTP methods mappings.
        /// </summary>
        /// <param name="service">Service handling requests</param>
        public PetModule(PetService service) : base("/v2")
        { 
            Post["/pet"] = parameters =>
            {
                var body = this.Bind<Pet>();
                Preconditions.IsNotNull(body, "Required parameter: 'body' is missing at 'AddPet'");
                
                return service.AddPet(Context, body);
            };

            Delete["/pet/{petId}"] = parameters =>
            {
                var petId = Parameters.ValueOf<long?>(parameters, Context.Request, "petId", ParameterType.Path);
                var apiKey = Parameters.ValueOf<string>(parameters, Context.Request, "apiKey", ParameterType.Header);
                Preconditions.IsNotNull(petId, "Required parameter: 'petId' is missing at 'DeletePet'");
                
                return service.DeletePet(Context, petId, apiKey);
            };

            Get["/pet/findByStatus"] = parameters =>
            {
                var status = Parameters.ValueOf<FindPetsByStatusStatusEnum?>(parameters, Context.Request, "status", ParameterType.Query);
                Preconditions.IsNotNull(status, "Required parameter: 'status' is missing at 'FindPetsByStatus'");
                
                return service.FindPetsByStatus(Context, status).ToArray();
            };

            Get["/pet/findByTags"] = parameters =>
            {
                var tags = Parameters.ValueOf<List<string>>(parameters, Context.Request, "tags", ParameterType.Query);
                Preconditions.IsNotNull(tags, "Required parameter: 'tags' is missing at 'FindPetsByTags'");
                
                return service.FindPetsByTags(Context, tags).ToArray();
            };

            Get["/pet/{petId}"] = parameters =>
            {
                var petId = Parameters.ValueOf<long?>(parameters, Context.Request, "petId", ParameterType.Path);
                Preconditions.IsNotNull(petId, "Required parameter: 'petId' is missing at 'GetPetById'");
                
                return service.GetPetById(Context, petId);
            };

            Put["/pet"] = parameters =>
            {
                var body = this.Bind<Pet>();
                Preconditions.IsNotNull(body, "Required parameter: 'body' is missing at 'UpdatePet'");
                
                return service.UpdatePet(Context, body);
            };

            Post["/pet/{petId}"] = parameters =>
            {
                var petId = Parameters.ValueOf<long?>(parameters, Context.Request, "petId", ParameterType.Path);
                var name = Parameters.ValueOf<string>(parameters, Context.Request, "name", ParameterType.Undefined);
                var status = Parameters.ValueOf<string>(parameters, Context.Request, "status", ParameterType.Undefined);
                Preconditions.IsNotNull(petId, "Required parameter: 'petId' is missing at 'UpdatePetWithForm'");
                
                return service.UpdatePetWithForm(Context, petId, name, status);
            };

            Post["/pet/{petId}/uploadImage"] = parameters =>
            {
                var petId = Parameters.ValueOf<long?>(parameters, Context.Request, "petId", ParameterType.Path);
                var additionalMetadata = Parameters.ValueOf<string>(parameters, Context.Request, "additionalMetadata", ParameterType.Undefined);
                var file = Parameters.ValueOf<System.IO.Stream>(parameters, Context.Request, "file", ParameterType.Undefined);
                Preconditions.IsNotNull(petId, "Required parameter: 'petId' is missing at 'UploadFile'");
                
                return service.UploadFile(Context, petId, additionalMetadata, file);
            };
        }
    }

    /// <summary>
    /// Service handling Pet requests.
    /// </summary>
    public interface PetService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context">Context of request</param>
        /// <param name="body">Pet object that needs to be added to the store</param>
        /// <returns></returns>
        dynamic AddPet(NancyContext context, Pet body);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context">Context of request</param>
        /// <param name="petId">Pet id to delete</param>
        /// <param name="apiKey"> (optional)</param>
        /// <returns></returns>
        dynamic DeletePet(NancyContext context, long? petId, string apiKey);

        /// <summary>
        /// Multiple status values can be provided with comma separated strings
        /// </summary>
        /// <param name="context">Context of request</param>
        /// <param name="status">Status values that need to be considered for filter</param>
        /// <returns>List&lt;Pet&gt;</returns>
        dynamic FindPetsByStatus(NancyContext context, FindPetsByStatusStatusEnum? status);

        /// <summary>
        /// Muliple tags can be provided with comma separated strings. Use tag1, tag2, tag3 for testing.
        /// </summary>
        /// <param name="context">Context of request</param>
        /// <param name="tags">Tags to filter by</param>
        /// <returns>List&lt;Pet&gt;</returns>
        dynamic FindPetsByTags(NancyContext context, List<string> tags);

        /// <summary>
        /// Returns a single pet
        /// </summary>
        /// <param name="context">Context of request</param>
        /// <param name="petId">ID of pet to return</param>
        /// <returns>Pet</returns>
        dynamic GetPetById(NancyContext context, long? petId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context">Context of request</param>
        /// <param name="body">Pet object that needs to be added to the store</param>
        /// <returns></returns>
        dynamic UpdatePet(NancyContext context, Pet body);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context">Context of request</param>
        /// <param name="petId">ID of pet that needs to be updated</param>
        /// <param name="name">Updated name of the pet (optional)</param>
        /// <param name="status">Updated status of the pet (optional)</param>
        /// <returns></returns>
        dynamic UpdatePetWithForm(NancyContext context, long? petId, string name, string status);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context">Context of request</param>
        /// <param name="petId">ID of pet to update</param>
        /// <param name="additionalMetadata">Additional data to pass to server (optional)</param>
        /// <param name="file">file to upload (optional)</param>
        /// <returns>ApiResponse</returns>
        dynamic UploadFile(NancyContext context, long? petId, string additionalMetadata, System.IO.Stream file);
    }

    /// <summary>
    /// Abstraction of PetService.
    /// </summary>
    public abstract class AbstractPetService: PetService
    {
        public virtual dynamic AddPet(NancyContext context, Pet body)
        {
            return AddPet(body);
        }

        public virtual dynamic DeletePet(NancyContext context, long? petId, string apiKey)
        {
            return DeletePet(petId, apiKey);
        }

        public virtual dynamic FindPetsByStatus(NancyContext context, FindPetsByStatusStatusEnum? status)
        {
            return FindPetsByStatus(status);
        }

        public virtual dynamic FindPetsByTags(NancyContext context, List<string> tags)
        {
            return FindPetsByTags(tags);
        }

        public virtual dynamic GetPetById(NancyContext context, long? petId)
        {
            return GetPetById(petId);
        }

        public virtual dynamic UpdatePet(NancyContext context, Pet body)
        {
            return UpdatePet(body);
        }

        public virtual dynamic UpdatePetWithForm(NancyContext context, long? petId, string name, string status)
        {
            return UpdatePetWithForm(petId, name, status);
        }

        public virtual dynamic UploadFile(NancyContext context, long? petId, string additionalMetadata, System.IO.Stream file)
        {
            return UploadFile(petId, additionalMetadata, file);
        }

        protected abstract dynamic AddPet(Pet body);

        protected abstract dynamic DeletePet(long? petId, string apiKey);

        protected abstract dynamic FindPetsByStatus(FindPetsByStatusStatusEnum? status);

        protected abstract dynamic FindPetsByTags(List<string> tags);

        protected abstract dynamic GetPetById(long? petId);

        protected abstract dynamic UpdatePet(Pet body);

        protected abstract dynamic UpdatePetWithForm(long? petId, string name, string status);

        protected abstract dynamic UploadFile(long? petId, string additionalMetadata, System.IO.Stream file);
    }

}
