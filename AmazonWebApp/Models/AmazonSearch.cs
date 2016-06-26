using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AmazonWebApp.Models
{
    public class AmazonSearch
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Price { get; set; }
    }
}