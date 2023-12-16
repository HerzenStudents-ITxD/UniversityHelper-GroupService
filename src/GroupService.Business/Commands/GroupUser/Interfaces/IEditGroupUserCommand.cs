using System;
using System.Threading.Tasks;
using UniversityHelper.GroupService.Models.Dto.Requests.GroupUser;
using UniversityHelper.Core.Attributes;
using UniversityHelper.Core.Responses;
using Microsoft.AspNetCore.JsonPatch;

namespace UniversityHelper.GroupService.Business.Commands.GroupUser.Interfaces;

[AutoInject]
public interface IEditGroupUserCommand
{
  Task<OperationResultResponse<bool>> ExecuteAsync(Guid GroupUserId, JsonPatchDocument<EditGroupUserRequest> patch);
}
