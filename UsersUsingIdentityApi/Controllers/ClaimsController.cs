using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace UsersUsingIdentityApi.Controllers
{
    public class ClaimsController : Controller
    {
        [Authorize]
        public ViewResult Index() => View(User?.Claims);
    }
}
