using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniversityHelper.Models.Broker.Common;
using UniversityHelper.GroupService.Data.Interfaces;
using UniversityHelper.Core.BrokerSupport.Broker;
using MassTransit;

namespace UniversityHelper.GroupService.Broker;

public class CheckGroupsEntitiesExistenceConsumer : IConsumer<ICheckGroupsExistence>
{
  private readonly IGroupRepository _GroupRepository;
  private readonly IGroupCommentRepository _commentRepository;

  public CheckGroupsEntitiesExistenceConsumer(
    IGroupRepository GroupRepository,
    IGroupCommentRepository commentRepository)
  {
    _GroupRepository = GroupRepository;
    _commentRepository = commentRepository;
  }

  public async Task Consume(ConsumeContext<ICheckGroupsExistence> context)
  {
    List<Guid> existingGroups = await _GroupRepository.GetExisting(context.Message.GroupsIds);
    List<Guid> existingComments = await _commentRepository.GetExisting(context.Message.GroupsIds);
    object response = new();

    if (existingGroups.Any())
    {
      response = OperationResultWrapper.CreateResponse((_) => ICheckGroupsExistence.CreateObj(existingGroups), context);
    }
    else if (existingComments.Any())
    {
      response = OperationResultWrapper.CreateResponse((_) => ICheckGroupsExistence.CreateObj(existingComments), context);
    }
      
    await context.RespondAsync<IOperationResult<ICheckGroupsExistence>>(response);
  }
}
