using BLL;
using DTO;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppRoomManagement
{
    public partial class frmBillManagement : Form
    {
        private BillManagementBLL bmbll;
        private string idBill = string.Empty;

        public frmBillManagement()
        {
            InitializeComponent();
            bmbll = new BillManagementBLL();
            LoadBill();
            LoadBillDetail();
            LoadElectricity();
            LoadWater();

            if (frmLogin.role.Trim().Equals("Nhân viên")) linkUpdate.Visible = false;
            else linkUpdate.Visible = true;
        }

        private void LoadBill()
        {
            tblBill.DataSource = null;
            List<clsBMBill> lst = new List<clsBMBill>();
            lst = bmbll.GetListBill();
            tblBill.DataSource = lst;
        }

        private void SearchBill(string id)
        {
            tblBill.DataSource = null;
            List<clsBMBill> lst = new List<clsBMBill>();
            lst = bmbll.GetBillById(id);
            tblBill.DataSource = lst;
        }

        private void LoadBillDetail()
        {
            tblBillDetail.DataSource = null;
            List<clsBMBillDetail> lst = new List<clsBMBillDetail>();
            lst = bmbll.GetListBillDetail();
            tblBillDetail.DataSource = lst;
        }

        private void SearchBillDetail(string id)
        {
            tblBillDetail.DataSource = null;
            List<clsBMBillDetail> lst = new List<clsBMBillDetail>();
            lst = bmbll.GetListBillDetailById(id);
            tblBillDetail.DataSource = lst;
        }

        private void LoadElectricity()
        {
            tblReadElectricity.DataSource = null;
            List<clsBMReadingElectricity> lst = new List<clsBMReadingElectricity>();
            lst = bmbll.GetListElectricity();
            tblReadElectricity.DataSource = lst;
        }

        private void SearchElectricity(string id)
        {
            tblReadElectricity.DataSource = null;
            List<clsBMReadingElectricity> lst = new List<clsBMReadingElectricity>();
            lst = bmbll.GetListElectricityById(id);
            tblReadElectricity.DataSource = lst;
        }

        private void LoadWater()
        {
            tblReadWater.DataSource = null;
            List<clsBMReadingWater> lst = new List<clsBMReadingWater>();
            lst = bmbll.GetListWater();
            tblReadWater.DataSource = lst;
        }

        private void SearchWater(string id)
        {
            tblReadWater.DataSource = null;
            List<clsBMReadingWater> lst = new List<clsBMReadingWater>();
            lst = bmbll.GetListWaterById(id);
            tblReadWater.DataSource = lst;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearch.Text))
            {
                LoadBill();
                LoadBillDetail();
                LoadElectricity();
                LoadWater();
            }
            else
            {
                SearchBill(txtSearch.Text);
                SearchBillDetail(txtSearch.Text);
                SearchElectricity(txtSearch.Text);
                SearchWater(txtSearch.Text);
            }
        }

        private void btnPrintBill_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(idBill))
            {
                MessageBox.Show("Bạn chưa chọn hóa đơn nào để in\n" +
                                "Hãy chọn 1 hóa đơn có sẵn trên bảng!", "Thông báo", 
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                try
                {
                    clsBillToPDF itemBill = bmbll.GetBillToPrintPDF(idBill);
                    if (itemBill == null || string.IsNullOrEmpty(itemBill.idBill))
                    {
                        MessageBox.Show("Không tìm thấy thông tin hóa đơn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    else
                    {
                        List<clsBillDetailToPDF> lst = bmbll.GetBillDetailToPrintPDF(itemBill.idBill.Trim());

                        // Tạo hộp thoại SaveFileDialog
                        SaveFileDialog saveFileDialog = new SaveFileDialog();
                        saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
                        saveFileDialog.Title = "Lưu hóa đơn";
                        saveFileDialog.FileName = $"HoaDon_{itemBill.idBill.Trim()}.pdf";

                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            // Tạo tài liệu PDF
                            Document document = new Document();
                            Section section = document.AddSection();

                            // Tiêu đề hóa đơn
                            Paragraph title = section.AddParagraph("HÓA ĐƠN TIỀN NHÀ");
                            title.Format.Font.Size = 18;
                            title.Format.Alignment = ParagraphAlignment.Center;
                            title.Format.Font.Bold = true;

                            section.AddParagraph($"Hóa đơn: {itemBill.idBill}").Format.Alignment = ParagraphAlignment.Center;
                            section.AddParagraph($"Ngày tạo: {itemBill.createdAt:dd/MM/yyyy}").Format.Alignment = ParagraphAlignment.Center;
                            section.AddParagraph("\n");

                            // Thông tin phòng
                            section.AddParagraph($"Phòng: {itemBill.nameRoom}").Format.Font.Bold = true;
                            section.AddParagraph($"Khách hàng: {itemBill.nameCustomer} ({itemBill.phoneCustomer})");
                            section.AddParagraph($"Nhân viên xử lý: {itemBill.nameEmployee} ({itemBill.phoneEmployee})");
                            section.AddParagraph("\n");

                            // Bảng chi tiết chi phí
                            Table table = section.AddTable();
                            table.Borders.Width = 0.75;
                            table.AddColumn(Unit.FromCentimeter(1)); // STT
                            table.AddColumn(Unit.FromCentimeter(8)); // Mục
                            table.AddColumn(Unit.FromCentimeter(4)); // Số tiền

                            Row headerRow = table.AddRow();
                            headerRow.Cells[0].AddParagraph("STT");
                            headerRow.Cells[1].AddParagraph("Mục");
                            headerRow.Cells[2].AddParagraph("Số tiền");
                            headerRow.Format.Font.Bold = true;
                            headerRow.Height = Unit.FromMillimeter(5);

                            // Dòng dữ liệu: Tiền phòng
                            Row row1 = table.AddRow();
                            row1.Cells[0].AddParagraph("1");
                            row1.Cells[1].AddParagraph("Tiền phòng");
                            row1.Cells[2].AddParagraph($"{itemBill.priceRoom:N0} VNĐ");
                            row1.Height = Unit.FromMillimeter(5);

                            // Dòng dữ liệu: Các chi phí khác
                            int index = 2;
                            foreach (clsBillDetailToPDF item in lst)
                            {
                                Row row2 = table.AddRow();
                                row2.Cells[0].AddParagraph(index.ToString());
                                row2.Cells[1].AddParagraph(item.utilityName);
                                row2.Cells[2].AddParagraph($"{item.amount.ToString("N0")}đ");
                                row2.Height = Unit.FromMillimeter(5);
                                index++;
                            }

                            // Tổng cộng
                            Row totalRow = table.AddRow();
                            totalRow.Cells[0].MergeRight = 1;
                            totalRow.Cells[0].AddParagraph("TỔNG CỘNG");
                            totalRow.Cells[2].AddParagraph($"{itemBill.totalAmount:N0} VNĐ");
                            totalRow.Format.Font.Bold = true;
                            totalRow.Height = Unit.FromMillimeter(5);

                            section.AddParagraph("\n");

                            // Ghi chú
                            section.AddParagraph("Đề nghị quý khách đóng đủ số tiền trên trong vòng 3 ngày kể từ ngày gửi hóa đơn này.");
                            section.AddParagraph("Quý khách cần giữ hóa đơn để đối chiếu số điện và số nước tháng tiếp theo.");
                            section.AddParagraph($"Mọi thắc mắc xin liên hệ: {itemBill.phoneEmployee}").Format.Font.Bold = true;

                            section.AddParagraph("\n");

                            // Chữ ký
                            Paragraph footer = section.AddParagraph();
                            footer.AddFormattedText("Người thanh toán", TextFormat.Bold);
                            footer.AddTab();
                            footer.AddTab();
                            footer.AddFormattedText("Người nhận TT", TextFormat.Bold);
                            footer.Format.Alignment = ParagraphAlignment.Center;

                            // Kết xuất PDF
                            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true);
                            pdfRenderer.Document = document;
                            pdfRenderer.RenderDocument();

                            // Lưu file tại đường dẫn được chọn
                            pdfRenderer.PdfDocument.Save(saveFileDialog.FileName);
                            pdfRenderer.PdfDocument.Close();

                            MessageBox.Show("Hóa đơn đã được tạo và lưu thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Đã xảy ra lỗi khi tạo PDF: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void tblBill_Click(object sender, EventArgs e)
        {
            if (tblBill.CurrentCell != null)
            {
                idBill = tblBill.CurrentRow.Cells["idBill"].Value.ToString();
                label1.Text = $"Bạn đang chọn hóa đơn có mã: {idBill}";
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            frmMain frmMain = new frmMain();
            frmMain.Show();
            this.Close();
        }

        private void linkUpdate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if(string.IsNullOrEmpty(idBill))
                MessageBox.Show("Bạn chưa chọn hóa đơn nào để in\n" +
                                "Hãy chọn 1 hóa đơn có sẵn trên bảng!", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                bool ck1 = bmbll.UpdateStatusBill(idBill);
                if (ck1)
                {
                    MessageBox.Show($"Cập nhật hóa đơn {idBill} thành công\n" +
                                    $"Trạng thái mới: 'Đã thanh toán'", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadBill();
                    LoadBillDetail();
                    LoadElectricity();
                    LoadWater();
                }
                else
                    MessageBox.Show("Thay đổi thất bại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }    
        }

        private bool IsValidBill(clsBillCustom1 bill)
        {
            if (string.IsNullOrEmpty(bill.BillId))
                return false;
            if (string.IsNullOrEmpty(bill.ContractId))
                return false;
            if (bill.TotalAmount <= 0)
                return false;
            if (bill.DateCreated == null)
                return false;
            // Thêm các điều kiện validation khác
            return true;
        }
    }
}
