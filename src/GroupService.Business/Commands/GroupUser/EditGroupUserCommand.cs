using System;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using UniversityHelper.GroupService.Business.Commands.GroupUser.Interfaces;
using UniversityHelper.GroupService.Data.Interfaces;
using UniversityHelper.GroupService.Mappers.Patch.Interfaces;
using UniversityHelper.GroupService.Models.Dto.Requests.GroupUser;
using UniversityHelper.GroupService.Validation.GroupUser.Interfaces;
using UniversityHelper.Core.BrokerSupport.AccessValidatorEngine.Interfaces;
using UniversityHelper.Core.Constants;
using UniversityHelper.Core.Extensions;
using UniversityHelper.Core.Helpers.Interfaces;
using UniversityHelper.Core.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;

namespace UniversityHelper.GroupService.Business.Commands.GroupUser;

public class EditGroupUserCommand : IEditGroupUserCommand
{
  private readonly IEditGroupUserRequestValidator _validator;
  private readonly IGroupUserRepository _GroupUserRepository;
  private readonly IPatchDbGroupUserMapper _mapper;
  private readonly IResponseCreator _responseCreator;
  private readonly IAccessValidator _accessValidator;
  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly IHttpContextAccessor _contextAccessor;

  public EditGroupUserCommand(
    IEditGroupUserRequestValidator validator,
    IGroupUserRepository GroupUserRepository,
    IPatchDbGroupUserMapper mapper,
    IResponseCreator responseCreator,
    IAccessValidator accessValidator,
    IHttpContextAccessor httpContextAccessor,
    IHttpContextAccessor contextAccessor)
  {
    _validator = validator;
    _GroupUserRepository = GroupUserRepository;
    _mapper = mapper;
    _responseCreator = responseCreator;
    _accessValidator = accessValidator;
    _httpContextAccessor = httpContextAccessor;
    _contextAccessor = contextAccessor;
  }

  public async Task<OperationResultResponse<bool>> ExecuteAsync(
    Guid GroupUserId,
    JsonPatchDocument<EditGroupUserRequest> patch)
  {
    Guid senderId = _httpContextAccessor.HttpContext.GetUserId();

    if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveUsers) &&
        (await _GroupUserRepository.GetAsync(GroupUserId))?.UserId != senderId)
    {
      return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
    }

    ValidationResult validationResult = await _validator.ValidateAsync((GroupUserId, patch));

    if (!validationResult.IsValid)
    {
      return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.BadRequest,
        validationResult.Errors.ConvertAll(er => er.ErrorMessage));
    }

    OperationResultResponse<bool> response = new(body : await _GroupUserRepository.EditAsync(GroupUserId, _mapper.Map(patch)));

    if (!response.Body)
    {
      _contextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    }

    return response;
  }
}
