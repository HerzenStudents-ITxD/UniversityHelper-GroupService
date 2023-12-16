using UniversityHelper.GroupService.Models.Db;
using UniversityHelper.GroupService.Models.Dto.Requests.Category;
using UniversityHelper.Core.Attributes;

namespace UniversityHelper.GroupService.Mappers.Db.Interfaces
{
  [AutoInject]
  public interface IDbCategoryMapper
  {
    DbGroupCategory Map(CreateCategoryRequest request);
  }
}
