using System;
using System.Collections.Generic;
using System.Linq;
using UniversityHelper.GroupService.Mappers.Db.Interfaces;
using UniversityHelper.GroupService.Models.Db;
using UniversityHelper.GroupService.Models.Dto.Requests.GroupCategory;
using UniversityHelper.Core.Extensions;
using Microsoft.AspNetCore.Http;

namespace UniversityHelper.GroupService.Mappers.Db;

public class DbGroupCategoryMapper : IDbGroupCategoryMapper
{
  private readonly IHttpContextAccessor _contextAccessor;

  public DbGroupCategoryMapper(IHttpContextAccessor accessor)
  {
    _contextAccessor = accessor;
  }

  public List<DbGroupTeam> Map(CreateGroupCategoryRequest request)
  {
    return request is null
      ? null
      : request.CategoriesIds.Select(categoryId => new DbGroupTeam
      {
        Id = Guid.NewGuid(),
        GroupId = request.GroupId,
        CategoryId = categoryId,
        CreatedBy = _contextAccessor.HttpContext.GetUserId(),
        CreatedAtUtc = DateTime.UtcNow
      }).ToList();
  }
}
