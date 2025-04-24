using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class ViewDetailRoomBLL
    {
        QLPhongTroDataContext db;

        public ViewDetailRoomBLL()
        {
            db = new QLPhongTroDataContext();
        }

        public clsViewDetailRoom GetDetailRoom(int idRoom)
        {
            try
            {
                var detail = from ct in db.Contracts
                             join rm in db.Rooms on ct.RoomID equals rm.RoomID
                             join cs in db.Customers on ct.CustomerID equals cs.CustomerID
                             where rm.RoomID == idRoom && ct.Status.Trim().Equals("Hợp đồng có hiệu lực")
                             select new clsViewDetailRoom
                             {
                                 idContract = ct.ContractID,
                                 idRoom = (int)ct.RoomID,
                                 nameRoom = rm.RoomNumber,
                                 idCustomer = cs.CustomerID,
                                 nameCustomer = cs.LastName + " " + cs.FirstName,
                                 phoneCustomer = cs.PhoneNumber,
                                 priceRoom = (decimal)rm.Price,
                                 startedAt = ct.StartDate.Value,
                                 endedAt = ct.EndDate.Value,
                                 status = ct.Status,
                                 createdAt = ct.CreatedAt.Value,
                             };
                return detail.FirstOrDefault();
            }
            catch (Exception)
            {
                return new clsViewDetailRoom();
            }
        }

        public List<clsCustomerCustom2> GetListCustomer()
        {
            try
            {
                var cus = from cs in db.Customers
                          select new clsCustomerCustom2
                          {
                              id = cs.CustomerID.Trim(),
                              name = $"{cs.LastName.Trim()} {cs.FirstName.Trim()}",
                              phone = cs.PhoneNumber.Trim(),
                          };
                return cus.ToList();
            }
            catch (Exception)
            {
                return new List<clsCustomerCustom2>();
            }
        }

        public List<clsCustomerCustom2> SearchListCustomer(string idCustomer)
        {
            try
            {
                var cus = from cs in db.Customers
                          where cs.LastName.Trim().Contains(idCustomer.Trim()) || cs.FirstName.Trim().Contains(idCustomer.Trim())
                          select new clsCustomerCustom2
                          {
                              id = cs.CustomerID.Trim(),
                              name = $"{cs.LastName.Trim()} {cs.FirstName.Trim()}",
                              phone = cs.PhoneNumber.Trim(),
                          };
                return cus.ToList();
            }
            catch (Exception)
            {
                return new List<clsCustomerCustom2>();
            }
        }

        public bool CheckContractIsExitInElectricityReadings(string contractId)
        {
            try
            {
                bool exists = db.ElectricityReadings.Any(er => er.ContractID == contractId);
                return exists;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CheckContractIsExitInWaterReadings(string contractId)
        {
            try
            {
                bool exists = db.WaterReadings.Any(er => er.ContractID == contractId);
                return exists;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool IsElectricityReadingsNotEmpty()
        {
            try
            {
                bool hasRecords = db.ElectricityReadings.Any();
                return hasRecords;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool IsWaterReadingsNotEmpty()
        {
            try
            {
                bool hasRecords = db.WaterReadings.Any();
                return hasRecords;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public decimal GetTheLastElectricityReadingsByCreatedAt(int idRoom)
        {
            try
            {
                var lastReading = (from er in db.ElectricityReadings
                                   join ct in db.Contracts on er.ContractID equals ct.ContractID
                                   where ct.RoomID == idRoom
                                   orderby er.CreatedAt descending
                                   select er.NewReading).FirstOrDefault();

                return lastReading != null ? (decimal)lastReading : 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public decimal GetTheLastWaterReadingsByCreatedAt(int idRoom)
        {
            try
            {
                var lastReading = (from wr in db.WaterReadings
                                   join ct in db.Contracts on wr.ContractID equals ct.ContractID
                                   where ct.RoomID == idRoom
                                   orderby wr.CreatedAt descending
                                   select wr.NewReading).FirstOrDefault();

                return lastReading != null ? (decimal)lastReading : 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public bool InsertDataElectricityForContract(string idContract, string monthyear, decimal oldRead, decimal newRead)
        {
            try
            {
                var newReadingElectricity = new ElectricityReading
                {
                    ContractID = idContract,
                    MonthYear = monthyear,
                    OldReading = oldRead,
                    NewReading = newRead,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                db.ElectricityReadings.InsertOnSubmit(newReadingElectricity);
                db.SubmitChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool InsertDataWaterForContract(string idContract, string monthyear, decimal oldRead, decimal newRead)
        {
            try
            {
                var newReadingWater = new WaterReading
                {
                    ContractID = idContract,
                    MonthYear = monthyear,
                    OldReading = oldRead,
                    NewReading = newRead,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                db.WaterReadings.InsertOnSubmit(newReadingWater);
                db.SubmitChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public decimal GetUnitElectticity()
        {
            try
            {
                var elect = from ul in db.Utilities where ul.UtilityID.Trim().Equals("DV001") select ul.UnitPrice;
                return (decimal)elect.FirstOrDefault();
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public decimal GetUnitWater()
        {
            try
            {
                var elect = from ul in db.Utilities where ul.UtilityID.Trim().Equals("DV002") select ul.UnitPrice;
                return (decimal)elect.FirstOrDefault();
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public decimal SumPriceElectricityLastesByIdContract(string contractID)
        {
            try
            {
                var cls = (from er in db.ElectricityReadings
                           where er.ContractID.Trim().Equals(contractID.Trim())
                           orderby er.CreatedAt descending
                           select new clsElectricityOldAndNewReading
                           {
                               oldReading = er.OldReading ?? 0m,
                               newReading = er.NewReading ?? 0m
                           }).FirstOrDefault();

                if (cls == null)
                {
                    return 0;
                }

                decimal unitElectricity = (decimal)db.Utilities
                    .Where(ut => ut.UtilityID.Trim().Equals("DV001"))
                    .Select(ut => ut.UnitPrice)
                    .FirstOrDefault();

                if (unitElectricity == 0)
                {
                    return 0;
                }

                decimal sum = (cls.newReading - cls.oldReading) * unitElectricity;

                return sum;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 0;
            }
        }

        public decimal SumPriceWaterLastesByIdContract(string contractID)
        {
            try
            {
                var cls = (from er in db.WaterReadings
                           where er.ContractID.Trim().Equals(contractID.Trim())
                           orderby er.CreatedAt descending
                           select new clsWaterOldAndNewReading
                           {
                               oldReading = er.OldReading ?? 0m,
                               newReading = er.NewReading ?? 0m
                           }).FirstOrDefault();

                if (cls == null)
                {
                    return 0;
                }

                decimal unitWater = (decimal)db.Utilities
                    .Where(ut => ut.UtilityID.Trim().Equals("DV002"))
                    .Select(ut => ut.UnitPrice)
                    .FirstOrDefault();

                if (unitWater == 0)
                {
                    return 0;
                }

                decimal sum = (cls.newReading - cls.oldReading) * unitWater;

                return sum;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 0;
            }
        }

        public decimal GetUtility(string id)
        {
            try
            {
                var utl = from ut in db.Utilities
                          where ut.UtilityID.Trim().Equals(id)
                          select ut.UnitPrice;
                return (decimal)utl.FirstOrDefault();
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public bool CheckContractHaveExitInBill(string idContract)
        {
            try
            {
                bool exit = (from bi in db.Bills
                             where bi.ContractID.Trim() == idContract.Trim()
                             select bi).Any();
                return exit;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool InsertBill(string idBill, string idContract, string idEmployee, decimal total)
        {
            try
            {
                var newBill = new Bill
                {
                    BillID = idBill,
                    ContractID = idContract,
                    EmployeeID = idEmployee,
                    TotalAmount = total,
                    PaymentDate = null,
                    Status = "Đang chờ thanh toán",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                db.Bills.InsertOnSubmit(newBill);
                db.SubmitChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool InsertBillDetails(string idBill, string idUtility, decimal amount)
        {
            try
            {
                var newBillDetail = new BillDetail
                {
                    BillID = idBill,
                    UtilityID = idUtility,
                    Amount = amount,
                    CreatedAt = DateTime.Now,
                };

                db.BillDetails.InsertOnSubmit(newBillDetail);
                db.SubmitChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<clsUtilitiesIDAndUnit> GetListUtilitiesToAmountForBill()
        {
            try
            {
                var uti = from ut in db.Utilities
                          select new clsUtilitiesIDAndUnit
                          {
                              UtilityID = ut.UtilityID.Trim(),
                              UnitPrice = (decimal)ut.UnitPrice,
                          };

                return uti.ToList();
            }
            catch (Exception)
            {
                return new List<clsUtilitiesIDAndUnit>();
            }
        }

        public decimal GetElectricityLastesByIdContract(string contractID)
        {
            try
            {
                var cls = (from er in db.ElectricityReadings
                           where er.ContractID.Trim().Equals(contractID.Trim())
                           orderby er.CreatedAt descending
                           select new
                           {
                               OldReading = er.OldReading ?? 0m,
                               NewReading = er.NewReading ?? 0m
                           }).FirstOrDefault();

                if (cls == null)
                {
                    return 0;
                }

                return cls.NewReading - cls.OldReading;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.Message);
                return 0;
            }
        }


        public decimal GetWaterLastesByIdContract(string contractID)
        {
            try
            {
                var cls = (from er in db.WaterReadings
                           where er.ContractID.Trim().Equals(contractID.Trim())
                           orderby er.CreatedAt descending
                           select new
                           {
                               OldReading = er.OldReading ?? 0m,
                               NewReading = er.NewReading ?? 0m
                           }).FirstOrDefault();

                if (cls == null)
                {
                    return 0;
                }

                return cls.NewReading - cls.OldReading;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.Message);
                return 0;
            }
        }

        public string GetIdBillByIdContract(string contractID)
        {
            try
            {
                var idBill = from bi in db.Bills where bi.ContractID.Trim().Equals(contractID) select bi.BillID;
                return idBill.FirstOrDefault().ToString().Trim();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public bool CheckAllServicesAddedToBillDetails(string contractID)
        {
            try
            {
                int currentMonth = DateTime.Now.Month;
                int currentYear = DateTime.Now.Year;

                var allServices = db.Utilities.Select(u => u.UtilityID.Trim()).ToList();

                var addedServices = (from bd in db.BillDetails
                                     join b in db.Bills on bd.BillID equals b.BillID
                                     where b.ContractID.Trim() == contractID.Trim() &&
                                           b.CreatedAt.Value.Month == currentMonth &&
                                           b.CreatedAt.Value.Year == currentYear
                                     select bd.UtilityID.Trim()).ToList();

                return allServices.All(service => addedServices.Contains(service));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.Message);
                return false;
            }
        }

        public bool CheckElectricityReadingRecorded(string contractID)
        {
            try
            {
                int checkMonth = DateTime.Now.Month;
                int checkYear = DateTime.Now.Year;

                bool isRecorded = db.ElectricityReadings.Any(er =>
                    er.ContractID.Trim() == contractID.Trim() &&
                    er.CreatedAt.Value.Month == checkMonth &&
                    er.CreatedAt.Value.Year == checkYear);

                return isRecorded;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.Message);
                return false;
            }
        }

        public bool CheckWaterReadingRecorded(string contractID)
        {
            try
            {
                int checkMonth = DateTime.Now.Month;
                int checkYear = DateTime.Now.Year;

                bool isRecorded = db.WaterReadings.Any(er =>
                    er.ContractID.Trim() == contractID.Trim() &&
                    er.CreatedAt.Value.Month == checkMonth &&
                    er.CreatedAt.Value.Year == checkYear);

                return isRecorded;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.Message);
                return false;
            }
        }

        public string NotifyContractStatus(string idContract)
        {
            try
            {
                var query = from ct in db.Contracts
                            where ct.ContractID.Trim().Equals(idContract.Trim())
                            select ct.EndDate;

                var endDate = query.FirstOrDefault();
                if (endDate == null)
                {
                    return "Không tìm thấy hợp đồng.";
                }

                DateTime today = DateTime.Today;
                DateTime contractEndDate = endDate.Value;

                if (contractEndDate < today)
                {
                    return "Hợp đồng đã hết hạn, vui lòng gia hạn thêm.";
                }
                else if ((contractEndDate - today).TotalDays <= 30)
                {
                    return "Hợp đồng sắp hết hạn.";
                }
                else
                {
                    return "Hợp đồng còn thời hạn.";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi kiểm tra trạng thái hợp đồng: " + ex.Message);
            }
        }

        public bool RenewContractById(string idContract, DateTime newEndDate)
        {
            try
            {
                var updateStatusContract = from ct in db.Contracts where ct.ContractID.Trim().Equals(idContract) select ct;
                if (updateStatusContract != null)
                {
                    updateStatusContract.FirstOrDefault().EndDate = newEndDate;
                    updateStatusContract.FirstOrDefault().Status = "Hợp đồng có hiệu lực";

                    db.SubmitChanges();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CancleContractById(string idContract)
        {
            try
            {
                var updateStatus = from ct in db.Contracts where ct.ContractID.Trim().Equals(idContract) select ct;
                if (updateStatus != null)
                {
                    updateStatus.FirstOrDefault().Status = "Hợp đồng đã hủy";

                    db.SubmitChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateEmptyRoom(int idRoom)
        {
            try
            {
                var updateStatus = from rm in db.Rooms where rm.RoomID.Equals(idRoom) select rm;
                if (updateStatus != null)
                {
                    updateStatus.FirstOrDefault().Status = 0;

                    db.SubmitChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CheckHaveExitBillByIdContract(string idContract)
        {
            try
            {
                var findBillById = from bi in db.Bills where bi.ContractID.Trim().Equals(idContract) select bi;
                if (findBillById.Any())
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public clsBillToPDF GetBillToPrintPDF(string idContract)
        {
            try
            {
                idContract = idContract.Trim();

                var latestBill = (from bi in db.Bills
                                  where bi.ContractID == idContract
                                  orderby bi.CreatedAt descending
                                  select bi).FirstOrDefault();

                if (latestBill == null)
                    return new clsBillToPDF();

                var billDetail = (from bi in db.Bills
                                  join bd in db.BillDetails on bi.BillID equals bd.BillID
                                  join ct in db.Contracts on bi.ContractID equals ct.ContractID
                                  join em in db.Employees on bi.EmployeeID equals em.EmployeeID
                                  join cs in db.Customers on ct.CustomerID equals cs.CustomerID
                                  join rm in db.Rooms on ct.RoomID equals rm.RoomID
                                  where bi.BillID == latestBill.BillID
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

        public List<clsCustomerGroup> GetCustomerGroups(string idContract)
        {
            try
            {
                var group = from cg in db.CustomerGroups
                            join cs in db.Customers on cg.CustomerID equals cs.CustomerID
                            where cg.ContractID.Trim().Equals(idContract.Trim())
                            select new clsCustomerGroup
                            {
                                idContract = cg.ContractID,
                                idCustomer = cg.CustomerID,
                                nameCustomer = $"{cs.LastName} {cs.FirstName}",
                                createdAt = (DateTime)cg.CreatedAt,
                                updatedAt = (DateTime)cg.UpdatedAt,
                            };
                if (group != null )
                    return group.ToList();
                else
                    return new List<clsCustomerGroup>();
            }
            catch (Exception)
            {
                return new List<clsCustomerGroup>();
            }
        }

        public bool CheckHaveExitCustomerInCustomerGroup(string idCustomer)
        {
            try
            {
                return db.CustomerGroups.Any(cs => cs.CustomerID.Trim() == idCustomer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        public bool InsertCustomerToCustomerGroup(string idContract, string idCustomer)
        {
            try
            {
                var newCustomerGroup = new CustomerGroup
                {
                    ContractID = idContract,
                    CustomerID = idCustomer,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                db.CustomerGroups.InsertOnSubmit(newCustomerGroup);
                db.SubmitChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteCustomerFromCustomerGroup(string idCustomer)
        {
            try
            {
                var findCustomerById = from cg in db.CustomerGroups where cg.CustomerID.Trim().Equals(idCustomer) select cg;
                if (findCustomerById != null)
                {
                    db.CustomerGroups.DeleteOnSubmit(findCustomerById.FirstOrDefault());
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


        public string GetLatestContractId()
        {
            try
            {
                var latestContract = db.Contracts
                                       .OrderByDescending(ct => ct.CreatedAt)
                                       .FirstOrDefault();

                return latestContract?.ContractID ?? string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public clsContractsToPDF GetDetailContractToPdf(string contractId)
        {
            try
            {
                var contract = from ct in db.Contracts
                               join em in db.Employees on ct.EmployeeID equals em.EmployeeID
                               join cs in db.Customers on ct.CustomerID equals cs.CustomerID
                               join ro in db.Rooms on ct.RoomID equals ro.RoomID
                               where ct.ContractID.Trim().Equals(contractId)
                               select new clsContractsToPDF
                               {
                                   idContract = contractId,
                                   nameOwner = $"{em.LastName} {em.FirstName}",
                                   identityOwner = em.IdentityNumber,
                                   phoneOwner = em.PhoneNumber,
                                   addressOwner = $"{em.Address}, {em.Ward}, {em.District}, {em.City}",
                                   roomNumber = ro.RoomNumber,
                                   priceRoom = (decimal)ro.Price,
                                   deposite = (decimal)ct.Deposit,
                                   nameCustomer = $"{cs.LastName} {cs.FirstName}",
                                   identityCustomer = cs.IdentityNumber,
                                   phoneCustomer = cs.PhoneNumber,
                                   addressCustomer = $"{cs.Address}, {cs.Ward}, {cs.District}, {cs.City}",
                                   startedAt = (DateTime)ct.StartDate,
                                   endedAt = (DateTime)ct.EndDate,
                               };

                return contract.FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
