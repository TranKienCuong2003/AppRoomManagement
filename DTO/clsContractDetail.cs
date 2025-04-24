using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class clsContractDetail
    {
        public string idContract {  get; set; }
        public int idRoom { get; set; }
        public string roomName { get; set; }
        public string roomType { get; set; }
        public decimal roomPrice { get; set; }
        public string idCustomer { get; set; }
        public string nameCustomer { get; set; }
        public string phoneCustomer { get; set; }
        public string idEmployee { get; set; }
        public string nameEmployee { get; set; }
        public string phoneEmployee { get; set; }
        public DateTime startedAt { get; set; }
        public DateTime endedAt { get; set; }
        public decimal deposite {  get; set; }
        public string status { get; set; }
        public DateTime createdAt { get; set; }
    }
}
