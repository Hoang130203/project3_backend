using AuthService.Application.Filters;
using AuthService.Domain.Enums;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Models;
using Microsoft.AspNetCore.Mvc;
using AuthService.Application.CQRS.Commands.Groups.CreateGroup;
using AuthService.Application.DTOs.Group;
using AuthService.Application.CQRS.Commands.Groups.UpdateGroup;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GroupController : ControllerBase
    {
        private readonly ISender _mediator;

        public GroupController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<CreateGroupResult>> CreateGroup([FromBody] CreateGroupRequest request)
        {
            try
            {
                // Get user ID from token (added by gateway)
                var userIdString = HttpContext.Request.Headers["X-UserId"].FirstOrDefault();
                var userTypeString = HttpContext.Request.Headers["X-UserType"].FirstOrDefault();
                
                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                {
                    return Unauthorized();
                }

                var command = new CreateGroupCommand
                {
                    Name = request.Name,
                    Description = request.Description,
                    Visibility = request.Visibility,
                    CreatorId = userId
                };

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPut("{groupId}")]
        public async Task<ActionResult<UpdateGroupResult>> UpdateGroup(
            Guid groupId,
            [FromBody] UpdateGroupRequest request)
        {
            try
            {
                var userIdString = HttpContext.Request.Headers["X-UserId"].FirstOrDefault();
                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                {
                    return Unauthorized();
                }

                var command = new UpdateGroupCommand
                {
                    Name = request.Name,
                    Description= request.Description,
                    Visibility= request.Visibility ? GroupVisibility.Public : GroupVisibility.Private,
                    GroupId = groupId,
                    UserId= userId
                };

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An error occurred while updating the group" });
            }
        }
    }

}
