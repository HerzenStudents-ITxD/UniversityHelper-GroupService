using System;
using System.Collections.Generic;
using FluentValidation;
using UniversityHelper.GroupService.Data.Interfaces;
using UniversityHelper.GroupService.Models.Dto.Enums;
using UniversityHelper.GroupService.Models.Dto.Requests.Category;
using UniversityHelper.GroupService.Validation.Category.Interfaces;
using UniversityHelper.Core.Validators;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace UniversityHelper.GroupService.Validation.Category;

public class EditCategoryRequestValidator : ExtendedEditRequestValidator<Guid, EditCategoryRequest>, IEditCategoryRequestValidator
{
  private void HandleInternalPropertyValidation(
    Operation<EditCategoryRequest> requestedOperation,
    ValidationContext<(Guid, JsonPatchDocument<EditCategoryRequest>)> context)
  {
    Context = context;
    RequestedOperation = requestedOperation;

    #region paths

    AddСorrectPaths(
      new List<string>
      {
        nameof(EditCategoryRequest.Color),
        nameof(EditCategoryRequest.Name),
        nameof(EditCategoryRequest.IsActive)
      });

    AddСorrectOperations(nameof(EditCategoryRequest.Color), new List<OperationType> { OperationType.Replace });
    AddСorrectOperations(nameof(EditCategoryRequest.Name), new List<OperationType> { OperationType.Replace });
    AddСorrectOperations(nameof(EditCategoryRequest.IsActive), new List<OperationType> { OperationType.Replace });

    #endregion

    #region Color

    AddFailureForPropertyIf(
      nameof(EditCategoryRequest.Color),
      x => x == OperationType.Replace,
      new()
      {
        { x => Enum.TryParse(x.value?.ToString(), out CategoryColor _), "Incorrect Color value." }
      });

    #endregion

    #region Name

    AddFailureForPropertyIf(
      nameof(EditCategoryRequest.Name),
      x => x == OperationType.Replace,
      new()
      {
        { x => !string.IsNullOrEmpty(x.value?.ToString().Trim()), "Name must not be empty." },
        { x => x.value?.ToString().Length < 21, "Name is too long." }
      }, CascadeMode.Stop);

    #endregion

    #region IsActive

    AddFailureForPropertyIf(
      nameof(EditCategoryRequest.IsActive),
      x => x == OperationType.Replace,
      new()
      {
        { x => bool.TryParse(x.value?.ToString(), out bool _), "Incorrect IsActive value." },
      });

    #endregion
  }

  public EditCategoryRequestValidator(
    ICategoryRepository categoryRepository)
  {
    RuleForEach(x => x.Item2.Operations)
      .Custom(HandleInternalPropertyValidation);

    RuleFor(categoryId => categoryId.Item1)
      .MustAsync((categoryId, _) => categoryRepository.DoExistAllAsync(new List<Guid> { categoryId }))
      .WithMessage("This Id doesn't exist.");
  }
}
