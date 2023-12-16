using UniversityHelper.Core.BrokerSupport.Attributes;
using UniversityHelper.Models.Broker.Requests.User;
using UniversityHelper.GroupService.Broker.Consumers;
using UniversityHelper.Core.BrokerSupport.Attributes;
using UniversityHelper.Core.BrokerSupport.Configurations;
using UniversityHelper.Models.Broker.Common;
using UniversityHelper.Models.Broker.Requests.Email;
using UniversityHelper.Models.Broker.Requests.File;
using UniversityHelper.Models.Broker.Requests.Image;
using UniversityHelper.Models.Broker.Requests.User;

namespace UniversityHelper.GroupService.Broker.Configuration;

public class RabbitMqConfig : BaseRabbitMqConfig
{
  #region receive endpoints

  [MassTransitEndpoint(typeof(UpdateUserBirthdayConsumer))]
  public string UpdateUserBirthdayEndpoint { get; init; }

  [MassTransitEndpoint(typeof(CheckGroupsEntitiesExistenceConsumer))]
  public string CheckGroupsEntitiesExistenceEndpoint { get; init; }

  #endregion

  // user

  [AutoInjectRequest(typeof(ICheckUsersExistence))]
  public string CheckUsersExistenceEndpoint { get; set; }

  [AutoInjectRequest(typeof(IGetUsersDataRequest))]
  public string GetUsersDataEndpoint { get; set; }

  [AutoInjectRequest(typeof(IFilteredUsersDataRequest))]
  public string FilteredUsersDataEndpoint { get; set; }

  [AutoInjectRequest(typeof(IGetUsersBirthdaysRequest))]
  public string GetUsersBirthdaysEndpoint { get; set; }

  //Email

  [AutoInjectRequest(typeof(ISendEmailRequest))]
  public string SendEmailEndpoint { get; set; }

  // file

  [AutoInjectRequest(typeof(IGetFilesRequest))]
  public string GetFilesEndpoint { get; init; }

  // image

  [AutoInjectRequest(typeof(ICreateImagesRequest))]
  public string CreateImagesEndpoint { get; init; }

  [AutoInjectRequest(typeof(IGetImagesRequest))]
  public string GetImagesEndpoint { get; init; }
}
