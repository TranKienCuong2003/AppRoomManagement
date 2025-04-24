using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BillManagementBLL
    {
        private QLPhongTroDataContext db;

        public BillManagementBLL()
        {
            db = new QLPhongTroDataContext();
        }

        public List<clsBMBill> GetListBill()
        {
            try
            {
                var bill = from bi in db.Bills
                           join ct in db.Contracts on bi.ContractID equals ct.ContractID
                           join cs in db.Customers on ct.CustomerID equals cs.CustomerID
                           join em in db.Employees on bi.EmployeeID equals em.EmployeeID
                           select new clsBMBill()
                           {
                               idBill = bi.BillID.Trim(),
                               idContract = bi.ContractID.Trim(),
                               customer = $"{cs.LastName.Trim()} {cs.FirstName.Trim()}",
                               employee = $"{em.LastName.Trim()} {em.FirstName.Trim()}",
                               price = (decimal)bi.TotalAmount,
                               payDate = bi.PaymentDate ?? (DateTime?)null,
                               status = bi.Status,
                               createdAt = (DateTime)bi.CreatedAt,
                               updatedAt = (DateTime)bi.UpdatedAt,
                           };
                return bill.ToList();
            }
            catch (Exception)
            {
                return new List<clsBMBill>();
            }
        }

        public List<clsBMBill> GetBillById(string idBill)
        {
            try
            {
                var bill = from bi in db.Bills
                           join ct in db.Contracts on bi.ContractID equals ct.ContractID
                           join cs in db.Customers on ct.CustomerID equals cs.CustomerID
                           join em in db.Employees on bi.EmployeeID equals em.EmployeeID
                           where bi.BillID.Trim().Contains(idBill)
                           select new clsBMBill()
                           {
                               idBill = bi.BillID.Trim(),
                               idContract = bi.ContractID.Trim(),
                               customer = $"{cs.LastName.Trim()} {cs.FirstName.Trim()}",
                               employee = $"{em.LastName.Trim()} {em.FirstName.Trim()}",
                               price = (decimal)bi.TotalAmount,
                               payDate = bi.PaymentDate ?? (DateTime?)null,
                               status = bi.Status,
                               createdAt = (DateTime)bi.CreatedAt,
                               updatedAt = (DateTime)bi.UpdatedAt,
                           };
                return bill.ToList();
            }
            catch (Exception)
            {
                return new List<clsBMBill>();
            }
        }

        public List<clsBMBillDetail> GetListBillDetail()
        {
            try
            {
                var billDetail = from bd in db.BillDetails
                                 join ut in db.Utilities on bd.UtilityID equals ut.UtilityID
                                 select new clsBMBillDetail()
                                 {
                                     idBillDetail = (int)bd.BillDetailID,
                                     idBill = bd.BillID,
                                     idUtility = ut.UtilityID,
                                     nameUtility = ut.UtilityName,
                                     amount = (decimal)bd.Amount,
                                     createdAt = (DateTime)bd.CreatedAt,
                                 };
                return billDetail.ToList();
            }
            catch (Exception)
            {
                return new List<clsBMBillDetail>();
            }
        }

        public List<clsBMBillDetail> GetListBillDetailById(string idBill)
        {
            try
            {
                var billDetail = from bd in db.BillDetails
                                 join ut in db.Utilities on bd.UtilityID equals ut.UtilityID
                                 where bd.BillID.Trim().Contains(idBill)
                                 select new clsBMBillDetail()
                                 {
                                     idBillDetail = (int)bd.BillDetailID,
                                     idBill = bd.BillID,
                                     idUtility = ut.UtilityID,
                                     nameUtility = ut.UtilityName,
                                     amount = (decimal)bd.Amount,
                                     createdAt = (DateTime)bd.CreatedAt,
                                 };
                return billDetail.ToList();
            }
            catch (Exception)
            {
                return new List<clsBMBillDetail>();
            }
        }

        public List<clsBMReadingElectricity> GetListElectricity()
        {
            try
            {
                var elect = from re in db.ElectricityReadings
                            select new clsBMReadingElectricity()
                            {
                                id = re.ReadingID,
                                contractID = re.ContractID,
                                monthYear = re.MonthYear,
                                oldRead = (decimal)re.OldReading,
                                newRead = (decimal)re.NewReading,
                                createdAt = (DateTime)re.CreatedAt,
                                updatedAt = (DateTime)re.UpdatedAt,
                            };
                return elect.ToList();
            }
            catch (Exception)
            {
                return new List<clsBMReadingElectricity>();
            }
        }

        public List<clsBMReadingElectricity> GetListElectricityById(string idBill)
        {
            try
            {
                var elect = from re in db.ElectricityReadings
                            join ct in db.Contracts on re.ContractID equals ct.ContractID
                            join bi in db.Bills on ct.ContractID equals bi.ContractID
                            where bi.BillID.Trim().Contains(idBill)
                            select new clsBMReadingElectricity()
                            {
                                id = re.ReadingID,
                                contractID = re.ContractID,
                                monthYear = re.MonthYear,
                                oldRead = (decimal)re.OldReading,
                                newRead = (decimal)re.NewReading,
                                createdAt = (DateTime)re.CreatedAt,
                                updatedAt = (DateTime)re.UpdatedAt,
                            };
                return elect.ToList();
            }
            catch (Exception)
            {
                return new List<clsBMReadingElectricity>();
            }
        }

        public List<clsBMReadingWater> GetListWater()
        {
            try
            {
                var elect = from re in db.ElectricityReadings
                            select new clsBMReadingWater()
                            {
                                id = re.ReadingID,
                                contractID = re.ContractID,
                                monthYear = re.MonthYear,
                                oldRead = (decimal)re.OldReading,
                                newRead = (decimal)re.NewReading,
                                createdAt = (DateTime)re.CreatedAt,
                                updatedAt = (DateTime)re.UpdatedAt,
                            };
                return elect.ToList();
            }
            catch (Exception)
            {
                return new List<clsBMReadingWater>();
            }
        }

        public List<clsBMReadingWater> GetListWaterById(string idBill)
        {
            try
            {
                var elect = from re in db.ElectricityReadings
                            join ct in db.Contracts on re.ContractID equals ct.ContractID
                            join bi in db.Bills on ct.ContractID equals bi.ContractID
                            where bi.BillID.Trim().Contains(idBill)
                            select new clsBMReadingWater()
                            {
                                id = re.ReadingID,
                                contractID = re.ContractID,
                                monthYear = re.MonthYear,
                                oldRead = (decimal)re.OldReading,
                                newRead = (decimal)re.NewReading,
                                createdAt = (DateTime)re.CreatedAt,
                                updatedAt = (DateTime)re.UpdatedAt,
                            };
                return elect.ToList();
            }
            catch (Exception)
            {
                return new List<clsBMReadingWater>();
            }
        }

        public clsBillToPDF GetBillToPrintPDF(string idBill)
        {
            try
            {
                var billDetail = (from bi in db.Bills
                                  join bd in db.BillDetails on bi.BillID equals bd.BillID
                                  join ct in db.Contracts on bi.ContractID equals ct.ContractID
                                  join em in db.Employees on bi.EmployeeID equals em.EmployeeID
                                  join cs in db.Customers on ct.CustomerID equals cs.CustomerID
                                  join rm in db.Rooms on ct.RoomID equals rm.RoomID
                                  where bi.BillID.Trim().Equals(idBill)
                                  select new clsBillToPDF
                                  {
                                      idBill = bi.BillID,
                                      idContract = bi.ContractID,
                                      idEmployee = bi.EmployeeID,
                                      nameEmployee = $"{em.LastName} {em.FirstName}",
                                      phoneEmployee = em.PhoneNumber,
                                      idCustomer = ct.CustomerID,
                                      nameCustomer = $"{cs.LastName} {cs.FirstName}",
                                      phoneCustomer = cs.PhoneNumber,
                                      idRoom = (int)ct.RoomID,
                                      nameRoom = rm.RoomNumber,
                                      priceRoom = (decimal)rm.Price,
                                      totalAmount = (decimal)bi.TotalAmount,
                                      status = bi.Status,
                                      createdAt = (DateTime)bi.CreatedAt,
                                  }).FirstOrDefault();

                return billDetail ?? new clsBillToPDF();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
                return new clsBillToPDF();
            }
        }

        public List<clsBillDetailToPDF> GetBillDetailToPrintPDF(string idBill)
        {
            try
            {
                var billDetail = from bd in db.BillDetails
                                 join ut in db.Utilities on bd.UtilityID equals ut.UtilityID
                                 where bd.BillID.Trim().Equals(idBill.Trim())
                                 select new clsBillDetailToPDF
                                 {
                                     utilityID = bd.UtilityID,
                                     utilityName = ut.UtilityName,
                                     amount = (decimal)bd.Amount,
                                 };
                return billDetail.ToList();
            }
            catch (Exception)
            {
                return new List<clsBillDetailToPDF>();
            }
        }

        public bool UpdateStatusBill(string idBill)
        {
            try
            {
                var billToFind = db.Bills.FirstOrDefault(bi => bi.BillID.Trim().Equals(idBill));

                if (billToFind != null)
                {
                    billToFind.PaymentDate = DateTime.Now;
                    billToFind.Status = "Đã thanh toán";
                    billToFind.UpdatedAt = DateTime.Now;

                    db.SubmitChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

    }
}
