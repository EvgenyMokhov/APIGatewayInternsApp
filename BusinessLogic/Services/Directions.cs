using Rabbit;
using InternsTestModels.Models.DTOs;
using MassTransit.Initializers;

namespace BusinessLogic.Services
{
    public class Directions
    {
        private readonly RabbitManager Rabbit;
        public Directions(RabbitManager rabbit)
        {
            Rabbit = rabbit;
        }

        public async Task<List<DirectionHttpDto>> GetDirectionsAsync(Guid transactionId)
        {
            List<DirectionDto> directions = await Rabbit.Directions.GetAllDirectionsAsync(transactionId);
            return (await Task.WhenAll(directions.Select(async dir => await CreateDirectionHttpDto(dir, transactionId)))).Where(dir => dir.IsActive).ToList();
        }

        public async Task<List<DirectionLowDetailHttpDto>> GetDirectionsLowDetailAsync(Guid transactionId)
        {
            List<DirectionDto> directions = await Rabbit.Directions.GetAllDirectionsAsync(transactionId);
            return directions.Select(CreateDirectionLowDetailHttpDto).Where(direction => direction.IsActive).ToList();
        }

        public async Task<DirectionHttpDto> GetDirectionAsync(Guid directionId, Guid transactionId)
        {
            DirectionDto direction = await Rabbit.Directions.GetDirectionAsync(directionId, transactionId);
            return await CreateDirectionHttpDto(direction, transactionId);
        }

        public async Task CreateDirectionAsync(DirectionHttpDto direction, Guid transactionId)
        {
            await Rabbit.Directions.CreateDirectionAsync(CreateDirectionDto(direction), transactionId);
        }


        public async Task UpdateDirectionAsync(DirectionHttpDto direction, Guid transactionId)
        {
            HashSet<Guid> oldInternsIdsSet = (await Rabbit.Interns.GetInternsByDirection(direction.Id, transactionId))
                .Select(intern => intern.Id)
                .ToHashSet();
            foreach (InternLowDetailHttpDto newIntern in direction.Interns)
                if (!oldInternsIdsSet.Contains(newIntern.Id))
                {
                    InternDto internDto = await Rabbit.Interns.GetInternAsync(newIntern.Id, transactionId);
                    internDto.InternshipDirectionId = direction.Id;
                    await Rabbit.Interns.UpdateInternAsync(internDto, transactionId);
                }
            await Rabbit.Directions.UpdateDirectionAsync(CreateDirectionDto(direction), transactionId);
        }

        private async Task<DirectionHttpDto> CreateDirectionHttpDto(DirectionDto direction, Guid transactionId)
        {
            Interns internsService = new(Rabbit);
            return new()
            {
                Id = direction.Id,
                Description = direction.Description,
                Interns = (await Task.WhenAll(
                    (await Rabbit.Interns.GetInternsByDirection(direction.Id, transactionId))
                    .Select(async intern => await internsService.CreateInternLowDetailHttpDto(intern, transactionId))))
                    .ToList(),
                Name = direction.Name,
                IsActive = direction.IsActive,
            };
        }

        private DirectionLowDetailHttpDto CreateDirectionLowDetailHttpDto(DirectionDto direction)
        {
            return new()
            {
                Id = direction.Id,
                Name = direction.Name,
                Description = direction.Description,
                IsActive = direction.IsActive
            };
        }

        private DirectionDto CreateDirectionDto(DirectionHttpDto direction)
        {
            return new()
            {
                Id = direction.Id,
                Description = direction.Description,
                Name = direction.Name,
                IsActive = direction.IsActive
            };
        }
    }
}
