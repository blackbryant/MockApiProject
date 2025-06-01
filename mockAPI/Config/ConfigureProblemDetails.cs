using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace mockAPI.Config
{
    public static class ConfigureProblemDetails
    {
        public static void AddConfigureProblemDetails(this IServiceCollection services)
        {
            services.AddProblemDetails(options =>
            {
               options.CustomizeProblemDetails = (context)=>
               {
                   var httpContext = context.HttpContext;
                   context.ProblemDetails.Extensions["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;
                   context.ProblemDetails.Extensions["supportContact"] = "support@example.com";
                   if (context.ProblemDetails.Status == StatusCodes.Status401Unauthorized)
                   {
                       context.ProblemDetails.Title = "Resource Not Found";
                       context.ProblemDetails.Detail = "The requested resource could not be found.";
                   }
                   else if (context.ProblemDetails.Status == StatusCodes.Status500InternalServerError)
                   {
                       context.ProblemDetails.Title = "Internal Server Error";
                       context.ProblemDetails.Detail = "An unexpected error occurred. Please try again later.";
                   }
                   else if (context.ProblemDetails.Status == StatusCodes.Status404NotFound)
                   {
                       context.ProblemDetails.Title = "Resource Not Found";
                       context.ProblemDetails.Detail = "The requested resource could not be found.";
                   }
                   else
                   { 
                          context.ProblemDetails.Title = "An error occurred";
                          context.ProblemDetails.Detail = "An unexpected error occurred. Please try again later.";

                   }



               };
            });
        }

    }
}