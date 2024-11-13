namespace Integration.data.Models
{
    public class ColumnFrom
    {
        public int Id { get; set; }
        public string ColumnFromName { get; set; }
        public string ColumnToName { get; set; }
        public int ModuleId { get; set; }
        public Module Module { get; set; }
        public bool isReference { get; set; }
        public string? TableReferenceName { get; set; }
    }

}


