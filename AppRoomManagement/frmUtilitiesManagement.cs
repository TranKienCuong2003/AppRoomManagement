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
    public partial class frmUtilitiesManagement : Form
    {
        private UtilitiesManagementBLL umbll;
        private string id = string.Empty;

        public frmUtilitiesManagement()
        {
            InitializeComponent();
            umbll = new UtilitiesManagementBLL();

            LoadDataList();
        }

        private void LoadDataList()
        {
            tblUtilities.DataSource = null;
            List<clsUtilitiesCustom1> lst = new List<clsUtilitiesCustom1>();
            lst = umbll.GetListUtilities();
            tblUtilities.DataSource = lst;

            tblUtilities.Columns["id"].HeaderText = "ID";
            tblUtilities.Columns["name"].HeaderText = "Tên dịch vụ";
            tblUtilities.Columns["price"].HeaderText = "Giá";
            tblUtilities.Columns["createdAt"].HeaderText = "Ngày tạo";
            tblUtilities.Columns["updatedAt"].HeaderText = "Ngày cập nhật";

            tblUtilities.Columns["id"].Width = 100;
            tblUtilities.Columns["price"].Width = 100;

        }

        private void Reset()
        {
            LoadDataList();
            txtName.Text = string.Empty;
            txtPrice.Text = string.Empty;
            txtSearch.Text = string.Empty;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string idUtility = umbll.CreateNewIdUtility();

            if (string.IsNullOrEmpty(idUtility))
                MessageBox.Show("Tạo khóa chính thất bại","Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                if (string.IsNullOrEmpty(txtName.Text) || string.IsNullOrEmpty(txtPrice.Text))
                    MessageBox.Show("Phải điền đủ thông tin", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                {
                    bool rs = umbll.InsertData(idUtility, txtName.Text, decimal.Parse(txtPrice.Text));
                    if (rs)
                    {
                        MessageBox.Show("Thêm thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Reset();
                    }
                    else
                        MessageBox.Show("Thêm thất bại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }    
            }    
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn xóa không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                bool rs = umbll.DeleteData(id);
                if (rs)
                {
                    MessageBox.Show("Xóa thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Reset();
                }
                else
                    MessageBox.Show("Xóa thất bại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tblUtilities_Click(object sender, EventArgs e)
        {
            if (tblUtilities.CurrentRow != null)
            {
                id = tblUtilities.CurrentRow.Cells["id"].Value?.ToString().Trim();
                txtName.Text = tblUtilities.CurrentRow.Cells["name"].Value?.ToString().Trim();
                txtPrice.Text = tblUtilities.CurrentRow.Cells["price"].Value?.ToString().Trim();
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn sửa không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                bool rs = umbll.UpdateData(id, txtName.Text, decimal.Parse(txtPrice.Text));
                if (rs)
                {
                    MessageBox.Show("Sửa thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Reset();
                }
                else
                    MessageBox.Show("Sửa thất bại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                return;
            }
        }

        private bool IsValidUtility(clsUtilitiesCustom1 utility)
        {
            if (string.IsNullOrEmpty(utility.UtilityId))
                return false;
            if (string.IsNullOrEmpty(utility.UtilityName))
                return false;
            if (utility.Price <= 0)
                return false;
            // Thêm các điều kiện validation khác
            return true;
        }
    }
}
