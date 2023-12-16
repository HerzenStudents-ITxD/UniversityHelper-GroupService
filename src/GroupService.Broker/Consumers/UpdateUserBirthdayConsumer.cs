using System.Threading.Tasks;
using UniversityHelper.Models.Broker.Publishing.Subscriber.User;
using UniversityHelper.GroupService.Data.Interfaces;
using MassTransit;

namespace UniversityHelper.GroupService.Broker.Consumers;

public class UpdateUserBirthdayConsumer : IConsumer<IUpdateUserBirthdayPublish>
{
  private readonly IUserBirthdayRepository _userBirthdayRepository;

  private async Task UpdateUserBirthdayAsync(IUpdateUserBirthdayPublish publish)
  {
    if (publish is null)
    {
      return;
    }

    await _userBirthdayRepository.UpdateUserBirthdayAsync(publish.UserId, publish.DateOfBirth);
  }

  public UpdateUserBirthdayConsumer(
    IUserBirthdayRepository userBirthdayRepository)
  {
    _userBirthdayRepository = userBirthdayRepository;
  }

  public async Task Consume(ConsumeContext<IUpdateUserBirthdayPublish> context)
  {
    await UpdateUserBirthdayAsync(context.Message);
  }
}
