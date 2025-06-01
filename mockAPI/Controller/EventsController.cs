using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using mockAPI.Models;
using mockAPI.Services;

namespace mockAPI.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {

        private readonly ILogger<EventsController> _logger;
        private readonly IEventRegistractionsService _eventReadService;

        public EventsController(IEventRegistractionsService eventReadService, ILogger<EventsController> logger)
        {
            _eventReadService = eventReadService ?? throw new ArgumentNullException(nameof(eventReadService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        [HttpGet("{id}")]
        [ProducesResponseType(statusCode: 200, Type = typeof(IEnumerable<EventRegistrationDTO>))]
        [ProducesResponseType(statusCode: 401, Type = typeof(ProblemDetails))]
        [ProducesResponseType(statusCode: 404, Type = typeof(ProblemDetails))]
        [ProducesResponseType(statusCode: 500, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return Problem(
                        detail: "Event registration ID is required.",
                        title: "Bad Request",
                        statusCode: StatusCodes.Status400BadRequest,
                        instance: HttpContext.TraceIdentifier);
                }

                var eventRegistration = await _eventReadService.GetByIdAsync(int.Parse(id));

                if (eventRegistration == null)
                {
                    return NotFound(new ProblemDetails
                    {
                        Title = "Not Found",
                        Detail = $"Event registration with ID '{id}' not found.",
                        Status = StatusCodes.Status404NotFound,
                        Instance = HttpContext.TraceIdentifier
                    });
                }

                return Ok(eventRegistration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the event registration.");
                return Problem(
                    detail: "An error occurred while processing your request.",
                    title: "Internal Server Error",
                    statusCode: StatusCodes.Status500InternalServerError,
                    instance: HttpContext.TraceIdentifier);
            }

        }



        [HttpPost]
        [ProducesResponseType(statusCode: 200, Type = typeof(string))]
        [ProducesResponseType(statusCode: 400, Type = typeof(ProblemDetails))]
        [ProducesResponseType(statusCode: 401, Type = typeof(ProblemDetails))]
        [ProducesResponseType(statusCode: 404, Type = typeof(ProblemDetails))]
        [ProducesResponseType(statusCode: 500, Type = typeof(ProblemDetails))]
        public Task<IActionResult> CreateEvent([FromBody] EventRegistrationDTO eventRegistrationDTO)
        {
            if (eventRegistrationDTO == null)
            {
                return Task.FromResult<IActionResult>(Problem(
                    detail: "Event registration data is required.",
                    title: "Bad Request",
                    statusCode: StatusCodes.Status400BadRequest,
                    instance: HttpContext.TraceIdentifier));
            }

            try
            {
                if(!ModelState.IsValid)
                {
                    return Task.FromResult<IActionResult>(BadRequest(ModelState));
                }

                _eventReadService.CreateEventRegistrationAsync(eventRegistrationDTO);
               
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the event registration.");
                return Task.FromResult<IActionResult>(Problem(
                    detail: "An error occurred while processing your request.",
                    title: "Internal Server Error",
                    statusCode: StatusCodes.Status500InternalServerError,
                    instance: HttpContext.TraceIdentifier));
            }


             return Task.FromResult<IActionResult>(Ok("Event registration created successfully."));
        }


        
    }
}