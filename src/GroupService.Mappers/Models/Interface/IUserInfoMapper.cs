using System.Collections.Generic;
using UniversityHelper.Models.Broker.Models.User;
using UniversityHelper.GroupService.Models.Dto.Models;
using UniversityHelper.Core.Attributes;

namespace UniversityHelper.GroupService.Mappers.Models.Interface;

[AutoInject]
public interface IUserInfoMapper
{
  List<UserInfo> Map(List<UserData> usersData);
}
