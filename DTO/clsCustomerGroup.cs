using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class clsCustomerGroup
    {
        public string idContract {  get; set; }
        public string idCustomer {  get; set; }
        public string nameCustomer { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
    }
}
