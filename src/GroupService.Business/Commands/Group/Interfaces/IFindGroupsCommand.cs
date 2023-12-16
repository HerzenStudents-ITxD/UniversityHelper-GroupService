using System.Threading;
using System.Threading.Tasks;
using UniversityHelper.GroupService.Models.Dto.Models;
using UniversityHelper.GroupService.Models.Dto.Requests.Group;
using UniversityHelper.Core.Attributes;
using UniversityHelper.Core.Responses;

namespace UniversityHelper.GroupService.Business.Commands.Group.Interfaces;

[AutoInject]
public interface IFindGroupsCommand
{
  Task<FindResultResponse<GroupInfo>> ExecuteAsync(
    FindGroupsFilter filter,
    CancellationToken ct = default);
}
