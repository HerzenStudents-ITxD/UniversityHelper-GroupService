using System;
using System.Threading.Tasks;
using UniversityHelper.GroupService.Models.Dto.Requests.Group;
using UniversityHelper.Core.Attributes;
using UniversityHelper.Core.Responses;

namespace UniversityHelper.GroupService.Business.Commands.Group.Interfaces;

[AutoInject]
public interface ICreateGroupCommand
{
  Task<OperationResultResponse<Guid?>> ExecuteAsync(CreateGroupRequest request);
}
