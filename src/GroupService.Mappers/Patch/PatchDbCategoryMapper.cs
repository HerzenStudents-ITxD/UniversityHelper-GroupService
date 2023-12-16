using UniversityHelper.GroupService.Mappers.Patch.Interfaces;
using UniversityHelper.GroupService.Models.Db;
using UniversityHelper.GroupService.Models.Dto.Requests.Category;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace UniversityHelper.GroupService.Mappers.Patch;

public class PatchDbCategoryMapper : IPatchDbCategoryMapper
{
  public JsonPatchDocument<DbGroupCategory> Map(JsonPatchDocument<EditCategoryRequest> request)
  {
    if (request is null)
    {
      return null;
    }

    JsonPatchDocument<DbGroupCategory> dbCategoryPatch = new();

    foreach (Operation<EditCategoryRequest> item in request.Operations)
    {
      dbCategoryPatch.Operations.Add(new Operation<DbGroupCategory>(
        item.op,
        item.path,
        item.from,
        string.IsNullOrEmpty(item.value?.ToString().Trim())
          ? null
          : item.value.ToString().Trim()));
    }

    return dbCategoryPatch;
  }
}
