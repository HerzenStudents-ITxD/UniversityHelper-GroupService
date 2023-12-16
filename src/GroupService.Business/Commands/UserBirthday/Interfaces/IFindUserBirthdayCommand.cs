using System.Threading;
using System.Threading.Tasks;
using UniversityHelper.GroupService.Models.Dto.Models;
using UniversityHelper.GroupService.Models.Dto.Requests.UserBirthday;
using UniversityHelper.Core.Attributes;
using UniversityHelper.Core.Responses;

namespace UniversityHelper.GroupService.Business.Commands.UserBirthday.Interfaces;

[AutoInject]
public interface IFindUserBirthdayCommand
{
  Task<FindResultResponse<UserBirthdayInfo>> ExecuteAsync(
    FindUsersBirthdaysFilter filter,
    CancellationToken cancellationToken = default);
}
