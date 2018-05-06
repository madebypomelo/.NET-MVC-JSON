using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using Dashboard.Models;
using Newtonsoft.Json;

namespace Dashboard.Controllers
{
    public class DashboardController : Controller
    {
        public ActionResult Index()
        {
            //Create new instance of jsonViewModel.
            jsonViewModel jsonViewModel = new jsonViewModel();

            //Create variable for storing JSON.
            var JSON = "";
            //Create variable for query-string.
            var OnlyUnsent = Request.QueryString["OnlyUnsent"];

            //Load JSON from API
            using (WebClient httpClient = new WebClient())
            {
                //Set WebClient encoding.
                httpClient.Encoding = Encoding.UTF8;
                //Get JSON and store as a string.
                JSON = httpClient.DownloadString("http://api.fribikeshop.dk/noos-transmitted-dashboard");
            }

            //JsonConvert the JSON string.
            var Orders = JsonConvert.DeserializeObject<List<RootObject>>(JSON);

            //Create new list for orders.
            jsonViewModel.Orders = new List<ordersListed>();

            foreach (var newOrder in Orders)
            {
                //Check if store has been set.
                if (newOrder.Store == null)
                {
                    //Store isn't set. 
                    //Set it, and give "OrderContent -> Store -> Name" a value.
                    newOrder.Store = new store() { Name = "Ingen butik valgt" };
                }

                //Check if supplier is already listed for the specific date.
                if (!jsonViewModel.Orders.Any(A => A.Supplier.Guid == newOrder.Supplier.Guid && A.CreatedDate.ToString("dd/MM yyyy") == newOrder.CreatedDate.ToString("dd/MM yyyy")))
                {
                    //The supplier was not listed for the specific date.

                    //Only add the order to the list, if it has statuscode 1
                    if (OnlyUnsent == "true")
                    {
                        if (newOrder.Status == 1)
                        {
                            addToList();
                        }
                    }
                    else
                    {
                        addToList();
                    }
                    void addToList ()
                    {
                        jsonViewModel.Orders.Add(new ordersListed() { Supplier = new supplier() { Name = newOrder.Supplier.Name, Guid = newOrder.Supplier.Guid }, OrderContent = new List<RootObject>() { new RootObject() { Store = new store { Name = newOrder.Store.Name }, Status = newOrder.Status, OrderLines = new List<orderline>() } }, CreatedDate = newOrder.CreatedDate });

                        //Add OrderLines to the new OrderContent.
                        foreach (var item in newOrder.OrderLines)
                        {
                            jsonViewModel.Orders[jsonViewModel.Orders.Count() - 1].OrderContent[0].OrderLines.Add(new orderline() { Name = item.Name, Sku = item.Sku });
                        }
                    }

                } else
                {
                    //The supplier was already listed for the specific date. Add some new OrderContent to it.
                    if (OnlyUnsent == "true")
                    {
                        if (newOrder.Status == 1)
                        {
                            AddItemToList();
                        }
                    } else
                    {
                        AddItemToList();
                    }

                    void AddItemToList ()
                    {
                        var mainOrder = jsonViewModel.Orders.Where(A => A.CreatedDate.ToString("dd/MM yyyy") == newOrder.CreatedDate.ToString("dd/MM yyyy") && A.Supplier.Guid == newOrder.Supplier.Guid).FirstOrDefault();
                        mainOrder.OrderContent.Add(new RootObject() { Status = newOrder.Status, Store = new store() { Name = newOrder.Store.Name }, OrderLines = new List<orderline>() });

                        //Add OrderLines to the new SpecificOrder.
                        foreach (var item in newOrder.OrderLines)
                        {
                            mainOrder.OrderContent[mainOrder.OrderContent.Count() - 1].OrderLines.Add(new orderline() { Name = item.Name, Sku = item.Sku });
                        }
                    }
                }
            }

            //Order the Orders-list by newest date first, and limit it to the last 14 days.
            jsonViewModel.Orders = jsonViewModel.Orders.OrderByDescending(A => A.CreatedDate)
                                                       .Where(A => A.CreatedDate >= DateTime.Now.AddDays(-14)).ToList();

            //Order the Orders -> OrderContent-list by statuscodes.
            foreach (var Order in jsonViewModel.Orders)
            {
                Order.OrderContent = Order.OrderContent.OrderBy(A => A.Status).ToList();
            }

            //Return View, and pass ViewModel.
            return View(jsonViewModel);
        }
    }
}