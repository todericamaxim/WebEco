using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using WebEcom.Models;

namespace WebEcom.Controllers
{
    public class AdminController : Controller
    {
        WebEconEntities db = new WebEconEntities();
        
        public ActionResult A()
        {
            return View();
        }

        public ActionResult CreateCategory()
        {
            return View();
        }

        public ActionResult ViewCategory()
        {
            var categories = db.Category.ToList();
            return View(categories);
        }
        [HttpPost]
        public ActionResult CreateCategory(Category category, HttpPostedFileBase image)
        {
            category.ImageMimeType = image.ContentType;
            category.ImageData = new byte[image.ContentLength];
            image.InputStream.Read(category.ImageData, 0, image.ContentLength);
            db.Category.Add(category);
            db.SaveChanges();
            return RedirectToAction("CreateCategory");
        }

        public PartialViewResult List()
        {
            List<string> adminlist = new List<string>() {"A","ViewCategory", "CreateCategory", "ViewUsers" };
            return PartialView(adminlist.AsEnumerable()); ;
        }

        public ActionResult ViewUsers()
        {
            var users = db.User.ToList();
            
            return View(users);
        }

        public ActionResult DeleteUser(int id)
        {
            var user = db.User.Where(x => x.User_Id == id).FirstOrDefault();
            return View(user);
        }

        [HttpPost]
        public ActionResult DeleteUser(string id)
        {
            int ids = int.Parse(id);
            var user = db.User.Where(x => x.User_Id == ids).FirstOrDefault();
            db.User.Remove(user);
            db.SaveChanges();
            return RedirectToAction("ViewUsers");
        }

        public ActionResult EditUser(int id)
        {
            var eduser = db.User.Where(x => x.User_Id == id).FirstOrDefault();
            return View(eduser);
        }

        public ActionResult DetailUser(int id)
        {
            var detuser = db.User.Where(x => x.User_Id == id).FirstOrDefault();
           
            var prod = db.Product.Where(z => z.Id_USER == detuser.User_Id).ToList();
            ViewBag.Prod = prod;
            return View(detuser);
        }

        public ActionResult GetImageCategory(int id)
        {
            var categ = db.Category.FirstOrDefault(x => x.Id == id);
            return File(categ.ImageData, categ.ImageMimeType);
        }

        public ActionResult GetImageUser(int ids)
        {
            var users = db.User.FirstOrDefault(x => x.User_Id == ids);
            return File(users.ImageDATA, users.ImageMMType);
        }

        public ActionResult SendEmail(int send)
        {
            var obj = db.User.Where(x => x.User_Id == send).FirstOrDefault();
            Session["Sender"] = obj.User_Email;
            Session["Email"] = obj.User_Email.ToString();
            return View();
        }
        [HttpPost]
        public ViewResult SendEmail(WebEcom.Models.MailModel _objModelMail,int send)        {
            if (ModelState.IsValid)
            {
                MailMessage mail = new MailMessage();
                var obj = db.User.Where(x => x.User_Id == send).FirstOrDefault();

                _objModelMail.To = Session["Sender"].ToString();                
                mail.To.Add(_objModelMail.To);
                _objModelMail.From = "maxtod1987@gmail.com";
                mail.From = new MailAddress(_objModelMail.From);
                mail.Subject = _objModelMail.Subject;
                string Body = _objModelMail.Body;
                mail.Body = Body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("maxtod1987@gmail.com", "tcmist1987");   
                smtp.EnableSsl = true;
                smtp.Send(mail);
                
                return View("SendEmail", _objModelMail);
            }
            else
            {
                return View();
            }
        }

        public ActionResult DeleteCategory(int id)
        {
            var del = db.Category.Where(x => x.Id == id).FirstOrDefault();
            return View(del);
        }

        [HttpPost]
        public ActionResult DeleteCategory(string id)
        {
            int ids = int.Parse(id);
            var del = db.Category.Where(x => x.Id == ids).FirstOrDefault();
            var prod = db.Product.Where(x => x.Product_CategoryID == ids).ToList();
            foreach (var item in prod)
            {
                db.Product.Remove(item);
                db.SaveChanges();
            }
            db.Category.Remove(del);
            db.SaveChanges();
            return RedirectToAction("ViewCategory");
        }

    }
}
