namespace Integration.data.Models
{
    public class DataBase
    {
        public int Id { get; set; }
        public string DbName { get; set; }
        public string ConnectionString { get; set; }
        public DataBaseType dataBaseType { get; set; }
    }


}
