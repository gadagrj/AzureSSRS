using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using AzureSQLRestApi.Models;

namespace AzureSQLRestApi.Infrastructure
{
    public class AzureRestCaller
    {
        private static string subscriptionID = "yourSubscrptionID";

        private static string certificateThumbprint = "‎YourTHumbPrint";

        private static string x_ms_version = "2013-05-19";

        private static string certificateFile = @"D:\RaviSqlRestAzure.cer";
        private List<SqlRestServiceData> _list { get; set; }

        public AzureListViewModel getAzureList(string subscriptionID)
        {
            SqlRestMetadata objMetdata = new SqlRestMetadata();
            try
            {
                //First read the certificate from certificate store which will be used to authenticate the request.
                X509Store certificateStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                certificateStore.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection certificates = certificateStore.Certificates;
                certificateThumbprint = Regex.Replace(certificateThumbprint, @"[^\u0000-\u007F]", string.Empty); // to remove whitespace. 
                var matchingCertificates = certificates.Find(X509FindType.FindByThumbprint, certificateThumbprint, false);
                var managementCert = matchingCertificates[0];

                X509Certificate2 cert = new X509Certificate2(certificateFile);
                string requestUrl = string.Format(CultureInfo.InvariantCulture,
                                                  "https://management.database.windows.net:8443/{0}/servers", subscriptionID);
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(requestUrl);
                req.Method = "GET";
                req.Headers.Add("x-ms-version", "1.0");
                req.ClientCertificates.Add(managementCert);

                using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
                {
                    _list = new List<SqlRestServiceData>();
                    objMetdata.Requestid = resp.Headers["x-ms-request-id"];
                    objMetdata.returnStatusCode = (int)((HttpWebResponse)resp).StatusCode;

                    using (Stream stream = resp.GetResponseStream())
                    {
                        using (StreamReader sr = new StreamReader(stream))
                        {
                            var xdoc = XElement.Load(sr);
                            XNamespace ns = "http://schemas.microsoft.com/sqlazure/2010/12/";
                            foreach (var urlElement in xdoc.Elements(ns + "Server"))
                            {
                                SqlRestServiceData returnData = new SqlRestServiceData();
                                returnData.ServerName = urlElement.Element(ns + "Name").Value;
                                returnData.AdminLogin = urlElement.Element(ns + "AdministratorLogin").Value;
                                returnData.DataCenterLocation = urlElement.Element(ns + "Location").Value;
                                _list.Add(returnData);
                            }
                        }
                    }
                }
            }
            catch (WebException webEx)
            {
                var resp = webEx.Response;
                objMetdata = new SqlRestMetadata();
                objMetdata.Requestid = resp.Headers["x-ms-request-id"];
                objMetdata.returnStatusCode = (int)((HttpWebResponse)resp).StatusCode;
                objMetdata.ErrorMsg = webEx.Message;
            }

            var returnValues = new AzureListViewModel();
            returnValues.resultMetadata = objMetdata;
            returnValues.serverList = _list;
            return returnValues;
        }


        public AzureCreateServerViewModel CreateServer(CreateServerModel objParams)
        {
            AzureCreateServerViewModel objCreateServerModel = new AzureCreateServerViewModel();
            //First read the certificate from certificate store which will be used to authenticate the request.
            try
            {
                X509Store certificateStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                certificateStore.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection certificates = certificateStore.Certificates;
                certificateThumbprint = Regex.Replace(certificateThumbprint, @"[^\u0000-\u007F]", string.Empty); // to remove whitespace. 
                var matchingCertificates = certificates.Find(X509FindType.FindByThumbprint, certificateThumbprint, false);
                var managementCert = matchingCertificates[0];

                X509Certificate2 cert = new X509Certificate2(certificateFile);
                string requestUrl = string.Format(CultureInfo.InvariantCulture,
                                                  "https://management.database.windows.net:8443/{0}/servers", objParams.subscriptionid);

                HttpWebRequest webRequest = HttpWebRequest.Create(requestUrl) as HttpWebRequest;
                webRequest.ClientCertificates.Add(managementCert);
                webRequest.Headers["x-ms-version"] = "1.0";
                webRequest.Method = "POST";

                //=== Add the Request Payload ===//

                string xmlBody = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                                  "<Server xmlns=\"http://schemas.microsoft.com/sqlazure/2010/12/\">\n" +
                                  "  <AdministratorLogin>" + objParams.AdminLogin + "</AdministratorLogin>\n" +
                                  "  <AdministratorLoginPassword>" + objParams.AdminLoginPassword + "</AdministratorLoginPassword>\n" +
                                  "  <Location>" + objParams.DataCenterLocation + "</Location>\n" +
                                  "</Server>";

                byte[] bytes = Encoding.UTF8.GetBytes(xmlBody);
                webRequest.ContentLength = bytes.Length;
                webRequest.ContentType = "application/xml;charset=utf-8";
                using (Stream requestStream = webRequest.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                }


                //=== Send the request and wait on the response ===//
                using (WebResponse webResponse = webRequest.GetResponse())
                {

                    //=== x-ms-request-id is useful for troubleshooting request failures with Microsoft ===//
                    objCreateServerModel.Requestid = webResponse.Headers["x-ms-request-id"];
                    objCreateServerModel.returnStatusCode = (int)((HttpWebResponse)webResponse).StatusCode;
                    objCreateServerModel.AdminName = objParams.AdminLogin;
                    objCreateServerModel.DataCenterLocation = objParams.DataCenterLocation;
                    using (Stream stream = webResponse.GetResponseStream())
                    {
                        using (XmlTextReader xmlReader = new XmlTextReader(stream))
                        {
                            while (xmlReader.Read())
                            {
                                switch (xmlReader.NodeType)
                                {
                                    case XmlNodeType.Element:
                                        if (xmlReader.Name == "ServerName")
                                        {
                                            if (xmlReader.Read() && (xmlReader.NodeType == XmlNodeType.Text))
                                            {
                                                objCreateServerModel.NewServerName = xmlReader.Value.ToString();
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }

            }
            catch (WebException webEx)
            {
                var resp = webEx.Response;
                objCreateServerModel = new AzureCreateServerViewModel();
                objCreateServerModel.Requestid = resp.Headers["x-ms-request-id"];
                objCreateServerModel.returnStatusCode = (int)((HttpWebResponse)resp).StatusCode;
                objCreateServerModel.ErrorMsg = webEx.Message;
            }

            return objCreateServerModel;
        }

    }
}