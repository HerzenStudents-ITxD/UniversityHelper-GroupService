using System.Threading.Tasks;
using UniversityHelper.GroupService.Models.Dto.Requests.GroupUser;
using UniversityHelper.Core.Attributes;
using UniversityHelper.Core.Responses;

namespace UniversityHelper.GroupService.Business.Commands.GroupUser.Interfaces;

[AutoInject]
public interface ICreateGroupUserCommand
{
  Task<OperationResultResponse<bool>> ExecuteAsync(CreateGroupUserRequest request);
}
