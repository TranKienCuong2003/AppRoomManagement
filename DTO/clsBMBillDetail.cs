using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class clsBMBillDetail
    {
        public int idBillDetail {  get; set; }
        public string idBill {  get; set; }
        public string idUtility { get; set; }
        public string nameUtility { get; set; }
        public decimal amount {  get; set; }
        public DateTime createdAt { get; set; }
    }
}
