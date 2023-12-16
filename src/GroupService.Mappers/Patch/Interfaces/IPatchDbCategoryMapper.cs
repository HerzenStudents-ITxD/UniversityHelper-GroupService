using UniversityHelper.GroupService.Models.Db;
using UniversityHelper.GroupService.Models.Dto.Requests.Category;
using UniversityHelper.Core.Attributes;
using Microsoft.AspNetCore.JsonPatch;

namespace UniversityHelper.GroupService.Mappers.Patch.Interfaces;

[AutoInject]
public interface IPatchDbCategoryMapper
{
  JsonPatchDocument<DbGroupCategory> Map(JsonPatchDocument<EditCategoryRequest> request);
}
