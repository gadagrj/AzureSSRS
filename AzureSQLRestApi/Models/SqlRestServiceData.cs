using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureSQLRestApi.Models
{
    public class SqlRestServiceData
    {

        public string ServerName { get; set; }
        public string AdminLogin { get; set; }
        public string DataCenterLocation { get; set; }
    }

    public class SqlRestMetadata
    {
        public string Requestid { get; set; }
        public int returnStatusCode { get; set; }
        public string ErrorMsg { get; set; }
    }
}

