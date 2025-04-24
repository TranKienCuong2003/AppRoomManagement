using BLL;
using DTO;
using Guna.UI2.WinForms;
using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Media;
using System.Diagnostics.Metrics;

namespace AppRoomManagement
{
    public partial class frmMain : Form
    {
        private MainBLL mainBLL;
        public static string nameRoom = string.Empty, priceRoom;
        public static int idRoom = 0;

        public frmMain()
        {
            InitializeComponent();
            mainBLL = new MainBLL();

            LoadLayoutListRoom();
            Delegatefunctions();
            label8.Text = "Hôm nay là: " + DateTime.Now.ToString("dddd, dd/MM/yyyy") + " || Giờ: " + DateTime.Now.ToString("HH:mm");
            label2.Text = $"Tên nhân viên: {mainBLL.GetNameEmployee(frmLogin.idEmployee)}";
            label3.Text = $"Vai trò: {frmLogin.role}";

            ColumnChart1();
            int count1 = mainBLL.CountAvailableRooms();
            int count2 = mainBLL.CountOccupiedRooms();
            PieChart1(count1, count2);
            List<clsTypeRoomCount> lst = new List<clsTypeRoomCount>();
            lst = mainBLL.CountRoomsByType();
            PieChart2(lst);
            List<clsStatisticalUtilitiesForFormMain> lst1 = new List<clsStatisticalUtilitiesForFormMain>();
            lst1 = mainBLL.GetUtilityData();
            ColumnChart2(lst1);
        }

        private void PieChart1(int count1, int count2)
        {
            pieChart3.Series = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Đang trống",
                    Values = new ChartValues<int> {count1},
                    DataLabels = true,
                    LabelPoint = chartPoint => string.Format("{0:P}", chartPoint.Participation),
                },
                new PieSeries
                {
                    Title = "Đang sử dụng",
                    Values = new ChartValues<int> {count2},
                    DataLabels = true,
                    LabelPoint = chartPoint => string.Format("{0:P}", chartPoint.Participation),
                },
            };
            pieChart3.LegendLocation = LegendLocation.Bottom;
        }

        private void PieChart2(List<clsTypeRoomCount> typeRoomCounts)
        {
            pieChart4.Series = new SeriesCollection();

            foreach (var typeRoom in typeRoomCounts)
            {
                pieChart4.Series.Add(new PieSeries
                {
                    Title = $"Loại {typeRoom.TypeRoomName}",
                    Values = new ChartValues<int> { typeRoom.RoomCount },
                    DataLabels = true,
                    LabelPoint = chartPoint => string.Format("{0:P}", chartPoint.Participation)
                });
            }

            pieChart4.LegendLocation = LegendLocation.Bottom;
        }


        private void Delegatefunctions()
        {
            if (frmLogin.role.Trim().Equals("Nhân viên"))
            {
                btnChooseRoom.Visible = false;
                btnChooseCustomer.Visible = true;
                btnChooseContracts.Visible = true;
                btnChooseUltilities.Visible = false;
                btnChooseEmployee.Visible = false;
                btnChooseAccountEmployee.Visible = false;
                btnChooseBill.Visible = true;
                btnChooseChangePassword.Visible = true;
                btnChooseTypeRoom.Visible = false;
            }
            else
            {
                btnChooseRoom.Visible = true;
                btnChooseCustomer.Visible = true;
                btnChooseContracts.Visible = true;
                btnChooseUltilities.Visible = true;
                btnChooseEmployee.Visible = true;
                btnChooseAccountEmployee.Visible = true;
                btnChooseBill.Visible = true;
                btnChooseChangePassword.Visible = true;
                btnChooseTypeRoom.Visible = true;
            }    
        }

        private void ColumnChart1()
        {
            try
            {
                List<clsStatisticalForFormMain> roomReadings = mainBLL.GetListRoomElectricityAndWater();

                // Xóa các nội dung cũ trong biểu đồ
                cartesianChart1.Series.Clear();
                cartesianChart1.AxisX.Clear();
                cartesianChart1.AxisY.Clear();

                // Khởi tạo bộ sưu tập cột (Series)
                SeriesCollection seriesCollection = new SeriesCollection();

                // Tạo cột cho số điện
                ColumnSeries electricitySeries = new ColumnSeries
                {
                    Title = "Số điện",
                    Values = new ChartValues<decimal>(),
                    Fill = new SolidColorBrush(System.Windows.Media.Colors.MediumSeaGreen)
                };

                // Tạo cột cho số nước
                ColumnSeries waterSeries = new ColumnSeries
                {
                    Title = "Số nước",
                    Values = new ChartValues<decimal>(),
                    Fill = new SolidColorBrush(System.Windows.Media.Colors.SkyBlue)
                };

                // Thêm dữ liệu vào từng cột
                foreach (var reading in roomReadings)
                {
                    electricitySeries.Values.Add(reading.electricityReading);
                    waterSeries.Values.Add(reading.waterReading);
                }

                var gradientStopCollection = new GradientStopCollection
                {
                    new GradientStop(System.Windows.Media.Colors.DarkOrange, 1),
                    new GradientStop(System.Windows.Media.Colors.OrangeRed, 0)
                };
                var fillBrush = new LinearGradientBrush(gradientStopCollection);
                electricitySeries.Fill = fillBrush;

                var gradientStopCollection1 = new GradientStopCollection
                {
                    new GradientStop(System.Windows.Media.Colors.Blue, 1),
                    new GradientStop(System.Windows.Media.Colors.BlueViolet, 0)
                };
                var fillBrush1 = new LinearGradientBrush(gradientStopCollection1);
                waterSeries.Fill = fillBrush1;

                // Thêm cột vào bộ sưu tập
                seriesCollection.Add(electricitySeries);
                seriesCollection.Add(waterSeries);

                // Cấu hình trục X (Danh sách tên phòng)
                cartesianChart1.AxisX.Add(new Axis
                {
                    Title = "Tên phòng",
                    Labels = roomReadings.Select(r => r.nameRoom).ToList()
                });

                cartesianChart1.AxisY.Add(new Axis
                {
                    Title = "Giá trị (kWh / m³)"
                });

                cartesianChart1.Series = seriesCollection;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }

        public void ColumnChart2(List<clsStatisticalUtilitiesForFormMain> utilities)
        {
            // Clear previous series on the chart
            cartesianChart2.Series.Clear();

            // Create a new SeriesCollection for the chart
            SeriesCollection seriesCollection = new SeriesCollection();

            // Create a new ColumnSeries for the data
            ColumnSeries columnSeries = new ColumnSeries
            {
                Title = "Tiện ích và Giá",
                Values = new ChartValues<decimal>()
            };

            // Add UnitPrice data to the chart
            foreach (var item in utilities)
            {
                if (item.UnitPrice.HasValue)
                {
                    columnSeries.Values.Add(item.UnitPrice.Value);
                }
                else
                {
                    columnSeries.Values.Add(0); // Add 0 if UnitPrice is null
                }
            }

            // Add the series to the series collection
            seriesCollection.Add(columnSeries);

            // Bind the series collection to the chart
            cartesianChart2.Series = seriesCollection;

            // Optionally, set the labels for the X-axis
            cartesianChart2.AxisX.Add(new Axis
            {
                Title = "Tên Tiện ích",
                Labels = utilities.Select(item => item.UtilityName).ToList()
            });

            // Optionally, set the Y-axis title
            cartesianChart2.AxisY.Add(new Axis
            {
                Title = "Giá (VNĐ)"
            });
        }

        private void LoadLayoutListRoom()
        {
            List<clsRoomButtonFormMain> list = mainBLL.GetListRoom();

            foreach (clsRoomButtonFormMain room in list)
            {
                Guna2GradientButton btnRoom = new Guna2GradientButton();
                btnRoom.Size = new System.Drawing.Size(165, 165);
                btnRoom.BackColor = System.Drawing.Color.Transparent;
                btnRoom.BorderRadius = 30;
                btnRoom.Margin = new Padding(10);
                btnRoom.Font = new System.Drawing.Font("Segoe UI", 11, System.Drawing.FontStyle.Bold);

                ContextMenuStrip menu = new ContextMenuStrip();
                ToolStripMenuItem item1 = new ToolStripMenuItem("Tạo hợp đồng");
                ToolStripMenuItem item2 = new ToolStripMenuItem("Xem chi tiết");

                item1.Click += (sender, e) =>
                {
                    //MessageBox.Show($"Tạo hợp đồng cho phòng: {room.RoomNumber}");
                    idRoom = room.idRoom;
                    nameRoom = room.roomName;
                    priceRoom = $"{((decimal)room.price).ToString("N0")}đ";
                    frmBookRoom frm = new frmBookRoom();
                    frm.Show();
                    this.Hide();
                };

                item2.Click += (sender, e) =>
                {
                    //MessageBox.Show($"Xem chi tiết phòng: {room.roomName}");
                    idRoom = room.idRoom;
                    nameRoom = room.roomName;
                    priceRoom = $"{((decimal)room.price).ToString("N0")}đ";
                    frmViewDetailRoom frm = new frmViewDetailRoom();
                    frm.Show();
                    this.Hide();
                };

                if (room.status == 0)
                {
                    btnRoom.FillColor = System.Drawing.Color.MediumSeaGreen;
                    btnRoom.FillColor2 = System.Drawing.Color.MediumAquamarine;
                    btnRoom.Text = $"{room.roomName} \n Còn trống \n {room.typeroom}";

                    item1.Visible = true;
                    item2.Visible = false;
                }
                else
                {
                    btnRoom.FillColor = System.Drawing.Color.Tomato;
                    btnRoom.FillColor2 = System.Drawing.Color.OrangeRed;
                    btnRoom.Text = $"{room.roomName} \n Đã có người thuê \n {room.typeroom}";

                    item1.Visible = false;
                    item2.Visible = true;
                }

                menu.Items.AddRange(new ToolStripItem[] { item1, item2 });
                btnRoom.ContextMenuStrip = menu;

                flowLayoutPanel2.Controls.Add(btnRoom);
            }
        }

        private void btnChooseRoom_Click(object sender, EventArgs e)
        {
            frmRoomManagement frmRoomManagement = new frmRoomManagement();
            frmRoomManagement.Show();
            this.Hide();
        }

        private void btnChooseTypeRoom_Click(object sender, EventArgs e)
        {
            frmTypeRoomManagement frmTypeRoomManagement = new frmTypeRoomManagement();
            frmTypeRoomManagement.Show();
            this.Hide();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            frmLogin frm = new frmLogin();
            frm.Show();
            this.Close();
        }

        private void btnChooseUltilities_Click(object sender, EventArgs e)
        {
            frmUtilitiesManagement frmUtilitiesManagement = new frmUtilitiesManagement();
            frmUtilitiesManagement.ShowDialog();
        }

        private void btnChooseChangePassword_Click(object sender, EventArgs e)
        {
            frmChangePassword frmChangePassword = new frmChangePassword();
            frmChangePassword.ShowDialog();
        }

        private void btnChooseAccountEmployee_Click(object sender, EventArgs e)
        {
            frmAccountManagement frmAccountManagement = new frmAccountManagement();
            frmAccountManagement.ShowDialog();
        }

        private void btnChooseEmployee_Click(object sender, EventArgs e)
        {
            frmEmployeeManagement frmEmployeeManagement = new frmEmployeeManagement();
            frmEmployeeManagement.ShowDialog();
        }

        private void btnChooseContracts_Click(object sender, EventArgs e)
        {
            frmContractManagement frmContractManagement = new frmContractManagement();
            frmContractManagement.ShowDialog();
        }

        private void btnChooseBill_Click(object sender, EventArgs e)
        {
            frmBillManagement frmBillManagement = new frmBillManagement();
            frmBillManagement.Show();
            this.Hide();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void btnChooseCustomer_Click(object sender, EventArgs e)
        {
            frmCustomerManagement frmCustomerManagement = new frmCustomerManagement();
            frmCustomerManagement.Show();
            this.Hide();
        }
    }
}
