namespace Integration.business.DTOs.ModuleDTOs
{
    public class ColumnFromDTOOOOO
    {
        public int Id { get; set; }
        public string ColumnFromName { get; set; }
        public string ColumnToName { get; set; }
        public int ModuleId { get; set; }
        public bool isReference { get; set; }
        public bool IsAlter { get; set; }
        public string? TableReferenceName { get; set; }
        public bool isUsed { get; set; }
        public string key { get; set; }
    }
}
