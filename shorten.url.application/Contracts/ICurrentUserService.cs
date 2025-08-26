using System;
using System.Collections.Generic;
using System.Text;

namespace shorten.url.application.Contracts
{
    public interface ICurrentUserService
    {
        Guid? ApiClientId { get; }
    }
}
