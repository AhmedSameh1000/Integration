namespace Integration.data.Models
{
    public class ConditionTo
    {
        public int Id { get; set; }
        public string Operation { get; set; }
        public int ModuleId {  get; set; }
        public Module Module { get; set; }
    }


}


