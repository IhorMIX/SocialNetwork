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

            var errorModel = new ErrorModel { Error = exception.Message, ExceptionType = exception.GetType().Name };

            var responseJson = JsonConvert.SerializeObject(errorModel); // in format to json 
            return (code, responseJson); // return code and our variable which we formated to json
        }



    }
    
}
