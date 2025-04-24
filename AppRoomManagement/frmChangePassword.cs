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

namespace AppRoomManagement
{
    public partial class frmChangePassword : Form
    {
        private ChangePasswordBLL cpbll;

        public frmChangePassword()
        {
            InitializeComponent();
            cpbll = new ChangePasswordBLL();
        }

        private void chk1_CheckedChanged(object sender, EventArgs e)
        {
            if (chk1.Checked)
            {
                txt1.PasswordChar = '\0';
            }
            else
            {
                txt1.PasswordChar = '*';
            }
        }

        private void chk2_CheckedChanged(object sender, EventArgs e)
        {
            if (chk2.Checked)
            {
                txt2.PasswordChar = '\0';
            }
            else
            {
                txt2.PasswordChar = '*';
            }
        }

        private void chk3_CheckedChanged(object sender, EventArgs e)
        {
            if (chk3.Checked)
            {
                txt3.PasswordChar = '\0';
            }
            else
            {
                txt3.PasswordChar = '*';
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txt1.Text) || string.IsNullOrEmpty(txt2.Text) || string.IsNullOrEmpty(txt3.Text))
                MessageBox.Show("Điền đầy đủ thông tin", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                bool chk1 = BCrypt.Net.BCrypt.Verify(txt1.Text, cpbll.GetPassword(frmLogin.idEmployee));
                if (chk1)
                {
                    if (txt2.Text.Equals(txt3.Text))
                    {
                        bool chk2 = cpbll.UpdatePasswordById(frmLogin.idEmployee, txt2.Text);
                        if (chk2)
                        {
                            txt1.Text = string.Empty;
                            txt2.Text = string.Empty;
                            txt3.Text = string.Empty;
                            MessageBox.Show("Thay đổi mật khẩu thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }    
                        else
                            MessageBox.Show("Thay đổi mật khẩu thất bại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                        MessageBox.Show("Xác nhận mật khẩu chưa trùng khớp với mật khẩu mới", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                    MessageBox.Show("Mật khẩu cũ không chính xác", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
