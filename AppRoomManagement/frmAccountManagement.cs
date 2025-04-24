using BLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace AppRoomManagement
{
    public partial class frmAccountManagement : Form
    {
        private AccountManagementBLL ambll;
        private int idChoose = 0;

        public frmAccountManagement()
        {
            InitializeComponent();
            ambll = new AccountManagementBLL();

            LoadDataGridView();
            LoadComboBox();
        }

        private void LoadDataGridView()
        {
            tblAcc.DataSource = null;
            List<EmployeeAccount> employees = new List<EmployeeAccount>();
            employees = ambll.GetListAccount();
            tblAcc.DataSource = employees;

            tblAcc.Columns["AccountID"].HeaderText = "ID";
            tblAcc.Columns["EmployeeID"].HeaderText = "Nhân viên";
            tblAcc.Columns["Username"].HeaderText = "Tài khoản";
            tblAcc.Columns["PasswordHash"].HeaderText = "";
            tblAcc.Columns["Status"].HeaderText = "Trạng thái";
            tblAcc.Columns["Role"].HeaderText = "Vai trò";
            tblAcc.Columns["CreatedAt"].HeaderText = "";
            tblAcc.Columns["UpdatedAt"].HeaderText = "";

            tblAcc.Columns["PasswordHash"].Visible = false;
            tblAcc.Columns["CreatedAt"].Visible = false;
            tblAcc.Columns["UpdatedAt"].Visible = false;
            tblAcc.Columns["Employee"].Visible = false;
        }

        private void LoadComboBox()
        {
            cboEmployee.DataSource = null;
            List<string> list = new List<string>();
            list = ambll.GetListEmployee();
            cboEmployee.DataSource = list;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tblAcc_Click(object sender, EventArgs e)
        {
            if (tblAcc.CurrentRow != null)
            {
                idChoose = int.Parse(tblAcc.CurrentRow.Cells["AccountID"].Value?.ToString().Trim());
                cboEmployee.Enabled = false;
                btnCreate.Enabled = false;
                txtUser.Text = tblAcc.CurrentRow.Cells["Username"].Value?.ToString().Trim();
                cboRole.Text = tblAcc.CurrentRow.Cells["Role"].Value?.ToString().Trim();

                if (tblAcc.CurrentRow.Cells["Status"].Value?.ToString().Trim() == "1")
                    cboStatus.SelectedIndex = 0;
                else
                    cboStatus.SelectedIndex = 1;
            }
        }

        private void txtUser_MouseClick(object sender, MouseEventArgs e)
        {
            cboEmployee.Enabled = true;
            btnCreate.Enabled = true;
        }

        private void Reset()
        {
            LoadDataGridView();
            LoadComboBox();
            txtUser.Text = string.Empty;
            txt1.Text = string.Empty;
            txt2.Text = string.Empty;
            cboStatus.SelectedIndex = 0;
            cboRole.SelectedIndex = 0;
            cboEmployee.Enabled = false;
            btnCreate.Enabled = false;
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            string id = cboEmployee.Text.Substring(0, 5);
            bool checkExit = ambll.CheckEmployeeAccountExists(id);
            if (checkExit)
                MessageBox.Show($"Nhân viên có ID: {id} đã có tài khoản", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                if (string.IsNullOrEmpty(txtUser.Text))
                    MessageBox.Show("Bắt buộc nhập tên tài khoản", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                {
                    bool checkUsername = ambll.CheckUsernameExists(txtUser.Text.Trim());
                    if (checkUsername)
                        MessageBox.Show($"Đã tồn tại tên tài khoản này", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else
                    {
                        int status;
                        string role;
                        if (cboStatus.SelectedIndex == 0)
                            status = 1;
                        else
                            status = 0;
                        if (cboStatus.SelectedIndex == 0)
                            role = "Nhân viên";
                        else
                            role = "Quản lý";

                        bool rs = ambll.InsertData(id, txtUser.Text.Trim(), "123", status, role);

                        if (rs)
                        {
                            MessageBox.Show("Tạo tài khoản thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Reset();
                        }
                        else
                            MessageBox.Show("Tạo tài khoản thất bại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }    
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {
            if (idChoose == 0)
            {
                MessageBox.Show("Bạn hãy chọn 1 tài khoản để reset mật khẩu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn reset mật khẩu cho tài khoản này không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    bool rs = ambll.ChangePassword(idChoose, "123");
                    if (rs)
                    {
                        MessageBox.Show("Reset mật khẩu thành công\nMật khẩu mới là: 123", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Reset();
                    }
                    else
                    {
                        MessageBox.Show("Reset mật khẩu thất bại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnChangeStatus_Click(object sender, EventArgs e)
        {
            if (idChoose == 0)
            {
                MessageBox.Show("Bạn hãy chọn 1 tài khoản để thay đổi", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                int status;
                if (cboStatus.SelectedIndex == 0)
                    status = 1;
                else
                    status = 0;
                bool rs = ambll.ChangeStatus(idChoose, status);
                if (rs)
                {
                    MessageBox.Show("Thay đổi trạng thái thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Reset();
                }
                else
                {
                    MessageBox.Show("Thay đổi trạng thái thất bại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnChangeRole_Click(object sender, EventArgs e)
        {
            if (idChoose == 0)
            {
                MessageBox.Show("Bạn hãy chọn 1 tài khoản để thay đổi", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                bool rs = ambll.ChangeRole(idChoose, cboRole.Text);
                if (rs)
                {
                    MessageBox.Show("Thay đổi vai trò thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Reset();
                }
                else
                {
                    MessageBox.Show("Thay đổi vai trò thất bại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool IsValidAccount(clsAccountCustom1 account)
        {
            if (string.IsNullOrEmpty(account.Username))
                return false;
            if (string.IsNullOrEmpty(account.Password))
                return false;
            if (string.IsNullOrEmpty(account.EmployeeId))
                return false;
            // Thêm các điều kiện validation khác
            return true;
        }
    }
}
