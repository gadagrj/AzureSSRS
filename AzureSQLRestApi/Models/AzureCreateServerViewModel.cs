using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureSQLRestApi.Models
{
    public class AzureCreateServerViewModel
    {
        public string Requestid { get; set; }
        public int returnStatusCode { get; set; }
        public string ErrorMsg { get; set; }
        public string NewServerName { get; set; }
        public string DataCenterLocation { get; set; }
        public string AdminName { get; set; }
    }
}