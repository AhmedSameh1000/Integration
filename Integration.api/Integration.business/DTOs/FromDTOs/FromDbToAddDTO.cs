using Integration.data.Models;

namespace Integration.business.DTOs.FromDTOs
{
    public class DbToAddDTO
    {
        public string Name { get; set; }
        public string Connection { get; set; }

        public string DataBaseType { get; set; }
    } 


    public class DbToEditDTO
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Connection { get; set; }
        public string DataBaseType { get; set; }

    }
    public class DbToReturn
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string DataBaseType { get; set; }
        public string Connection { get; set; }

    }  
    public class ColumnToAdd
    {

        public int dbId { get; set; }
        public string query { get; set; }
    }

}
