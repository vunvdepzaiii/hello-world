using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace API.Common
{
    public class ReceiptDocument : IDocument
    {
        private readonly ReceiptModel _model;

        public ReceiptDocument(ReceiptModel model)
        {
            _model = model;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(20);
                page.Size(PageSizes.A5.Landscape());

                page.DefaultTextStyle(x => x.FontFamily("Times New Roman").FontSize(12));

                page.Content().Column(col =>
                {
                    // Header
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text(_model.CompanyName).SemiBold();
                            c.Item().Text(_model.CompanyAddress).FontSize(10);
                        });

                        row.ConstantItem(150).AlignRight().Column(c =>
                        {
                            c.Item().AlignCenter().Text(t =>
                            {
                                t.Span("Mẫu số: ").SemiBold().FontSize(10);
                                t.Span(_model.TemplateNo).FontSize(10);
                            });
                            c.Item().AlignCenter().Text(t =>
                            {
                                t.Span("Ban hành theo QĐ số: ").FontSize(8);
                                t.Span(_model.Decision).FontSize(8);
                            });
                            c.Item().AlignCenter().Text(_model.PlaceOfDecision).FontSize(8);
                        });
                    });

                    col.Item().PaddingVertical(10);

                    // Title
                    col.Item().AlignCenter().Text("PHIẾU THU").Bold().FontSize(18);
                    col.Item().Row(r =>
                    {
                        r.RelativeItem(1).Padding(5); // Cột trống trái

                        string date = _model.ReceiptDate.HasValue ? $"Ngày {_model.ReceiptDate.Value.Day} tháng {_model.ReceiptDate.Value.Month} năm {_model.ReceiptDate.Value.Year}"
                            : "Ngày ... tháng ... năm ...";
                        r.RelativeItem(1).AlignCenter().Padding(5).Text(text =>
                        {
                            text.Span(date).Italic();
                        });

                        r.RelativeItem(1).AlignRight().PaddingRight(20).Border(0.5f).Padding(5).Text(text =>
                        {
                            text.Span(_model.ReceiptCode).SemiBold().FontSize(11);
                        });
                    });

                    col.Item().PaddingVertical(5);

                    // Info
                    col.Item().Column(c =>
                    {
                        c.Item().PaddingBottom(2).Text(t =>
                        {
                            t.Span("Họ tên người nộp tiền: ").SemiBold();
                            t.Span("\u00A0\u00A0\u00A0" + _model.CustomerName).SemiBold();
                        });
                        string address = $"{_model.BuildingViewModel?.Code + "." ?? ""}{_model.ApartmentViewModel?.Number ?? ""}";
                        c.Item().PaddingBottom(2).Text(t =>
                        {
                            t.Span("Địa chỉ: ").SemiBold();
                            t.Span("\u00A0\u00A0\u00A0" + address);
                        });
                        c.Item().PaddingBottom(2).Text(t =>
                        {
                            t.Span("Lý do nộp: ").SemiBold();
                            t.Span("\u00A0\u00A0\u00A0" + _model.Content);
                        });
                        c.Item().PaddingBottom(2).Text(t =>
                        {
                            t.Span("Số tiền: ").SemiBold();
                            t.Span("\u00A0\u00A0\u00A0" + $"{_model.Amount:N0} đồng");
                        });
                        c.Item().PaddingBottom(2).Text(t =>
                        {
                            t.Span("Bằng chữ: ").SemiBold();
                            t.Span("\u00A0\u00A0\u00A0" + _model.AmountInWords);
                        });
                        c.Item().PaddingBottom(2).Text(t =>
                        {
                            t.Span("Hình thức thanh toán: ").SemiBold();
                            t.Span("\u00A0\u00A0\u00A0" + _model.PaymentMethod);
                        });
                    });

                    col.Item().PaddingVertical(10);

                    // Signatures
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().AlignCenter().Text("Giám đốc").SemiBold();
                            c.Item().AlignCenter().Text("(Ký, họ tên)").Italic().FontSize(10);
                        });

                        row.RelativeItem().Column(c =>
                        {
                            c.Item().AlignCenter().Text("Kế toán trưởng").SemiBold();
                            c.Item().AlignCenter().Text("(Ký, họ tên)").Italic().FontSize(10);
                        });

                        row.RelativeItem().Column(c =>
                        {
                            c.Item().AlignCenter().Text("Thủ quỹ").SemiBold();
                            c.Item().AlignCenter().Text("(Ký, họ tên)").Italic().FontSize(10);
                            c.Item().PaddingVertical(40);
                            c.Item().AlignCenter().Text(_model.Cashier).SemiBold();
                        });

                        row.RelativeItem().Column(c =>
                        {
                            c.Item().AlignCenter().Text("Người lập phiếu").SemiBold();
                            c.Item().AlignCenter().Text("(Ký, họ tên)").Italic().FontSize(10);
                            c.Item().PaddingVertical(40);
                            c.Item().AlignCenter().Text(_model.UserCreatedViewModel?.FullName).SemiBold();
                        });

                        row.RelativeItem().Column(c =>
                        {
                            c.Item().AlignCenter().Text("Người nộp tiền").SemiBold();
                            c.Item().AlignCenter().Text("(Ký, họ tên)").Italic().FontSize(10);
                            c.Item().PaddingVertical(40);
                            c.Item().AlignCenter().Text(_model.CustomerName).SemiBold();
                        });
                    });
                });
            });
        }
    }
}
