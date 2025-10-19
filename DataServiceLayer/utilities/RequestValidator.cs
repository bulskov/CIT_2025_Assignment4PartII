
using System.Text.Json;
using System.Collections.Generic;
using DataServiceLayer.Models;
namespace DataServiceLayer.Utilities
{
    public class RequestValidator
    {
        private readonly List<string> validMethods = new()
        {
            "create", "read", "update", "delete", "echo"
        };

        public Response ValidateRequest(Request request)
        {
            // Check if method exists
            if (string.IsNullOrEmpty(request.Method))
                return new Response { Status = "missing method" };

            // Check if method is allowed
            if (!validMethods.Contains(request.Method.ToLower()))
                return new Response { Status = "illegal method" };

            // Check if path is missing
            if (string.IsNullOrEmpty(request.Path))
                return new Response { Status = "missing path" };

            // Check if date is missing
            if (string.IsNullOrEmpty(request.Date))
                return new Response { Status = "missing date" };

            // Check if date is a valid value (should be a number)
            if (!long.TryParse(request.Date, out _))
                return new Response { Status = "illegal date" };

            // For create, update, log : require body
            if ((request.Method == "create" || request.Method == "update" || request.Method == "echo")
                && string.IsNullOrEmpty(request.Body))
                return new Response { Status = "missing body" };

            // If body exists for create/update, it must be valid JSON
            if (!string.IsNullOrEmpty(request.Body) &&
                (request.Method == "create" || request.Method == "update"))
            {
                try
                {
                    JsonDocument.Parse(request.Body);
                }
                catch
                {
                    return new Response { Status = "illegal body" };
                }
            }

            // Everything passed
            return new Response { Status = "1 Ok" };
        }
    }
}
