using Rabbit;
using InternsTestModels.Models.DTOs;

namespace BusinessLogic.Services
{
    public class Interns
    {
        private readonly RabbitManager Rabbit;
        public Interns(RabbitManager rabbit) => Rabbit = rabbit;

        public async Task<List<InternHttpDto>> GetInternsAsync(Guid transactionId)
        {
            List<InternDto> interns = await Rabbit.Interns.GetAllInternsAsync(transactionId);
            return (await Task.WhenAll(interns.Select(async intern => await GetInternHttpDto(intern, transactionId)))).ToList();
        }

        public async Task<InternHttpDto> GetInternAsync(Guid id, Guid transactionId)
        {
            InternDto intern = await Rabbit.Interns.GetInternAsync(id, transactionId);
            return await GetInternHttpDto(intern, transactionId);
        }

        public async Task CreateInternAsync(InternHttpDto intern, Guid transactionId)
        {
            await Rabbit.Interns.CreateInternAsync(GetInternDto(intern), transactionId);
        }

        public async Task UpdateInternAsync(InternHttpDto intern, Guid transactionId)
        {
            await Rabbit.Interns.UpdateInternAsync(GetInternDto(intern), transactionId);
        }

        public async Task<InternLowDetailHttpDto> CreateInternLowDetailHttpDto(InternDto intern, Guid transactionId)
        {
            InternLowDetailHttpDto result = new();
            result.Id = intern.Id;
            result.User = await Rabbit.Users.GetUserAsync(intern.UserId, transactionId);
            return result;
        }

        public async Task<InternHttpDto> GetInternHttpDto(InternDto intern, Guid transactionId, ProjectDto project)
        {
            InternHttpDto result = new InternHttpDto();
            result.Id = intern.Id;
            result.User = await Rabbit.Users.GetUserAsync(intern.UserId, transactionId);
            result.Project = project;
            result.Direction = await Rabbit.Directions.GetDirectionAsync(intern.InternshipDirectionId, transactionId);
            return result;
        }

        public async Task<InternHttpDto> GetInternHttpDto(InternDto intern, Guid transactionId, DirectionDto direction)
        {
            InternHttpDto result = new InternHttpDto();
            result.Id = intern.Id;
            result.User = await Rabbit.Users.GetUserAsync(intern.UserId, transactionId);
            result.Project = await Rabbit.Projects.GetProjectAsync(intern.ProjectId, transactionId);
            result.Direction = direction;
            return result;
        }

        private async Task<InternHttpDto> GetInternHttpDto(InternDto intern, Guid transactionId)
        {
            InternHttpDto result = new InternHttpDto();
            result.Id = intern.Id;
            result.User = await Rabbit.Users.GetUserAsync(intern.UserId, transactionId);
            result.Project = await Rabbit.Projects.GetProjectAsync(intern.ProjectId, transactionId);
            result.Direction = await Rabbit.Directions.GetDirectionAsync(intern.InternshipDirectionId, transactionId);
            return result;
        }

        public InternDto GetInternDto(InternHttpDto intern)
        {
            return new()
            {
                Id = intern.Id,
                ProjectId = intern.Project.Id,
                UserId = intern.User.Id,
                InternshipDirectionId = intern.Direction.Id
            };
        }
    }
}
