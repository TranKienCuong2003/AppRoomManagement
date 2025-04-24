using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class clsViewDetailRoom
    {
        public string idContract { get; set; }
        public int idRoom { get; set; }
        public string nameRoom { get; set; }
        public string idCustomer { get; set; }
        public string nameCustomer { get; set; }
        public string phoneCustomer { get; set; }
        public decimal priceRoom { get; set; }
        public DateTime startedAt { get; set; }
        public DateTime endedAt { get; set; }
        public string status { get; set; }
        public DateTime createdAt { get; set; }
    }
}
