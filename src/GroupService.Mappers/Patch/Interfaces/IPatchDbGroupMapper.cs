using UniversityHelper.GroupService.Models.Db;
using UniversityHelper.GroupService.Models.Dto.Requests.Group;
using UniversityHelper.Core.Attributes;
using Microsoft.AspNetCore.JsonPatch;

namespace UniversityHelper.GroupService.Mappers.Patch.Interfaces;

[AutoInject]
public interface IPatchDbGroupMapper
{
  JsonPatchDocument<DbGroup> Map(JsonPatchDocument<EditGroupRequest> request);
}
