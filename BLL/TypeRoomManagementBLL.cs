using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class TypeRoomManagementBLL
    {
        QLPhongTroDataContext db;

        public TypeRoomManagementBLL()
        {
            db = new QLPhongTroDataContext();
        }

        public List<clsTypeRoomCustom1> GetListTypeRoom()
        {
            try
            {
                var rooms = from tr in db.TypeRooms
                            select new clsTypeRoomCustom1()
                            {
                                idtyperoom = tr.TypeRoomID,
                                nametyperoom = tr.TypeRoomName,
                                quantity = (int)tr.Quantity,
                                created = (DateTime)tr.CreatedAt,
                                updated = (DateTime)tr.UpdatedAt,
                            };
                return rooms.ToList();
            }
            catch (Exception)
            {
                return new List<clsTypeRoomCustom1>();
            }
        }

        public bool InsertTypeRoom(string name, int quantity)
        {
            try
            {
                var typeroom = new TypeRoom
                {
                    TypeRoomName = name,
                    Quantity = quantity,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                db.TypeRooms.InsertOnSubmit(typeroom);
                db.SubmitChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteTypeRoom(int idtypeRoom)
        {
            try
            {
                var typeroom = db.TypeRooms.SingleOrDefault(r => r.TypeRoomID == idtypeRoom);

                if (typeroom == null)
                    return false;

                db.TypeRooms.DeleteOnSubmit(typeroom);
                db.SubmitChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateTypeRoom(int idTypeRoom, string name, int quantity)
        {
            try
            {
                var typeroom = db.TypeRooms.SingleOrDefault(r => r.TypeRoomID == idTypeRoom);

                if (typeroom == null)
                    return false;

                typeroom.TypeRoomID = idTypeRoom;
                typeroom.TypeRoomName = name;
                typeroom.Quantity = quantity;
                typeroom.UpdatedAt = DateTime.Now;

                db.SubmitChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
