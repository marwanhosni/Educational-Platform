using Educational_Platform.Core.Dtos.Groups;
using Educational_Platform.Core.Entities;
using Educational_Platform.Core.EnumGrade;
using Educational_Platform.Core.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Educational_Platform.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = $"{Roles.Admin},{Roles.Secretary}")]
public class GroupsController : ControllerBase
{
    private readonly IGroupService _groupService;

    public GroupsController(IGroupService groupService)
    {
        _groupService = groupService;
    }

    
    [HttpGet]
    public async Task<IActionResult> GetAllGroups([FromQuery] Grade? grade, [FromQuery] bool? activeOnly)
    {
        var result = await _groupService.GetAllGroupsAsync(grade, activeOnly);
        return Ok(result);
    }

    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetGroupById(int id)
    {
        var result = await _groupService.GetGroupByIdAsync(id);
        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

   
    [HttpPost]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _groupService.CreateGroupAsync(dto);
        return CreatedAtAction(nameof(GetGroupById), new { id = result.Data!.Id }, result);
    }

  
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateGroup(int id, [FromBody] UpdateGroupDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _groupService.UpdateGroupAsync(id, dto);
        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

   
    [HttpPatch("{id:int}/toggle-status")]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var result = await _groupService.ToggleGroupStatusAsync(id);
        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    
    [HttpPost("move-student")]
    public async Task<IActionResult> MoveStudent([FromBody] MoveStudentGroupDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var isAdmin = User.IsInRole(Roles.Admin);
        var result = await _groupService.MoveStudentAsync(dto, isAdmin);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}