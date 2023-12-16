using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UniversityHelper.GroupService.Models.Db;
using UniversityHelper.GroupService.Models.Dto.Requests.Group;
using UniversityHelper.Core.Attributes;
using Microsoft.AspNetCore.JsonPatch;

namespace UniversityHelper.GroupService.Data.Interfaces;

[AutoInject]
public interface IGroupRepository
{
  Task<Guid?> CreateAsync(DbGroup dbGroup);
  Task<bool> EditAsync(Guid GroupId, Guid senderId, JsonPatchDocument<DbGroup> request);
  Task<bool> DoesExistAsync(Guid GroupId, bool? isActive);
  Task<bool> IsGroupCompletedAsync(Guid GroupId);
  Task<List<Guid>> GetExisting(List<Guid> GroupsIds);
  Task<DbGroup> GetAsync(Guid GroupId, GetGroupFilter filter = null);
  Task<(List<DbGroup>, int totalCount)> FindAsync(
    FindGroupsFilter filter,
    CancellationToken ct);
  Task<DbGroup> GetAsync(Guid GroupId);
  Task<(List<Guid> filesIds, List<Guid> imagesIds)> RemoveDataAsync(Guid GroupId);
}
