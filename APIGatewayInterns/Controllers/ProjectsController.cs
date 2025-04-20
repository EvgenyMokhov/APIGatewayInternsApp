using BusinessLogic;
using Rabbit;
using InternsTestModels.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace APIGatewayInterns.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly ServiceManager ServiceManager;
        public ProjectsController(RabbitManager rabbit) => ServiceManager = new(rabbit);

        [HttpGet("all")]
        public async Task<IActionResult> GetAllProjects()
        {
            Guid transactionId = Guid.NewGuid();
            return Ok(ServiceManager.Projects.GetProjectsAsync(transactionId));
        }

        [HttpGet("low_detail_all")]
        public async Task<IActionResult> GetLowDetailProjects()
        {
            Guid transactionid = Guid.NewGuid();
            return Ok(await ServiceManager.Projects.GetLowDetailProjectsAsync(transactionid));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject([FromRoute] Guid id)
        {
            Guid transactionId = Guid.NewGuid();
            return Ok(ServiceManager.Projects.GetProjectAsync(id, transactionId));
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateProject([FromBody] ProjectHttpDto project)
        {
            Guid transactionId = Guid.NewGuid();
            await ServiceManager.Projects.CreateProjectAsync(project, transactionId);
            return Ok();
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateProject([FromBody] ProjectHttpDto project)
        {
            if (!project.IsActive && project.Interns.Count != 0)
                return BadRequest("Cannot delete direction with active interns");
            Guid transactionId = Guid.NewGuid();
            await ServiceManager.Projects.UpdateProjectAsync(project, transactionId);
            return Ok();
        }
    }
}
