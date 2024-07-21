using Journey.Api.Resources;
using Journey.Application.UseCases.Trips.GetAll;
using Quartz;

namespace Journey.Api.Jobs
{
    public class CheckTripsJob : IJob
    {
        private readonly GetAllTripsUseCase _getAllTripsUseCase;

        private EmailService _emailService = new(
                smtpServer: ResourceEmailConfig.SMTP_SERVER,
                smtpPort: int.Parse(ResourceEmailConfig.SMTP_PORT),
                smtpUser: ResourceEmailConfig.SMTP_USER,
                smtpPass: ResourceEmailConfig.SMTP_PASSWORD
                );
        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("Checking Trips: " + DateTime.Now);

            var tripsList = _getAllTripsUseCase.Execute();

            Console.WriteLine(tripsList.Trips);

            var threshold = TimeSpan.FromDays(7);

            foreach (var trip in tripsList.Trips)
            {
                if(DateTime.Now < trip.StartDate &&  (trip.StartDate - DateTime.Now) <= threshold)
                {
                    _emailService.SendEmail(
                        toEmail: trip.Owner.Email,
                        subject: $"Não perca sua viagem para {trip.Country}!",
                        body: new TripWarn(trip).HtmlContent,
                        isBodyHtml: true
                    );
                }
            }

            return Task.CompletedTask;
        }
    }
}
