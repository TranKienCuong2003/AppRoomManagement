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

namespace AppRoomManagement
{
    public partial class frmTypeRoomManagement : Form
    {
        TypeRoomManagementBLL trmbll;

        public frmTypeRoomManagement()
        {
            InitializeComponent();
            trmbll = new TypeRoomManagementBLL();

            LoadDataGridView();
        }

        public void LoadDataGridView()
        {
            List<clsTypeRoomCustom1> lst = new List<clsTypeRoomCustom1>();
            lst = trmbll.GetListTypeRoom();
            tblTypeRoom.DataSource = lst;

            tblTypeRoom.Columns["idtyperoom"].HeaderText = "Mã Loại Phòng";
            tblTypeRoom.Columns["nametyperoom"].HeaderText = "Loại Phòng";
            tblTypeRoom.Columns["quantity"].HeaderText = "Số Người Tối Đa";
            tblTypeRoom.Columns["created"].Visible = false;
            tblTypeRoom.Columns["updated"].Visible = false;
        }

        public void Reset()
        {
            LoadDataGridView();
            lblId.Text = "";
            txtNumber.Text = "";
            txtName.Text = "";
            lblCreatedAt.Text = "";
            lblupdateAt.Text = "";
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            frmMain frm = new frmMain();
            frm.Show();
            this.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            bool rs = trmbll.InsertTypeRoom(txtName.Text, int.Parse(txtNumber.Text));
            if (rs)
            {
                MessageBox.Show("Thêm thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Reset();
            }
            else
                MessageBox.Show("Thêm thất bại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void tblTypeRoom_Click(object sender, EventArgs e)
        {
            if (tblTypeRoom.CurrentRow != null)
            {
                lblId.Text = tblTypeRoom.CurrentRow.Cells["idtyperoom"].Value.ToString().Trim();
                txtName.Text = tblTypeRoom.CurrentRow.Cells["nametyperoom"].Value.ToString().Trim();
                txtNumber.Text = tblTypeRoom.CurrentRow.Cells["quantity"].Value.ToString().Trim();
                lblCreatedAt.Text = tblTypeRoom.CurrentRow.Cells["created"].Value.ToString().Trim();
                lblupdateAt.Text = tblTypeRoom.CurrentRow.Cells["updated"].Value.ToString().Trim();
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (lblId.Text == string.Empty)
            {
                MessageBox.Show("Bạn chưa chọn loại phòng để cập nhật!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                var result = MessageBox.Show("Bạn có chắc chắn muốn cập nhật loại phòng này không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    bool rs = trmbll.UpdateTypeRoom(int.Parse(lblId.Text.Trim()), txtName.Text, int.Parse(txtNumber.Text));
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lblId.Text == string.Empty)
            {
                MessageBox.Show("Bạn chưa chọn loại phòng để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                var result = MessageBox.Show("Bạn có chắc chắn muốn xóa loại phòng này không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    bool rs = trmbll.DeleteTypeRoom(int.Parse(lblId.Text.Trim()));
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

        private void txtNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                return;
            }
        }

        private bool IsValidTypeRoom(clsTypeRoomCustom1 typeRoom)
        {
            if (string.IsNullOrEmpty(typeRoom.TypeRoomId))
                return false;
            if (string.IsNullOrEmpty(typeRoom.TypeRoomName))
                return false;
            if (typeRoom.Price <= 0)
                return false;
            // Thêm các điều kiện validation khác
            return true;
        }
    }
}
