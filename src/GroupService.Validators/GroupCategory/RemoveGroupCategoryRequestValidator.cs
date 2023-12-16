using FluentValidation;
using UniversityHelper.GroupService.Data.Interfaces;
using UniversityHelper.GroupService.Models.Dto.Requests.GroupCategory;
using UniversityHelper.GroupService.Validation.GroupCategory.Interfaces;

namespace UniversityHelper.GroupService.Validation.GroupCategory;

public class RemoveGroupCategoryRequestValidator : AbstractValidator<RemoveGroupCategoryRequest>, IRemoveGroupCategoryRequestValidator
{
  public RemoveGroupCategoryRequestValidator(
    IGroupRepository GroupRepository,
    ICategoryRepository categoryRepository,
    IGroupCategoryRepository GroupCategoryRepository)
  {
    RuleFor(request => request.GroupId)
      .MustAsync((x, _) => GroupRepository.DoesExistAsync(x, true))
      .WithMessage("This Group doesn't exist.");

    RuleFor(request => request.CategoriesIds)
      .NotEmpty()
      .WithMessage("There are no categories to delete.")
      .MustAsync((categories, _) => categoryRepository.DoExistAllAsync(categories))
      .WithMessage("Some categories doesn't exist.");

    RuleFor(request => request)
      .Must(x => GroupCategoryRepository.DoesExistAsync(x.GroupId, x.CategoriesIds))
      .WithMessage("This Group doesn't belong to all categories in the list.");
  }
}
