using System;
using FluentValidation;
using UniversityHelper.GroupService.Models.Dto.Requests.Category;
using UniversityHelper.Core.Attributes;
using Microsoft.AspNetCore.JsonPatch;

namespace UniversityHelper.GroupService.Validation.Category.Interfaces;

[AutoInject]
public interface IEditCategoryRequestValidator : IValidator<(Guid, JsonPatchDocument<EditCategoryRequest>)>
{
}
