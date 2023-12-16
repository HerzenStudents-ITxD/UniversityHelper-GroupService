using System;
using System.Linq;
using FluentValidation;
using UniversityHelper.GroupService.Broker.Requests.Interfaces;
using UniversityHelper.GroupService.Data.Interfaces;
using UniversityHelper.GroupService.Models.Dto.Enums;
using UniversityHelper.GroupService.Models.Dto.Requests.Group;
using UniversityHelper.GroupService.Validation.Category.Interfaces;
using UniversityHelper.GroupService.Validation.Group.Interfaces;
using UniversityHelper.GroupService.Validation.Image.Interfaces;
using UniversityHelper.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace UniversityHelper.GroupService.Validation.Group;

public class CreateGroupRequestValidator : AbstractValidator<CreateGroupRequest>, ICreateGroupRequestValidator
{
  public CreateGroupRequestValidator(
    IUserService userService,
    IHttpContextAccessor contextAccessor,
    ICategoryRepository categoryRepository,
    ICreateCategoryRequestValidator categoryValidator,
    IImageValidator imageValidator)
  {
    RuleFor(ev => ev.Name)
      .MaximumLength(150)
      .WithMessage("Name should not exceed maximum length of 150 symbols");

    When(ev => !string.IsNullOrWhiteSpace(ev.Description), () =>
    {
      RuleFor(ev => ev.Description)
        .MaximumLength(500)
        .WithMessage("Description should not exceed maximum length of 500 symbols");
    });

    RuleFor(ev => ev.Address)
      .MaximumLength(400)
      .WithMessage("Address should not exceed maximum length of 400 symbols");

    RuleFor(ev => ev.Date)
      .Must(d => d > DateTime.UtcNow)
      .WithMessage("The Group date must be later than the date the Group was created");

    When(ev => ev.EndDate.HasValue, () =>
    {
      RuleFor(ev => ev)
        .Must(ev => ev.EndDate > ev.Date)
        .WithMessage("The end date must be later than the Group date.");
    });

    RuleLevelCascadeMode = CascadeMode.Stop;
    RuleFor(ev => ev.Users)
      .NotEmpty()
      .WithMessage("User list must not be empty.")
      .Must((ev, users) =>
        users.Select(user => user.UserId).Contains(contextAccessor.HttpContext.GetUserId()))
      .WithMessage("Group organizer must be in list of participants.")
      .MustAsync(async (users, _) =>
        await userService.CheckUsersExistenceAsync(users.Select(userRequest => userRequest.UserId).ToList()))
      .WithMessage("Some users doesn't exist.");

    When(ev => ev.Access == AccessType.Closed, () =>
    {
      RuleFor(ev => ev.Users)
        .Must(users => users.Count > 1)
        .WithMessage("There should be at least one invited user in closed Group");
    });

    RuleFor(ev => ev.Users)
      .Must((ev, users) => users.All(user => user.NotifyAtUtc is null || (user.NotifyAtUtc > DateTime.UtcNow && user.NotifyAtUtc < ev.Date)))
      .WithMessage("Some notification time is not valid, notification time mustn't be earlier than now or later than date of the Group");

    When(ev => !ev.CategoriesRequests.IsNullOrEmpty(), () =>
    {
      RuleForEach(request => request.CategoriesRequests)
        .SetValidator(categoryValidator);
    });
    
    When(ev => !ev.GroupImages.IsNullOrEmpty(), () =>
    {
      RuleForEach(request => request.GroupImages)
        .SetValidator(imageValidator);
    });

    When(ev => !ev.CategoriesIds.IsNullOrEmpty() || !ev.CategoriesRequests.IsNullOrEmpty(), () =>
    {
      When(ev => !ev.CategoriesIds.IsNullOrEmpty(), () =>
      {
        RuleFor(ev => ev.CategoriesIds)
        .MustAsync((categories, _) => categoryRepository.DoExistAllAsync(categories))
        .WithMessage("Some of categories in the list doesn't exist.");
      });

      RuleFor(ev => ev)
        .Must((ev) =>
        {
          int countCategories = 0;

          if (!ev.CategoriesRequests.IsNullOrEmpty())
          {
            if (!ev.CategoriesIds.IsNullOrEmpty())
            {
              countCategories = ev.CategoriesIds.Count();
            }

            countCategories = countCategories + ev.CategoriesRequests.Count();
          }
          else
          {
            countCategories = ev.CategoriesIds.Count();
          };

          return countCategories < 2;
        })
        .WithMessage("Count of categories to Group must be no more than 1.");
    });
  }
}
