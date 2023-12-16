using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UniversityHelper.GroupService.Data.Interfaces;
using UniversityHelper.GroupService.Data.Provider;
using UniversityHelper.GroupService.Models.Db;
using UniversityHelper.GroupService.Models.Dto.Requests.Group;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace UniversityHelper.GroupService.Data;

public class GroupRepository : IGroupRepository
{
  private readonly IDataProvider _provider;

  private async Task<(List<DbGroup>, int totalCount)> CreateFindPredicate(FindGroupsFilter filter, CancellationToken ct)
  {
    IQueryable<DbGroup> query = _provider.Groups.Include(e => e.GroupsCategories).AsNoTracking();

    if (!filter.IncludeDeactivated)
    {
      query = query.Where(p => p.IsActive);
    }

    if (!string.IsNullOrWhiteSpace(filter.NameIncludeSubstring))
    {
      query = query.Where(p =>
        p.Name.Contains(filter.NameIncludeSubstring) ||
        p.Description.Contains(filter.NameIncludeSubstring)).OrderByDescending(p => p.Date);
    }

    if (!string.IsNullOrWhiteSpace(filter.CategoryNameIncludeSubstring))
    {
      query = query.Where(p =>
        p.GroupsCategories.Where(ec => ec.Category.Name.Contains(filter.CategoryNameIncludeSubstring)).Any()).OrderByDescending(p => p.Date);
    }

    if (filter.Color.HasValue)
    {
      query = query.Where(p =>
        p.GroupsCategories.Any(ec => ec.Category.Color == filter.Color.Value)).OrderByDescending(p => p.Date);
    }

    if (filter.Access.HasValue)
    {
      query = query.Where(p => p.Access == filter.Access.Value);
    }

    if (filter.UserId.HasValue)
    {
      query = query.Where(p =>
        p.Users.Any(u => u.Id == filter.UserId.Value)).OrderByDescending(p => p.Date);
    }

    if (filter.StartTime.HasValue)
    {
      query = query.Where(p => p.Date >= filter.StartTime.Value);
    }

    if (filter.EndTime.HasValue)
    {
      query = query.Where(p => p.Date <= filter.EndTime.Value);
    }

    return (
      await query
        .Skip(filter.SkipCount)
        .Take(filter.TakeCount)
        .ToListAsync(ct),
      await query.CountAsync(ct));
  }

  private Task<DbGroup> CreateGetPredicate(GetGroupFilter filter)
  {
    IQueryable<DbGroup> query = _provider.Groups.AsNoTracking();

    if (filter.IncludeCategories)
    {
      query = query.Include(e => e.GroupsCategories);
    }

    if (filter.IncludeUsers)
    {
      query = query.Include(e => e.Users);
    }

    if (filter.IncludeImages)
    {
      query = query.Include(e => e.Images);
    }

    if (filter.IncludeFiles)
    {
      query = query.Include(e => e.Files);
    }

    if (filter.IncludeComments)
    {
      query = query.Include(e => e.Comments.OrderBy(c => c.CreatedAtUtc));
    }

    return query.FirstOrDefaultAsync(e => e.Id == filter.GroupId);
  }

  public GroupRepository(
    IDataProvider provider)
  {
    _provider = provider;
  }

  public async Task<Guid?> CreateAsync(DbGroup dbGroup)
  {
    if (dbGroup is null)
    {
      return null;
    }

    _provider.Groups.Add(dbGroup);
    _provider.GroupsUsers.AddRange(dbGroup.Users);

    if (!dbGroup.GroupsCategories.IsNullOrEmpty())
    {
      _provider.GroupsCategories.AddRange(dbGroup.GroupsCategories);
    }

    await _provider.SaveAsync();

    return dbGroup.Id;
  }

  public async Task<bool> EditAsync(Guid GroupId, Guid senderId, JsonPatchDocument<DbGroup> request)
  {
    DbGroup dbGroup = await _provider.Groups.Include(e => e.Comments).FirstOrDefaultAsync(x => x.Id == GroupId);

    if (dbGroup is null || request is null)
    {
      return false;
    }

    bool oldIsActive = dbGroup.IsActive;

    request.ApplyTo(dbGroup);
    dbGroup.ModifiedBy = senderId;
    dbGroup.ModifiedAtUtc = DateTime.UtcNow;

    bool newIsActive = dbGroup.IsActive;

    if (oldIsActive != newIsActive)
    {
      IEnumerable<DbGroupComment> comments = dbGroup.Comments
        .Where(x => x.GroupId == GroupId && (x.Content != null));

      foreach (DbGroupComment comment in comments)
      {
        comment.IsActive = newIsActive;
        comment.ModifiedBy = senderId;
        comment.ModifiedAtUtc = DateTime.UtcNow;
      }
    }

    await _provider.SaveAsync();

    return true;
  }

  public Task<bool> DoesExistAsync(Guid GroupId, bool? isActive)
  {
    if (isActive is not null)
    {
      return _provider.Groups.AnyAsync(e => e.Id == GroupId && e.IsActive);
    }
    else
    {
      return _provider.Groups.AnyAsync(e => e.Id == GroupId);
    }
  }

  public async Task<bool> IsGroupCompletedAsync(Guid GroupId)
  {
    DbGroup dbGroup = await _provider.Groups.FirstOrDefaultAsync(x => x.Id == GroupId);

    if (!dbGroup.IsActive ||
         dbGroup.IsActive &&
            (dbGroup.EndDate is null && dbGroup.Date > DateTime.UtcNow  ||
            dbGroup.EndDate > DateTime.UtcNow))
    {
      return true;
    }

    return false;
  }

  public Task<List<Guid>> GetExisting(List<Guid> GroupsIds)
  {
    return _provider.Groups.AsNoTracking().Where(p => GroupsIds.Contains(p.Id)).Select(p => p.Id).ToListAsync();
  }

  public Task<DbGroup> GetAsync(
    Guid GroupId,
    GetGroupFilter filter = null)
  {
    if (filter is null)
    {
      return _provider.Groups.AsNoTracking().FirstOrDefaultAsync(e => e.Id == GroupId);
    }

    return CreateGetPredicate(filter);
  }

  public async Task<(List<DbGroup>, int totalCount)> FindAsync(FindGroupsFilter filter, CancellationToken ct)
  {
    if (filter is null)
    {
      return default;
    }

    return await CreateFindPredicate(filter, ct);
  }

  public Task<DbGroup> GetAsync(Guid GroupId)
  {
    return _provider.Groups.AsNoTracking().FirstOrDefaultAsync(e => e.Id == GroupId);
  }

  public async Task<(List<Guid> filesIds, List<Guid> imagesIds)> RemoveDataAsync(Guid GroupId)
  {
    DbGroup dbGroup = await _provider.Groups
      .Include(e => e.Users)
      .Include(e => e.Files)
      .Include(e => e.Images)
      .Include(e => e.Comments)
        .ThenInclude(c => c.Images)
      .Include(e => e.Comments)
        .ThenInclude(c => c.Files)
      .FirstOrDefaultAsync(p => p.Id == GroupId);

    List<Guid> filesIds = dbGroup.Files.Select(file => file.FileId).ToList();
    List<Guid> imagesIds = dbGroup.Images.Select(image => image.ImageId).ToList();
    List<DbGroupComment> comments = dbGroup.Comments
       .Where(x => x.GroupId == GroupId && (x.Content != null)).ToList();

    foreach (DbGroupComment comment in comments)
    {
      filesIds.AddRange(comment.Files.Select(f => f.FileId));
      imagesIds.AddRange(comment.Images.Select(f => f.ImageId));

      _provider.Images.RemoveRange(comment.Images);
      _provider.Files.RemoveRange(comment.Files);
    }

    _provider.GroupsUsers.RemoveRange(dbGroup.Users);

    _provider.Images.RemoveRange(dbGroup.Images);

    _provider.Files.RemoveRange(dbGroup.Files);

    await _provider.SaveAsync();

    return (filesIds, imagesIds);
  }
}
