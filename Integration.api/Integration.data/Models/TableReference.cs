namespace Integration.data.Models
{
    public class TableReference
    {
        public int Id { get; set; }
        public string TableFromName { get; set; }
        public string LocalName { get; set; }
        public string PrimaryName { get; set; }
        public int? ModuleId { get; set; }
        public Module Module { get; set; }
    }
}


