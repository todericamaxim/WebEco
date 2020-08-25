using System.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;       
using System.Web.UI.WebControls;
using WebEcom.Models;

namespace WebEcom.Controllers
{
    public class PrincipalController : Controller
    {
        WebEconEntities db = new WebEconEntities();
        // GET: Principal
        public ActionResult Index()
        {
            var category = db.Category.ToList();
            return View(category);
        }

        [HttpGet]
        public ActionResult ListProduct(int id=1)
        {
            var prodlist = db.Product.Where(x => x.Product_CategoryID == id).ToList();
            return View(prodlist);
        }

        public ActionResult ProductDetail(int id)
        {
            var proddetail = db.Product.Where(x => x.Product_Id == id).FirstOrDefault();            
            var obj = db.User.Where(x => x.User_Id == proddetail.Id_USER).FirstOrDefault();
            Session["NameUser"] = obj.User_login;
            return View(proddetail);
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User user)
        {
            var obj = db.User.Where(x => x.User_login.Equals(user.User_login)
              && x.User_password.Equals(user.User_password)).FirstOrDefault();
            if (obj != null)
            {
                Session["UserID"] = obj.User_Id;
                Session["UserName"] = obj.User_login.ToString();
                return RedirectToRoute(new { controller = "User", action = "Home" });
            }
            else
            {
                return View("Login");
            }
        }

        [HttpGet]
        public ActionResult Register()
        { 
            return View();
        }

        [HttpPost]
        public ActionResult Register(User user,HttpPostedFileBase image)
        {
            
            user.ImageMMType = image.ContentType;
            user.ImageDATA = new byte[image.ContentLength];
            image.InputStream.Read(user.ImageDATA, 0, image.ContentLength); 
            
            db.User.Add(user);
            db.SaveChanges();
            
            return RedirectToAction("Login");
        }

        public FileContentResult GetImageProduct(int id)
        {
            var prod = db.Product.FirstOrDefault(x => x.Product_Id == id);
            return File(prod.ImageData, prod.ImageMimeType);
        }

        public PartialViewResult ListHome()
        {
            var list = db.Category.ToList();
            Dictionary<string, int> dic = new Dictionary<string, int>();
            foreach (var item in list)
            {
                dic.Add(item.Name.ToString(),(int)item.Id);               
            }            
            return PartialView(dic); ;
        }

        public ActionResult GetImage(int id)
        {
            var userimage = db.User.FirstOrDefault(x => x.User_Id == id);
            return File(userimage.ImageDATA, userimage.ImageMMType);
        }

        



        [HttpPost]
        public ActionResult Client_create(Client client)
        {
            var newclient = db.Client.FirstOrDefault();
            ViewBag.Client = newclient;
            List<int> product_IDs = new List<int>();
            
            foreach (var i in (List<Item>)Session["cart"])
            {
                product_IDs.Add(i.Product.Product_Id);
            }

            string json = JsonConvert.SerializeObject(product_IDs, Formatting.Indented);            
            client.Client_json = json;
            db.Client.Add(client);
            db.SaveChanges();
            SendMessageToUser();
            return RedirectToRoute("", new { controller = "Principal", action = "Index"});
        }
        
        public void SendMessageToUser()
        {
            var clientlist = db.Client.ToList();

            foreach (var item in clientlist)
            {
                if (item.Client_json != null)
                {
                    List<int> deserializeJsonList = JsonConvert.DeserializeObject<List<int>>(item.Client_json);
                    foreach (var i in deserializeJsonList)
                    {
                        var produs = db.Product.Where(x => x.Product_Id == i).FirstOrDefault();
                        if (produs!=null)
                        {                        
                        var user = db.User.Where(z => z.User_Id == produs.Id_USER).FirstOrDefault();

                        user.User_message = $"Client {item.Client_name} want to buy {produs.Product_Name} " +
                            $"and deliver to address {item.Client_adress}. You can connect with {item.Client_name} " +
                            $"throught client email: {item.Client_Email} ";
                        MessageTB message = new MessageTB();
                        message.Message = user.User_message;
                        message.ID_User = user.User_Id;
                        db.MessageTB.Add(message);
                        db.SaveChanges();
                        }
                    }

                }
            }

        }



    }
    
}
