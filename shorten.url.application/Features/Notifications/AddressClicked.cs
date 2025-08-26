using Mediator;

namespace shorten.url.application.Features.Notifications
{
    public class AddressClicked : INotification
    {
        public Guid AddressId { get; set; }
    }
}
