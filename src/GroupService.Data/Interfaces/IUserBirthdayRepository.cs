using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UniversityHelper.GroupService.Models.Db;
using UniversityHelper.Core.Attributes;

namespace UniversityHelper.GroupService.Data.Interfaces
{
  [AutoInject]
  public interface IUserBirthdayRepository
  {
    Task UpdateUserBirthdayAsync(Guid userId, DateTime? dateOfBirth);
    Task<List<DbUserBirthday>> FindAsync(CancellationToken cancellationToken);
  }
}
