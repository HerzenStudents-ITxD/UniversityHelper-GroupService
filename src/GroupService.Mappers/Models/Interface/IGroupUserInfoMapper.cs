using System.Collections.Generic;
using UniversityHelper.GroupService.Models.Db;
using UniversityHelper.GroupService.Models.Dto.Models;
using UniversityHelper.Core.Attributes;

namespace UniversityHelper.GroupService.Mappers.Models.Interface;

[AutoInject]
public interface IGroupUserInfoMapper
{
  List<GroupUserInfo> Map(List<UserInfo> userInfos, List<DbGroupUser> GroupUsers);
}
