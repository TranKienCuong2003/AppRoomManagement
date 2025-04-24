using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class clsBillToPDF
    {
        public string idBill {  get; set; }
        public string idContract { get; set; }
        public string idEmployee { get; set; }
        public string nameEmployee { get; set; }
        public string phoneEmployee { get; set; }
        public string idCustomer { get; set; }
        public string nameCustomer { get; set; }
        public string phoneCustomer { get; set; }
        public int idRoom { get; set; }
        public string nameRoom { get; set; }
        public decimal priceRoom { get; set; }
        public decimal totalAmount { get; set; }
        public string status { get; set; }
        public DateTime createdAt { get; set; }
    }
}
