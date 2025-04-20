using BusinessLogic;
using Rabbit;
using InternsTestModels.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace APIGatewayInterns.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InternsController : ControllerBase
    {
        private readonly ServiceManager ServiceManager;
        public InternsController(RabbitManager rabbit) => ServiceManager = new(rabbit);

        [HttpGet("all")]
        public async Task<IActionResult> GetAllInterns()
        {
            Guid transactionId = Guid.NewGuid();
            await ServiceManager.Interns.GetInternsAsync(transactionId);
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetIntern([FromRoute] Guid id)
        {
            Guid transactionId = Guid.NewGuid();
            return Ok(await ServiceManager.Interns.GetInternAsync(id, transactionId));
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateIntern([FromBody] InternHttpDto internHttpDto)
        {
            if (!Regex.IsMatch(internHttpDto.User.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return BadRequest("Incorrect email");
            if (!Regex.IsMatch(internHttpDto.User.PhoneNumber, @"^\+7\d{10}$"))
                return BadRequest("Incorrect phone number");
            internHttpDto.Id = Guid.NewGuid();
            Guid transactionId = Guid.NewGuid();
            await ServiceManager.Interns.CreateInternAsync(internHttpDto, transactionId);
            return Ok();
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateIntern([FromBody] InternHttpDto internHttpDto)
        {
            if (!Regex.IsMatch(internHttpDto.User.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return BadRequest("Incorrect email");
            if (!Regex.IsMatch(internHttpDto.User.PhoneNumber, @"^\+7\d{10}$"))
                return BadRequest("Incorrect phone number");
            Guid transactionId = Guid.NewGuid();
            await ServiceManager.Interns.UpdateInternAsync(internHttpDto, transactionId);
            return Ok();
        }
    }
}
