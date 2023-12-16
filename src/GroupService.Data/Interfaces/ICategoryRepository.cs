using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UniversityHelper.GroupService.Models.Db;
using UniversityHelper.GroupService.Models.Dto.Enums;
using UniversityHelper.GroupService.Models.Dto.Requests.Category;
using UniversityHelper.Core.Attributes;
using Microsoft.AspNetCore.JsonPatch;

namespace UniversityHelper.GroupService.Data.Interfaces;

[AutoInject]
public interface ICategoryRepository
{
  Task<bool> DoExistAllAsync(List<Guid> categoriesIds);
  Task<bool> DoesExistAsync(string name, CategoryColor color);
  Task<Guid?> CreateAsync(DbGroupCategory dbCategory);
  Task CreateAsync(List<DbGroupCategory> dbCategories);
  Task<(List<DbGroupCategory> dbCategories, int totalCount)> FindAsync(
    FindCategoriesFilter filter,
    CancellationToken cancellationToken = default);
  Task<bool> EditAsync(Guid categoryId, JsonPatchDocument<DbGroupCategory> request);
}
