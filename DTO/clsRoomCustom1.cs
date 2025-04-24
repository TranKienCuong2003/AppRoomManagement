using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class clsRoomCustom1
    {
        public int idroom {  get; set; }
        public string typeroomname { get; set; }
        public string roomname { get; set; }
        public decimal price { get; set; }
        public string status { get; set; }
        public DateTime createdat {  get; set; }
        public DateTime updatedat { get; set; }
    }
}
