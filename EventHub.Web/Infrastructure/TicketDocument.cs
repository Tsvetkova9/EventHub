using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using EventHub.Core.Entities;

namespace EventHub.Web.Infrastructure
{
    /// <summary>Generates a simple PDF ticket with QuestPDF.</summary>
    // TODO: add QR code support later
    public class TicketDocument : IDocument
    {
        private readonly Ticket _ticket;

        public TicketDocument(Ticket ticket)
        {
            _ticket = ticket;
        }

        public DocumentMetadata GetMetadata() => new DocumentMetadata
        {
            Title = $"EventHub Ticket #{_ticket.Id}",
            Author = "EventHub"
        };

        public DocumentSettings GetSettings() => DocumentSettings.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().AlignCenter().Text(t =>
                {
                    t.Span("EventHub — ").FontColor(Colors.Grey.Medium);
                    t.Span("Thank you for your purchase!").Italic().FontColor(Colors.Grey.Medium);
                });
            });
        }

        private void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("EventHub").FontSize(26).Bold().FontColor(Colors.Blue.Darken2);
                    col.Item().Text("Ticket Confirmation").FontSize(12).FontColor(Colors.Grey.Darken1);
                });

                row.ConstantItem(120).AlignRight().Column(col =>
                {
                    col.Item().Text($"Ticket #{_ticket.Id}").FontSize(10).FontColor(Colors.Grey.Medium);
                    col.Item().Text(_ticket.PurchasedOn.ToString("MMM dd, yyyy"))
                        .FontSize(10).FontColor(Colors.Grey.Medium);
                });
            });

            container.PaddingTop(4).LineHorizontal(1).LineColor(Colors.Blue.Lighten2);
        }

        private void ComposeContent(IContainer container)
        {
            container.PaddingTop(20).Column(col =>
            {
                col.Item().Text(_ticket.Event.Title).FontSize(18).Bold();
                col.Item().PaddingTop(4).Text(t =>
                {
                    t.Span("Status: ").Bold();
                    t.Span(_ticket.Status.ToString()).FontColor(Colors.Green.Darken2);
                });

                col.Item().PaddingTop(20).Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn(2);
                        cols.RelativeColumn(3);
                    });

                    void AddRow(string label, string value)
                    {
                        table.Cell().Padding(6).Background(Colors.Grey.Lighten4).Text(label).Bold();
                        table.Cell().Padding(6).Text(value);
                    }

                    AddRow("Event", _ticket.Event.Title);
                    AddRow("Date", _ticket.Event.StartDate.ToString("dddd, MMM dd, yyyy h:mm tt"));
                    AddRow("Venue", _ticket.Event.Venue?.Name ?? "—");
                    AddRow("Address", _ticket.Event.Venue?.Address ?? "—");
                    AddRow("Quantity", _ticket.Quantity.ToString());
                    AddRow("Price per Ticket", (_ticket.TotalPrice / _ticket.Quantity).ToString("C"));
                    AddRow("Total Paid", _ticket.TotalPrice.ToString("C"));
                    AddRow("Purchased On", _ticket.PurchasedOn.ToString("MMM dd, yyyy HH:mm"));
                });

                col.Item().PaddingTop(30).AlignCenter().Text(t =>
                {
                    t.Span("Please present this ticket at the venue entrance.").Italic().FontColor(Colors.Grey.Darken1);
                });
            });
        }
    }
}
