using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebEcom.Models;
using WebGrease.Css.Extensions;


namespace WebEcom.Controllers
{
    public class UserController : Controller
    {
        WebEconEntities db = new WebEconEntities();


        public ActionResult Home()
        {
           int ids= (int)Session["UserID"];
            var message = db.MessageTB.Where(x => x.ID_User == ids).ToList();
            if (message!=null)
            {
                ViewBag.Message = message;
            }
            return View();
        }
        public ActionResult CreateCategory()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult CreateCategory(Category category, HttpPostedFileBase image)
        {
            category.ImageMimeType = image.ContentType;
            category.ImageData = new byte[image.ContentLength];
            image.InputStream.Read(category.ImageData, 0, image.ContentLength);
            db.Category.Add(category);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult CreateProduct()
        {
            SelectList categories = new SelectList(db.Category, "Id", "Name");
            ViewBag.Categories = categories;
            return View();
        }

        [HttpPost]
        public ActionResult CreateProduct(Product product, HttpPostedFileBase image)
        {
            product.Id_USER = (int)Session["UserID"];
            if (Session["UserName"] != null)
            {
                product.ImageMimeType = image.ContentType;
                product.ImageData = new byte[image.ContentLength];
                image.InputStream.Read(product.ImageData, 0, image.ContentLength);
                
                db.Product.Add(product);
                db.SaveChanges();
                return RedirectToAction("ProductList");
            }
            else { return RedirectToAction("Index"); }
            
        }
        public ActionResult DeleteProduct()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DeleteProduct(int id)
        {
            if (Session["UserName"]!=null)
            {
                
                var delete = db.Product.Where(x => x.Product_Id == id).FirstOrDefault();
                db.Product.Remove(delete);
                db.SaveChanges();
                return RedirectToAction("ProductList");
            }
            return RedirectToAction("Index");
        }

        public ActionResult EditProduct(int id)
        {
            var edit = db.Product.Where(x => x.Product_Id == id).FirstOrDefault();
            SelectList categories = new SelectList(db.Category, "Id", "Name");
            ViewBag.Categories = categories;
            return View(edit);
        }

        [HttpPost]
        public ActionResult EditProduct(Product product, HttpPostedFileBase image)
        {
            if (Session["UserName"]!=null && Session["UserID"]!=null)
            {
                var editProd = db.Product.Where(x => x.Product_Id == product.Product_Id).FirstOrDefault();

                if (product.ImageData != null)
                {
                    product.ImageData = new byte[image.ContentLength];
                    image.InputStream.Read(product.ImageData, 0, image.ContentLength);
                }
                else
                {
                    product.ImageData = editProd.ImageData;
                    //product.ImageMimeType = editProd.ImageMimeType;
                }

                if (product.ImageMimeType != null)
                {
                    //product.ImageMimeType = image.ContentType;
                    product.ImageMimeType = editProd.ImageMimeType;

                }
                else
                {
                    product.ImageMimeType = editProd.ImageMimeType;
                }

                editProd.Product_Name = product.Product_Name;
                editProd.Product_Descr = product.Product_Descr;
                editProd.Product_CategoryID = product.Product_CategoryID;
                editProd.Product_price = product.Product_price;

                editProd.Id_USER = (int)Session["UserID"];
                db.SaveChanges();
                return RedirectToAction("ProductList");

            }
            return RedirectToAction("Index");
        }



        public ActionResult ProductList()
        {
            if (Session["UserName"] != null)
            {
                int ids = (int)Session["UserID"];
                var products = db.Product.Where(x=>x.Id_USER==ids).ToList();
                
                
                return View(products);
            }
            return RedirectToAction("CreateProduct");
        } 
        
        public PartialViewResult CreateProd()
        {
            List<string> cat = new List<string>() {"Home", "CreateProduct", "ProductList" };
            return PartialView(cat.AsEnumerable()); ;
        }


        public ActionResult GetImage(int id)
        {
            var categ = db.Category.FirstOrDefault(x => x.Id == id);
            return File(categ.ImageData, categ.ImageMimeType);
        }

        public FileContentResult GetImageProduct(int id)
        {
            var prod = db.Product.FirstOrDefault(x => x.Product_Id == id);
            return File(prod.ImageData, prod.ImageMimeType);
        }

        public ActionResult GetImageUser(int id)
        {
            var categ = db.User.FirstOrDefault(x => x.User_Id == id);
            return File(categ.ImageDATA, categ.ImageMMType);
        }

        public ActionResult Abandon()
        {
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            return RedirectToRoute(new { controller = "Principal", action = "Index" });
        }
    }
}