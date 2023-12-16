using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UniversityHelper.Models.Broker.Models.Image;
using UniversityHelper.Models.Broker.Models.User;
using FluentValidation.Results;
using UniversityHelper.GroupService.Broker.Publishes.Interfaces;
using UniversityHelper.GroupService.Broker.Requests.Interfaces;
using UniversityHelper.GroupService.Business.Commands.Group.Interfaces;
using UniversityHelper.GroupService.Data.Interfaces;
using UniversityHelper.GroupService.Mappers.Db.Interfaces;
using UniversityHelper.GroupService.Models.Db;
using UniversityHelper.GroupService.Models.Dto.Requests.Group;
using UniversityHelper.GroupService.Models.Dto.Requests.GroupCategory;
using UniversityHelper.GroupService.Models.Dto.Requests.GroupUser;
using UniversityHelper.GroupService.Validation.Group.Interfaces;
using UniversityHelper.Core.BrokerSupport.AccessValidatorEngine.Interfaces;
using UniversityHelper.Core.Constants;
using UniversityHelper.Core.Extensions;
using UniversityHelper.Core.Helpers.Interfaces;
using UniversityHelper.Core.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace UniversityHelper.GroupService.Business.Commands.Group;

public class CreateGroupCommand : ICreateGroupCommand
{
  private readonly IGroupRepository _GroupRepository;
  private readonly ICategoryRepository _categoryRepository;
  private readonly IGroupCategoryRepository _GroupCategoryRepository;
  private readonly ICreateGroupRequestValidator _validator;
  private readonly IDbGroupMapper _GroupMapper;
  private readonly IDbCategoryMapper _categoryMapper;
  private readonly IDbGroupCategoryMapper _GroupCategoryMapper;
  private readonly IAccessValidator _accessValidator;
  private readonly IResponseCreator _responseCreator;
  private readonly IHttpContextAccessor _contextAccessor;
  private readonly IEmailService _emailService;
  private readonly IUserService _userService;
  private readonly IImageService _imageService;
  private readonly IPublish _publish;

  private const int ResizeMaxValue = 1000;
  private const int ConditionalWidth = 4;
  private const int ConditionalHeight = 3;

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

  public CreateGroupCommand(
    IGroupRepository repository,
    ICategoryRepository categoryRepository,
    IGroupCategoryRepository GroupCategoryRepository,
    ICreateGroupRequestValidator validator,
    IDbGroupMapper GroupMapper,
    IDbCategoryMapper categoryMapper,
    IDbGroupCategoryMapper GroupCategoryMapper,
    IAccessValidator accessValidator,
    IResponseCreator responseCreator,
    IHttpContextAccessor contextAccessor,
    IUserService userService,
    IEmailService emailService,
    IImageService imageService,
    IPublish publish)
  {
    _GroupRepository = repository;
    _GroupMapper = GroupMapper;
    _validator = validator;
    _accessValidator = accessValidator;
    _responseCreator = responseCreator;
    _contextAccessor = contextAccessor;
    _userService = userService;
    _emailService = emailService;
    _imageService = imageService;
    _categoryMapper = categoryMapper;
    _categoryRepository = categoryRepository;
    _GroupCategoryMapper = GroupCategoryMapper;
    _GroupCategoryRepository = GroupCategoryRepository;
    _publish = publish;
  }

  public async Task<OperationResultResponse<Guid?>> ExecuteAsync(CreateGroupRequest request)
  {
    Guid senderId = _contextAccessor.HttpContext.GetUserId();

    if (!await _accessValidator.HasRightsAsync(senderId, Rights.AddEditRemoveUsers))
    {
      return _responseCreator.CreateFailureResponse<Guid?>(HttpStatusCode.Forbidden);
    }

    request.Users.Add(new UserRequest { UserId = senderId });
    request.Users = request.Users.Distinct().ToList();
    request.CategoriesIds = request.CategoriesIds?.Distinct().ToList();

    ValidationResult validationResult = await _validator.ValidateAsync(request);
    if (!validationResult.IsValid)
    {
      return _responseCreator.CreateFailureResponse<Guid?>(
        HttpStatusCode.BadRequest,
        validationResult.Errors.ConvertAll(er => er.ErrorMessage));
    }

    OperationResultResponse<Guid?> response = new();

    List<Guid> imagesIds = null;
    if (request.GroupImages is not null && request.GroupImages.Any())
    {
      imagesIds = await _imageService.CreateImagesAsync(
        request.GroupImages,
        new ResizeParameters(
          maxResizeValue: ResizeMaxValue,
          maxSizeCompress: null,
          previewParameters: new PreviewParameters(
            conditionalWidth: ConditionalWidth,
            conditionalHeight: ConditionalHeight,
            resizeMaxValue: null,
            maxSizeCompress: null)),
        response.Errors);
    }

    DbGroup dbGroup = _GroupMapper.Map(request, senderId, imagesIds);

    response.Body = await _GroupRepository.CreateAsync(dbGroup);

    List<DbGroupCategory> dbCategories = new();

    if (response.Body is not null)
    {
      await SendInviteEmailsAsync(dbGroup.Users.Select(x => x.UserId).ToList(), dbGroup.Name);

      if (!request.CategoriesRequests.IsNullOrEmpty())
      {
        dbCategories.AddRange(request.CategoriesRequests.ConvertAll(_categoryMapper.Map));
        
        await _categoryRepository.CreateAsync(dbCategories);

        List<DbGroupTeam> GroupCategories = _GroupCategoryMapper.Map(
          new CreateGroupCategoryRequest {
            GroupId = response.Body.Value,
            CategoriesIds = dbCategories.Select(c => c.Id).ToList() })
          .ToList();

        await _GroupCategoryRepository.CreateAsync(GroupCategories);
      }

      _contextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;
    }
    else
    {
      await _publish.RemoveImagesAsync(imagesIds);

      _contextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    }

    return response;
  }
}
