using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eshop.ViewModels
{
    public class AddOrUpdateProductViewModel
    {
        public int id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<decimal> price { get; set; }
        public Nullable<int> stock { get; set; }
        public string image1 { get; set; }
        public string image2 { get; set; }
        public string image3 { get; set; }
        public string thumbnail { get; set; }
        public Nullable<int> sales { get; set; }
        public Nullable<System.DateTime> createdAt { get; set; }
        public Nullable<int> categoryId { get; set; }

        public HttpPostedFileBase Image1 { get; set; }
        public HttpPostedFileBase Image2 { get; set; }
        public HttpPostedFileBase Image3 { get; set; }
        public HttpPostedFileBase Thumbnail { get; set; }

        public int CategoryID { get; set; }
        public string _string { get; set; }
        public string Type { get; set; }
    }
}