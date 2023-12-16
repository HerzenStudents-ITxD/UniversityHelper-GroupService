using FluentValidation;
using UniversityHelper.GroupService.Models.Dto.Requests.GroupCategory;
using UniversityHelper.Core.Attributes;

namespace UniversityHelper.GroupService.Validation.GroupCategory.Interfaces;

[AutoInject]
public interface IRemoveGroupCategoryRequestValidator : IValidator<RemoveGroupCategoryRequest>
{
}
