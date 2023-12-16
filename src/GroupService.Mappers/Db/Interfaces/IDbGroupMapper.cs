using System;
using System.Collections.Generic;
using UniversityHelper.GroupService.Models.Db;
using UniversityHelper.GroupService.Models.Dto.Requests.Group;
using UniversityHelper.Core.Attributes;

namespace UniversityHelper.GroupService.Mappers.Db.Interfaces;

[AutoInject]
public interface IDbGroupMapper
{
  DbGroup Map(CreateGroupRequest request, Guid senderId, List<Guid> imagesIds);
}
