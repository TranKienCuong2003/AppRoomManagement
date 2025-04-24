using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class UtilitiesManagementBLL
    {
        QLPhongTroDataContext db;

        public UtilitiesManagementBLL()
        {
            db = new QLPhongTroDataContext();
        }

        public List<clsUtilitiesCustom1> GetListUtilities()
        {
            try
            {
                var uti = from ut in db.Utilities
                          select new clsUtilitiesCustom1
                          {
                              id = ut.UtilityID,
                              name = ut.UtilityName,
                              price = (decimal)ut.UnitPrice,
                              createdAt = (DateTime)ut.CreatedAt,
                              updatedAt = (DateTime)ut.UpdatedAt,
                          };
                return uti.ToList();
            }
            catch (Exception)
            {
                return new List<clsUtilitiesCustom1>();
            }
        }

        public string CreateNewIdUtility()
        {
            try
            {
                // Lấy mã khách hàng lớn nhất từ cơ sở dữ liệu (Giả sử hàm GetLastCustomerId() có sẵn)
                string lastUtilityId = db.Utilities.OrderByDescending(c => c.UtilityID).Select(c => c.UtilityID.Trim()).FirstOrDefault();

                // Nếu không có khách hàng nào, gán mã mặc định là KH000001
                if (string.IsNullOrEmpty(lastUtilityId))
                {
                    return "DV001";
                }

                // Tách phần số từ mã khách hàng (ví dụ KH000003 -> 000003)
                string numericPart = lastUtilityId.Substring(2); // Lấy phần số (sau 'KH')
                int currentId = int.Parse(numericPart); // Chuyển đổi phần số thành integer

                // Tăng ID lên 1
                currentId++;

                // Tạo mã khách hàng mới với định dạng KH + 6 chữ số
                // Sử dụng ToString("D6") để đảm bảo rằng mã khách hàng luôn có 6 chữ số
                string newUltilityId = "DV" + currentId.ToString("D3"); // Đảm bảo luôn có 6 chữ số (D6)

                return newUltilityId;
            }
            catch (Exception ex)
            {
                throw new Exception("Error generating new customer ID: " + ex.Message);
            }
        }

        public bool InsertData(string id, string name, decimal price)
        {
            try
            {
                var newUtility = new Utility
                {
                    UtilityID = id,
                    UtilityName = name,
                    UnitPrice = price,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                db.Utilities.InsertOnSubmit(newUtility);
                db.SubmitChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteData(string id)
        {
            try
            {
                var utility = db.Utilities.SingleOrDefault(u => u.UtilityID == id);

                if (utility != null)
                {
                    db.Utilities.DeleteOnSubmit(utility);
                    db.SubmitChanges();
                    return true;
                }
                return false; // Không tìm thấy utility
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateData(string id, string name, decimal price)
        {
            try
            {
                var utility = db.Utilities.SingleOrDefault(u => u.UtilityID == id);

                if (utility != null)
                {
                    utility.UtilityName = name;
                    utility.UnitPrice = price;
                    utility.UpdatedAt = DateTime.Now;

                    db.SubmitChanges();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
