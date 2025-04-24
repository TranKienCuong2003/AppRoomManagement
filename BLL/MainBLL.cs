using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class MainBLL
    {
        QLPhongTroDataContext db;

        public MainBLL()
        {
            db = new QLPhongTroDataContext();
        }

        public List<clsRoomButtonFormMain> GetListRoom()
        {
            List<clsRoomButtonFormMain> lst = new List<clsRoomButtonFormMain>();
            try
            {
                var rooms = from ro in db.Rooms
                            join tr in db.TypeRooms on ro.TypeRoomID equals tr.TypeRoomID
                            select new clsRoomButtonFormMain
                {
                    idRoom = ro.RoomID,
                    roomName = ro.RoomNumber,
                    price = (decimal)ro.Price,
                    status = (int)ro.Status,
                    typeroom = tr.TypeRoomName,
                };
                return rooms.ToList();
            }
            catch (Exception)
            {
                return new List<clsRoomButtonFormMain>();
            }
        }

        public string GetNameEmployee(string idEmployee)
        {
            try
            {
                var emp = from em in db.Employees where em.EmployeeID.Equals(idEmployee) select $"{em.LastName} {em.FirstName}";
                return emp.FirstOrDefault().ToString();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public List<clsStatisticalForFormMain> GetListRoomElectricityAndWater()
        {
            try
            {
                using (var context = new QLPhongTroDataContext())
                {
                    // Lấy tháng và năm hiện tại
                    string currentMonthYear = DateTime.Now.ToString("yyyy-MM");

                    // Truy vấn lấy dữ liệu phòng và số ghi điện/nước
                    var result = (from room in context.Rooms
                                  join contract in context.Contracts on room.RoomID equals contract.RoomID
                                  join elec in context.ElectricityReadings
                                      on new { ContractID = contract.ContractID, MonthYear = currentMonthYear }
                                      equals new { elec.ContractID, elec.MonthYear } into elecGroup
                                  from elec in elecGroup.DefaultIfEmpty()
                                  join water in context.WaterReadings
                                      on new { ContractID = contract.ContractID, MonthYear = currentMonthYear }
                                      equals new { water.ContractID, water.MonthYear } into waterGroup
                                  from water in waterGroup.DefaultIfEmpty()
                                  where room.Status == 1
                                  select new clsStatisticalForFormMain
                                  {
                                      nameRoom = room.RoomNumber,
                                      electricityReading = elec != null ? elec.NewReading : 0,
                                      waterReading = water != null ? water.NewReading : 0
                                  }).ToList();

                    return result;
                }
            }
            catch (Exception)
            {
                return new List<clsStatisticalForFormMain>();
            }
        }

        public int CountAvailableRooms()
        {
            using (var context = new QLPhongTroDataContext())
            {
                int count = context.Rooms.Count(room => room.Status == 0);
                return count;
            }
        }

        public int CountOccupiedRooms()
        {
            using (var context = new QLPhongTroDataContext())
            {
                int count = context.Rooms.Count(room => room.Status == 1);
                return count;
            }
        }

        public List<clsTypeRoomCount> CountRoomsByType()
        {
            using (var context = new QLPhongTroDataContext())
            {
                var result = (
                    from room in context.Rooms
                    join typeRoom in context.TypeRooms on room.TypeRoomID equals typeRoom.TypeRoomID
                    where room.TypeRoomID != null
                    group room by new { typeRoom.TypeRoomID, typeRoom.TypeRoomName } into grouped
                    select new clsTypeRoomCount
                    {
                        TypeRoomID = grouped.Key.TypeRoomID,
                        TypeRoomName = grouped.Key.TypeRoomName,
                        RoomCount = grouped.Count()
                    }
                ).ToList();

                return result;
            }
        }

        public List<clsStatisticalUtilitiesForFormMain> GetUtilityData()
        {
            using (var context = new QLPhongTroDataContext())
            {
                var result = context.Utilities
                    .Select(utility => new clsStatisticalUtilitiesForFormMain
                    {
                        UtilityID = utility.UtilityID.ToString(),
                        UtilityName = utility.UtilityName,
                        UnitPrice = utility.UnitPrice
                    })
                    .ToList();

                return result;
            }
        }

    }
}
