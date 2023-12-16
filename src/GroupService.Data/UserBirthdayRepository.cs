using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UniversityHelper.GroupService.Data.Interfaces;
using UniversityHelper.GroupService.Data.Provider;
using UniversityHelper.GroupService.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace UniversityHelper.GroupService.Data
{
  public class UserBirthdayRepository : IUserBirthdayRepository
  {
    private readonly IDataProvider _provider;

    public UserBirthdayRepository(IDataProvider provider)
    {
      _provider = provider;
    }

    public Task<List<DbUserBirthday>> FindAsync(CancellationToken cancellationToken)
    {
      return _provider.UsersBirthdays.AsNoTracking().Where(ub => ub.IsActive).ToListAsync(cancellationToken);
    }

    public async Task UpdateUserBirthdayAsync(Guid userId, DateTime? dateOfBirth)
    {
      DbUserBirthday existingBirthday = await _provider.UsersBirthdays.FirstOrDefaultAsync(b => b.UserId == userId);

      if (existingBirthday is null && dateOfBirth.HasValue)
      {
        _provider.UsersBirthdays.Add(new DbUserBirthday
          {
            UserId = userId,
            DateOfBirth = dateOfBirth.Value,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        });
      }
      else if (existingBirthday is not null &&
        existingBirthday.DateOfBirth != dateOfBirth &&
        dateOfBirth.HasValue)
      {
        existingBirthday.DateOfBirth = dateOfBirth.Value;
        existingBirthday.IsActive = dateOfBirth.HasValue;
        existingBirthday.ModifiedAtUtc = DateTime.UtcNow;
      }
      else if (existingBirthday is not null && existingBirthday.IsActive != dateOfBirth.HasValue)
      {
        existingBirthday.IsActive = dateOfBirth.HasValue;
        existingBirthday.ModifiedAtUtc = DateTime.UtcNow;
      }

      await _provider.SaveAsync();
    }
  }
}
