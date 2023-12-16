using System;
using System.Collections.Generic;
using System.Linq;
using UniversityHelper.GroupService.Mappers.Db.Interfaces;
using UniversityHelper.GroupService.Models.Db;
using UniversityHelper.GroupService.Models.Dto.Enums;
using UniversityHelper.GroupService.Models.Dto.Requests.Group;

namespace UniversityHelper.GroupService.Mappers.Db;

public class DbGroupMapper : IDbGroupMapper
{
  private readonly IDbImageMapper _imageMapper;

  private List<DbGroupUser> MapGroupUsers(
    CreateGroupRequest request,
    Guid senderId,
    Guid GroupId)
  {
    return request.Users.ConvertAll(u => new DbGroupUser
    {
      Id = Guid.NewGuid(),
      GroupId = GroupId,
      UserId = u.UserId,
      Status = u.UserId == senderId
        ? GroupUserStatus.Participant
        : GroupUserStatus.Invited,
      NotifyAtUtc = u.NotifyAtUtc,
      CreatedBy = senderId,
      CreatedAtUtc = DateTime.UtcNow
    });
  }

  private List<DbGroupTeam> MapGroupCategories(
    CreateGroupRequest request,
    Guid senderId,
    Guid GroupId)
  {
    return request.CategoriesIds is null
      ? null
      : request.CategoriesIds.ConvertAll(categoryId => new DbGroupTeam
      {
        Id = Guid.NewGuid(),
        GroupId = GroupId,
        CategoryId = categoryId,
        CreatedBy = senderId,
        CreatedAtUtc = DateTime.UtcNow
      });
  }

  public DbGroupMapper(
    IDbImageMapper imageMapper)
  {
    _imageMapper = imageMapper;
  }

  public DbGroup Map(
    CreateGroupRequest request,
    Guid senderId,
    List<Guid> imagesIds)
  {
    Guid GroupId = Guid.NewGuid();

    return request is null
      ? null
      : new DbGroup
      {
        Id = GroupId,
        Name = request.Name,
        Address = request.Address,
        Description = request.Description,
        Date = request.Date,
        EndDate = request.EndDate,
        Format = request.Format,
        Access = request.Access,
        IsActive = true,
        CreatedBy = senderId,
        CreatedAtUtc = DateTime.UtcNow,
        Users = MapGroupUsers(request, senderId, GroupId),
        GroupsCategories = MapGroupCategories(request, senderId, GroupId),
        Images = imagesIds?
          .ConvertAll(imageId => _imageMapper.Map(imageId, GroupId))
      };
  }
}
