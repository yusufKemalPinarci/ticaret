using System.Globalization;
using System.IO;
using Microsoft.Extensions.Logging;
using NotebookTherapy.Core.Entities;
using NotebookTherapy.Core.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace NotebookTherapy.Infrastructure.Services;

public class InvoiceService : IInvoiceService
{
    private readonly ILogger<InvoiceService> _logger;
    private readonly IFileStorageService _storage;

    static InvoiceService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public InvoiceService(IFileStorageService storage, ILogger<InvoiceService> logger)
    {
        _storage = storage;
        _logger = logger;
    }

    public async Task<string> GenerateInvoiceAsync(Order order, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var fileName = $"{order.OrderNumber}.pdf";
        var relativePath = Path.Combine("invoices", fileName).Replace("\\", "/");

        var culture = CultureInfo.GetCultureInfo("en-US");
        var issuedDate = DateTime.UtcNow;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(36);
                page.Size(PageSizes.A4);

                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("NotebookTherapy").FontSize(18).SemiBold();
                        col.Item().Text("Invoice").FontSize(12).Medium();
                        col.Item().Text($"Issued: {issuedDate:yyyy-MM-dd}").FontSize(10);
                        col.Item().Text($"Order: {order.OrderNumber}").FontSize(10);
                    });
                    row.RelativeItem().AlignRight().Column(col =>
                    {
                        col.Item().Text("Shipping Address").SemiBold();
                        col.Item().Text(order.ShippingAddress).FontSize(10);
                        col.Item().LineHorizontal(0.5f);
                        col.Item().Text("Billing Address").SemiBold();
                        col.Item().Text(order.BillingAddress).FontSize(10);
                    });
                });

                page.Content().PaddingVertical(10).Column(col =>
                {
                    col.Item().Text("Items").FontSize(12).SemiBold();
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(4);
                            columns.RelativeColumn(1.2f);
                            columns.RelativeColumn(1.2f);
                            columns.RelativeColumn(1.2f);
                            columns.RelativeColumn(1.4f);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Item").SemiBold();
                            header.Cell().AlignRight().Text("Qty").SemiBold();
                            header.Cell().AlignRight().Text("Unit").SemiBold();
                            header.Cell().AlignRight().Text("Tax").SemiBold();
                            header.Cell().AlignRight().Text("Line Total").SemiBold();
                        });

                        foreach (var line in order.Items)
                        {
                            var name = line.Product?.Name ?? "Item";
                            var variant = line.ProductVariant;
                            var variantText = variant == null ? string.Empty : $" ({variant.SKU ?? variant.Color ?? variant.Size})";
                            table.Cell().Text(name + variantText).FontSize(10);
                            table.Cell().AlignRight().Text(line.Quantity.ToString()).FontSize(10);
                            table.Cell().AlignRight().Text(line.UnitPrice.ToString("C", culture)).FontSize(10);
                            table.Cell().AlignRight().Text((line.TaxAmount ?? 0m).ToString("C", culture)).FontSize(10);
                            table.Cell().AlignRight().Text(line.TotalPrice.ToString("C", culture)).FontSize(10);
                        }
                    });

                    col.Item().PaddingTop(10).Column(summary =>
                    {
                        summary.Item().Row(row =>
                        {
                            row.RelativeItem();
                            row.ConstantItem(200).Column(values =>
                            {
                                values.Item().Row(r => { r.RelativeItem().Text("Subtotal"); r.ConstantItem(90).AlignRight().Text(order.SubTotal.ToString("C", culture)); });
                                values.Item().Row(r => { r.RelativeItem().Text("Shipping"); r.ConstantItem(90).AlignRight().Text(order.ShippingCost.ToString("C", culture)); });
                                values.Item().Row(r => { r.RelativeItem().Text("Tax"); r.ConstantItem(90).AlignRight().Text(order.Tax.ToString("C", culture)); });
                                values.Item().Row(r => { r.RelativeItem().Text("Discount"); r.ConstantItem(90).AlignRight().Text((order.DiscountAmount * -1).ToString("C", culture)); });
                                values.Item().LineHorizontal(0.5f);
                                values.Item().Row(r => { r.RelativeItem().Text("Total").SemiBold(); r.ConstantItem(90).AlignRight().Text(order.TotalAmount.ToString("C", culture)).SemiBold(); });
                            });
                        });
                    });
                });

                page.Footer().AlignCenter().Text("Thank you for your purchase!").FontSize(10);
            });
        });

        await using var buffer = new MemoryStream();
        try
        {
            await Task.Run(() => document.GeneratePdf(buffer), cancellationToken);
            buffer.Position = 0;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate invoice for order {OrderNumber}", order.OrderNumber);
            throw;
        }

        var savedPath = await _storage.SaveAsync(buffer, relativePath, "application/pdf", cancellationToken);
        return _storage.GetSignedUrl(savedPath);
    }
}
