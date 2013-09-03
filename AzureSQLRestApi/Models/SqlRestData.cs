using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureSQLRestApi.Models
{
    public class SqlRestData
    {
        public  string RequestID { get; set; }
        public string ServerName { get; set; }
        public string AdminLogin { get; set; }
        public string DataCenterLocation { get; set; }
    }
}