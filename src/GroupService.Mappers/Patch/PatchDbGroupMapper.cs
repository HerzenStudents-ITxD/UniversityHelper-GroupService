using UniversityHelper.GroupService.Mappers.Patch.Interfaces;
using UniversityHelper.GroupService.Models.Db;
using UniversityHelper.GroupService.Models.Dto.Requests.Group;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace UniversityHelper.GroupService.Mappers.Patch;

public class PatchDbGroupMapper : IPatchDbGroupMapper
{
  public JsonPatchDocument<DbGroup> Map(JsonPatchDocument<EditGroupRequest> request)
  {
    if (request is null)
    {
      return null;
    }

    JsonPatchDocument<DbGroup> dbGroupPatch = new();

    foreach (Operation<EditGroupRequest> item in request.Operations)
    {
      dbGroupPatch.Operations.Add(new Operation<DbGroup>(
        item.op,
        item.path,
        item.from,
        string.IsNullOrEmpty(item.value?.ToString().Trim())
          ? null
          : item.value.ToString().Trim()));
    }

    return dbGroupPatch;
  }
}
