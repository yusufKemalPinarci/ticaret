using System.Threading.Tasks;

namespace NotebookTherapy.Core.Interfaces;

public interface IEmailService
{
    Task SendAsync(string to, string subject, string body);
}
