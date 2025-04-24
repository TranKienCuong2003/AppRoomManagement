using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class clsBMReadingElectricity
    {
        public int id {  get; set; }
        public string contractID { get; set; }
        public string monthYear { get; set; }
        public decimal oldRead {  get; set; }
        public decimal newRead { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
    }
}
