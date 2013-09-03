using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureSQLRestApi.Models
{
    public class CreateServerModel
    {
        public string subscriptionid { get; set; }
        public string AdminLogin { get; set; }
        public string AdminLoginPassword { get; set; }
        public string DataCenterLocation { get; set; }
    }
}