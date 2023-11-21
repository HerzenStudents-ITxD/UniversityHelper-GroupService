using UniversityHelper.GroupService.Business.Commands.Feedback.Interfaces;
using UniversityHelper.GroupService.Models.Dto;
using UniversityHelper.GroupService.Models.Dto.Models;
using UniversityHelper.GroupService.Models.Dto.Requests;
using UniversityHelper.GroupService.Models.Dto.Requests.Filter;
using UniversityHelper.Core.Responses;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace UniversityHelper.GroupService.Controllers;

[Route("[controller]")]
[ApiController]
public class GroupController : ControllerBase
{
  [HttpGet("get")]
  public async Task<OperationResultResponse<GroupResponse>> GetAsync(
    [FromQuery] Guid groupId,
    [FromServices] IGetGroupCommand command)
  {
    return await command.ExecuteAsync(groupId);
  }

  [HttpGet("find")]
  public async Task<FindResultResponse<GroupInfo>> FindAsync(
    [FromQuery] FindGroupsFilter filter,
    [FromServices] IFindGroupsCommand command)
  {
    return await command.ExecuteAsync(filter);
  }

  [HttpPost("create")]
  public async Task<OperationResultResponse<Guid?>> CreateAsync(
    [FromBody] CreateGroupRequest request,
    [FromServices] ICreateGroupCommand command)
  {
    return await command.ExecuteAsync(request);
  }

  [HttpPut("edit")]
  public async Task<OperationResultResponse<bool>> EditGroupAsync(
    [FromBody] EditGroupRequest request,
    [FromServices] IEditGroupCommand command)
  {
    return await command.ExecuteAsync(request);
  }
}
