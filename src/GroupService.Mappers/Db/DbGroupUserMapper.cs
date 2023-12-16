using System;
using System.Collections.Generic;
using System.Linq;
using UniversityHelper.GroupService.Mappers.Db.Interfaces;
using UniversityHelper.GroupService.Models.Db;
using UniversityHelper.GroupService.Models.Dto.Enums;
using UniversityHelper.GroupService.Models.Dto.Requests.GroupUser;

namespace UniversityHelper.GroupService.Mappers.Db;

public class DbGroupUserMapper : IDbGroupUserMapper
{
  public List<DbGroupUser> Map(
    CreateGroupUserRequest request,
    AccessType access,
    Guid senderId)
  {
    return request is null
      ? null
      : request.Users.Select(u => new DbGroupUser
      {
        Id = Guid.NewGuid(),
        GroupId = request.GroupId,
        UserId = u.UserId,
        Status = (access == AccessType.Opened && u.UserId == senderId)
          ? GroupUserStatus.Participant
          : GroupUserStatus.Invited,
        NotifyAtUtc = u.NotifyAtUtc,
        CreatedBy = senderId,
        CreatedAtUtc = DateTime.UtcNow
      }).ToList();
  }
}
