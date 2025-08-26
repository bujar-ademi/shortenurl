using Mediator;
using shorten.url.application.Contracts;

namespace shorten.url.application.Features.Notifications.NotificationsHandler
{
    public class UpdateAddressClickedNotificationHandler : INotificationHandler<AddressClicked>
    {
        private readonly IRepository repository;

        public UpdateAddressClickedNotificationHandler(IRepository repository)
        {
            this.repository = repository;
        }
        public async ValueTask Handle(AddressClicked notification, CancellationToken cancellationToken)
        {
            // just increase hits
            await repository.ExecuteNonQueryAsync("UPDATE Addresses SET Hits += 1 WHERE Id = @Id", new Models.QueryParameter { ParameterName = "Id", ParameterValue = notification.AddressId });
        }
    }
}
