namespace Integration.data.Models
{
    public class Module
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int priority { get; set; }
        public string? TableFromName { get; set; }
        public string? TableToName { get; set; }
        public string? ToPrimaryKeyName { get; set; }
        public string? fromPrimaryKeyName { get; set; }
        public string? LocalIdName { get; set; }
        public string? CloudIdName { get; set; }
        public int ToDbId { get; set; }
        public DataBase ToDb { get; set; }
        public int FromDbId { get; set; }
        public DataBase FromDb { get; set; }
        public SyncType SyncType { get; set; }
        public List<ColumnFrom> columnFroms { get; set; }
        public List<Operation>  Operations { get; set; }
        public string ToInsertFlagName { get; set; }
        public string ToUpdateFlagName { get; set; } 
        public string FromInsertFlagName { get; set; }
        public string FromUpdateFlagName { get; set; }
        public string ToDeleteFlagName { get; set; }
        public string FromDeleteFlagName { get; set; }
        public string? condition { get; set; }
        public bool isDisabled { get; set; }
    }









}


