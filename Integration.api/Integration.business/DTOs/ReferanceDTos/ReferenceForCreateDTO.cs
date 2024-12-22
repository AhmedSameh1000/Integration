namespace Integration.business.DTOs.ReferenceDTos
{
    public class ReferenceForCreateDTO
    {
        public int moduleId { get; set; }
        public string TableFromName { get; set; }
        public string LocalName { get; set; }
        public string PrimaryName { get; set; }
        public string? Alter { get; set; }
    }
}

