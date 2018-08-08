using FirstMVCProject.Email;
using FirstMVCProject.Models;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FirstMVCProject.Controllers
{
    public class UserController : Controller
    {
        mail newItem = new mail();
        columbaEntities1 _db = new columbaEntities1();
        public int sender;

        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(username model)
        {
            username login = _db.usernames.Where(p => p.name.Equals(model.name) && p.password_.Equals(model.password_)).SingleOrDefault();
            if (login != null)
            {
                Session["Id"] = login.Id;
                sender = ((int)Session["Id"]);
                return RedirectToAction("Index", "Home", new { UserId = sender });
            }
            ViewBag.issuccess = false;
            return View();
        }

        [HttpGet]
        public ActionResult Forgot()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Forgot(username model)
        {
            username fforgot = _db.usernames.Where(p => p.mail.Equals(model.mail)).SingleOrDefault();
            if (fforgot != null)
            {
                Session["email"] = fforgot.mail;
                Session["name"] = fforgot.name;
                Session["pass"] = fforgot.password_;

                string title, body;
                string main = "on";
                bool result = false;
                if (main == "on")
                {
                    title = "Columba - Password Reminder";
                    body = "Hi, <br><br>" + Session["name"].ToString().ToUpper() + ", <br/><br/>Your password is: " + Session["pass"];
                    result = newItem.SendMail(Session["email"].ToString(), title, body);

                    if (result == true)
                    {
                        ViewBag.issuccess = true;
                    }
                    else
                    {
                        ViewBag.issuccess = false;
                    }
                }
                return View();
            }
            else
                ViewBag.issuccess = false;

            return View();
        }

        [HttpGet]
        public ActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignUp(username model, HttpPostedFileBase image1, string pass)
        {
            username sign = _db.usernames.Where(p => p.mail.Equals(model.mail)).SingleOrDefault();

            if (sign == null)
            {
                if (pass != model.password_)
                {
                    ViewBag.notEqualPasswords = false;
                    return View();
                }
                else
                    ViewBag.issuccess = true;
                
                Session["email"] = model.mail;
                Session["name"] = model.name;
                Session["password"] = model.password_;

                if (image1 != null)
                {
                    model.image = new Byte[image1.ContentLength];
                    image1.InputStream.Read(model.image, 0, image1.ContentLength);
                }
                _db.usernames.Add(model);
                _db.SaveChanges();

                string title;
                string body;
                string main = "on";
                bool result = false;
                if (main == "on")
                {
                    title = "Columba - Sign Up Activation-Link";
                    body = "Hi, <br><br>" + Session["name"].ToString().ToUpper() +
                        ", <br/><br/><p>Your membership is active now, you can use Columba! </p>";
                    result = newItem.SendMail(Session["email"].ToString(), title, body);
                }
                model.isActive = 1;
                return View();
            }
            ViewBag.issuccess = false;
            return View();
        }
    }
}