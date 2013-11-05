using System.Web.Mvc;

namespace WebApiSample.Service.WebApi.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
