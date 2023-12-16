using System;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using UniversityHelper.GroupService.Business.Commands.Category.Interfaces;
using UniversityHelper.GroupService.Data.Interfaces;
using UniversityHelper.GroupService.Mappers.Patch.Interfaces;
using UniversityHelper.GroupService.Models.Dto.Requests.Category;
using UniversityHelper.GroupService.Validation.Category.Interfaces;
using UniversityHelper.Core.BrokerSupport.AccessValidatorEngine.Interfaces;
using UniversityHelper.Core.Constants;
using UniversityHelper.Core.Helpers.Interfaces;
using UniversityHelper.Core.Responses;
using Microsoft.AspNetCore.JsonPatch;

namespace UniversityHelper.GroupService.Business.Commands.Category;

public class EditCategoryCommand : IEditCategoryCommand
{
  private readonly IAccessValidator _accessValidator;
  private readonly IResponseCreator _responseCreator;
  private readonly ICategoryRepository _repository;
  private readonly IPatchDbCategoryMapper _mapper;
  private readonly IEditCategoryRequestValidator _validator;

  public EditCategoryCommand(
    IAccessValidator accessValidator,
    IResponseCreator responseCreator,
    ICategoryRepository repository,
    IPatchDbCategoryMapper mapper,
    IEditCategoryRequestValidator validator)
  {
    _accessValidator = accessValidator;
    _responseCreator = responseCreator;
    _repository = repository;
    _mapper = mapper;
    _validator = validator;
  }

  public async Task<OperationResultResponse<bool>> ExecuteAsync(Guid categoryId, JsonPatchDocument<EditCategoryRequest> patch)
  {
    if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveUsers))
    {
      return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
    }

    ValidationResult validationResult = await _validator.ValidateAsync((categoryId, patch));

    if (!validationResult.IsValid)
    {
      return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.BadRequest,
        validationResult.Errors.ConvertAll(er => er.ErrorMessage));
    }

    OperationResultResponse<bool> response = new(body: await _repository.EditAsync(categoryId, _mapper.Map(patch)));

    if (!response.Body)
    {
      return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.BadRequest);
    }

    return response;
  }
}
