using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace shorten.url.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private IMediator mediator;

        protected IMediator Mediator
        {
            get
            {
                return mediator ??= HttpContext.RequestServices.GetService<IMediator>();
            }
        }
    }
}
