using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Integration.data.Models;

namespace Integration.business.DTOs.OperationDTOs
{
    public class OperationForReturnDTO
    {
        public int Id { get; set; }
        public string? LocalId { get; set; }
        public string? CloudId { get; set; }
        public string? TableFrom { get; set; }
        public string? TableTo { get; set; }
        public string? PriceFrom { get; set; }
        public string? PriceTo { get; set; }
        public string? ItemId { get; set; }
        public string? fromInsertFlag { get; set; }
        public string? fromInsertDate { get; set; }
        public string? customerId { get; set; }
        public string? storeId { get; set; }
        public string? OPToPrimary { get; set; }
        public string? fromUpdateFlag { get; set; }
        public string? fromDeleteFlag { get; set; }
        public string? ToInsertFlag { get; set; }
        public string? ToUpdateFlag { get; set; }
        public string? ToDeleteFlag { get; set; }
        public string? Condition { get; set; }
        public string? OpCustomerReference { get; set; }
        public string? OPProductReference { get; set; }
        public OperationType operationType { get; set; }
        public string? OpToCustomerId { get; set; }
        public string? OpToProductId { get; set; }
        public int ModuleId { get; set; }

        public string? OpFromPrimary { get; set; }
        public string? ItemParent { get; set; }
        public string? OpInsertDate { get; set; }
        public string? OpUpdateDate { get; set; }
        public string? OpDeleteDate { get; set; }
    }
}
