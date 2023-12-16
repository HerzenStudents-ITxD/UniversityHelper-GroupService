using System;
using System.Collections.Generic;
using UniversityHelper.GroupService.Models.Dto.Enums;

namespace UniversityHelper.GroupService.Models.Dto.Models;

public record GroupUserInfo
{
  public Guid Id { get; set; }
  public GroupUserStatus Status { get; set; }
  public DateTime? NotifyAtUtc { get; set; }
  public List<UserInfo> UserInfo { get; set; }
}
