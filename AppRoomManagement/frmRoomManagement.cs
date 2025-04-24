using BLL;
using DTO;
using Guna.UI2.WinForms;
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
    public partial class frmRoomManagement : Form
    {
        RoomManagementBLL rmbll;
        public frmRoomManagement()
        {
            InitializeComponent();
            rmbll = new RoomManagementBLL();

            LoadDataGridView();
            LoadCombobox();
            
            // Vô hiệu hóa ComboBox trạng thái
            cboStatus.Enabled = false;
        }

        public void LoadDataGridView()
        {
            List<clsRoomCustom1> lst = new List<clsRoomCustom1>();
            lst = rmbll.GetListRoom();
            tblRoom.DataSource = lst;

            tblRoom.Columns["idroom"].HeaderText = "Mã Phòng";
            tblRoom.Columns["typeroomname"].HeaderText = "Loại Phòng";
            tblRoom.Columns["roomname"].HeaderText = "Tên Phòng";
            tblRoom.Columns["price"].HeaderText = "Giá";
            tblRoom.Columns["status"].HeaderText = "Trạng Thái";
            tblRoom.Columns["createdat"].Visible = false;
            tblRoom.Columns["updatedat"].Visible = false;
        }

        public void LoadCombobox()
        {
            List<string> lst = new List<string>();
            lst = rmbll.GetListTypeRoom();
            cboTypeRoom.DataSource = lst;
        }

        public void Reset()
        {
            LoadDataGridView();
            LoadCombobox();
            cboStatus.SelectedIndex = 0;
            lblId.Text = string.Empty;
            txtRoomNumber.Text = string.Empty;
            txtPrice.Text = string.Empty;
            lblCreatedAt.Text = string.Empty;
            lblUpdateAt.Text = string.Empty;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            frmMain frm = new frmMain();
            frm.Show();
            this.Close();
        }

        private void tblRoom_Click(object sender, EventArgs e)
        {
            if (tblRoom.CurrentRow != null)
            {
                lblId.Text = tblRoom.CurrentRow.Cells["idroom"].Value.ToString().Trim();
                cboTypeRoom.Text = tblRoom.CurrentRow.Cells["typeroomname"].Value.ToString().Trim();
                txtRoomNumber.Text = tblRoom.CurrentRow.Cells["roomname"].Value.ToString().Trim();
                txtPrice.Text = tblRoom.CurrentRow.Cells["price"].Value.ToString().Trim();
                cboStatus.Text = tblRoom.CurrentRow.Cells["status"].Value.ToString().Trim();
                lblCreatedAt.Text = tblRoom.CurrentRow.Cells["createdat"].Value.ToString().Trim();
                lblUpdateAt.Text = tblRoom.CurrentRow.Cells["updatedat"].Value.ToString().Trim();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            int idtype = rmbll.GetIdTypeRoom(cboTypeRoom.Text.Trim());
            bool rs = rmbll.InsertRoom(idtype, txtRoomNumber.Text, decimal.Parse(txtPrice.Text), cboStatus.SelectedIndex);
            if (rs)
            {
                MessageBox.Show("Thêm thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Reset();
            }
            else
                MessageBox.Show("Thêm thất bại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lblId.Text == string.Empty)
            {
                MessageBox.Show("Bạn chưa chọn phòng để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                var result = MessageBox.Show("Bạn có chắc chắn muốn xóa phòng này không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    bool rs = rmbll.DeleteRoom(int.Parse(lblId.Text.Trim()));
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
                MessageBox.Show("Bạn chưa chọn phòng để cập nhật!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show("Bạn có chắc chắn muốn cập nhật thông tin phòng này không?", "Xác nhận cập nhật", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Lấy trạng thái hiện tại của phòng
                var currentRoom = rmbll.GetRoomById(int.Parse(lblId.Text.Trim()));
                if (currentRoom == null)
                {
                    MessageBox.Show("Không tìm thấy thông tin phòng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int idtype = rmbll.GetIdTypeRoom(cboTypeRoom.Text.Trim());
                bool rs = rmbll.UpdateRoom(int.Parse(lblId.Text.Trim()), idtype, txtRoomNumber.Text.Trim(), decimal.Parse(txtPrice.Text), currentRoom.Status);
                
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

        private void txtPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                return;
            }
        }

        private bool IsValidRoom(clsRoomCustom1 room)
        {
            if (string.IsNullOrEmpty(room.RoomNumber))
                return false;
            if (room.Price <= 0)
                return false;
            // Thêm các điều kiện validation khác
            return true;
        }
    }
}
