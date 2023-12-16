using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using UniversityHelper.Models.Broker.Models.User;
using UniversityHelper.GroupService.Broker.Requests.Interfaces;
using UniversityHelper.GroupService.Business.Commands.GroupUser.Interfaces;
using UniversityHelper.GroupService.Data.Interfaces;
using UniversityHelper.GroupService.Mappers.Models.Interface;
using UniversityHelper.GroupService.Models.Db;
using UniversityHelper.GroupService.Models.Dto.Models;
using UniversityHelper.GroupService.Models.Dto.Requests.GroupUser.Filter;
using UniversityHelper.Core.Helpers.Interfaces;
using UniversityHelper.Core.Responses;

namespace UniversityHelper.GroupService.Business.Commands.GroupUser;

public class FindGroupUserCommand : IFindGroupUserCommand
{
  private readonly IGroupUserRepository _GroupUserRepository;
  private readonly IGroupRepository _GroupRepository;
  private readonly IUserService _userService;
  private readonly IGroupUserInfoMapper _GroupUserInfoMapper;
  private readonly IUserInfoMapper _userInfoMapper;
  private readonly IResponseCreator _responseCreator;

  public FindGroupUserCommand(
    IGroupUserRepository GroupUserRepository,
    IGroupRepository GroupRepository,
    IUserService userService,
    IGroupUserInfoMapper GroupUserInfoMapper,
    IUserInfoMapper userInfoMapper,
    IResponseCreator responseCreator)
  {
    _GroupUserRepository = GroupUserRepository;
    _GroupRepository = GroupRepository;
    _userService = userService;
    _GroupUserInfoMapper = GroupUserInfoMapper;
    _userInfoMapper = userInfoMapper;
    _responseCreator = responseCreator;
  }

  public async Task<FindResultResponse<GroupUserInfo>> ExecuteAsync(
    Guid GroupId,
    FindGroupUsersFilter filter,
    CancellationToken cancellationToken)
  {
    if (!await _GroupRepository.DoesExistAsync(GroupId, true))
    {
      return _responseCreator.CreateFailureFindResponse<GroupUserInfo>(HttpStatusCode.NotFound);
    }

    List<DbGroupUser> GroupUsers =
      await _GroupUserRepository.FindAsync(GroupId: GroupId, filter: filter, cancellationToken: cancellationToken);

    if (GroupUsers is null || !GroupUsers.Any())
    {
      return new();
    }

    (List<UserData> usersData, int totalCount) = await _userService.FilteredUsersDataAsync(
      usersIds:GroupUsers.Select(e => e.UserId).ToList(),
      skipCount: filter.SkipCount,
      takeCount: filter.TakeCount,
      ascendingSort: filter.IsAscendingSort,
      fullNameIncludeSubstring: filter.UserFullNameIncludeSubstring);

    return new FindResultResponse<GroupUserInfo>(
      totalCount: totalCount,
      body: _GroupUserInfoMapper.Map(
        userInfos: _userInfoMapper.Map(usersData),
        GroupUsers: GroupUsers));
  }
}
