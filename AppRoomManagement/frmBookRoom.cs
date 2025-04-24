using BLL;
using DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;

namespace AppRoomManagement
{
    public partial class frmBookRoom : Form
    {
        private BookRoomBLL brbll;
        private AllFunctionLibraries func;
        private string idCustomer = string.Empty, nameCustomer = string.Empty, idNewContract;
        private bool checkIsCreated = true;

        public frmBookRoom()
        {
            InitializeComponent();
            brbll = new BookRoomBLL();
            func = new AllFunctionLibraries();
            LoadListCustomer();

            label6.Text = $"Chủ trọ: {brbll.GetNameOwnerManagement()}";
            label8.Text = $"Tên phòng: {frmMain.nameRoom}";
            label10.Text = $"Giá phòng: {frmMain.priceRoom}";
            startedDate.Value = DateTime.Now;
            endedDate.Value = DateTime.Now;
        }

        public void LoadListCustomer()
        {
            tblCustomer.DataSource = null;
            List<clsCustomerForContracts> list = new List<clsCustomerForContracts>();
            list = brbll.GetListCustomer();
            tblCustomer.DataSource = list;

            DesignDataGridView();
        }

        public void SearchCustomer(string search)
        {
            tblCustomer.DataSource = null;
            List<clsCustomerForContracts> list = new List<clsCustomerForContracts>();
            list = brbll.SearchCustomer(search);
            tblCustomer.DataSource = list;

            DesignDataGridView();
        }

        private void DesignDataGridView()
        {
            tblCustomer.Columns["id"].HeaderText = "ID";
            tblCustomer.Columns["name"].HeaderText = "Tên khách hàng";
            tblCustomer.Columns["identity"].HeaderText = "CCCD";
            tblCustomer.Columns["phone"].HeaderText = "SĐT";
        }

        public void Reset()
        {
            label5.Text = string.Empty;
            label6.Text = string.Empty;
            label8.Text = string.Empty;
            label10.Text = string.Empty;
            txtDeposite.Text = string.Empty;
            startedDate.Value = DateTime.Now;
            endedDate.Value = DateTime.Now;
        }

        private void guna2GradientButton3_Click(object sender, EventArgs e)
        {
            if (!checkIsCreated)
            {
                DialogResult result = MessageBox.Show(
                "Mọi thông tin sẽ mất\nBạn có chắc chắn muốn thoát không?",
                "Xác nhận thoát",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    brbll.DeleteContractAndContractHistory(idNewContract);

                    frmMain frmMain = new frmMain();
                    frmMain.Show();
                    this.Close();
                }
            }
            else
            {
                DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn thoát không?",
                "Xác nhận thoát",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    frmMain frmMain = new frmMain();
                    frmMain.Show();
                    this.Close();
                }
            }

        }

        private void tblCustomer_Click(object sender, EventArgs e)
        {
            if (tblCustomer.CurrentRow != null)
            {
                label5.Text = $"Tên khách hàng: {tblCustomer.CurrentRow.Cells["name"].Value.ToString().Trim()}";
                idCustomer = tblCustomer.CurrentRow.Cells["id"].Value.ToString().Trim();
                nameCustomer = tblCustomer.CurrentRow.Cells["name"].Value.ToString().Trim();
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearch.Text))
            {
                LoadListCustomer();
            }
            else
            {
                SearchCustomer(txtSearch.Text);
            }    
        }

        private void txtDeposite_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }


        private void btnFinishContract_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(label5.Text.Trim()))
            {
                MessageBox.Show("Chọn 1 khách hàng làm chủ hợp đồng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (string.IsNullOrEmpty(txtDeposite.Text.Trim()))
            {
                MessageBox.Show("Chưa nhập tiền đặt cọc", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (endedDate.Value < startedDate.Value)
            {
                MessageBox.Show("Ngày kết thúc không được nhỏ hơn ngày bắt đầu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if ((endedDate.Value - startedDate.Value).Days < 30)
            {
                MessageBox.Show("Thời gian hợp đồng phải có ít nhất 1 tháng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                string idEmployee = brbll.GetIdOwnerManagement();

                idNewContract = $"CON" +
                    $"-{DateTime.Now.ToString("yyyy")}{DateTime.Now.ToString("MM")}{DateTime.Now.ToString("dd")}" +
                    $"{DateTime.Now.ToString("HH")}{DateTime.Now.ToString("mm")}{DateTime.Now.ToString("ss")}";

                bool rs = brbll.CreateNewContract(idNewContract.Trim(), frmMain.idRoom, idCustomer, idEmployee,
                    startedDate.Value, endedDate.Value, decimal.Parse(txtDeposite.Text));

                if (rs)
                {
                    MessageBox.Show($"Hợp đồng: {idNewContract}\nKhách hàng: {nameCustomer}\nĐược tạo thành công", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    bool rsChangeStatusRoom = brbll.ChangeStatusRoom(frmMain.idRoom);
                    if (rsChangeStatusRoom)
                    {
                        MessageBox.Show($"Chuyển trạng thái phòng: {frmMain.idRoom} thành công\nPhòng {frmMain.idRoom}: Đã có người thuê", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        checkIsCreated = true;
                        btnPrintContract.Enabled = true;
                        btnFinishContract.Enabled = false;
                        Reset();
                    }
                    else
                    {
                        MessageBox.Show("Chuyển trạng thái phòng thất bại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        checkIsCreated = false;
                    }
                }
                else
                {
                    MessageBox.Show("Tạo hợp đồng thất bại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnPrintContract_Click(object sender, EventArgs e)
        {
            string addressHomestay = "Ngõ 36, Đường Xuân Thủy, Quận Cầu Giấy, Thành Phố Hà Nội";
            string theLastIdContract = brbll.GetLatestContractId();
            clsContractsToPDF item = new clsContractsToPDF();
            item = brbll.GetDetailContractToPdf(theLastIdContract);
            string numberOfRentalDay = func.CalculateMonthsAndDays(item.startedAt, item.endedAt);
            string priceRoomToWords = func.ConvertNumberToWords(item.priceRoom);
            string depositeToWords = func.ConvertNumberToWords(item.deposite);

            try
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
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi tạo PDF: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
