using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using UniversityHelper.GroupService.Business.Commands.Group.Interfaces;
using UniversityHelper.GroupService.Data.Interfaces;
using UniversityHelper.GroupService.Mappers.Models.Interface;
using UniversityHelper.GroupService.Models.Db;
using UniversityHelper.GroupService.Models.Dto.Models;
using UniversityHelper.GroupService.Models.Dto.Requests.Group;
using UniversityHelper.Core.BrokerSupport.AccessValidatorEngine.Interfaces;
using UniversityHelper.Core.Constants;
using UniversityHelper.Core.Extensions;
using UniversityHelper.Core.Helpers.Interfaces;
using UniversityHelper.Core.Responses;
using Microsoft.AspNetCore.Http;

namespace UniversityHelper.GroupService.Business.Commands.Group;

public class FindGroupsCommand : IFindGroupsCommand
{
  private readonly IGroupRepository _repository;
  private readonly IGroupInfoMapper _mapper;
  private readonly IAccessValidator _accessValidator;
  private readonly IResponseCreator _responseCreator;
  private readonly IHttpContextAccessor _contextAccessor;

  public FindGroupsCommand(
    IGroupRepository repository,
    IGroupInfoMapper mapper,
    IAccessValidator accessValidator,
    IResponseCreator responseCreator,
    IHttpContextAccessor contextAccessor)
  {
    _repository = repository;
    _mapper = mapper;
    _accessValidator = accessValidator;
    _responseCreator = responseCreator;
    _contextAccessor = contextAccessor;
  }

  public async Task<FindResultResponse<GroupInfo>> ExecuteAsync(FindGroupsFilter filter, CancellationToken ct = default)
  {
    if (filter.IncludeDeactivated &&
      !await _accessValidator.HasRightsAsync(_contextAccessor.HttpContext.GetUserId(), Rights.AddEditRemoveUsers))
    {
      return _responseCreator.CreateFailureFindResponse<GroupInfo>(HttpStatusCode.Forbidden);
    }

    (List<DbGroup> Groups, int totalCount) =
      await _repository.FindAsync(filter, ct);

    if (Groups is null || !Groups.Any())
    {
      return new();
    }

    return new FindResultResponse<GroupInfo>
    {
      Body = Groups.ConvertAll(_mapper.Map),
      TotalCount = totalCount
    };
  }
}
