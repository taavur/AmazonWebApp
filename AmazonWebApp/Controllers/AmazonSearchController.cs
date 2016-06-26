using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using AmazonProductAdvtApi;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace AmazonWebApp.Controllers {
    public class AmazonSearchController : Controller {
        private const string keyID = "AKIAJLWDSUOXUJKC2FWA";
        private const string secKey = "aZIiyItwX8rkpJrauU90Be4xxpWu0H95jMTkEDau";
        private const string tag = "prograpracti-21";
        private const string destination = "webservices.amazon.co.uk";
        public string jsonResultsString = "None";
        XNamespace ns = "http://webservices.amazon.com/AWSECommerceService/2011-08-01";

        public ActionResult Index() {
            return View();
        }

        public ActionResult Search(string searchString) {
            if (searchString.Length < 3) {
                ViewBag.Message = "Searching keyword must be longer than 3 characters.";
                return View("Error");
            }
            // Delete existing json file that may contain old data
            System.IO.File.Delete(Server.MapPath("~/Content/results.json"));
            int totalPages = 0;

            // Requesting 1st results page and save it in variable
            string requestUrl = GetRequestUrl("1", searchString);
            var xmlResults = GetResponse(requestUrl);

            // Getting number of total pages
            totalPages = Int32.Parse(xmlResults.Element(ns + "Items").Element(ns + "TotalResults").Value);
            ViewBag.Message = "Found " + xmlResults.Element(ns + "Items").Element(ns + "TotalResults").Value + " items";

            XElement resultItems = new XElement("Items");
            resultItems.Add(xmlResults.Element(ns + "Items").Elements(ns + "Item"));

            // Collecting all accessible data from request
            if (totalPages > 1) {
                for (int i = 2; (i <= 5 && i <= totalPages); i++) {
                    requestUrl = GetRequestUrl(i.ToString(), searchString);
                    xmlResults = GetResponse(requestUrl);
                    resultItems.Add(xmlResults.Element(ns + "Items").Elements(ns + "Item"));
                }
            }

            // Converting XML to Json and saving it
            jsonResultsString = JsonConvert.SerializeXNode(resultItems, Formatting.Indented);
            System.IO.File.WriteAllText(Server.MapPath("~/Content/results.json"), jsonResultsString);

            return View("search");
        }

        private string GetRequestUrl(string pageNr, string searchString) {
            SignedRequestHelper signingHelper = new SignedRequestHelper(keyID, tag, secKey, destination);

            // Assembling request
            IDictionary<string, string> request = new Dictionary<string, String>();
            request["Service"] = "AWSECommerceService";
            request["Operation"] = "ItemSearch";
            request["SearchIndex"] = "All";
            request["Keywords"] = searchString;
            request["ResponseGroup"] = "Medium";
            request["ItemPage"] = pageNr;

            return signingHelper.Sign(request);
        }

        private XElement GetResponse(string url) {
            // Sending request to Amazon and getting response
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            return XElement.Load(response.GetResponseStream());
        }
    }
}
