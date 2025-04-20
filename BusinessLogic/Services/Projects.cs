using Rabbit;
using Rabbit.Services;
using InternsTestModels.Models.DTOs;

namespace BusinessLogic.Services
{
    public class Projects
    {
        private readonly RabbitManager Rabbit;
        public Projects(RabbitManager rabbit)
        {
            Rabbit = rabbit;
        }

        public async Task<List<ProjectHttpDto>> GetProjectsAsync(Guid transactionId)
        {
            List<ProjectDto> projects = await Rabbit.Projects.GetAllProjectsAsync(transactionId);
            return (await Task.WhenAll(projects.Select(async dir => await CreateProjectHttpDto(dir, transactionId)))).Where(dir => !dir.IsActive).ToList();
        }

        public async Task<List<ProjectLowDetailHttpDto>> GetLowDetailProjectsAsync(Guid transactionId)
        {
            List<ProjectDto> projects = await Rabbit.Projects.GetAllProjectsAsync(transactionId);
            return projects.Select(CreateLowDetailProjectHttpDto).Where(project => !project.IsActive).ToList();
        }

        public async Task<ProjectHttpDto> GetProjectAsync(Guid projectId, Guid transactionId)
        {
            ProjectDto project = await Rabbit.Projects.GetProjectAsync(projectId, transactionId);
            return await CreateProjectHttpDto(project, transactionId);
        }

        public async Task CreateProjectAsync(ProjectHttpDto project, Guid transactionId)
        {
            await Rabbit.Projects.CreateProjectAsync(CreateProjectDto(project), transactionId);
        }

        public async Task UpdateProjectAsync(ProjectHttpDto project, Guid transactionId)
        {
            HashSet<Guid> oldInternsIdsSet = (await Rabbit.Interns.GetInternsByProject(project.Id, transactionId))
                .Select(intern => intern.Id)
                .ToHashSet();
            foreach (InternLowDetailHttpDto newIntern in project.Interns)
                if (!oldInternsIdsSet.Contains(newIntern.Id))
                {
                    InternDto internDto = await Rabbit.Interns.GetInternAsync(newIntern.Id, transactionId);
                    internDto.InternshipDirectionId = project.Id;
                    await Rabbit.Interns.UpdateInternAsync(internDto, transactionId);
                }
            await Rabbit.Projects.UpdateProjectAsync(CreateProjectDto(project), transactionId);
        }

        private async Task<ProjectHttpDto> CreateProjectHttpDto(ProjectDto project, Guid transactionId)
        {
            Interns internsService = new(Rabbit);
            return new()
            {
                Id = project.Id,
                Description = project.Description,
                Interns = (await Task.WhenAll(
                    (await Rabbit.Interns.GetInternsByProject(project.Id, transactionId))
                    .Select(async intern => await internsService.CreateInternLowDetailHttpDto(intern, transactionId))))
                    .ToList(),
                Name = project.Name,
                IsActive = project.IsActive,
            };
        }

        private ProjectLowDetailHttpDto CreateLowDetailProjectHttpDto(ProjectDto project)
        {
            return new()
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                IsActive = project.IsActive
            };
        }

        private ProjectDto CreateProjectDto(ProjectHttpDto project)
        {
            return new()
            {
                Id = project.Id,
                Description = project.Description,
                Name = project.Name,
                IsActive = project.IsActive
            };
        }
    }
}
