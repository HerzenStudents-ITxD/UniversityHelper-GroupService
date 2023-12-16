using System;
using System.Collections.Generic;
using UniversityHelper.GroupService.Models.Db;
using UniversityHelper.GroupService.Models.Dto.Enums;
using UniversityHelper.GroupService.Models.Dto.Requests.GroupUser;
using UniversityHelper.Core.Attributes;

namespace UniversityHelper.GroupService.Mappers.Db.Interfaces;
[AutoInject]
public interface IDbGroupUserMapper
{
  List<DbGroupUser> Map(CreateGroupUserRequest request, AccessType access, Guid senderId);
}
