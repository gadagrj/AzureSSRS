using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureSQLRestApi.Models
{
    public class AzureListViewModel
    {
        public SqlRestMetadata resultMetadata { get; set; }
        public List<SqlRestServiceData> serverList { get; set; }
    }
}