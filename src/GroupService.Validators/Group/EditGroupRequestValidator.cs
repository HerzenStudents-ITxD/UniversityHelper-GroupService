using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using UniversityHelper.GroupService.Data.Interfaces;
using UniversityHelper.GroupService.Models.Db;
using UniversityHelper.GroupService.Models.Dto.Enums;
using UniversityHelper.GroupService.Models.Dto.Requests.Group;
using UniversityHelper.GroupService.Validation.Group.Interfaces;
using UniversityHelper.Core.Validators;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace UniversityHelper.GroupService.Validation.Group;

public class EditGroupRequestValidator : ExtendedEditRequestValidator<Guid, EditGroupRequest>, IEditGroupRequestValidator
{
  private void HandleInternalPropertyValidation(
    Operation<EditGroupRequest> requestedOperation,
    ValidationContext<(Guid, JsonPatchDocument<EditGroupRequest>)> context)
  {
    Context = context;
    RequestedOperation = requestedOperation;

    #region paths

    AddСorrectPaths(
      new List<string>
      {
        nameof(EditGroupRequest.Name),
        nameof(EditGroupRequest.Address),
        nameof(EditGroupRequest.Description),
        nameof(EditGroupRequest.Date),
        nameof(EditGroupRequest.EndDate),
        nameof(EditGroupRequest.Format),
        nameof(EditGroupRequest.Access),
        nameof(EditGroupRequest.IsActive)
      });

    AddСorrectOperations(nameof(EditGroupRequest.Name), new List<OperationType> { OperationType.Replace });
    AddСorrectOperations(nameof(EditGroupRequest.Address), new List<OperationType> { OperationType.Replace });
    AddСorrectOperations(nameof(EditGroupRequest.Description), new List<OperationType> { OperationType.Replace });
    AddСorrectOperations(nameof(EditGroupRequest.Date), new List<OperationType> { OperationType.Replace });
    AddСorrectOperations(nameof(EditGroupRequest.EndDate), new List<OperationType> { OperationType.Replace });
    AddСorrectOperations(nameof(EditGroupRequest.Format), new List<OperationType> { OperationType.Replace });
    AddСorrectOperations(nameof(EditGroupRequest.Access), new List<OperationType> { OperationType.Replace });
    AddСorrectOperations(nameof(EditGroupRequest.IsActive), new List<OperationType> { OperationType.Replace });

    #endregion

    #region Name

    AddFailureForPropertyIf(
      nameof(EditGroupRequest.Name),
      x => x == OperationType.Replace,
      new()
      {
        { x => !string.IsNullOrEmpty(x.value?.ToString().Trim()), "Name must not be empty." },
        { x => x.value?.ToString().Length < 151, "Name is too long." }
      }, CascadeMode.Stop);

    #endregion

    #region Address

    AddFailureForPropertyIf(
      nameof(EditGroupRequest.Address),
      x => x == OperationType.Replace,
      new()
      {
        { x => x.value?.ToString().Length < 401, "Address is too long." }
      }, CascadeMode.Stop);

    #endregion

    #region Description

    AddFailureForPropertyIf(
      nameof(EditGroupRequest.Description),
      x => x == OperationType.Replace,
      new()
      {
        { x => x.value?.ToString().Length < 501, "Description is too long." }
      }, CascadeMode.Stop);

    #endregion

    #region Date

    AddFailureForPropertyIf(
      nameof(EditGroupRequest.Date),
      x => x == OperationType.Replace,
      new()
      {
        { x => string.IsNullOrEmpty(x.value?.ToString().Trim()) || DateTime.TryParse(x.value?.ToString().Trim(), out _), "Incorrect date value." },
        { x => (DateTime.TryParse(x.value.ToString().Trim(), out DateTime date) &&
                date > DateTime.UtcNow), "Date must be later than the date the Group was created." }
      }, CascadeMode.Stop);

    #endregion

    #region EndDate

    AddFailureForPropertyIf(
      nameof(EditGroupRequest.EndDate),
      x => x == OperationType.Replace,
      new()
      {
        { x => x.value is null || (DateTime.TryParse(x.value.ToString().Trim(), out _)), "Incorrect end date value." }
      }, CascadeMode.Stop);

    #endregion

    #region Format

    AddFailureForPropertyIf(
      nameof(EditGroupRequest.Format),
      x => x == OperationType.Replace,
      new()
      {
        { x => Enum.TryParse(x.value?.ToString(), out FormatType _), "Incorrect format value." }
      });

    #endregion

    #region Access

    AddFailureForPropertyIf(
      nameof(EditGroupRequest.Access),
      x => x == OperationType.Replace,
      new()
      {
        { x => Enum.TryParse(x.value?.ToString(), out AccessType _), "Incorrect access value." },
        { x => (Enum.TryParse(x.value.ToString(), out AccessType accessType) &&
                accessType == AccessType.Opened), "Cannot change to a closed Group." }
      });

    #endregion

    #region IsActive

    AddFailureForPropertyIf(
      nameof(EditGroupRequest.IsActive),
      x => x == OperationType.Replace,
      new()
      {
        { x => bool.TryParse(x.value?.ToString(), out bool _), "Incorrect IsActive value." },
      });

    #endregion
  }

  public EditGroupRequestValidator(IGroupRepository repository)
  {
    RuleForEach(x => x.Item2.Operations)
      .Custom(HandleInternalPropertyValidation);

    RuleFor(request => request.Item1)
      .MustAsync((GroupId, _) => repository.DoesExistAsync(GroupId, null))
      .WithMessage("This Group doesn't exist.");

    When(request => !request.Item2.Operations.Any(
      o => o.path.Equals("/" + nameof(EditGroupRequest.IsActive), StringComparison.OrdinalIgnoreCase)), () =>
      {
        RuleFor(request => request.Item1)
        .MustAsync((GroupId, _) => repository.IsGroupCompletedAsync(GroupId))
        .WithMessage("Can not edit completed Group.");
      });

    When(request => request.Item2.Operations.Any(
      o => o.path.Equals("/" + nameof(EditGroupRequest.IsActive), StringComparison.OrdinalIgnoreCase)),
        () =>
        {
          RuleFor(request => request)
          .MustAsync(async (request, _) =>
          {
            bool isActive = (bool.TryParse(request.Item2.Operations.FirstOrDefault(
                x => x.path.Equals("/" + nameof(EditGroupRequest.IsActive), StringComparison.OrdinalIgnoreCase))?.value?.ToString(),
                out bool isActiveValue) && isActiveValue);

            if (isActive)
            {
              DbGroup editedGroup = await repository.GetAsync(request.Item1);

              if ((editedGroup.EndDate is not null && editedGroup.EndDate < DateTime.UtcNow) ||
              editedGroup.Date < DateTime.UtcNow)
              {
                bool endDateOp = request.Item2.Operations.Any(
                  o => o.path.Equals("/" + nameof(EditGroupRequest.EndDate), StringComparison.OrdinalIgnoreCase));

                bool dateOp = request.Item2.Operations.Any(
                  o => o.path.Equals("/" + nameof(EditGroupRequest.Date), StringComparison.OrdinalIgnoreCase));

                return dateOp || endDateOp;
              }
            }

            return true;
          })
          .WithMessage("Must specify a new date.");
        });

    When(request => request.Item2.Operations.Any(
      o => o.path.Equals("/" + nameof(EditGroupRequest.EndDate), StringComparison.OrdinalIgnoreCase)
      || o.path.Equals("/" + nameof(EditGroupRequest.Date), StringComparison.OrdinalIgnoreCase)),
      () =>
      {
        RuleFor(request => request)
          .MustAsync(async (request, _) =>
          {
            DbGroup editedGroup = await repository.GetAsync(request.Item1);

            bool endDateOp = request.Item2.Operations.Any(
              o => o.path.Equals("/" + nameof(EditGroupRequest.EndDate), StringComparison.OrdinalIgnoreCase));

            bool dateOp = request.Item2.Operations.Any(
              o => o.path.Equals("/" + nameof(EditGroupRequest.Date), StringComparison.OrdinalIgnoreCase));

            DateTime? endDateValue;
            DateTime dateValue;

            if (endDateOp)
            {
              endDateValue = DateTime.TryParse(request.Item2.Operations.FirstOrDefault(
                x => x.path.Equals("/" + nameof(EditGroupRequest.EndDate), StringComparison.OrdinalIgnoreCase))?.value?.ToString().Trim(),
                out DateTime endDate)
              ? endDate
              : null;
            }
            else
            {
              endDateValue = editedGroup.EndDate;
            }

            if (dateOp)
            {
              bool isParsedDate = DateTime.TryParse(request.Item2.Operations.FirstOrDefault(
                x => x.path.Equals("/" + nameof(EditGroupRequest.Date), StringComparison.OrdinalIgnoreCase))?.value?.ToString().Trim(),
                out DateTime date);

              dateValue = date;
            }
            else
            {
              dateValue = editedGroup.Date;
            }

            return dateValue < endDateValue || endDateValue is null;
          })
          .WithMessage("The end date must be later than the Group date.");
      });
  }
}
