using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using UniversityHelper.GroupService.Business.Commands.GroupCategory.Interfaces;
using UniversityHelper.GroupService.Data.Interfaces;
using UniversityHelper.GroupService.Models.Dto.Requests.GroupCategory;
using UniversityHelper.GroupService.Validation.GroupCategory.Interfaces;
using UniversityHelper.Core.BrokerSupport.AccessValidatorEngine.Interfaces;
using UniversityHelper.Core.Constants;
using UniversityHelper.Core.Helpers.Interfaces;
using UniversityHelper.Core.Responses;
using Microsoft.AspNetCore.Http;

namespace UniversityHelper.GroupService.Business.Commands.GroupCategory;

public class RemoveGroupCategoryCommand : IRemoveGroupCategoryCommand
{
  private readonly IHttpContextAccessor _contextAccessor;
  private readonly IAccessValidator _accessValidator;
  private readonly IGroupCategoryRepository _repository;
  private readonly IRemoveGroupCategoryRequestValidator _validator;
  private readonly IResponseCreator _responseCreator;

  public RemoveGroupCategoryCommand(
    IHttpContextAccessor contextAccessor,
    IAccessValidator accessValidator,
    IGroupCategoryRepository repository,
    IRemoveGroupCategoryRequestValidator validator,
    IResponseCreator responseCreator)
  {
    _contextAccessor = contextAccessor;
    _accessValidator = accessValidator;
    _repository = repository;
    _validator = validator;
    _responseCreator = responseCreator;
  }

  public async Task<OperationResultResponse<bool>> ExecuteAsync(RemoveGroupCategoryRequest request)
  {
    if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveUsers))
    {
      return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
    }

    ValidationResult validationResult = await _validator.ValidateAsync(request);

    if (!validationResult.IsValid)
    {
      return _responseCreator.CreateFailureResponse<bool>(
        HttpStatusCode.BadRequest,
        validationResult.Errors.ConvertAll(er => er.ErrorMessage));
    }

    OperationResultResponse<bool> response = new()
    {
      Body = await _repository.RemoveAsync(request.GroupId, request.CategoriesIds)
    };

    if (!response.Body)
    {
      return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.BadRequest);
    }

    _contextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;

    return response;
  }
}
