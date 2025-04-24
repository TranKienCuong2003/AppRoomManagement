using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using BLL;
using System.Windows.Forms;
using DTO;

namespace AppRoomManagement
{
    public partial class frmLogin : Form
    {
        LoginBLL loginBLL = new LoginBLL();
        public static string idEmployee = string.Empty, role = string.Empty;

        public frmLogin()
        {
            InitializeComponent();
        }

        private void Login()
        {
            clsGetInfoAfterLogin item = new clsGetInfoAfterLogin();
            if (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Text))
                MessageBox.Show("Nhập đủ thông tin", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                item = loginBLL.GetInfoAfterLogin(txtUsername.Text.Trim());
                if (item.status != 1)
                    MessageBox.Show("Tài khoản đã bị khóa đăng nhập", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                {
                    bool login = loginBLL.GetLogin(txtUsername.Text.Trim(), txtPassword.Text.Trim());
                    if (login)
                    {
                        idEmployee = item.idEmployee;
                        role = item.role;
                        //idEmployee = loginBLL.GetIdEmployee(txtUsername.Text.Trim());
                        frmMain frmMain = new frmMain();
                        frmMain.Show();
                        this.Hide();
                    }
                    else
                        MessageBox.Show("Tên tài khoản hoặc mật khẩu chưa chính xác", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            Login();
        }

        private void chk1_CheckedChanged(object sender, EventArgs e)
        {
            if (chk1.Checked)
            {
                txtPassword.PasswordChar = '\0';
            }
            else
            {
                txtPassword.PasswordChar = '*';
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Login();
                e.Handled = true; // Ngăn không cho sự kiện được xử lý thêm
                e.SuppressKeyPress = true; // Ngăn tiếng 'beep' khi nhấn Enter
            }
        }
    }
}
