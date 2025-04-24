using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class ContractManagementBLL
    {
        QLPhongTroDataContext db;

        public ContractManagementBLL()
        {
            db = new QLPhongTroDataContext();
        }

        public List<clsContractDetail> GetData()
        {
            try
            {
                var contract = from ct in db.Contracts
                               join rm in db.Rooms on ct.RoomID equals rm.RoomID
                               join tr in db.TypeRooms on rm.TypeRoomID equals tr.TypeRoomID
                               join cs in db.Customers on ct.CustomerID equals cs.CustomerID
                               join em in db.Employees on ct.EmployeeID equals em.EmployeeID
                               select new clsContractDetail
                               {
                                   idContract = ct.ContractID.Trim(),
                                   idRoom = (int)ct.RoomID,
                                   roomName = rm.RoomNumber,
                                   roomType = tr.TypeRoomName,
                                   roomPrice = (decimal)rm.Price,
                                   idCustomer = ct.CustomerID.Trim(),
                                   nameCustomer = $"{cs.LastName} {cs.FirstName}",
                                   phoneCustomer = cs.PhoneNumber,
                                   idEmployee = ct.EmployeeID,
                                   nameEmployee = $"{em.LastName} {em.FirstName}",
                                   phoneEmployee = em.PhoneNumber,
                                   startedAt = (DateTime)ct.StartDate,
                                   endedAt = (DateTime)ct.EndDate,
                                   deposite = (decimal)ct.Deposit,
                                   status = ct.Status,
                                   createdAt = (DateTime)ct.CreatedAt,
                               };
                return contract.ToList();
            }
            catch (Exception)
            {
                return new List<clsContractDetail>();
            }
        }

        public List<clsContractDetail> SearchData(string search)
        {
            try
            {
                var contract = from ct in db.Contracts
                               join rm in db.Rooms on ct.RoomID equals rm.RoomID
                               join tr in db.TypeRooms on rm.TypeRoomID equals tr.TypeRoomID
                               join cs in db.Customers on ct.CustomerID equals cs.CustomerID
                               join em in db.Employees on ct.EmployeeID equals em.EmployeeID
                               where ct.ContractID.Trim().Contains(search)
                               select new clsContractDetail
                               {
                                   idContract = ct.ContractID.Trim(),
                                   idRoom = (int)ct.RoomID,
                                   roomName = rm.RoomNumber,
                                   roomType = tr.TypeRoomName,
                                   roomPrice = (decimal)rm.Price,
                                   idCustomer = ct.CustomerID.Trim(),
                                   nameCustomer = $"{cs.LastName} {cs.FirstName}",
                                   phoneCustomer = cs.PhoneNumber,
                                   idEmployee = ct.EmployeeID,
                                   nameEmployee = $"{em.LastName} {em.FirstName}",
                                   phoneEmployee = em.PhoneNumber,
                                   startedAt = (DateTime)ct.StartDate,
                                   endedAt = (DateTime)ct.EndDate,
                                   deposite = (decimal)ct.Deposit,
                                   status = ct.Status,
                                   createdAt = (DateTime)ct.CreatedAt,
                               };
                return contract.ToList();
            }
            catch (Exception)
            {
                return new List<clsContractDetail>();
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
