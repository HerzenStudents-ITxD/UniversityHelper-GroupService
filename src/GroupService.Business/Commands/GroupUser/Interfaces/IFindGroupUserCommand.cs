using UniversityHelper.GroupService.Models.Dto.Models;
using UniversityHelper.GroupService.Models.Dto.Requests.GroupUser.Filter;
using UniversityHelper.Core.Responses;
using System.Threading.Tasks;
using System.Threading;
using System;
using UniversityHelper.Core.Attributes;

namespace UniversityHelper.GroupService.Business.Commands.GroupUser.Interfaces;

[AutoInject]
public interface IFindGroupUserCommand
{
  Task<FindResultResponse<GroupUserInfo>> ExecuteAsync(
    Guid GroupId,
    FindGroupUsersFilter filter,
    CancellationToken cancellationToken = default);
}
