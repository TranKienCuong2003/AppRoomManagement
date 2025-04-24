using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class clsContractsToPDF
    {
        public string idContract { get; set; }
        public string nameOwner { get; set; }
        public string identityOwner { get; set; }
        public string phoneOwner { get; set; }
        public string addressOwner { get; set; }
        public string roomNumber {  get; set; }
        public decimal priceRoom {  get; set; }
        public decimal deposite {  get; set; }
        public string nameCustomer { get; set; }
        public string identityCustomer { get; set; }
        public string phoneCustomer { get; set; }
        public string addressCustomer { get; set; }
        public DateTime startedAt { get; set; }
        public DateTime endedAt { get; set; }
    }
}
