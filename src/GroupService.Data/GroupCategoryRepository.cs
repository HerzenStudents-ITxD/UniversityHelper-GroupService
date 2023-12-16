using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniversityHelper.GroupService.Data.Interfaces;
using UniversityHelper.GroupService.Data.Provider;
using UniversityHelper.GroupService.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace UniversityHelper.GroupService.Data;

public class GroupCategoryRepository : IGroupCategoryRepository
{
  private readonly IDataProvider _provider;

  public GroupCategoryRepository(IDataProvider provider)
  {
    _provider = provider;
  }

  public async Task<bool> CreateAsync(List<DbGroupTeam> dbGroupCategories)
  {
    if (dbGroupCategories is null)
    {
      return false;
    }

    _provider.GroupsCategories.AddRange(dbGroupCategories);
    await _provider.SaveAsync();

    return true;
  }

  public Task<int> CountCategoriesAsync(Guid GroupId)
  {
    return _provider.GroupsCategories.AsNoTracking().CountAsync(ec => ec.GroupId == GroupId);
  }

  public bool DoesExistAsync(Guid GroupId, List<Guid> categoriesIds)
  {
    return categoriesIds.All(categoryId =>
      _provider.GroupsCategories.AnyAsync(ec => ec.CategoryId == categoryId && ec.GroupId == GroupId).Result);
  }

  public async Task<bool> RemoveAsync(Guid GroupId, List<Guid> categoriesIds)
  {
    if (categoriesIds is null || !categoriesIds.Any())
    {
      return false;
    }

    _provider.GroupsCategories.RemoveRange(
      _provider.GroupsCategories.Where(ec => categoriesIds.Contains(ec.CategoryId) && ec.GroupId == GroupId));
    await _provider.SaveAsync();

    return true;
  }
}
