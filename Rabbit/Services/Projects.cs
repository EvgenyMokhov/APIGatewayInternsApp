using InternsTestModels.Models.DTOs;
using InternsTestModels.Models.Rabbit.Projects.Requests;
using InternsTestModels.Models.Rabbit.Projects.Responses;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Rabbit.Services
{
    public class Projects
    {
        private readonly IServiceProvider Provider;
        private readonly ILogger<Projects> Logger;
        public Projects(IServiceProvider provider, ILogger<Projects> logger)
        {
            Provider = provider;
            Logger = logger;
        }

        public async Task CreateProjectAsync(ProjectDto project, Guid transactionId)
        {
            IPublishEndpoint publishEndpoint = Provider.GetRequiredService<IPublishEndpoint>();
            await publishEndpoint.Publish<CreateProjectRequest>(new() { RequestData = project, TransactionId = transactionId });
        }

        public async Task UpdateProjectAsync(ProjectDto project, Guid transactionId)
        {
            IPublishEndpoint publishEndpoint = Provider.GetRequiredService<IPublishEndpoint>();
            await publishEndpoint.Publish<UpdateProjectRequest>(new() { RequestData = project, TransactionId = transactionId });
        }

        public async Task<List<ProjectDto>> GetAllProjectsAsync(Guid transactionId)
        {
            IRequestClient<GetAllProjectsRequest> requestClient = Provider.GetRequiredService<IRequestClient<GetAllProjectsRequest>>();
            return (await requestClient.GetResponse<GetAllProjectsResponse>(new() { TransactionId = transactionId})).Message.ResponseData;
        }

        public async Task<ProjectDto> GetProjectAsync(Guid id, Guid transactionId)
        {
            IRequestClient<GetProjectRequest> requestClient = Provider.GetRequiredService<IRequestClient<GetProjectRequest>>();
            return (await requestClient.GetResponse<GetProjectResponse>(new() { Id = id, TransactionId = transactionId})).Message.ResponseData;
        }
    }
}
