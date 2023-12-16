using System;
using System.Threading.Tasks;
using UniversityHelper.GroupService.Models.Dto.Requests.GroupCategory;
using UniversityHelper.Core.Attributes;
using UniversityHelper.Core.Responses;

namespace UniversityHelper.GroupService.Business.Commands.GroupCategory.Interfaces;

[AutoInject]
public interface ICreateGroupCategoryCommand
{
  Task<OperationResultResponse<bool>> ExecuteAsync(CreateGroupCategoryRequest request);
}
