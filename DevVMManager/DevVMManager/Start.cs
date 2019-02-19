namespace DevVMManager
{
    using System;
    using System.Threading.Tasks;

    using Management;
    using Management.Models;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Azure Function to start a VM
    /// </summary>
    public static class Start
    {
        private static readonly Credentials Credentials = new Credentials
        {
            SubscriptionId = Environment.GetEnvironmentVariable("subscription"),
            TenantId = Environment.GetEnvironmentVariable("tenant"),
            Key = Environment.GetEnvironmentVariable("key"),
            ClientId = Environment.GetEnvironmentVariable("client")
        };

        private static readonly VmManager VmManager = new VmManager(Credentials);

        [FunctionName("Start")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            return await VmManager.StartVm(req, log);
        }
    }
}
