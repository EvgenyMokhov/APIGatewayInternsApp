using Rabbit;
using BusinessLogic;
using InternsTestModels.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace APIGatewayInterns.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DirectionsController : ControllerBase
    {
        private readonly ServiceManager ServiceManager;
        private readonly ILogger<DirectionsController> Logger;
        public DirectionsController(RabbitManager rabbit, ILogger<DirectionsController> logger) 
        { 
            ServiceManager = new(rabbit);  
            Logger = logger; 
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllDirections()
        {
            Guid transactionId = Guid.NewGuid();
            return Ok(await ServiceManager.Directions.GetDirectionsAsync(transactionId));
        }

        [HttpGet("low_detail_all")]
        public async Task<IActionResult> GetLowDetailDirections()
        {
            Guid transactionId = Guid.NewGuid();
            return Ok(await ServiceManager.Directions.GetDirectionsLowDetailAsync(transactionId));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDirection([FromRoute] Guid id)
        {
            Guid transactionId = Guid.NewGuid();
            return Ok(await ServiceManager.Directions.GetDirectionAsync(id, transactionId));
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateDirection([FromBody] DirectionHttpDto direciton)
        {
            Guid transactionId = Guid.NewGuid();
            await ServiceManager.Directions.CreateDirectionAsync(direciton, transactionId);
            return Ok();
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateDirection([FromBody] DirectionHttpDto direction)
        {
            if (!direction.IsActive && direction.Interns.Count != 0)
                return BadRequest("Cannot delete direction with active interns");
            Guid transactionId = Guid.NewGuid();
            await ServiceManager.Directions.UpdateDirectionAsync(direction, transactionId);
            return Ok();
        } 
    }
}
