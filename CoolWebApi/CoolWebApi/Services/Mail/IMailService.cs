using System.Threading.Tasks;
using CoolWebApi.Models.Requests.Mail;

namespace CoolWebApi.Services.Mail
{
    public interface IMailService
    {
        Task SendAsync(MailRequest request);
    }
}