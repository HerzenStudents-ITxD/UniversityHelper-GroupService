using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using UniversityHelper.Models.Broker.Models.User;
using UniversityHelper.GroupService.Broker.Requests.Interfaces;
using UniversityHelper.GroupService.Business.Commands.Group.Interfaces;
using UniversityHelper.GroupService.Data.Interfaces;
using UniversityHelper.GroupService.Mappers.Models.Interface;
using UniversityHelper.GroupService.Models.Db;
using UniversityHelper.GroupService.Models.Dto.Models;
using UniversityHelper.GroupService.Models.Dto.Requests.Group;
using UniversityHelper.GroupService.Models.Dto.Responses.Group;
using UniversityHelper.Core.Helpers.Interfaces;
using UniversityHelper.Core.Responses;
using UniversityHelper.Models.Broker.Models.File;

namespace UniversityHelper.GroupService.Business.Commands.Group;

public class GetGroupCommand : IGetGroupCommand
{
  private readonly IGroupRepository _repository;
  private readonly IGroupResponseMapper _mapper;
  private readonly IResponseCreator _responseCreator;
  private readonly IImageService _imageService;
  private readonly IUserService _userService;
  private readonly IFileService _fileService;
  private readonly ICommentInfoMapper _commentInfoMapper;

  public GetGroupCommand(
    IGroupRepository repository,
    IGroupResponseMapper mapper,
    IResponseCreator responseCreator,
    IImageService imageService,
    IUserService userService,
    IFileService fileService,
    ICommentInfoMapper commentInfoMapper)
  {
    _repository = repository;
    _mapper = mapper;
    _responseCreator = responseCreator;
    _imageService = imageService;
    _userService = userService;
    _fileService = fileService;
    _commentInfoMapper = commentInfoMapper;
  }

  public async Task<OperationResultResponse<GroupResponse>> ExecuteAsync(GetGroupFilter filter, CancellationToken ct)
  {
    DbGroup dbGroup = await _repository.GetAsync(filter.GroupId, filter);

    if (dbGroup is null)
    {
      return _responseCreator.CreateFailureResponse<GroupResponse>(HttpStatusCode.NotFound);
    }

    Task<List<FileCharacteristicsData>> filesTask = filter.IncludeFiles
      ? _fileService.GetFilesCharacteristicsAsync(dbGroup.Files.Select(f => f.FileId).ToList())
      : Task.FromResult(null as List<FileCharacteristicsData>);

    Task<List<UserData>> usersTask = filter.IncludeUsers
      ? _userService.GetUsersDataAsync(dbGroup.Users.Select(u => u.UserId).ToList())
      : Task.FromResult(null as List<UserData>);

    Task<List<ImageInfo>> imagesTask = filter.IncludeImages
      ? _imageService.GetImagesAsync(dbGroup.Images.Select(i => i.ImageId).ToList())
      : Task.FromResult(null as List<ImageInfo>);

    List<FileCharacteristicsData> files = await filesTask;
    List<UserData> users = await usersTask;
    List<ImageInfo> images = await imagesTask;

    List<CommentInfo> comments = null;

    if (filter.IncludeComments)
    {
      comments = dbGroup.Comments.Select(_commentInfoMapper.Map).ToList();

      foreach (CommentInfo comment in comments)
      {
        comment.Comment.AddRange(comments.Where(c => c.ParentId == comment.Id));
      }

      comments.RemoveAll(x => x.ParentId is not null);
    }

    return new OperationResultResponse<GroupResponse>
    {
      Body = _mapper.Map(
        dbGroup: dbGroup,
        usersData: users,
        images: images,
        files: files,
        comments: comments)
    };
  }
}
