using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PollWebServiceTest_UPC
{
    class UPC
    {

    }

    public class Offer
    {
        public string merchant { get; set; }
        public string domain { get; set; }
        public string title { get; set; }
        public string currency { get; set; }
        public string list_price { get; set; }
        public double price { get; set; }
        public string shipping { get; set; }
        public string condition { get; set; }
        public string availability { get; set; }
        public string link { get; set; }
        public int updated_t { get; set; }
    }

    public class Item
    {
        public string ean { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string upc { get; set; }
        public string brand { get; set; }
        public string model { get; set; }
        public string color { get; set; }
        public string size { get; set; }
        public string dimension { get; set; }
        public string weight { get; set; }
        public string category { get; set; }
        // override int to float
        public float lowest_recorded_price { get; set; }
        public double highest_recorded_price { get; set; }
        public IList<string> images { get; set; }
        public IList<Offer> offers { get; set; }
        public string asin { get; set; }
        public string elid { get; set; }
    }

    public class UPCAPI
    {
        public string code { get; set; }
        public int total { get; set; }
        public int offset { get; set; }
        public IList<Item> items { get; set; }
    }

}
