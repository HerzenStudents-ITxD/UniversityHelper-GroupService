using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniversityHelper.Models.Broker.Models.User;
using UniversityHelper.Models.Broker.Requests.User;
using UniversityHelper.Models.Broker.Responses.User;
using UniversityHelper.GroupService.Broker.Requests.Interfaces;
using UniversityHelper.Core.BrokerSupport.Helpers;
using UniversityHelper.Models.Broker.Common;
using UniversityHelper.Models.Broker.Requests.User;
using UniversityHelper.Models.Broker.Responses.User;
using MassTransit;

namespace UniversityHelper.GroupService.Broker.Requests;

public class UserService : IUserService
{
  private readonly IRequestClient<ICheckUsersExistence> _rcCheckUserExistence;
  private readonly IRequestClient<IGetUsersDataRequest> _rcGetUsersData;
  private readonly IRequestClient<IFilteredUsersDataRequest> _rcFilteredUsersData;
  private readonly IRequestClient<IGetUsersBirthdaysRequest> _rcGetUsersBirthdaysData;

  public UserService(
    IRequestClient<ICheckUsersExistence> rcCheckUserExistence,
    IRequestClient<IGetUsersDataRequest> rcGetUsersData,
    IRequestClient<IFilteredUsersDataRequest> rcFilteredUsersData,
    IRequestClient<IGetUsersBirthdaysRequest> rcGetUsersBirthdaysData)
  {
    _rcCheckUserExistence = rcCheckUserExistence;
    _rcGetUsersData = rcGetUsersData;
    _rcFilteredUsersData = rcFilteredUsersData;
    _rcGetUsersBirthdaysData = rcGetUsersBirthdaysData;
  }

  public async Task<bool> CheckUsersExistenceAsync(List<Guid> usersIds, List<string> errors = null)
  {
    if (usersIds is null || !usersIds.Any())
    {
      return false;
    }

    List<Guid> existingUserIds = (await RequestHandler.ProcessRequest<ICheckUsersExistence, ICheckUsersExistence>(
        _rcCheckUserExistence,
        ICheckUsersExistence.CreateObj(usersIds),
        errors))
      ?.UserIds;
    return new HashSet<Guid>(usersIds.Distinct()).SetEquals(existingUserIds);
  }

  public async Task<List<UserData>> GetUsersDataAsync(List<Guid> usersIds)
  {
    if (usersIds is null || !usersIds.Any())
    {
      return null;
    }

    return (await _rcGetUsersData.ProcessRequest<IGetUsersDataRequest, IGetUsersDataResponse>(
      IGetUsersDataRequest.CreateObj(usersIds)))
      ?.UsersData;
  }

  public async Task<(List<UserData> usersData, int totalCount)> FilteredUsersDataAsync(
    List<Guid> usersIds,
    int skipCount = 0,
    int takeCount = 1,
    bool? ascendingSort = null,
    string fullNameIncludeSubstring = null)
  {
    IFilteredUsersDataResponse response =
      await _rcFilteredUsersData.ProcessRequest<IFilteredUsersDataRequest, IFilteredUsersDataResponse>(
        IFilteredUsersDataRequest.CreateObj(
        usersIds: usersIds,
        skipCount: skipCount,
        takeCount: takeCount,
        ascendingSort: ascendingSort,
        fullNameIncludeSubstring: fullNameIncludeSubstring));

    return response is null
      ? default
      : (response.UsersData, response.TotalCount);
  }

  public async Task<List<UserBirthday>> GetUsersBirthdaysAsync()
  {
    return (await _rcGetUsersBirthdaysData.ProcessRequest<IGetUsersBirthdaysRequest, IGetUsersBirthdaysResponse>(
      IGetUsersBirthdaysRequest.CreateObj()))
      ?.UsersBirthdays;
  }
}
