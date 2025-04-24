using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DTO;

namespace BLL
{
    public class RoomManagementBLL
    {
        QLPhongTroDataContext db;

        public RoomManagementBLL()
        {
            db = new QLPhongTroDataContext();
        }

        public List<clsRoomCustom1> GetListRoom()
        {
            try
            {
                var rooms = from ro in db.Rooms
                            join tr in db.TypeRooms on ro.TypeRoomID equals tr.TypeRoomID
                            select new clsRoomCustom1()
                            {
                                idroom = ro.RoomID,
                                typeroomname = tr.TypeRoomName,
                                roomname = ro.RoomNumber,
                                price = (decimal)ro.Price,
                                status = ro.Status == 0 ? "Còn trống" : ro.Status == 1 ? "Đã có người thuê" : "Chưa xác định",
                                createdat = (DateTime)ro.CreatedAt,
                                updatedat = (DateTime)ro.UpdatedAt,
                            };
                return rooms.ToList();
            }
            catch (Exception)
            {
                return new List<clsRoomCustom1>();
            }
        }
        
        public List<string> GetListTypeRoom()
        {
            try
            {
                var typeroom = from tr in db.TypeRooms select tr.TypeRoomName;
                return typeroom.ToList();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public int GetIdTypeRoom(string type)
        {
            try
            {
                var idTypeRoom = from ty in db.TypeRooms where ty.TypeRoomName == type select ty.TypeRoomID;
                return idTypeRoom.SingleOrDefault();
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public bool InsertRoom(int idTypeRoom, string roomNumber, decimal price, int status)
        {
            try
            {
                var room = new Room
                {
                    TypeRoomID = idTypeRoom,
                    RoomNumber = roomNumber,
                    Price = price,
                    Status = status,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                db.Rooms.InsertOnSubmit(room);
                db.SubmitChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteRoom(int idRoom)
        {
            try
            {
                var room = db.Rooms.SingleOrDefault(r => r.RoomID == idRoom);

                if (room == null)
                    return false;

                db.Rooms.DeleteOnSubmit(room);
                db.SubmitChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateRoom(int idRoom, int idTypeRoom, string roomNumber, decimal price, int status)
        {
            try
            {
                var room = db.Rooms.SingleOrDefault(r => r.RoomID == idRoom);

                if (room == null)
                    return false;

                room.TypeRoomID = idTypeRoom;
                room.RoomNumber = roomNumber;
                room.Price = price;
                room.Status = status;
                room.UpdatedAt = DateTime.Now;

                db.SubmitChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Room GetRoomById(int idRoom)
        {
            try
            {
                return db.Rooms.SingleOrDefault(r => r.RoomID == idRoom);
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
