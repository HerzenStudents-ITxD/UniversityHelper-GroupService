using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UniversityHelper.GroupService.Data.Provider;
using UniversityHelper.GroupService.Data.Interfaces;
using UniversityHelper.GroupService.Models.Db;
using UniversityHelper.GroupService.Models.Dto.Requests.GroupUser.Filter;
using UniversityHelper.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace UniversityHelper.GroupService.Data;

public class GroupUserRepository : IGroupUserRepository
{
  private readonly IDataProvider _provider;
  private readonly IHttpContextAccessor _httpContextAccessor;

  public GroupUserRepository(
    IDataProvider provider,
    IHttpContextAccessor httpContextAccessor)
  {
    _provider = provider;
    _httpContextAccessor = httpContextAccessor;
  }

  public Task<bool> DoesExistAsync(List<Guid> userId, Guid GroupId)
  {
    return _provider.GroupsUsers.AsNoTracking().AnyAsync(eu => userId.Contains(eu.UserId) && eu.GroupId == GroupId);
  }

  public Task<bool> DoesExistAsync(Guid GroupUserId)
  {
    return _provider.GroupsUsers.AsNoTracking().AnyAsync(eu => eu.Id == GroupUserId);
  }

  public async Task<bool> CreateAsync(List<DbGroupUser> dbGroupUsers)
  {
    if (dbGroupUsers is null)
    {
      return false;
    }

    _provider.GroupsUsers.AddRange(dbGroupUsers);
    await _provider.SaveAsync();

    return true;
  }

  public Task<List<DbGroupUser>> FindAsync(
    Guid GroupId,
    FindGroupUsersFilter filter,
    CancellationToken cancellationToken)
  {
    IQueryable<DbGroupUser> GroupUsersQuery = _provider.GroupsUsers.AsNoTracking().Where(eu =>
      eu.GroupId == GroupId);

    if (filter.Status.HasValue)
    {
      GroupUsersQuery = GroupUsersQuery.Where(s => s.Status == filter.Status);
    }

    return GroupUsersQuery.ToListAsync(cancellationToken: cancellationToken);
  }

  public Task<DbGroupUser> GetAsync(Guid GroupUserId)
  {
    return _provider.GroupsUsers.AsNoTracking().FirstOrDefaultAsync(eu => eu.Id == GroupUserId);
  }

  public async Task<bool> EditAsync(Guid GroupUserId, JsonPatchDocument<DbGroupUser> request)
  {
    DbGroupUser dbGroupUser = await _provider.GroupsUsers.FirstOrDefaultAsync(x => x.Id == GroupUserId);

    if (dbGroupUser is null || request is null)
    {
      return false;
    }
    
    request.ApplyTo(dbGroupUser);
    dbGroupUser.ModifiedBy = _httpContextAccessor.HttpContext.GetUserId();
    dbGroupUser.ModifiedAtUtc = DateTime.UtcNow;

    await _provider.SaveAsync();

    return true;
  }
}
