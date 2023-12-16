using System;
using UniversityHelper.GroupService.Mappers.Models.Interface;
using UniversityHelper.GroupService.Models.Db;
using UniversityHelper.GroupService.Models.Dto.Models;

namespace UniversityHelper.GroupService.Mappers.Models;

public class UserBirthdayInfoMapper : IUserBirthdayInfoMapper
{
  public UserBirthdayInfo Map(DbUserBirthday userBirthday, DateTime dateOfBirth)
  {
    return userBirthday is null
      ? null
      : new UserBirthdayInfo
      {
        UserId = userBirthday.UserId,
        DateOfBirth = dateOfBirth,
      };
  }
}
