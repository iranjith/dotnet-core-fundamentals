namespace CityInfo.API.Services
{
    public class LocalMailServices : IMailService
    {
        private string _mailTo = string.Empty;
        private string _mailFrom = string.Empty;

        public LocalMailServices(IConfiguration configuration) {

            _mailTo = configuration["mailSettings:mailToAddress"];
            _mailFrom = configuration["mailSettings:mailFromAddress"];
        }


        public void Send(string subject, string message)
        {
            Console.WriteLine($"Mail from {_mailFrom} to {_mailTo}, with the {nameof(LocalMailServices)} ");
            Console.WriteLine($"Subject {subject}");
            Console.WriteLine($"Message {message}");

        }

    }
}
