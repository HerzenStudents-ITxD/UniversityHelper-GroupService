using System.Collections.Generic;
using System.Linq;
using UniversityHelper.Models.Broker.Models.User;
using UniversityHelper.GroupService.Mappers.Models.Interface;
using UniversityHelper.GroupService.Mappers.Models.Interfaces;
using UniversityHelper.GroupService.Models.Db;
using UniversityHelper.GroupService.Models.Dto.Models;
using UniversityHelper.GroupService.Models.Dto.Responses.Group;
using UniversityHelper.Models.Broker.Models.File;

namespace UniversityHelper.GroupService.Mappers.Models;

public class GroupResponseMapper : IGroupResponseMapper
{
  private readonly ICategoryInfoMapper _categoryInfoMapper;
  private readonly IUserInfoMapper _userInfoMapper;
  private readonly IFileInfoMapper _fileInfoMapper;

  public GroupResponseMapper(
    ICategoryInfoMapper categoryInfoMapper,
    IUserInfoMapper userInfoMapper,
    IFileInfoMapper fileInfoMapper)
  {
    _categoryInfoMapper = categoryInfoMapper;
    _userInfoMapper = userInfoMapper;
    _fileInfoMapper = fileInfoMapper;
  }

  public GroupResponse Map(
    DbGroup dbGroup,
    List<UserData> usersData,
    List<ImageInfo> images,
    List<FileCharacteristicsData> files,
    List<CommentInfo> comments)
  {
    return dbGroup is null
      ? null
      : new GroupResponse
      {
        Id = dbGroup.Id,
        Name = dbGroup.Name,
        Address = dbGroup.Address,
        Description = dbGroup.Description,
        Date = dbGroup.Date,
        Format = dbGroup.Format,
        Access = dbGroup.Access,
        CreatedAtUtc = dbGroup.CreatedAtUtc,
        GroupCategories = dbGroup.GroupsCategories.Any()
          ? dbGroup.GroupsCategories?.Select(ec => _categoryInfoMapper.Map(ec.Category)).ToList()
          : null,
        GroupUsers = _userInfoMapper.Map(usersData),
        GroupImages = images,
        GroupFiles = files?.ConvertAll(_fileInfoMapper.Map),
        GroupComments = comments
      };
  }
}
