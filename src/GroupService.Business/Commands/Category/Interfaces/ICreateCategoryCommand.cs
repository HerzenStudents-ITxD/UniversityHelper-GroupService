using System;
using System.Threading.Tasks;
using UniversityHelper.GroupService.Models.Dto.Requests.Category;
using UniversityHelper.Core.Attributes;
using UniversityHelper.Core.Responses;

namespace UniversityHelper.GroupService.Business.Commands.Category.Interfaces;

[AutoInject]
public interface ICreateCategoryCommand
{
  Task<OperationResultResponse<Guid?>> ExecuteAsync(CreateCategoryRequest request);
}

