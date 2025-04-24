using DTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public class BookRoomBLL
    {
        QLPhongTroDataContext db;

        public BookRoomBLL()
        {
            db = new QLPhongTroDataContext();
        }

        public List<clsCustomerForContracts> GetListCustomer()
        {
            try
            {
                var customer = from cs in db.Customers
                               select new clsCustomerForContracts
                               {
                                   id = cs.CustomerID,
                                   name = $"{cs.LastName} {cs.FirstName}",
                                   identity = cs.IdentityNumber,
                                   phone = cs.PhoneNumber,
                               };
                return customer.ToList();
            }
            catch (Exception)
            {
                return new List<clsCustomerForContracts>();
            }
        }

        public List<clsCustomerForContracts> SearchCustomer(string search)
        {
            try
            {
                var customer = from cs in db.Customers
                               where (cs.LastName.Trim() + " " + cs.FirstName.Trim()).Contains(search)
                               select new clsCustomerForContracts
                               {
                                   id = cs.CustomerID,
                                   name = $"{cs.LastName} {cs.FirstName}",
                                   identity = cs.IdentityNumber,
                                   phone = cs.PhoneNumber,
                               };
                return customer.ToList();
            }
            catch (Exception)
            {
                return new List<clsCustomerForContracts>();
            }
        }

        public string GetNameOwnerManagement()
        {
            try
            {
                var owner = from em in db.Employees
                            join ea in db.EmployeeAccounts on em.EmployeeID equals ea.EmployeeID
                            where ea.Role.Trim().Equals("Chủ trọ")
                            select $"{em.LastName} {em.FirstName}";
                return owner.SingleOrDefault().ToString();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public string GetIdOwnerManagement()
        {
            try
            {
                var owner = from em in db.Employees
                            join ea in db.EmployeeAccounts on em.EmployeeID equals ea.EmployeeID
                            where ea.Role.Trim().Equals("Chủ trọ")
                            select em.EmployeeID;
                return owner.SingleOrDefault().ToString();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public bool CreateNewContract(string idContract, int idRoom, string idCustomer, string idEmployee,
            DateTime started, DateTime ended, decimal deposite)
        {
            try
            {
                var newContract = new Contract
                {
                    ContractID = idContract,
                    RoomID = idRoom,
                    CustomerID = idCustomer,
                    EmployeeID = idEmployee,
                    StartDate = started,
                    EndDate = ended,
                    Deposit = deposite,
                    Status = "Hợp đồng có hiệu lực",
                    CreatedAt = DateTime.Now,
                };

                db.Contracts.InsertOnSubmit(newContract);
                db.SubmitChanges();

                var newContractHistory = new ContractsHistory
                {
                    ContractID = idContract,
                    Content = $"Thêm hợp đồng mới || Mã hợp đồng: {idContract}",
                    UpdatedAt = DateTime.Now,
                };

                db.ContractsHistories.InsertOnSubmit(newContractHistory);
                db.SubmitChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteContractAndContractHistory(string idContract)
        {
            try
            {
                var histories = db.ContractsHistories.Where(ch => ch.ContractID == idContract);
                db.ContractsHistories.DeleteAllOnSubmit(histories.ToList());

                var contract = db.Contracts.SingleOrDefault(c => c.ContractID == idContract);
                if (contract != null)
                {
                    db.Contracts.DeleteOnSubmit(contract);
                }

                db.SubmitChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool ChangeStatusRoom(int idRoom)
        {
            try
            {
                var room = db.Rooms.SingleOrDefault(rm => rm.RoomID == idRoom);

                if (room != null)
                {
                    room.Status = 1;
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

    }
}
