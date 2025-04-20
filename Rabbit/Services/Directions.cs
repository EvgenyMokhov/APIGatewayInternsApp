using InternsTestModels.Models.DTOs;
using InternsTestModels.Models.Rabbit.Direction.Requests;
using InternsTestModels.Models.Rabbit.Direction.Responses;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Rabbit.Services
{
    public class Directions
    {
        private readonly IServiceProvider Provider;
        private readonly ILogger<Directions> Logger;
        public Directions(IServiceProvider provider, ILogger<Directions> logger)
        {
            Provider = provider;
            Logger = logger;
        }

        public async Task CreateDirectionAsync(DirectionDto direction, Guid transactionId)
        {
            IPublishEndpoint publishEndpoint = Provider.GetRequiredService<IPublishEndpoint>();
            await publishEndpoint.Publish<CreateDirectionRequest>(new() { RequestData = direction, TransactionId = transactionId });
        }

        public async Task UpdateDirectionAsync(DirectionDto direction, Guid transactionId)
        {
            IPublishEndpoint publishEndpoint = Provider.GetRequiredService<IPublishEndpoint>();
            await publishEndpoint.Publish<UpdateDirectionRequest>(new() { RequestData = direction , TransactionId = transactionId });
        }

        public async Task<DirectionDto> GetDirectionAsync(Guid id, Guid transactionId)
        {
            IRequestClient<GetDirectionRequest> requestClient = Provider.GetRequiredService<IRequestClient<GetDirectionRequest>>();
            return (await requestClient.GetResponse<GetDirectionResponse>(new() { Id = id, TransactionId = transactionId })).Message.ResponseData;
        }

        public async Task<List<DirectionDto>> GetAllDirectionsAsync(Guid transactionId)
        {
            IRequestClient<GetAllDirectionsRequest> requestClient = Provider.GetRequiredService<IRequestClient<GetAllDirectionsRequest>>();
            Logger.LogInformation("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
            return (await requestClient.GetResponse<GetAllDirectionsResponse>(new() { TransactionId = transactionId})).Message.ResponseData;
        }
    }
}
