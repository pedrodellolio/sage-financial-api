using Hangfire;

namespace SageFinancialAPI.Jobs
{
    public class HelloWorldJob(IBackgroundJobClient backgroundJobClient)
    {
        private readonly IBackgroundJobClient _backgroundJobClient = backgroundJobClient;

        public Task Execute()
        {
            Console.WriteLine("Hello world");

            // Schedule the next execution after 30 seconds
            _backgroundJobClient.Schedule(() => Execute(), TimeSpan.FromSeconds(30));

            return Task.CompletedTask;
        }
    }
}