using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UniversityHelper.GroupService.Models.Db;
using UniversityHelper.GroupService.Models.Dto.Requests.GroupUser.Filter;
using UniversityHelper.Core.Attributes;
using Microsoft.AspNetCore.JsonPatch;

namespace UniversityHelper.GroupService.Data.Interfaces;

[AutoInject]
public interface IGroupUserRepository
{
  Task<bool> DoesExistAsync(List<Guid> userId, Guid GroupId);
  Task<bool> DoesExistAsync(Guid GroupUserId);
  Task<bool> CreateAsync(List<DbGroupUser> dbGroupUsers);
  Task<List<DbGroupUser>> FindAsync(
    Guid GroupId, 
    FindGroupUsersFilter filter, 
    CancellationToken cancellationToken);
  Task<DbGroupUser> GetAsync(Guid GroupUserId);
  Task<bool> EditAsync(Guid GroupUserId, JsonPatchDocument<DbGroupUser> request);
}
