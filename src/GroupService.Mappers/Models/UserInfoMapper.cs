using System.Collections.Generic;
using System.Linq;
using UniversityHelper.Models.Broker.Models.User;
using UniversityHelper.GroupService.Mappers.Models.Interface;
using UniversityHelper.GroupService.Models.Dto.Models;

namespace UniversityHelper.GroupService.Mappers.Models;

public class UserInfoMapper : IUserInfoMapper
{
  public List<UserInfo> Map(List<UserData> usersData)
  {
    return usersData?.Select(u => new UserInfo
    {
      UserId = u.Id,
      FirstName = u.FirstName,
      LastName = u.LastName,
      MiddleName = u.MiddleName,
      ImageId = u.ImageId
    }).ToList();
  }
}
