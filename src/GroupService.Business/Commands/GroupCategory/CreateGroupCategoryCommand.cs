using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using UniversityHelper.GroupService.Business.Commands.GroupCategory.Interfaces;
using UniversityHelper.GroupService.Data.Interfaces;
using UniversityHelper.GroupService.Mappers.Db.Interfaces;
using UniversityHelper.GroupService.Models.Dto.Requests.GroupCategory;
using UniversityHelper.GroupService.Validation.GroupCategory.Interfaces;
using UniversityHelper.Core.BrokerSupport.AccessValidatorEngine.Interfaces;
using UniversityHelper.Core.Constants;
using UniversityHelper.Core.Helpers.Interfaces;
using UniversityHelper.Core.Responses;
using Microsoft.AspNetCore.Http;

namespace UniversityHelper.GroupService.Business.Commands.GroupCategory;

public class CreateGroupCategoryCommand : ICreateGroupCategoryCommand
{
  private readonly IAccessValidator _accessValidator;
  private readonly IGroupCategoryRepository _repository;
  private readonly IDbGroupCategoryMapper _mapper;
  private readonly ICreateGroupCategoryRequestValidator _validator;
  private readonly IResponseCreator _responseCreator;
  private readonly IHttpContextAccessor _contextAccessor;

  public CreateGroupCategoryCommand(
    IAccessValidator accessValidator,
    IGroupCategoryRepository repository,
    IDbGroupCategoryMapper mapper,
    ICreateGroupCategoryRequestValidator validator,
    IResponseCreator responseCreator,
    IHttpContextAccessor contextAccessor)
  {
    _accessValidator = accessValidator;
    _repository = repository;
    _mapper = mapper;
    _validator = validator;
    _responseCreator = responseCreator;
    _contextAccessor = contextAccessor;
  }

  public async Task<OperationResultResponse<bool>> ExecuteAsync(CreateGroupCategoryRequest request)
  {
    if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveUsers))
    {
      return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
    }

    request.CategoriesIds = request.CategoriesIds.Distinct().ToList();

    ValidationResult validationResult = await _validator.ValidateAsync(request);

    if (!validationResult.IsValid)
    {
      return _responseCreator.CreateFailureResponse<bool>(
        HttpStatusCode.BadRequest,
        validationResult.Errors.ConvertAll(er => er.ErrorMessage));
    }

    OperationResultResponse<bool> response = new();
    response.Body = await _repository.CreateAsync(_mapper.Map(request));
    
    if (!response.Body)
    {
      _contextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    }

    _contextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

    return response;
  }
}
