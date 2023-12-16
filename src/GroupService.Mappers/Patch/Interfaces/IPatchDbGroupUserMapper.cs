using UniversityHelper.GroupService.Models.Db;
using UniversityHelper.GroupService.Models.Dto.Requests.GroupUser;
using UniversityHelper.Core.Attributes;
using Microsoft.AspNetCore.JsonPatch;

namespace UniversityHelper.GroupService.Mappers.Patch.Interfaces;

[AutoInject]
public interface IPatchDbGroupUserMapper
{
  JsonPatchDocument<DbGroupUser> Map(JsonPatchDocument<EditGroupUserRequest> request);
}
