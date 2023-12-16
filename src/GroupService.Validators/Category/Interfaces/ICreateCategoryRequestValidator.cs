using FluentValidation;
using UniversityHelper.GroupService.Models.Dto.Requests.Category;
using UniversityHelper.Core.Attributes;

namespace UniversityHelper.GroupService.Validation.Category.Interfaces;

[AutoInject]
public interface ICreateCategoryRequestValidator : IValidator<CreateCategoryRequest>
{
}

