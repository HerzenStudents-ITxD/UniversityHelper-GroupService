using System.Collections.Generic;
using UniversityHelper.GroupService.Models.Db;
using UniversityHelper.GroupService.Models.Dto.Requests.GroupCategory;
using UniversityHelper.Core.Attributes;

namespace UniversityHelper.GroupService.Mappers.Db.Interfaces;

[AutoInject]
public interface IDbGroupCategoryMapper
{
  List<DbGroupTeam> Map(CreateGroupCategoryRequest request);
}
