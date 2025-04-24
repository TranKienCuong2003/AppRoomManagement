using BLL;
using DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppRoomManagement
{
    public partial class frmCustomerManagement : Form
    {
        private readonly CustomerManagementBLL cmbll;
        private readonly ContextMenuStrip contextMenu;
        private byte[] img1 = null, img2 = null;

        public frmCustomerManagement()
        {
            InitializeComponent();
            cmbll = new CustomerManagementBLL();

            LoadDataGridView();
            LoadProvinces();
        }

        public void LoadDataGridView()
        {
            tblCustomer.DataSource = null;
            tblCustomer.DataSource = cmbll.GetListCustomer();

            tblCustomer.Columns["Id"].HeaderText = "ID";
            tblCustomer.Columns["FullName"].HeaderText = "Tên khách hàng";
            tblCustomer.Columns["PhoneNumber"].HeaderText = "SĐT";
            tblCustomer.Columns["DateOfBirth"].HeaderText = "Ngày sinh";
            tblCustomer.Columns["FullAddress"].HeaderText = "Địa chỉ";
            tblCustomer.Columns["CreatedAt"].HeaderText = "Ngày tạo";
            tblCustomer.Columns["UpdatedAt"].HeaderText = "Ngày đổi";

            tblCustomer.Columns["Id"].Width = 80;
            tblCustomer.Columns["PhoneNumber"].Width = 120;
            tblCustomer.Columns["CreatedAt"].Width = 120;
            tblCustomer.Columns["UpdatedAt"].Width = 120;
            tblCustomer.Columns["DateOfBirth"].Width = 100;

            tblCustomer.Columns["FirstName"].Visible = false;
            tblCustomer.Columns["LastName"].Visible = false;
            tblCustomer.Columns["Identity"].Visible = false;
            tblCustomer.Columns["Address"].Visible = false;
            tblCustomer.Columns["Ward"].Visible = false;
            tblCustomer.Columns["District"].Visible = false;
            tblCustomer.Columns["City"].Visible = false;
        }

        public void LoadProvinces()
        {
            cboProvinces.Items.Clear();
            List<string> lst = new List<string>();
            lst = cmbll.GetListProvinces();
            cboProvinces.DataSource = lst;
        }

        public void LoadDistricts(string provinces)
        {
            List<string> lst = new List<string>();
            int idProvinces = cmbll.GetIdProvinces(provinces);
            lst = cmbll.GetListDistricts(idProvinces);

            cboDistricts.DataSource = null;
            cboDistricts.Items.Clear();
            cboDistricts.DataSource = lst;
        }

        public void LoadWards(string district)
        {
            List<string> lst = new List<string>();
            int idDistricts = cmbll.GetIdDistrict(district);
            lst = cmbll.GetListWard(idDistricts);

            cboWards.DataSource = null;
            cboWards.Items.Clear();
            cboWards.DataSource = lst;
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            frmMain frm = new frmMain();
            frm.Show();
            this.Close();
        }

        private void CboProvinces_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDistricts(cboProvinces.Text.Trim());
        }

        private void CboDistricts_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadWards(cboDistricts.Text.Trim());
        }

        private void tblCustomer_Click(object sender, EventArgs e)
        {
            if (tblCustomer.CurrentRow == null)
                return;

            var row = tblCustomer.CurrentRow;
            LoadCustomerData(row);
            LoadCustomerImages(row.Cells["Id"].Value?.ToString().Trim());
        }

        private void LoadCustomerData(DataGridViewRow row)
        {
            lblId.Text = row.Cells["Id"].Value?.ToString().Trim();
            txtFirstName.Text = row.Cells["FirstName"].Value?.ToString().Trim();
            txtLastName.Text = row.Cells["LastName"].Value?.ToString().Trim();
            txtIdentify.Text = row.Cells["Identity"].Value?.ToString().Trim();
            txtPhone.Text = row.Cells["PhoneNumber"].Value?.ToString().Trim();
            dateOfBirth.Text = row.Cells["DateOfBirth"].Value?.ToString().Trim();
            txtAddress.Text = row.Cells["Address"].Value?.ToString().Trim();

            string city = row.Cells["City"].Value?.ToString().Trim();
            string district = row.Cells["District"].Value?.ToString().Trim();
            string ward = row.Cells["Ward"].Value?.ToString().Trim();

            LoadLocationData(city, district, ward);
        }

        private void LoadLocationData(string city, string district, string ward)
        {
            cboProvinces.Text = city;
            LoadDistricts(city);

            cboDistricts.Text = district;
            LoadWards(district);

            cboWards.Text = ward;
        }

        private void LoadCustomerImages(string customerId)
        {
            if (string.IsNullOrEmpty(customerId))
                return;

            var customerImages = cmbll.GetImageCustomer(customerId);
            if (customerImages == null)
                return;

            LoadImage(pic1, customerImages.img1, ref img1);
            LoadImage(pic2, customerImages.img2, ref img2);
        }

        private void LoadImage(Guna.UI2.WinForms.Guna2PictureBox pictureBox, byte[] imageData, ref byte[] targetImage)
        {
            if (imageData == null || imageData.Length == 0)
            {
                pictureBox.Image = null;
                targetImage = null;
                return;
            }

            using (var ms = new MemoryStream(imageData))
            {
                pictureBox.Image = Image.FromStream(ms);
            }
            targetImage = imageData;
        }

        private void HandleImageClick(Guna.UI2.WinForms.Guna2PictureBox pictureBox, ref byte[] imageData)
        {
            if (pictureBox.Image == null)
            {
                using (var openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif";
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        pictureBox.Image = Image.FromFile(openFileDialog.FileName);
                        using (var ms = new MemoryStream())
                        {
                            pictureBox.Image.Save(ms, pictureBox.Image.RawFormat);
                            imageData = ms.ToArray();
                        }
                    }
                }
            }
            else
            {
                pictureBox.Image = null;
                imageData = null;
            }
        }

        private void Pic1_Click(object sender, EventArgs e)
        {
            HandleImageClick(pic1, ref img1);
        }

        private void Pic2_Click(object sender, EventArgs e)
        {
            HandleImageClick(pic2, ref img2);
        }

        public void Reset()
        {
            lblId.Text = string.Empty;
            txtFirstName.Text = string.Empty;
            txtLastName.Text = string.Empty;
            txtIdentify.Text = string.Empty;
            txtPhone.Text = string.Empty;
            dateOfBirth.Value = DateTime.Now;
            txtAddress.Text = string.Empty;

            cboProvinces.SelectedIndex = 0;
            cboDistricts.DataSource = null;
            cboDistricts.Items.Clear();
            LoadDistricts(cboProvinces.Text);
            cboWards.DataSource = null;
            cboWards.Items.Clear();
            LoadWards(cboDistricts.Text);

            img1 = null;
            img2 = null;

            pic1.Image = null;
            pic2.Image = null;

            LoadDataGridView();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateCustomerInput())
                return;

            string newIdCustomer = cmbll.CreateNewIdCustomer();
            bool rs = cmbll.InsertCustomer(newIdCustomer, txtFirstName.Text, txtLastName.Text, txtIdentify.Text, txtPhone.Text,
                dateOfBirth.Value, txtAddress.Text, cboWards.Text, cboDistricts.Text, cboProvinces.Text, img1, img2);

            if (rs)
            {
                MessageBox.Show("Đã thêm mới thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Reset();
            }
            else
                MessageBox.Show("Thêm thất bại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(lblId.Text))
            {
                MessageBox.Show("Bạn chưa chọn khách hàng để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show("Bạn có chắc chắn muốn xóa khách hàng này không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                bool rs = cmbll.DeleteCustomer(lblId.Text.Trim());
                if (rs)
                {
                    MessageBox.Show("Xóa thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Reset();
                }
                else
                {
                    MessageBox.Show("Xóa thất bại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(lblId.Text))
            {
                MessageBox.Show("Bạn chưa chọn khách hàng để sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateCustomerInput())
                return;

            var result = MessageBox.Show("Bạn có chắc chắn muốn sửa khách hàng này không?", "Xác nhận sửa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                bool rs = cmbll.UpdateCustomer(lblId.Text, txtFirstName.Text, txtLastName.Text, txtIdentify.Text, txtPhone.Text,
                    dateOfBirth.Value, txtAddress.Text, cboWards.Text, cboDistricts.Text, cboProvinces.Text, img1, img2);

                if (rs)
                {
                    MessageBox.Show("Sửa thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Reset();
                }
                else
                {
                    MessageBox.Show("Sửa thất bại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private bool ValidateCustomerInput()
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên khách hàng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("Vui lòng nhập họ khách hàng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtIdentify.Text))
            {
                MessageBox.Show("Vui lòng nhập CCCD khách hàng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Vui lòng nhập SĐT khách hàng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                MessageBox.Show("Vui lòng nhập địa chỉ khách hàng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cboProvinces.SelectedIndex == -1 || cboDistricts.SelectedIndex == -1 || cboWards.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn đầy đủ thông tin địa chỉ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void TxtIdentity_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                return;
            }
            if (txtIdentify.Text.Length >= 12)
            {
                e.Handled = true;
            }
        }

        private void TxtPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                return;
            }
            if (txtPhone.Text.Length >= 10)
            {
                e.Handled = true;
            }
        }

        private bool IsValidCustomer(clsCustomerCustom1 customer)
        {
            if (string.IsNullOrEmpty(customer.CustomerId))
                return false;
            if (string.IsNullOrEmpty(customer.CustomerName))
                return false;
            if (string.IsNullOrEmpty(customer.PhoneNumber))
                return false;
            // Thêm các điều kiện validation khác
            return true;
        }
    }
}
