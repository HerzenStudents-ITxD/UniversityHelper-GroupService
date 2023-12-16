using System;
using FluentValidation;
using UniversityHelper.GroupService.Models.Dto.Requests.Group;
using UniversityHelper.Core.Attributes;
using Microsoft.AspNetCore.JsonPatch;

namespace UniversityHelper.GroupService.Validation.Group.Interfaces;

[AutoInject]
public interface IEditGroupRequestValidator : IValidator<(Guid, JsonPatchDocument<EditGroupRequest>)>
{
}
