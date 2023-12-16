using System.Collections.Generic;
using System.Linq;
using UniversityHelper.GroupService.Mappers.Models.Interface;
using UniversityHelper.GroupService.Models.Db;
using UniversityHelper.GroupService.Models.Dto.Models;

namespace UniversityHelper.GroupService.Mappers.Models;

public class GroupUserInfoMapper : IGroupUserInfoMapper
{
  public List<GroupUserInfo> Map(List<UserInfo> userInfos, List<DbGroupUser> GroupUsers)
  {
    return GroupUsers?.Select(eu => new GroupUserInfo
    {
      Id = eu.Id,
      Status = eu.Status,
      NotifyAtUtc = eu.NotifyAtUtc,
      UserInfo = userInfos.Where(u => u.UserId == eu.UserId).ToList(),
    }).ToList();
  }
}
