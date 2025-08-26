using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shorten.url.application.Features.Commands;

namespace shorten.url.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AddressController : ApiController
    {

        [HttpPost]
        [Route("shorten")]
        public async Task<IActionResult> ShortenUrlAsync([FromBody] CreateShortenAddressCommand request)
        {
            var model = await Mediator.Send(request).ConfigureAwait(false);

            return Ok(model);
        }
    }
}
