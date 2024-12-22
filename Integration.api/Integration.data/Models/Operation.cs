namespace Integration.data.Models
{
    public class Operation
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
        public string? fromUpdateFlag { get; set; }
        public string? fromDeleteFlag { get; set; } 
        public string? ToInsertFlag { get; set; }
        public string? ToUpdateFlag { get; set; }
        public string? ToDeleteFlag { get; set; }
        public string? Condition { get; set; }
        public string? fromPriceInsertDate { get; set; }
        public string? OpCustomerReference { get; set; }
        public string? OPProductReference { get; set; }


        public string? customerId { get; set; }
        public string? storeId { get; set; }

        public string? OPToItemPrimary { get; set; }
        public OperationType  operationType { get; set; }
        public string? OpToCustomerId { get; set; }
        public string? OpToProductId { get; set; }
        public int ModuleId { get; set; }
        public Module Module { get; set; }
  
        public string? OpFromPrimary { get; set; }
        public string? FromItemParent { get; set; }
        public string? OpFromInsertDate { get; set; }
        public string? OpFromUpdateDate { get; set; }
        public string? OpFromDeleteDate { get; set; }
    
    }



    public enum OperationType
    {
        Product,
        Customer,
        Store
    }
}


