using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dashboard.Models
{
    //Declaring classes
    public class jsonViewModel
    {
        public List<ordersListed> Orders { get; set; }
    }

    public class ordersListed
    {
        public DateTime CreatedDate { get; set; }
        public supplier Supplier { get; set; }
        public List<RootObject> OrderContent { get; set; }
    }

    public class RootObject
    {
        public store Store { get; set; }
        public supplier Supplier { get; set; }
        public int Status { get; set; }
        public List<orderline> OrderLines { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class supplier
    {
        public string Guid { get; set; }
        public string Name { get; set; }
    }

    public class store
    {
        public string Name { get; set; }
    }

    public class orderline
    {
        public string Name { get; set; }
        public string Sku { get; set; }
    }
}