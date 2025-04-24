using BLL;
using DTO;
using MigraDoc.DocumentObjectModel.Tables;
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
    public partial class frmEmployeeManagement : Form
    {
        private EmployeeManagementBLL embll;
        private byte[] img1 = null, img2 = null;

        public frmEmployeeManagement()
        {
            InitializeComponent();
            embll = new EmployeeManagementBLL();

            LoadDataGridView();
            LoadProvinces();
        }

        private void LoadDataGridView()
        {
            tblEmp.DataSource = null;
            List<clsEmployeeCustom1> lst = new List<clsEmployeeCustom1>();
            lst = embll.GetListEmployees();
            tblEmp.DataSource = lst;

            tblEmp.Columns["id"].HeaderText = "ID";
            tblEmp.Columns["fullName"].HeaderText = "Tên nhân viên";
            tblEmp.Columns["identity"].HeaderText = "CCCD";
            tblEmp.Columns["phoneNumber"].HeaderText = "SDT";
            tblEmp.Columns["dateOfBirth"].HeaderText = "Ngày sinh";
            tblEmp.Columns["fullAddress"].HeaderText = "Địa chỉ";
            tblEmp.Columns["createdAt"].HeaderText = "Ngày tạo";
            tblEmp.Columns["updatedAt"].HeaderText = "Ngày sửa";

            tblEmp.Columns["firstName"].Visible = false;
            tblEmp.Columns["lastName"].Visible = false;
            tblEmp.Columns["address"].Visible = false;
            tblEmp.Columns["ward"].Visible = false;
            tblEmp.Columns["district"].Visible = false;
            tblEmp.Columns["city"].Visible = false;

            tblEmp.Columns["id"].Width = 80;
            tblEmp.Columns["identity"].Width = 100;
            tblEmp.Columns["phoneNumber"].Width = 100;
            tblEmp.Columns["dateOfBirth"].Width = 100;
        }

        public void LoadProvinces()
        {
            cboProvinces.Items.Clear();
            List<string> lst = new List<string>();
            lst = embll.GetListProvinces();
            cboProvinces.DataSource = lst;
        }

        public void LoadDistricts(string provinces)
        {
            List<string> lst = new List<string>();
            int idProvinces = embll.GetIdProvinces(provinces);
            lst = embll.GetListDistricts(idProvinces);

            cboDistricts.DataSource = null;
            cboDistricts.Items.Clear();
            cboDistricts.DataSource = lst;
        }

        public void LoadWards(string district)
        {
            List<string> lst = new List<string>();
            int idDistricts = embll.GetIdDistrict(district);
            lst = embll.GetListWard(idDistricts);

            cboWards.DataSource = null;
            cboWards.Items.Clear();
            cboWards.DataSource = lst;
        }

        private void cboProvinces_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDistricts(cboProvinces.Text.Trim());
        }

        private void cboDistricts_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadWards(cboDistricts.Text.Trim());
        }

        private void tblEmp_Click(object sender, EventArgs e)
        {
            if (tblEmp.CurrentRow != null)
            {
                lblId.Text = tblEmp.CurrentRow.Cells["id"].Value?.ToString().Trim();
                txtLastName.Text = tblEmp.CurrentRow.Cells["lastName"].Value?.ToString().Trim();
                txtName.Text = tblEmp.CurrentRow.Cells["firstName"].Value?.ToString().Trim();
                txtIdentity.Text = tblEmp.CurrentRow.Cells["identity"].Value?.ToString().Trim();
                txtPhone.Text = tblEmp.CurrentRow.Cells["phoneNumber"].Value?.ToString().Trim();
                dateTimePicker1.Text = tblEmp.CurrentRow.Cells["dateOfBirth"].Value?.ToString().Trim();
                txtAddress.Text = tblEmp.CurrentRow.Cells["address"].Value?.ToString().Trim();
                lblCreated.Text = tblEmp.CurrentRow.Cells["createdAt"].Value?.ToString().Trim();
                lblUpdate.Text = tblEmp.CurrentRow.Cells["updatedAt"].Value?.ToString().Trim();

                string city = tblEmp.CurrentRow.Cells["city"].Value?.ToString().Trim();
                string district = tblEmp.CurrentRow.Cells["district"].Value?.ToString().Trim();
                string ward = tblEmp.CurrentRow.Cells["ward"].Value?.ToString().Trim();

                cboProvinces.Text = city;
                LoadDistricts(city);

                cboDistricts.Text = district;
                LoadWards(district);

                cboWards.Text = ward;

                clsImageIdentityEmployee clsImageIdentityCustomer = embll.GetImageEmployee(lblId.Text.Trim());

                if (clsImageIdentityCustomer?.img1 != null && clsImageIdentityCustomer.img1.Length > 0)
                {
                    using (MemoryStream ms = new MemoryStream(clsImageIdentityCustomer.img1))
                    {
                        pic1.Image = Image.FromStream(ms);
                    }
                }
                else
                {
                    pic1.Image = null;
                }

                if (clsImageIdentityCustomer?.img2 != null && clsImageIdentityCustomer.img2.Length > 0)
                {
                    using (MemoryStream ms = new MemoryStream(clsImageIdentityCustomer.img2))
                    {
                        pic2.Image = Image.FromStream(ms);
                    }
                }
                else
                {
                    pic2.Image = null;
                }

                img1 = clsImageIdentityCustomer.img1;
                img2 = clsImageIdentityCustomer.img2;
            }
        }

        private void pic1_Click(object sender, EventArgs e)
        {
            if (pic1.Image == null)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    pic1.Image = Image.FromFile(openFileDialog.FileName);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        pic1.Image.Save(ms, pic1.Image.RawFormat);
                        img1 = ms.ToArray();
                    }
                }
            }
            else
            {
                pic1.Image = null;
                img1 = null;
            }
        }

        private void pic2_Click(object sender, EventArgs e)
        {
            if (pic2.Image == null)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    pic2.Image = Image.FromFile(openFileDialog.FileName);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        pic2.Image.Save(ms, pic2.Image.RawFormat);
                        img2 = ms.ToArray();
                    }
                }
            }
            else
            {
                pic2.Image = null;
                img2 = null;
            }
        }

        public void Reset()
        {
            lblId.Text = string.Empty;
            txtName.Text = string.Empty;
            txtLastName.Text = string.Empty;
            txtIdentity.Text = string.Empty;
            txtPhone.Text = string.Empty;
            dateTimePicker1.Value = DateTime.Now;
            txtAddress.Text = string.Empty;
            lblCreated.Text = string.Empty;
            lblUpdate.Text = string.Empty;

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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lblId.Text == string.Empty)
            {
                MessageBox.Show("Bạn chưa chọn để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                var result = MessageBox.Show("Bạn có chắc chắn muốn xóa không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    bool rs = embll.DeleteEmployee(lblId.Text.Trim());
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
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (lblId.Text == string.Empty)
            {
                MessageBox.Show("Bạn chưa chọn để sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                var result = MessageBox.Show("Bạn có chắc chắn muốn sửa không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    bool rs = embll.UpdateEmployee(lblId.Text, txtName.Text, txtLastName.Text, txtIdentity.Text, txtPhone.Text,
                dateTimePicker1.Value, txtAddress.Text, cboWards.Text, cboDistricts.Text, cboProvinces.Text, img1, img2);

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
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtIdentity_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                return;
            }
            if (txtIdentity.Text.Length >= 12)
            {
                e.Handled = true;
                return;
            }    
        }

        private void txtPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                return;
            }
            if (txtPhone.Text.Length >= 10)
            {
                e.Handled = true;
                return;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string newId = embll.CreateNewIdEmployee();
            bool rs = embll.InsertData(newId, txtName.Text, txtLastName.Text, txtIdentity.Text, txtPhone.Text, dateTimePicker1.Value,
                txtAddress.Text, cboWards.Text, cboDistricts.Text, cboProvinces.Text, img1, img2);
            if (rs)
            {
                MessageBox.Show("Đã thêm mới thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Reset();
            }
            else
                MessageBox.Show("Thêm thất bại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private bool IsValidEmployee(clsEmployeeCustom1 employee)
        {
            if (string.IsNullOrEmpty(employee.EmployeeId))
                return false;
            if (string.IsNullOrEmpty(employee.EmployeeName))
                return false;
            if (string.IsNullOrEmpty(employee.PhoneNumber))
                return false;
            if (string.IsNullOrEmpty(employee.Email))
                return false;
            // Thêm các điều kiện validation khác
            return true;
        }
    }
}
