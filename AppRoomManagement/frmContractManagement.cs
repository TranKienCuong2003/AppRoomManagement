using BLL;
using DTO;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppRoomManagement
{
    public partial class frmContractManagement : Form
    {
        private ContractManagementBLL cmbll;
        private AllFunctionLibraries func;
        private string idContract = string.Empty;

        public frmContractManagement()
        {
            InitializeComponent();
            cmbll = new ContractManagementBLL();
            func = new AllFunctionLibraries();
            LoadDataGridView();
        }

        private void LoadDataGridView()
        {
            tblContract.DataSource = null;
            List<clsContractDetail> lst = cmbll.GetData();
            tblContract.DataSource = lst;

            DesignDataGridView();
        }

        private void SearchDataGridView(string search)
        {
            tblContract.DataSource = null;
            List<clsContractDetail> lst = cmbll.SearchData(search);
            tblContract.DataSource = lst;

            DesignDataGridView();
        }

        private void DesignDataGridView()
        {
            tblContract.Columns["idContract"].HeaderText = "Mã hợp đồng";
            tblContract.Columns["idRoom"].HeaderText = "Mã phòng";
            tblContract.Columns["roomName"].HeaderText = "Tên phòng";
            tblContract.Columns["roomType"].HeaderText = "Loại phòng";
            tblContract.Columns["roomPrice"].HeaderText = "Giá phòng";
            tblContract.Columns["idCustomer"].HeaderText = "Mã khách hàng";
            tblContract.Columns["nameCustomer"].HeaderText = "Tên khách hàng";
            tblContract.Columns["phoneCustomer"].HeaderText = "Số điện thoại khách hàng";
            tblContract.Columns["idEmployee"].HeaderText = "Mã nhân viên";
            tblContract.Columns["nameEmployee"].HeaderText = "Tên nhân viên";
            tblContract.Columns["phoneEmployee"].HeaderText = "Số điện thoại nhân viên";
            tblContract.Columns["startedAt"].HeaderText = "Ngày bắt đầu";
            tblContract.Columns["endedAt"].HeaderText = "Ngày kết thúc";
            tblContract.Columns["deposite"].HeaderText = "Tiền đặt cọc";
            tblContract.Columns["status"].HeaderText = "Trạng thái";
            tblContract.Columns["createdAt"].HeaderText = "Ngày tạo";

            tblContract.Columns["idRoom"].Visible = false;
            tblContract.Columns["roomType"].Visible = false;
            tblContract.Columns["phoneCustomer"].Visible = false;
            tblContract.Columns["phoneEmployee"].Visible = false;
        }

        private void tblContract_Click(object sender, EventArgs e)
        {
            if (tblContract.CurrentRow != null)
            {
                idContract = tblContract.CurrentRow.Cells["idContract"].Value.ToString().Trim();
                label1.Text = $"Bạn đang chọn hợp đồng mã: {idContract}";
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(idContract))
            {
                string addressHomestay = "Ngõ 36, Đường Xuân Thủy, Quận Cầu Giấy, Thành Phố Hà Nội";
                clsContractsToPDF item = new clsContractsToPDF();
                item = cmbll.GetDetailContractToPdf(idContract);
                string numberOfRentalDay = func.CalculateMonthsAndDays(item.startedAt, item.endedAt);
                string priceRoomToWords = func.ConvertNumberToWords(item.priceRoom);
                string depositeToWords = func.ConvertNumberToWords(item.deposite);

                try
                {
                    var confirmResult = MessageBox.Show(
                            $"Bạn có chắc chắn muốn in hợp đồng: {idContract} không?",
                            "Xác nhận in",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                    if (confirmResult == DialogResult.Yes)
                    {
                        SaveFileDialog saveFileDialog = new SaveFileDialog();
                        saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
                        saveFileDialog.Title = "Lưu hợp đồng";
                        saveFileDialog.FileName = $"HopDong_{item.idContract.Trim()}.pdf";

                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            Document document = new Document();
                            Section section = document.AddSection();

                            section.AddParagraph("CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM").Format.Alignment = ParagraphAlignment.Center;
                            Paragraph paragraph = section.AddParagraph("Độc lập – Tự do – Hạnh phúc");
                            paragraph.Format.Alignment = ParagraphAlignment.Center;
                            paragraph.Format.Font.Bold = true;
                            paragraph.Format.Font.Underline = Underline.Single;

                            Paragraph title = section.AddParagraph("HỢP ĐỒNG THUÊ PHÒNG TRỌ");
                            title.Format.Font.Size = 18;
                            title.Format.Alignment = ParagraphAlignment.Center;
                            title.Format.Font.Bold = true;
                            section.AddParagraph("\n");

                            section.AddParagraph($"\nHà Nội, ngày {DateTime.Now.ToString("dd")} tháng {DateTime.Now.ToString("MM")} năm {DateTime.Now.ToString("yyyy")}, tại căn nhà số {addressHomestay}." +
                                $"\nTheo mã hợp đồng: {item.idContract}, chúng tôi ký tên dưới đây gồm có:\n");

                            section.AddParagraph("\nBÊN CHO THUÊ PHÒNG TRỌ (gọi tắt là Bên A):").Format.Font.Bold = true;
                            section.AddParagraph($"Ông/bà: {item.nameOwner}");
                            section.AddParagraph($"SDT: {item.phoneOwner}");
                            section.AddParagraph($"CMND/CCCD số: {item.identityOwner} cấp ngày ............... nơi cấp ..................");
                            section.AddParagraph($"Thường trú tại: {item.addressOwner}\n");

                            section.AddParagraph("\nBÊN THUÊ PHÒNG TRỌ (gọi tắt là Bên B):").Format.Font.Bold = true;
                            section.AddParagraph($"Ông/bà: {item.nameCustomer}");
                            section.AddParagraph($"SDT: {item.phoneCustomer}");
                            section.AddParagraph($"CMND/CCCD số: {item.identityCustomer} cấp ngày ............... nơi cấp ..................");
                            section.AddParagraph($"Thường trú tại: {item.addressCustomer}\n");

                            section.AddParagraph("\n1. Nội dung thuê phòng trọ");
                            section.AddParagraph($"\nBên A cho Bên B thuê 01 phòng trọ số {item.roomNumber} tại căn nhà số {addressHomestay}, thời hạn {numberOfRentalDay}, giá thuê {item.priceRoom.ToString("N0")} đồng (bằng chữ {priceRoomToWords}).");

                            section.AddParagraph("\n2. Trách nhiệm Bên A");
                            section.AddParagraph("\nĐảm bảo căn nhà cho thuê không có tranh chấp, khiếu kiện.").Format.Alignment = ParagraphAlignment.Justify; ;
                            section.AddParagraph("Đăng ký với chính quyền địa phương về thủ tục cho thuê phòng trọ.").Format.Alignment = ParagraphAlignment.Justify; ;

                            section.AddParagraph("\n3. Trách nhiệm Bên B");
                            section.AddParagraph($"\nĐặt cọc với số tiền là {item.deposite.ToString("N0")} đồng (bằng chữ {depositeToWords}), thanh toán tiền thuê phòng hàng tháng vào ngày {item.startedAt.Date} + tiền điện + nước.").Format.Alignment = ParagraphAlignment.Justify; ;
                            section.AddParagraph("Đảm bảo các thiết bị và sửa chữa các hư hỏng trong phòng trong khi sử dụng. Nếu không sửa chữa thì khi trả phòng, bên A sẽ trừ vào tiền đặt cọc, giá trị cụ thể được tính theo giá thị trường.").Format.Alignment = ParagraphAlignment.Justify; ;
                            section.AddParagraph("Chỉ sử dụng phòng trọ vào mục đích ở, không chứa các thiết bị gây cháy nổ, hàng cấm... cung cấp giấy tờ tùy thân để đăng ký tạm trú theo quy định, giữ gìn an ninh trật tự, nếp sống văn hóa đô thị; không tụ tập nhậu nhẹt, cờ bạc và các hành vi vi phạm pháp luật khác.").Format.Alignment = ParagraphAlignment.Justify; ;
                            section.AddParagraph("Không được tự ý cải tạo kiếm trúc phòng hoặc trang trí ảnh hưởng tới tường, cột, nền... Nếu có nhu cầu trên phải trao đổi với bên A để được thống nhất").Format.Alignment = ParagraphAlignment.Justify; ;


                            section.AddParagraph("\n4. Điều khoản thực hiện");
                            section.AddParagraph("\nHai bên nghiêm túc thực hiện những quy định trên trong thời hạn cho thuê, nếu bên A lấy phòng phải báo cho bên B ít nhất 01 tháng, hoặc ngược lại.").Format.Alignment = ParagraphAlignment.Justify; ;
                            section.AddParagraph($"Sau thời hạn cho thuê {numberOfRentalDay} tháng nếu bên B có nhu cầu hai bên tiếp tục thương lượng giá thuê để gia hạn hợp đồng bằng miệng hoặc thực hiện gia hạn").Format.Alignment = ParagraphAlignment.Justify; ;

                            section.AddParagraph("\n");
                            section.AddParagraph("\n");
                            Paragraph signB = section.AddParagraph("Bên B");
                            signB.Format.Alignment = ParagraphAlignment.Left;

                            Paragraph signA = section.AddParagraph("Bên A");
                            signA.Format.Alignment = ParagraphAlignment.Right;

                            section.AddParagraph("(Ký, ghi rõ họ tên)").Format.Alignment = ParagraphAlignment.Left;
                            section.AddParagraph("(Ký, ghi rõ họ tên)").Format.Alignment = ParagraphAlignment.Right;

                            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true);
                            pdfRenderer.Document = document;
                            pdfRenderer.RenderDocument();

                            pdfRenderer.PdfDocument.Save(saveFileDialog.FileName);
                            pdfRenderer.PdfDocument.Close();

                            MessageBox.Show("In hợp đồng thành công, file đã được lưu trong thư mục MyDocuments.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Đã xảy ra lỗi khi tạo PDF: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
                MessageBox.Show("Bạn chưa chọn hợp đồng nào để in", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearch.Text))
            {
                LoadDataGridView();
            }
            else
            {
                SearchDataGridView(txtSearch.Text);
            }    
        }

        private void guna2GradientButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private bool IsValidContract(clsContractCustom1 contract)
        {
            if (string.IsNullOrEmpty(contract.ContractId))
                return false;
            if (string.IsNullOrEmpty(contract.CustomerId))
                return false;
            if (string.IsNullOrEmpty(contract.RoomId))
                return false;
            if (contract.StartDate == null || contract.EndDate == null)
                return false;
            if (contract.StartDate >= contract.EndDate)
                return false;
            // Thêm các điều kiện validation khác
            return true;
        }
    }
}
