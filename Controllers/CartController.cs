using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebEcom.Models;

namespace WebEcom.Controllers
{
    public class CartController : Controller
    {
        WebEconEntities db = new WebEconEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Buy(int id)
        {
            var prod = db.Product.Where(x => x.Product_Id == id).FirstOrDefault();
            int ids = (int)prod.Product_CategoryID;
            if (Session["cart"] == null)
            {
                List<Item> cart = new List<Item>();
                var product = db.Product.Where(x => x.Product_Id == id).FirstOrDefault();
                ids =(int)product.Product_CategoryID;

                cart.Add(new Item { Product = product, Quantity = 1 });
                Session["cart"] = cart;
                ViewBag.cart = cart;

            }
            else
            {
                List<Item> cart = (List<Item>)Session["cart"];
                int index = IsExist(id);
                if (index != -1)
                {
                    cart[index].Quantity++;
                }
                else
                {
                    cart.Add(new Item { Product = db.Product.Where(x => x.Product_Id == id).FirstOrDefault(), Quantity = 1 });                                    
                }
                Session["cart"] = cart;
                ViewBag.cart = cart;
            }

             
            return RedirectToRoute("", new { controller = "Principal", action = "ListProduct", id=ids });
        }

        public ActionResult Remove(int id)
        {
            List<Item> cart = (List<Item>)Session["cart"];
            int index = IsExist(id);
            cart.RemoveAt(index);
            Session["cart"] = cart;
            return RedirectToAction("Index");
        }

        private int IsExist(int id)
        {
            List<Item> cart = (List<Item>)Session["cart"];
            for (int i = 0; i < cart.Count; i++)
                if (cart[i].Product.Product_Id.Equals(id))
                    return i;
            return -1;
        }

        public FileContentResult GetImageProd(int id)
        {
            var prod = db.Product.FirstOrDefault(x => x.Product_Id == id);
            return File(prod.ImageData, prod.ImageMimeType);
        }
    }
}

