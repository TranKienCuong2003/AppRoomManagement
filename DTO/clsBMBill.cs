using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class clsBMBill
    {
        public string idBill {  get; set; }
        public string idContract { get; set; }
        public string customer {  get; set; }
        public string employee { get; set; }
        public decimal price { get; set; }
        public DateTime? payDate { get; set; }
        public string status { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
    }
}
