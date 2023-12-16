using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UniversityHelper.Models.Broker.Models.User;
using FluentValidation.Results;
using UniversityHelper.GroupService.Broker.Requests.Interfaces;
using UniversityHelper.GroupService.Business.Commands.GroupUser.Interfaces;
using UniversityHelper.GroupService.Data.Interfaces;
using UniversityHelper.GroupService.Mappers.Db.Interfaces;
using UniversityHelper.GroupService.Models.Db;
using UniversityHelper.GroupService.Models.Dto.Enums;
using UniversityHelper.GroupService.Models.Dto.Requests.GroupUser;
using UniversityHelper.GroupService.Validation.GroupUser.Interfaces;
using UniversityHelper.Core.BrokerSupport.AccessValidatorEngine.Interfaces;
using UniversityHelper.Core.Constants;
using UniversityHelper.Core.Extensions;
using UniversityHelper.Core.Helpers.Interfaces;
using UniversityHelper.Core.Responses;
using Microsoft.AspNetCore.Http;

namespace UniversityHelper.GroupService.Business.Commands.GroupUser;

public class CreateGroupUserCommand : ICreateGroupUserCommand
{
  private readonly IAccessValidator _accessValidator;
  private readonly IGroupUserRepository _repository;
  private readonly IDbGroupUserMapper _mapper;
  private readonly ICreateGroupUserRequestValidator _validator;
  private readonly IResponseCreator _responseCreator;
  private readonly IHttpContextAccessor _contextAccessor;
  private readonly IGroupRepository _GroupRepository;
  private readonly IEmailService _emailService;
  private readonly IUserService _userService;

  public CreateGroupUserCommand(
    IAccessValidator accessValidator,
    IGroupUserRepository repository,
    IDbGroupUserMapper mapper,
    ICreateGroupUserRequestValidator validator,
    IResponseCreator responseCreator,
    IHttpContextAccessor contextAccessor,
    IGroupRepository GroupRepository,
    IEmailService emailService,
    IUserService userService)
  {
    _accessValidator = accessValidator;
    _repository = repository;
    _mapper = mapper;
    _validator = validator;
    _responseCreator = responseCreator;
    _contextAccessor = contextAccessor;
    _GroupRepository = GroupRepository;
    _emailService = emailService;
    _userService = userService;
  }

  private async Task SendInviteEmailsAsync(List<Guid> userIds, string GroupName)
  {
    List<UserData> usersData = await _userService.GetUsersDataAsync(userIds);

    if (usersData is null || !usersData.Any())
    {
      return;
    }

    foreach (UserData user in usersData)
    {
      await _emailService.SendAsync(
        user.Email,
        "Invite to Group",
        $"You have been invited to Group {GroupName}");
    }
  }

  public async Task<OperationResultResponse<bool>> ExecuteAsync(CreateGroupUserRequest request)
  {
    Guid senderId = _contextAccessor.HttpContext.GetUserId();
    DbGroup dbGroup = await _GroupRepository.GetAsync(request.GroupId);

    if (dbGroup is null)
    {
      return _responseCreator.CreateFailureResponse<bool>(
        HttpStatusCode.NotFound,
        new List<string> { "This Group doesn't exist." });
    }

    bool userHasRight = await _accessValidator.HasRightsAsync(senderId, Rights.AddEditRemoveUsers);

    if ((dbGroup.Access == AccessType.Closed && !userHasRight) ||
        (dbGroup.Access == AccessType.Opened &&
          !(!userHasRight && request.Users.Count == 1 && request.Users.Exists(x => x.UserId == senderId) || userHasRight)))
    {
      return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
    }

    OperationResultResponse<bool> response = new();

    if (request.Users.Distinct().Count() != request.Users.Count())
    {
      response.Errors = new List<string>() { "Some duplicate users have been removed from the list." };
      request.Users = request.Users.Distinct().ToList();
    }

    ValidationResult validationResult = await _validator.ValidateAsync(request);

    if (!validationResult.IsValid)
    {
      return _responseCreator.CreateFailureResponse<bool>(
        HttpStatusCode.BadRequest,
        validationResult.Errors.ConvertAll(er => er.ErrorMessage));
    }

    List<DbGroupUser> dbGroupUsers = _mapper.Map(request, dbGroup.Access, senderId);
    response.Body = await _repository.CreateAsync(dbGroupUsers);
    _contextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

    await SendInviteEmailsAsync(dbGroupUsers.Where(x => x.Status == GroupUserStatus.Invited).Select(x => x.UserId).ToList(), dbGroup.Name);

    return response;
  }
}
