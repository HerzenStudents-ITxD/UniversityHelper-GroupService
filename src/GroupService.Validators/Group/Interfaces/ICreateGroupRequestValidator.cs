using FluentValidation;
using UniversityHelper.GroupService.Models.Dto.Requests.Group;
using UniversityHelper.Core.Attributes;

namespace UniversityHelper.GroupService.Validation.Group.Interfaces;

[AutoInject]
public interface ICreateGroupRequestValidator : IValidator<CreateGroupRequest>
{
}
