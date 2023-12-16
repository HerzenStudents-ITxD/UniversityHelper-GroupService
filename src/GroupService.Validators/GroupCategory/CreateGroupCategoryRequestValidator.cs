using FluentValidation;
using UniversityHelper.GroupService.Data.Interfaces;
using UniversityHelper.GroupService.Models.Dto.Requests.GroupCategory;
using UniversityHelper.GroupService.Validation.GroupCategory.Interfaces;

namespace UniversityHelper.GroupService.Validation.GroupCategory;

public class CreateGroupCategoryRequestValidator : AbstractValidator<CreateGroupCategoryRequest>, ICreateGroupCategoryRequestValidator
{
  public CreateGroupCategoryRequestValidator(
    IGroupRepository GroupRepository,
    ICategoryRepository categoryRepository,
    IGroupCategoryRepository GroupCategoryRepository)
  {
    RuleFor(x => x.GroupId)
      .MustAsync((GroupId, _) => GroupRepository.DoesExistAsync(GroupId, true))
      .WithMessage("This Group doesn't exist.");

    RuleFor(x => x.CategoriesIds)
      .MustAsync((categories, _) => categoryRepository.DoExistAllAsync(categories))
      .WithMessage("Some of categories in the list doesn't exist.");

    RuleFor(x => x)
      .Must(ec => !GroupCategoryRepository.DoesExistAsync(ec.GroupId, ec.CategoriesIds))
      .WithMessage("This Group already belongs to this category.")
      .MustAsync(async (ec, _) => await GroupCategoryRepository.CountCategoriesAsync(ec.GroupId) + ec.CategoriesIds.Count < 2)
      .WithMessage("This Group already has 5 categories.");
  }
}
