using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniversityHelper.Models.Broker.Enums;
using UniversityHelper.GroupService.Broker.Publishes.Interfaces;
using UniversityHelper.Models.Broker.Enums;
using UniversityHelper.Models.Broker.Publishing.Subscriber.File;
using UniversityHelper.Models.Broker.Publishing.Subscriber.Image;
using MassTransit;

namespace UniversityHelper.GroupService.Broker.Publishes;

public class Publish : IPublish
{
  private readonly IBus _bus;

  public Publish(IBus bus)
  {
    _bus = bus;
  }

  public Task RemoveImagesAsync(List<Guid> imagesIds)
  {
    return _bus.Publish<IRemoveImagesPublish>(IRemoveImagesPublish.CreateObj(
      imagesIds: imagesIds,
      imageSource: ImageSource.Group));
  }

  public Task RemoveFilesAsync(List<Guid> filesIds)
  {
    return _bus.Publish<IRemoveFilesPublish>(IRemoveFilesPublish.CreateObj(FileSource.Group, filesIds));
  }
}
