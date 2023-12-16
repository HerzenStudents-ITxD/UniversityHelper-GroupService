using UniversityHelper.GroupService.Mappers.Models.Interface;
using UniversityHelper.GroupService.Models.Db;
using UniversityHelper.GroupService.Models.Dto.Models;

namespace UniversityHelper.GroupService.Mappers.Models;

public class CategoryInfoMapper : ICategoryInfoMapper
{
  public CategoryInfo Map(DbGroupCategory dbCategory)
  {
    return dbCategory is null
      ? null
      : new CategoryInfo 
      {
        Id = dbCategory.Id, 
        Name = dbCategory.Name, 
        Color = dbCategory.Color 
      };
  }
}
