using System.Threading.Tasks;
using UniversityHelper.GroupService.Broker.Requests.Interfaces;
using UniversityHelper.Core.BrokerSupport.Helpers;
using UniversityHelper.Models.Broker.Requests.Email;
using MassTransit;

namespace UniversityHelper.GroupService.Broker.Requests;

public class EmailService : IEmailService
{
  private readonly IRequestClient<ISendEmailRequest> _rcSendEmail;

  public EmailService(IRequestClient<ISendEmailRequest> rcSendEmail)
  {
    _rcSendEmail = rcSendEmail;
  }

  public async Task SendAsync(string email, string subject, string text)
  {
    await _rcSendEmail.ProcessRequest<ISendEmailRequest, bool>(
      ISendEmailRequest.CreateObj(
        email,
        subject,
        text));
  }
}
