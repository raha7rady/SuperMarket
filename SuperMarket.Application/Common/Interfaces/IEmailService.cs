using System.Threading.Tasks;

namespace SuperMarket.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(
            string to,
            string subject,
            string body,
            bool isHtml = true);

        Task SendEmailAsync(
            string to,
            string subject,
            string body,
            string? from = null,
            bool isHtml = true);
    }
}
