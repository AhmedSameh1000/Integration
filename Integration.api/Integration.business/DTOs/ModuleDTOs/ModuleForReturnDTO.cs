using Integration.data.Models;

namespace Integration.business.DTOs.ModuleDTOs
{
    public class ModuleForReturnDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TableFromName { get; set; }
        public string TableToName { get; set; }
        public string SyncType { get; set; }
    }
}
