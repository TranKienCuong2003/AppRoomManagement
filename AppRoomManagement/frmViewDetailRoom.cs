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
using MigraDoc.DocumentObjectModel.Tables;

namespace AppRoomManagement
{
    public partial class frmViewDetailRoom : Form
    {
        private ViewDetailRoomBLL vdrbll;
        private AllFunctionLibraries func;
        private string idContract = string.Empty, idCustomer = string.Empty, idCustomerToDelete = string.Empty;
        private DateTime? dateChooseValue = null;
        clsViewDetailRoom item = new clsViewDetailRoom();

        public frmViewDetailRoom()
        {
            InitializeComponent();
            vdrbll = new ViewDetailRoomBLL();
            func = new AllFunctionLibraries();
            item = vdrbll.GetDetailRoom(frmMain.idRoom);
            ShowInformationContract();
            LoadDataCustomer();
            LoadDataCustomerGroupMember();
            NotifyStatusContract();
        }

        private void ShowInformationContract()
        {
            idContract = item.idContract.Trim();
            label1.Text = "XEM CHI TIẾT PHÒNG CÓ HỢP ĐỒNG: " + item.idContract;
            label4.Text = item.nameRoom;
            label5.Text = item.idCustomer;
            label8.Text = item.nameCustomer;
            label6.Text = item.phoneCustomer;
            label11.Text = item.priceRoom.ToString("N0") + "đ";
            label23.Text = item.startedAt.ToString("dd/MM/yyyy");
            label25.Text = item.endedAt.ToString("dd/MM/yyyy");
            label27.Text = item.status;
            label30.Text = item.createdAt.ToString("dd/MM/yyyy");


            bool chk3 = vdrbll.IsElectricityReadingsNotEmpty();
            //Có dữ liệu trong bảng ElectricityReadings
            if (chk3)
            {
                decimal theLastWriteElectricity = vdrbll.GetTheLastElectricityReadingsByCreatedAt(frmMain.idRoom);
                txtOldReadingElectricity.Text = theLastWriteElectricity.ToString();
            }
            //Chưa có dữ liệu trong bảng ElectricityReadings
            else
            {
                txtOldReadingElectricity.Text = "0";
            }

            bool chk4 = vdrbll.IsWaterReadingsNotEmpty();
            //Có dữ liệu trong bảng WaterReadings
            if (chk4)
            {
                decimal theLastWriteWater = vdrbll.GetTheLastWaterReadingsByCreatedAt(frmMain.idRoom);
                txtOldReadingWater.Text = theLastWriteWater.ToString();
            }
            //Chưa có dữ liệu trong bảng WaterReadings
            else
            {
                txtOldReadingWater.Text = "0";
            }

            string sumPriceElec = vdrbll.SumPriceElectricityLastesByIdContract(item.idContract).ToString("N0");
            label15.Text = $"Tiền điện gần đây là: {sumPriceElec}đ";

            string sumPriceWater = vdrbll.SumPriceWaterLastesByIdContract(item.idContract).ToString("N0");
            label17.Text = $"Tiền nước gần đây là: {sumPriceWater}đ";

            string utl1 = vdrbll.GetUtility("DV003").ToString("N0") + "đ";
            label19.Text = utl1;

            string utl2 = vdrbll.GetUtility("DV004").ToString("N0") + "đ";
            label21.Text = utl2;
        }

        private void NotifyStatusContract()
        {
            string statusContractByEndDate = vdrbll.NotifyContractStatus(idContract);
            label31.Text = statusContractByEndDate.ToUpper();
        }

        private void LoadDataCustomer()
        {
            tblCustomer.DataSource = null;
            List<clsCustomerCustom2> lst = new List<clsCustomerCustom2>();
            lst = vdrbll.GetListCustomer();
            tblCustomer.DataSource = lst;

            tblCustomer.Columns["id"].HeaderText = "Mã KH";
            tblCustomer.Columns["name"].HeaderText = "Tên KH";
            tblCustomer.Columns["phone"].HeaderText = "SDT";
        }

        private void SerarchDataCustomer(string idCustomer)
        {
            tblCustomer.DataSource = null;
            List<clsCustomerCustom2> lst = new List<clsCustomerCustom2>();
            lst = vdrbll.SearchListCustomer(idCustomer);
            tblCustomer.DataSource = lst;

            tblCustomer.Columns["id"].HeaderText = "Mã KH";
            tblCustomer.Columns["name"].HeaderText = "Tên KH";
            tblCustomer.Columns["phone"].HeaderText = "SDT";
        }

        private void LoadDataCustomerGroupMember()
        {
            btnDeleteCustomerGroup.DataSource = null;
            List<clsCustomerGroup> lst = new List<clsCustomerGroup>();
            lst = vdrbll.GetCustomerGroups(item.idContract);
            btnDeleteCustomerGroup.DataSource = lst;

            btnDeleteCustomerGroup.Columns["idContract"].HeaderText = "Mã HĐ";
            btnDeleteCustomerGroup.Columns["idCustomer"].HeaderText = "Mã KH";
            btnDeleteCustomerGroup.Columns["nameCustomer"].HeaderText = "Tên KH";
            btnDeleteCustomerGroup.Columns["createdAt"].HeaderText = "Ngày tạo";
            btnDeleteCustomerGroup.Columns["updatedAt"].HeaderText = "Ngày sửa";

            btnDeleteCustomerGroup.Columns["idContract"].Visible = false;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnWriteElectricity_Click(object sender, EventArgs e)
        {
            //Nếu muốn test tính tiền hóa đơn có chính xác chưa?
            //Đổi vdrbll.CheckElectricityReadingRecorded(idContract) thành !vdrbll.CheckElectricityReadingRecorded(idContract)
            if (vdrbll.CheckElectricityReadingRecorded(idContract))
            {
                MessageBox.Show($"Tiền điện trong tháng {DateTime.Now.ToString("MM")} đã được ghi", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (decimal.TryParse(txtOldReadingElectricity.Text, out decimal oldReadingElectricity_output) &&
                    decimal.TryParse(txtNewReadingElectricity.Text, out decimal newReadingElectricity_output))
                {
                    if (oldReadingElectricity_output > newReadingElectricity_output)
                    {
                        MessageBox.Show("Số ghi điện mới phải lớn hơn số ghi điện cũ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtNewReadingElectricity.Focus();
                    }
                    else
                    {
                        bool chk1 = vdrbll.IsElectricityReadingsNotEmpty();
                        decimal theLastWriteElectricity = 0;

                        // Có dữ liệu trong bảng ElectricityReadings
                        if (chk1)
                        {
                            theLastWriteElectricity = vdrbll.GetTheLastElectricityReadingsByCreatedAt(frmMain.idRoom);
                            string monthyear = $"{DateTime.Now.ToString("yyyy")}-{DateTime.Now.ToString("MM")}";

                            // Kiểm tra xem giá trị mới nhập có hợp lệ không
                            if (decimal.TryParse(txtNewReadingElectricity.Text, out decimal newReadingElectricity))
                            {
                                bool rs = vdrbll.InsertDataElectricityForContract(idContract.Trim(), monthyear, theLastWriteElectricity, newReadingElectricity);

                                if (rs)
                                {
                                    MessageBox.Show("Thêm mức điện cho phòng này thành công");

                                    // Cập nhật lại mức điện cũ sau khi thêm thành công
                                    txtOldReadingElectricity.Text = vdrbll.GetTheLastElectricityReadingsByCreatedAt(frmMain.idRoom).ToString();
                                    txtNewReadingElectricity.Text = string.Empty;

                                    // Tính tiền điện
                                    decimal electricityUnit = vdrbll.GetUnitElectticity();
                                    decimal sumelectricityPrice = (newReadingElectricity - theLastWriteElectricity) * electricityUnit;

                                    lblElectricityPrice.Text = sumelectricityPrice.ToString("N0") + "đ";
                                }
                                else
                                {
                                    MessageBox.Show("Thêm thất bại");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Giá trị mức điện mới không hợp lệ");
                            }
                        }
                        // Chưa có dữ liệu trong bảng ElectricityReadings
                        else
                        {
                            theLastWriteElectricity = 0;
                            string monthyear = $"{DateTime.Now.ToString("yyyy")}-{DateTime.Now.ToString("MM")}";

                            // Kiểm tra xem giá trị mới nhập có hợp lệ không
                            if (decimal.TryParse(txtNewReadingElectricity.Text, out decimal newReadingElectricity))
                            {
                                bool rs = vdrbll.InsertDataElectricityForContract(idContract.Trim(), monthyear, theLastWriteElectricity, newReadingElectricity);

                                if (rs)
                                {
                                    // Cập nhật lại mức điện cũ sau khi thêm thành công
                                    txtOldReadingElectricity.Text = vdrbll.GetTheLastElectricityReadingsByCreatedAt(frmMain.idRoom).ToString();
                                    txtNewReadingElectricity.Text = string.Empty;

                                    // Tính tiền điện
                                    decimal electricityUnit = vdrbll.GetUnitElectticity();
                                    decimal sumelectricityPrice = (newReadingElectricity - theLastWriteElectricity) * electricityUnit;

                                    lblElectricityPrice.Text = sumelectricityPrice.ToString("N0") + "đ";

                                }
                                else
                                {
                                    MessageBox.Show("Thêm thất bại");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Giá trị mức điện mới không hợp lệ");
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Giá trị mức điện cũ hoặc mức điện mới không hợp lệ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNewReadingElectricity.Focus();
                }
            }
        }


        private void btnWriteWater_Click(object sender, EventArgs e)
        {
            //Nếu muốn test tính tiền hóa đơn có chính xác chưa?
            //Đổi vdrbll.CheckWaterReadingRecorded(idContract) thành !vdrbll.CheckWaterReadingRecorded(idContract)
            if (vdrbll.CheckWaterReadingRecorded(idContract))
            {
                MessageBox.Show($"Tiền nước trong tháng {DateTime.Now.ToString("MM")} đã được ghi", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (decimal.TryParse(txtOldReadingWater.Text, out decimal oldReadingWater_output) &&
                decimal.TryParse(txtNewReadingWater.Text, out decimal newReadingWater_output))
                {
                    if (oldReadingWater_output > newReadingWater_output)
                    {
                        MessageBox.Show("Số ghi nước mới phải lớn hơn số ghi nước cũ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtNewReadingWater.Focus();
                    }
                    else
                    {
                        bool chk1 = vdrbll.IsWaterReadingsNotEmpty();
                        decimal theLastWriteWater = 0;

                        // Có dữ liệu trong bảng WaterReadings
                        if (chk1)
                        {
                            theLastWriteWater = vdrbll.GetTheLastWaterReadingsByCreatedAt(frmMain.idRoom);
                            string monthyear = $"{DateTime.Now.ToString("yyyy")}-{DateTime.Now.ToString("MM")}";

                            // Kiểm tra xem giá trị mới nhập có hợp lệ không
                            if (decimal.TryParse(txtNewReadingWater.Text, out decimal newReadingWater))
                            {
                                bool rs = vdrbll.InsertDataWaterForContract(idContract.Trim(), monthyear, theLastWriteWater, newReadingWater);

                                if (rs)
                                {
                                    // Cập nhật lại mức nước cũ sau khi thêm thành công
                                    txtOldReadingWater.Text = vdrbll.GetTheLastWaterReadingsByCreatedAt(frmMain.idRoom).ToString();
                                    txtNewReadingWater.Text = string.Empty;

                                    // Tính tiền nước
                                    decimal waterUnit = vdrbll.GetUnitWater();
                                    decimal sumWaterPrice = (newReadingWater - theLastWriteWater) * waterUnit;

                                    lblWaterPrice.Text = sumWaterPrice.ToString("N0") + "đ";
                                }
                                else
                                {
                                    MessageBox.Show("Thêm thất bại");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Giá trị mức nước mới không hợp lệ");
                            }
                        }
                        // Chưa có dữ liệu trong bảng WaterReadings
                        else
                        {
                            theLastWriteWater = 0;
                            string monthyear = $"{DateTime.Now.ToString("yyyy")}-{DateTime.Now.ToString("MM")}";

                            // Kiểm tra xem giá trị mới nhập có hợp lệ không
                            if (decimal.TryParse(txtNewReadingWater.Text, out decimal newReadingWater))
                            {
                                bool rs = vdrbll.InsertDataWaterForContract(idContract.Trim(), monthyear, theLastWriteWater, newReadingWater);

                                if (rs)
                                {
                                    // Cập nhật lại mức nước cũ sau khi thêm thành công
                                    txtOldReadingWater.Text = vdrbll.GetTheLastWaterReadingsByCreatedAt(frmMain.idRoom).ToString();
                                    txtNewReadingWater.Text = string.Empty;

                                    // Tính tiền nước
                                    decimal waterUnit = vdrbll.GetUnitWater();
                                    decimal sumWaterPrice = (newReadingWater - theLastWriteWater) * waterUnit;

                                    lblWaterPrice.Text = sumWaterPrice.ToString("N0") + "đ";
                                }
                                else
                                {
                                    MessageBox.Show("Thêm thất bại");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Giá trị mức nước mới không hợp lệ");
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Giá trị mức nước cũ hoặc mức nước mới không hợp lệ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNewReadingWater.Focus();
                }
            }
        }

        private void txtNewReadingElectricity_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)8 && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && txtNewReadingElectricity.Text.Contains("."))
            {
                e.Handled = true;
            }
        }

        private void txtNewReadingWater_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)8 && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && txtNewReadingWater.Text.Contains("."))
            {
                e.Handled = true;
            }
        }

        private void guna2GradientButton5_Click(object sender, EventArgs e)
        {
            frmMain frmMain = new frmMain();
            frmMain.Show();
            this.Close();
        }

        private void addGiaHan_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            dateTimePicker1.Visible = true;
            addGiaHan.Visible = false;
            cancleGiaHan.Visible = true;
        }

        private void cancleGiaHan_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            dateTimePicker1.Visible = false;
            addGiaHan.Visible = true;
            cancleGiaHan.Visible = false;

            dateChooseValue = null;
        }

        private void dateGiaHan_ValueChanged(object sender, EventArgs e)
        {
            dateChooseValue = dateTimePicker1.Value;
        }

        private void btnSaveInfoBills_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtOldReadingElectricity.Text.Equals("0"))
                    MessageBox.Show("Bạn chưa ghi tiền điện!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                {
                    if (txtOldReadingWater.Text.Equals("0"))
                        MessageBox.Show("Bạn chưa ghi tiền nước!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else
                    {

                        decimal unitElectricity = vdrbll.GetElectricityLastesByIdContract(idContract);
                        decimal unitWater = vdrbll.GetWaterLastesByIdContract(idContract);
                        List<clsUtilitiesIDAndUnit> utilitiesList = vdrbll.GetListUtilitiesToAmountForBill();

                        bool contractExists = vdrbll.CheckContractHaveExitInBill(idContract);
                        if (contractExists)
                        {
                            bool result = vdrbll.CheckAllServicesAddedToBillDetails(idContract);
                            //Nếu muốn test tính tiền hóa đơn có chính xác chưa?
                            //Đổi result thành !result
                            if (result)
                            {
                                MessageBox.Show("Tất cả dịch vụ đã được thêm trong tháng này.");
                            }
                            else
                            {
                                decimal totalAmount = 0;
                                foreach (var utility in utilitiesList)
                                {
                                    if (utility.UtilityID.Trim().Equals("DV001"))
                                    {
                                        decimal electricityCost = utility.UnitPrice * unitElectricity;
                                        totalAmount += electricityCost;
                                    }
                                    else if (utility.UtilityID.Trim().Equals("DV002"))
                                    {
                                        decimal waterCost = utility.UnitPrice * unitWater;
                                        totalAmount += waterCost;
                                    }
                                    else
                                    {
                                        totalAmount += utility.UnitPrice;
                                    }
                                }

                                decimal totalAmountWithPriceRoom = totalAmount + item.priceRoom;
                                string billID = $"HD{DateTime.Now:ddMMyyyyHHmmss}";
                                bool rsBill = vdrbll.InsertBill(billID, idContract, frmLogin.idEmployee, totalAmountWithPriceRoom);

                                if (!rsBill)
                                {
                                    MessageBox.Show("Thêm hóa đơn thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }

                                string idBillAfterCreate = vdrbll.GetIdBillByIdContract(idContract);
                                bool isAllDetailsInserted = true;

                                foreach (var utility in utilitiesList)
                                {
                                    bool rs;
                                    if (utility.UtilityID.Trim().Equals("DV001"))
                                    {
                                        decimal electricityCost = utility.UnitPrice * unitElectricity;
                                        rs = vdrbll.InsertBillDetails(idBillAfterCreate, utility.UtilityID, electricityCost);
                                    }
                                    else if (utility.UtilityID.Trim().Equals("DV002"))
                                    {
                                        decimal waterCost = utility.UnitPrice * unitWater;
                                        rs = vdrbll.InsertBillDetails(idBillAfterCreate, utility.UtilityID, waterCost);
                                    }
                                    else
                                    {
                                        rs = vdrbll.InsertBillDetails(idBillAfterCreate, utility.UtilityID, utility.UnitPrice);
                                    }

                                    if (!rs)
                                    {
                                        isAllDetailsInserted = false;
                                        break;
                                    }
                                }

                                if (isAllDetailsInserted)
                                {
                                    MessageBox.Show("Thêm hóa đơn và chi tiết hóa đơn thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    MessageBox.Show("Thêm chi tiết hóa đơn thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }

                        }
                        else
                        {
                            decimal totalAmount = 0;
                            foreach (var utility in utilitiesList)
                            {
                                if (utility.UtilityID.Trim().Equals("DV001"))
                                {
                                    decimal electricityCost = utility.UnitPrice * unitElectricity;
                                    totalAmount += electricityCost;
                                }
                                else if (utility.UtilityID.Trim().Equals("DV002"))
                                {
                                    decimal waterCost = utility.UnitPrice * unitWater;
                                    totalAmount += waterCost;
                                }
                                else
                                {
                                    totalAmount += utility.UnitPrice;
                                }
                            }

                            decimal totalAmountWithPriceRoom = totalAmount + item.priceRoom;
                            string billID = $"HD{DateTime.Now:ddMMyyyyHHmmss}";
                            bool rsBill = vdrbll.InsertBill(billID, idContract, frmLogin.idEmployee, totalAmountWithPriceRoom);

                            if (!rsBill)
                            {
                                MessageBox.Show("Thêm hóa đơn thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            string idBillAfterCreate = vdrbll.GetIdBillByIdContract(idContract);
                            bool isAllDetailsInserted = true;

                            foreach (var utility in utilitiesList)
                            {
                                bool rs;
                                if (utility.UtilityID.Trim().Equals("DV001"))
                                {
                                    decimal electricityCost = utility.UnitPrice * unitElectricity;
                                    rs = vdrbll.InsertBillDetails(idBillAfterCreate, utility.UtilityID, electricityCost);
                                }
                                else if (utility.UtilityID.Trim().Equals("DV002"))
                                {
                                    decimal waterCost = utility.UnitPrice * unitWater;
                                    rs = vdrbll.InsertBillDetails(idBillAfterCreate, utility.UtilityID, waterCost);
                                }
                                else
                                {
                                    rs = vdrbll.InsertBillDetails(idBillAfterCreate, utility.UtilityID, utility.UnitPrice);
                                }

                                if (!rs)
                                {
                                    isAllDetailsInserted = false;
                                    break;
                                }
                            }

                            if (isAllDetailsInserted)
                            {
                                MessageBox.Show("Thêm hóa đơn và chi tiết hóa đơn thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("Thêm chi tiết hóa đơn thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRenewContract_Click(object sender, EventArgs e)
        {
            if (!dateChooseValue.HasValue)
            {
                MessageBox.Show("Bạn chưa chọn ngày để gia hạn", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (dateChooseValue.Value <= item.endedAt)
                {
                    MessageBox.Show("Bạn phải chọn ngày gia hạn sau ngày kết thúc của hợp đồng\n" +
                        $"Ngày kết thúc hợp đồng: {item.endedAt.ToString("dd/MM/yyyy")}\n" +
                        $"Ngày gia hạn đang chọn: {dateChooseValue.Value.ToString("dd/MM/yyyy")}\n" +
                        $"Lỗi", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if ((dateChooseValue.Value - item.endedAt).TotalDays < 30)
                {
                    MessageBox.Show("Thời gian gia hạn phải ít nhất là 1 tháng\n" +
                        $"Ngày kết thúc hợp đồng: {item.endedAt.ToString("dd/MM/yyyy")}\n" +
                        $"Ngày gia hạn đang chọn: {dateChooseValue.Value.ToString("dd/MM/yyyy")}\n" +
                        $"Lỗi", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    var confirmResult = MessageBox.Show(
                        $"Bạn có chắc chắn muốn gia hạn hợp đồng đến ngày {dateChooseValue.Value.ToString("dd/MM/yyyy")} không?",
                        "Xác nhận gia hạn",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (confirmResult == DialogResult.Yes)
                    {
                        bool rs = vdrbll.RenewContractById(idContract, dateChooseValue.Value);
                        if (!rs)
                        {
                            MessageBox.Show("Gia hạn hợp đồng thất bại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            MessageBox.Show("Gia hạn hợp đồng thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            NotifyStatusContract();
                            ShowInformationContract();
                            dateTimePicker1.Visible = false;
                            addGiaHan.Visible = true;
                            cancleGiaHan.Visible = false;

                            dateChooseValue = null;
                        }
                    }
                }
            }
        }

        private void btnCancleContract_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show(
                $"Bạn có chắc chắn muốn hủy hợp đồng: {idContract} này không?",
                "Xác nhận hủy",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );
            if (confirmResult == DialogResult.Yes)
            {
                bool rs = vdrbll.CancleContractById(idContract);
                if (rs)
                {
                    MessageBox.Show("Hủy hợp đồng thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    vdrbll.UpdateEmptyRoom(item.idRoom);
                    frmMain frm = new frmMain();
                    frm.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Hủy hợp đồng thất bại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnPrintBill_Click(object sender, EventArgs e)
        {
            bool rs = vdrbll.CheckHaveExitBillByIdContract(idContract);
            if (!rs)
            {
                MessageBox.Show("Bạn chưa lưu thông tin hóa đơn của hợp đồng này\n" +
                                "Hãy chọn 'Lưu thông tin hóa đơn'", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                try
                {
                    clsBillToPDF itemBill = vdrbll.GetBillToPrintPDF(idContract);
                    if (itemBill == null || string.IsNullOrEmpty(itemBill.idBill))
                    {
                        MessageBox.Show("Không tìm thấy thông tin hóa đơn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    else
                    {
                        List<clsBillDetailToPDF> lst = vdrbll.GetBillDetailToPrintPDF(itemBill.idBill.Trim());

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

        private void btnAddGroupMember_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(idCustomer))
                MessageBox.Show("Bạn chưa chọn khách hàng nào!\nHãy chọn khách hàng từ bảng để tạo nhóm ở ghép",
                                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                var confirmResult = MessageBox.Show(
                        $"Bạn có chắc chắn muốn thêm khách hàng có mã: {idCustomer} vào nhóm này không?",
                        "Xác nhận thêm",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                if (confirmResult == DialogResult.Yes)
                {
                    bool ck1 = vdrbll.CheckHaveExitCustomerInCustomerGroup(idCustomer);
                    if (ck1)
                        MessageBox.Show("Khách hàng này đã được ghép nhóm\nVui lòng chọn khách hàng khác",
                                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else
                    {
                        bool ck2 = vdrbll.InsertCustomerToCustomerGroup(item.idContract, idCustomer);
                        if (ck2)
                        {
                            MessageBox.Show("Thêm khách hàng vào nhóm thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            idCustomer = string.Empty;
                            LoadDataCustomerGroupMember();
                        }
                        else
                        {
                            MessageBox.Show("Thêm khách hàng vào nhóm thất bại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            idCustomer = string.Empty;
                        }
                    }
                }
            }
        }

        private void tblCustomer_Click(object sender, EventArgs e)
        {
            if (tblCustomer.CurrentRow != null)
            {
                idCustomer = tblCustomer.CurrentRow.Cells["id"].Value.ToString().Trim();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(idCustomerToDelete))
                MessageBox.Show("Bạn chưa chọn khách hàng nào!\nHãy chọn khách hàng từ bảng để xóa ở ghép",
                                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                var confirmResult = MessageBox.Show(
                        $"Bạn có chắc chắn muốn xóa khách hàng có mã: {idCustomerToDelete}, xóa khỏi nhóm này không?",
                        "Xác nhận xóa",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);
                if (confirmResult == DialogResult.Yes)
                {
                    bool ck1 = vdrbll.DeleteCustomerFromCustomerGroup(idCustomerToDelete);
                    if (ck1)
                    {
                        MessageBox.Show("Xóa khách hàng khỏi nhóm này thành công",
                                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadDataCustomerGroupMember();
                        idCustomerToDelete = string.Empty;
                    }
                    else
                    {
                        MessageBox.Show("Xóa khách hàng khỏi nhóm này thất bại",
                                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        LoadDataCustomerGroupMember();
                        idCustomerToDelete = string.Empty;
                    }    
                }    
            }
        }

        private void btnPrintContract_Click(object sender, EventArgs e)
        {
            string addressHomestay = "Ngõ 36, Đường Xuân Thủy, Quận Cầu Giấy, Thành Phố Hà Nội";
            string theLastIdContract = vdrbll.GetLatestContractId();
            clsContractsToPDF item = new clsContractsToPDF();
            item = vdrbll.GetDetailContractToPdf(theLastIdContract);
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

                    Paragraph title = section.AddParagraph("HỢP ĐỒNG THUÊ PHÒNG TRỌ");
                    title.Format.Font.Size = 18;
                    title.Format.Alignment = ParagraphAlignment.Center;
                    title.Format.Font.Bold = true;
                    section.AddParagraph("\n");

                    section.AddParagraph("CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM").Format.Alignment = ParagraphAlignment.Center;
                    Paragraph paragraph = section.AddParagraph("Độc lập – Tự do – Hạnh phúc");
                    paragraph.Format.Alignment = ParagraphAlignment.Center;
                    paragraph.Format.Font.Bold = true;
                    paragraph.Format.Font.Underline = Underline.Single;


                    section.AddParagraph($"\nHôm nay, ngày {DateTime.Now.ToString("dd")} tháng {DateTime.Now.ToString("MM")} năm {DateTime.Now.ToString("yyyy")}, tại căn nhà số {addressHomestay}." +
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

                    MessageBox.Show("In hợp đồng thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi tạo PDF: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tblCustomerGroup_Click(object sender, EventArgs e)
        {
            if (btnDeleteCustomerGroup.CurrentRow != null)
            {
                idCustomerToDelete = btnDeleteCustomerGroup.CurrentRow.Cells["idCustomer"].Value.ToString().Trim();
            }
        }

        private void txtSearchCustomer_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearchCustomer.Text))
                LoadDataCustomer();
            else
                SerarchDataCustomer(txtSearchCustomer.Text.Trim());
        }
    }
}
