using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;

namespace IsraelRail.Filters
{
    public class ViewBagFilter : Attribute, IActionFilter
    {
        private readonly string _aiInstrumentationKey;

        public ViewBagFilter(IConfiguration configuration)
        {
            _aiInstrumentationKey = configuration.GetValue<string>("ApplicationInsights:InstrumentationKey");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.Controller is Controller controller && !string.IsNullOrWhiteSpace(_aiInstrumentationKey))
            {
                controller.ViewData["AiInstrumentationKey"] = _aiInstrumentationKey;
            }
        }
    }
}
