using Newtonsoft.Json;
using SocialNetwork.BL.Exceptions;
using SocialNetwork.Web.Models;
using System.Net;


namespace SocialNetwork.Web.Extensions
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                var (code, response) = GetResponse(exception);
                //Set the response status code based on the code received from GetResponse
                context.Response.StatusCode = (int)code;
                //Set response content type to application/ json
                context.Response.ContentType = "application/json";
                //write error model in response
                await context.Response.WriteAsync(response);
            }
        }
        public (HttpStatusCode, string) GetResponse(Exception exception)
        {
            HttpStatusCode code = HttpStatusCode.BadRequest; // default code 
            string error = "Internal Server Error"; // default error
            string exceptionType = exception.GetType().Name; // give type of exception

            error = exception.Message; //if we have NOT default error we set in error our exception

            var errorModel = new ErrorModel 
            {
                Error = error,
                ExceptionType = exceptionType
            };

            var responseJson = JsonConvert.SerializeObject(errorModel); // in format to json 
            return (code, responseJson); // return code and our variable which we formated to json
        }



    }
    
}
