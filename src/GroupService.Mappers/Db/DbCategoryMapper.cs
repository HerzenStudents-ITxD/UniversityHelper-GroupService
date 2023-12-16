using System;
using UniversityHelper.GroupService.Mappers.Db.Interfaces;
using UniversityHelper.GroupService.Models.Db;
using UniversityHelper.GroupService.Models.Dto.Requests.Category;
using UniversityHelper.Core.Extensions;
using Microsoft.AspNetCore.Http;

namespace UniversityHelper.GroupService.Mappers.Db
{
  public class DbCategoryMapper : IDbCategoryMapper
  {
    private readonly IHttpContextAccessor _contextAccessor;

    public DbCategoryMapper(IHttpContextAccessor accessor)
    {
      _contextAccessor = accessor;
    }

    public DbGroupCategory Map(CreateCategoryRequest request)
    {
      return request is null
        ? null
        : new DbGroupCategory
        {
          Id = Guid.NewGuid(),
          IsActive = true,
          Name = request.Name,
          Color = request.Color,
          CreatedBy = _contextAccessor.HttpContext.GetUserId(),
          CreatedAtUtc = DateTime.UtcNow
        };
    }
  }
}
