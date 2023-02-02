using IsraelRail.Filters;
using Microsoft.AspNetCore.Mvc;

namespace IsraelRail.Controllers
{
    [TypeFilter(typeof(ViewBagFilter))]
    public class BaseController : Controller
    {
        
    }
}
