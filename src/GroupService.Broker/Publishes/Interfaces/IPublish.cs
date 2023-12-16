using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniversityHelper.Core.Attributes;

namespace UniversityHelper.GroupService.Broker.Publishes.Interfaces;

[AutoInject]
public interface IPublish
{
  Task RemoveImagesAsync(List<Guid> imagesIds);
  Task RemoveFilesAsync(List<Guid> filesIds);
}
