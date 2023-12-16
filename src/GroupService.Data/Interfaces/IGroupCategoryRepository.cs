using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniversityHelper.GroupService.Models.Db;
using UniversityHelper.Core.Attributes;

namespace UniversityHelper.GroupService.Data.Interfaces;

[AutoInject]
public interface IGroupCategoryRepository
{
  Task<bool> CreateAsync(List<DbGroupTeam> dbGroupCategory);

  bool DoesExistAsync(Guid GroupId, List<Guid> categoriesIds);

  Task<int> CountCategoriesAsync(Guid GroupId);

  Task<bool> RemoveAsync(Guid GroupId, List<Guid> categoriesIds);
}
