using System.Threading;
using System.Threading.Tasks;
using NotebookTherapy.Core.Entities;

namespace NotebookTherapy.Core.Interfaces;

public interface IInvoiceService
{
    Task<string> GenerateInvoiceAsync(Order order, CancellationToken cancellationToken = default);
}
