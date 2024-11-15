﻿using Integration.data.Models;

namespace Integration.business.DTOs.FromDTOs
{
    public class DbToAddDTO
    {
        public string Name { get; set; }
        public string Connection { get; set; }

        public DataBaseType DataBaseType { get; set; }
    } 


    public class DbToEditDTO
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Connection { get; set; }
        public DataBaseType DataBaseType { get; set; }

    }
    public class DbToReturn
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Connection { get; set; }
        public bool IsSelected { get; set; }
        public string DataBaseType { get; set; }

    }
}
