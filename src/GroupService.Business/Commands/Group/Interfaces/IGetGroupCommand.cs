using System;
using System.Threading;
using System.Threading.Tasks;
using UniversityHelper.GroupService.Models.Dto.Requests.Group;
using UniversityHelper.GroupService.Models.Dto.Responses.Group;
using UniversityHelper.Core.Attributes;
using UniversityHelper.Core.Responses;

namespace UniversityHelper.GroupService.Business.Commands.Group.Interfaces;

[AutoInject]
public interface IGetGroupCommand
{
  Task<OperationResultResponse<GroupResponse>> ExecuteAsync(GetGroupFilter filter, CancellationToken ct);
}
