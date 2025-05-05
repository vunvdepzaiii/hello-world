using API.Services.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace API.Common
{
    public class WarningDocument : IDocument
    {
        private readonly WarningModel _model;

        public WarningDocument(WarningModel model)
        {
            _model = model;
        }

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(20);
                page.Size(PageSizes.A4);

                page.DefaultTextStyle(x => x.FontFamily("Times New Roman").FontSize(12));

                page.Content().Column(col =>
                {
                    // Header
                    col.Item().Row(row =>
                    {
                        row.RelativeItem();

                        row.ConstantItem(170).Column(c =>
                        {
                            c.Item().AlignCenter().Text(t =>
                            {
                                t.Span(_model.ManageName).SemiBold().FontSize(10);
                            });
                            c.Item().AlignCenter().Text(t =>
                            {
                                t.Span($"Hotline hành chính: {_model.HostlineAdmin}").FontSize(8);
                            });
                            c.Item().AlignCenter().Text(t =>
                            {
                                t.Span($"Hotline kỹ thuật 24/24: {_model.HostlineTech}").FontSize(8);
                            });
                        });
                    });

                    col.Item().PaddingVertical(10);

                    // Title
                    col.Item().AlignCenter().Text("PHIẾU THÔNG BÁO").Bold().FontSize(16);
                    col.Item().AlignCenter().Text("BILLING STATEMENT").Italic().FontSize(12);
                    col.Item().AlignCenter().Text($"Tháng/Month: {_model.Month}/{_model.Year}").SemiBold().FontSize(12);

                    col.Item().PaddingVertical(10);

                    var total = _model.ManagementFeeForward + _model.VehicleFeeForward + _model.WaterFeeForward + _model.ManagementFee + _model.VehicleFee + _model.WaterFee;

                    // Info
                    col.Item().Column(c =>
                    {
                        c.Item().PaddingBottom(2).Text(t =>
                        {
                            t.Span("Ngày/Date: ").SemiBold();
                            t.Span("\u00A0\u00A0\u00A0" + DateTime.Now.ToString("dd/MM/yyyy")).SemiBold();
                        });

                        col.Item().PaddingBottom(2).Row(r =>
                        {
                            r.RelativeItem(1).AlignLeft().Text(t =>
                            {
                                t.Span("Khách hàng/Customer: ").SemiBold();
                                t.Span("\u00A0\u00A0\u00A0" + _model.Owner).SemiBold();
                            });

                            r.RelativeItem(1).AlignLeft().Text(t =>
                            {
                                t.Span("Căn hộ/Apartment: ").SemiBold();
                                t.Span($"\u00A0\u00A0\u00A0{_model.BuildingViewModel?.Code}.{_model.ApartmentViewModel?.Number}").SemiBold();
                            });
                        });

                        col.Item().PaddingVertical(10).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(30);     // STT
                                columns.RelativeColumn(3);      // Diễn giải
                                columns.ConstantColumn(100);    // Thành tiền
                            });

                            table.Header(header =>
                            {
                                header.Cell().Border(0.5f).Padding(5).Text("TT").AlignCenter().Bold();
                                header.Cell().Border(0.5f).Padding(5).Text("DIỄN GIẢI\nDESCRIPTION").AlignCenter().Bold();
                                header.Cell().Border(0.5f).Padding(5).Text("THÀNH TIỀN\nAMOUNT").AlignCenter().Bold();
                            });

                            // Dòng dữ liệu
                            table.Cell().Border(0.5f).Padding(5).Text("1").AlignCenter().SemiBold();
                            table.Cell().Border(0.5f).Padding(5).Text("NỢ KỲ TRƯỚC / BALANCE FORWARD").AlignLeft().SemiBold();
                            table.Cell().Border(0.5f).Padding(5).Text($"{_model.ManagementFeeForward + _model.VehicleFeeForward + _model.WaterFeeForward:N0}").AlignRight().SemiBold();

                            table.Cell().Border(0.5f).Padding(5).Text("2").AlignCenter().SemiBold();
                            table.Cell().Border(0.5f).Padding(5).Text("Phí dịch vụ / Management fee").AlignLeft().SemiBold();
                            table.Cell().Border(0.5f).Padding(5).Text($"{_model.ManagementFee:N0}").AlignRight().SemiBold();

                            table.Cell().Border(0.5f).Padding(5).Text("3").AlignCenter().SemiBold();
                            table.Cell().Border(0.5f).Padding(5).Text("Phí gửi xe / Parking fees").AlignLeft().SemiBold();
                            table.Cell().Border(0.5f).Padding(5).Text($"{_model.VehicleFee:N0}").AlignRight().SemiBold();

                            var listVehicleFee = _model.ApartmentViewModel?.ListVehicleFeeViewModel ?? new List<VehicleFeeViewModel>();
                            if (listVehicleFee.Count > 0)
                            {
                                table.Cell().Border(0.5f).Padding(5).Text("").AlignRight();

                                table.Cell().Table(tbl1 =>
                                {
                                    tbl1.ColumnsDefinition(col1 =>
                                    {
                                        col1.ConstantColumn(100);   // Loại xe
                                        col1.RelativeColumn(3);     // Biển số
                                        col1.ConstantColumn(100);   // Phí gửi xe
                                    });

                                    tbl1.Header(head1 =>
                                    {
                                        head1.Cell().Border(0.5f).Padding(5).Text("Loại xe").AlignCenter().Bold();
                                        head1.Cell().Border(0.5f).Padding(5).Text("Biển số").AlignCenter().Bold();
                                        head1.Cell().Border(0.5f).Padding(5).Text("Phí gửi xe/tháng").AlignCenter().Bold();
                                    });
                                    foreach (var item in listVehicleFee)
                                    {
                                        tbl1.Cell().Border(0.5f).Padding(5).Text(item.TypeName).AlignLeft();
                                        tbl1.Cell().Border(0.5f).Padding(5).Text(item.License).AlignLeft();
                                        tbl1.Cell().Border(0.5f).Padding(5).Text($"{item.Price:N0}").AlignRight();
                                    }
                                });

                                table.Cell().Border(0.5f).Padding(5).Text("").AlignRight();
                            }

                            var waterBill = _model.WaterBillViewModel.FirstOrDefault();
                            string thang = waterBill == null ? "" : $"- {waterBill.BillingCycle}";

                            table.Cell().Border(0.5f).Padding(5).Text("4").AlignCenter().SemiBold();
                            table.Cell().Border(0.5f).Padding(5).Text($"Tiền nước / Water Consumption {thang}").AlignLeft().SemiBold();
                            table.Cell().Border(0.5f).Padding(5).Text($"{_model.WaterFee:N0}").AlignRight().SemiBold();

                            if (waterBill != null)
                            {
                                table.Cell().Border(0.5f).Padding(5).Text("").AlignRight();

                                table.Cell().ColumnSpan(2).Table(tbl2 =>
                                {
                                    tbl2.ColumnsDefinition(col2 =>
                                    {
                                        col2.ConstantColumn(100);   // Chỉ số đầu
                                        col2.ConstantColumn(100);   // Chỉ số cuối
                                        col2.ConstantColumn(100);   // Tổng tiêu thụ
                                        col2.RelativeColumn(5);     // Định mức tiêu thụ
                                        col2.ConstantColumn(100);   // Thành tiền
                                    });

                                    tbl2.Header(head2 =>
                                    {
                                        head2.Cell().Border(0.5f).Padding(5).Text("Chỉ số đầu").AlignCenter().Bold();
                                        head2.Cell().Border(0.5f).Padding(5).Text("Chỉ số cuối").AlignCenter().Bold();
                                        head2.Cell().Border(0.5f).Padding(5).Text("Tổng tiêu thụ").AlignCenter().Bold();
                                        head2.Cell().Border(0.5f).Padding(5).Text("Đinh mức tiêu thụ").AlignCenter().Bold();
                                        head2.Cell().Border(0.5f).Padding(5).Text("Thành tiền").AlignCenter().Bold();
                                    });

                                    tbl2.Cell().Border(0.5f).Padding(5).Text($"{waterBill.BeginNumber:N0}").AlignCenter();
                                    tbl2.Cell().Border(0.5f).Padding(5).Text($"{waterBill.EndNumber:N0}").AlignCenter();
                                    tbl2.Cell().Border(0.5f).Padding(5).Text($"{waterBill.EndNumber - waterBill.BeginNumber:N0}").AlignCenter();
                                    tbl2.Cell().Border(0.5f).Padding(5).Text("Sinh hoạt").AlignLeft();
                                    tbl2.Cell().Border(0.5f).Padding(5).Text($"{waterBill.FeedWaterFee:N0}").AlignRight();

                                    tbl2.Cell().Border(0.5f).Padding(5).Text("").AlignCenter();
                                    tbl2.Cell().Border(0.5f).Padding(5).Text("").AlignCenter();
                                    tbl2.Cell().Border(0.5f).Padding(5).Text("").AlignCenter();
                                    tbl2.Cell().Border(0.5f).Padding(5).Text("Phí thoát nước").AlignLeft();
                                    tbl2.Cell().Border(0.5f).Padding(5).Text($"{waterBill.WasteWaterFee:N0}").AlignRight();
                                });
                            }

                            table.Cell().Border(0.5f).Padding(5).Text("5").AlignCenter().SemiBold();
                            table.Cell().Border(0.5f).Padding(5).Text("Tổng cộng tiền thanh toán / Total due amount").AlignLeft().SemiBold();
                            table.Cell().Border(0.5f).Padding(5).Text($"{total:N0}").AlignRight().SemiBold();
                        });
                    });

                    string amountInWord = ServiceCommon.NumberToWords(total ?? 0);
                    col.Item().PaddingBottom(2).Text($"Số tiền bằng chữ/Amount in words: {amountInWord}.").SemiBold();

                    col.Item().PaddingBottom(2).Text(t =>
                    {
                        t.Span("Quý căn hộ vui lòng thanh toán các khoản phí trên trước ngày/Please pay before: ");
                        t.Span($"\u00A0\u00A0\u00A0{_model.Deadline}/{_model.Month}/{_model.Year}").SemiBold();
                    });
                    col.Item().PaddingBottom(2).Text("Địa điểm thanh toán: Thanh toán bằng tiền mặt tại văn phòng BQL hoặc qua hình thức chuyển khoản.");
                    col.Item().PaddingBottom(2).Text(t =>
                    {
                        t.Span("STK: ");
                        t.Span($"\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0{_model.BankNumber}").SemiBold();
                    });
                    col.Item().PaddingBottom(2).Text(t =>
                    {
                        t.Span("Tên TK: ");
                        t.Span($"\u00A0\u00A0\u00A0{_model.BankOwner}").SemiBold();
                    });
                    col.Item().PaddingBottom(2).Text(t =>
                    {
                        t.Span("Tại NH: ");
                        t.Span($"\u00A0\u00A0\u00A0{_model.BankName}").SemiBold();
                    });
                    col.Item().PaddingBottom(2).Text("Vui lòng khi chuyển khoản ghi nội dung CK là số căn hộ nhà mình: (VD: CT1M.0608 chuyển khoản).");
                    col.Item().PaddingBottom(2).Text("Thời gian thu phí trong giờ hành chính các ngày trong tuần từ thứ 2 đến thứ 6 và sáng thứ 7.");
                    col.Item().PaddingBottom(2).Text("Đề nghị Quý căn hộ vui lòng thanh toán đúng hạn. Sau thời hạn trên, nếu Quý căn hộ chưa thanh toán các khoản phí theo thông báo, " +
                        "BQL Tòa nhà buộc phải tạm ngưng cung cấp dịch vụ đối với Căn hộ.").Italic();

                    col.Item().PaddingVertical(10);

                    col.Item().PaddingBottom(2).Text("Trân trọng thông báo!.").Bold().FontSize(14);
                });
            });
        }
    }
}
