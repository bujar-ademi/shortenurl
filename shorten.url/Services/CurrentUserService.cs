using shorten.url.application.Contracts;
using System.Security.Claims;

namespace shorten.url.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }

        public Guid? ApiClientId
        {
            get
            {
                var sid = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Sid);
                if (!string.IsNullOrEmpty(sid))
                {
                    return Guid.Parse(sid);
                }
                return null;
            }
        }
    }
}
