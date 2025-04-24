using System;

namespace DTO
{
    public class clsBillCustom1
    {
        public string BillId { get; set; }
        public string ContractId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime DateCreated { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
} 