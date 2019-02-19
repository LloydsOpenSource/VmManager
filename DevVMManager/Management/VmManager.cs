namespace Management
{
    using System.IO;
    using System.Threading.Tasks;

    using Microsoft.Azure.Management.Fluent;
    using Microsoft.Azure.Management.ResourceManager.Fluent;
    using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    using Models;

    using Newtonsoft.Json;

    public class VmManager
    {
        private readonly IAzure _azure;

        public VmManager(IAzure azure)
        {
            _azure = azure;
        }

        public VmManager(Credentials credentials)
        {
            var serviceCredentials = SdkContext.AzureCredentialsFactory.FromServicePrincipal(credentials.ClientId, credentials.Key, credentials.TenantId, AzureEnvironment.AzureGlobalCloud);

            _azure = Azure.Configure()
                .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
                .Authenticate(serviceCredentials)
                .WithSubscription(credentials.SubscriptionId);
        }

        public async Task<IActionResult> StartVm(HttpRequest req, ILogger log)
        {

            log.LogInformation("C# HTTP function triggered to start a VM.");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string vmName = req.Query["vmName"];
            vmName = vmName ?? data?.vmName;

            string groupName = req.Query["groupName"];
            groupName = groupName ?? data?.groupName;

            if (vmName == null || groupName == null)
            {
                log.LogInformation("vmName or groupName is null.");
                return new BadRequestObjectResult("Please pass a vmName and groupName on the query string or in the request body");
            }

            string async = req.Query["async"];
            async = async ?? data?.async;

            var vm = _azure.VirtualMachines.GetByResourceGroup(groupName, vmName);

            if (async != "true")
            {
                vm.Start();
                log.LogInformation($"VM {vmName} in resource group {groupName} has started.");

                return new OkObjectResult($"VM {vmName} started");
            }

            await vm.StartAsync().ConfigureAwait(false);
            log.LogInformation($"VM {vmName} in resource group {groupName} is starting...");

            return new OkObjectResult($"VM {vmName} starting...");
        }
    }
}
