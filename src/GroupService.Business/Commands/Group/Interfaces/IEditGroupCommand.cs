using System;
using System.Threading.Tasks;
using UniversityHelper.GroupService.Models.Dto.Requests.Group;
using UniversityHelper.Core.Attributes;
using UniversityHelper.Core.Responses;
using Microsoft.AspNetCore.JsonPatch;

namespace UniversityHelper.GroupService.Business.Commands.Group.Interfaces;

[AutoInject]
public interface IEditGroupCommand
{
  Task<OperationResultResponse<bool>> ExecuteAsync(Guid GroupId, JsonPatchDocument<EditGroupRequest> patch);
}
