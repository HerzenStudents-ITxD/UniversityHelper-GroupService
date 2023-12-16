using UniversityHelper.GroupService.Mappers.Patch.Interfaces;
using UniversityHelper.GroupService.Models.Db;
using UniversityHelper.GroupService.Models.Dto.Requests.GroupUser;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace UniversityHelper.GroupService.Mappers.Patch;

public class PatchDbGroupUserMapper : IPatchDbGroupUserMapper
{
  public JsonPatchDocument<DbGroupUser> Map(JsonPatchDocument<EditGroupUserRequest> request)
  {
    if (request is null)
    {
      return null;
    }

    JsonPatchDocument<DbGroupUser> dbGroupUserPatch = new();

    foreach (Operation<EditGroupUserRequest> item in request.Operations)
    {
      dbGroupUserPatch.Operations.Add(new Operation<DbGroupUser>(
        item.op,
        item.path,
        item.from,
        string.IsNullOrEmpty(item.value?.ToString().Trim())
          ? null
          : item.value.ToString().Trim()));
    }

    return dbGroupUserPatch;
  }
}
