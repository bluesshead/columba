using FirstMVCProject.Models;
using FirstMVCProject.Models.UIModel;
using System;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Media;

namespace FirstMVCProject.Controllers
{
    public class HomeController : Controller
    {
        columbaEntities1 _db = new columbaEntities1();
        MessageViewModel mvm = new MessageViewModel();
        SoundPlayer splayer;
        int i = 0;

        #region Pages

        public ActionResult Index(int id = 0)
        {
            i = id;

            if (Session["Id"] == null)
            {
                return Redirect("/User/Login");
            }
            int BenimId = Convert.ToInt32(Session["Id"].ToString());
            ViewBag.BenimId = BenimId;
            username login = _db.usernames.Where(p => p.Id.Equals(BenimId)).SingleOrDefault();
            login.isOnline = 1;
            _db.SaveChanges();

            if (id == 0)
            {
                return View();
            }
            else
            {
                mvm.User = _db.usernames.Where(p => p.Id.Equals(id)).SingleOrDefault();

                mvm.MessageList =
                    _db.chats.Where(r =>
                        (r.Receiver == BenimId && r.Sender.Equals(id)) ||
                        (r.Receiver == id && r.Sender == BenimId)).OrderBy(a => a.Id).ToList();
                if (WhoIsLastSender(id) != BenimId.ToString())
                {
                    for (int i = 0; i < mvm.MessageList.Count; i++)
                    {
                        mvm.MessageList[i].Readable = 1;
                        _db.SaveChanges();
                    }
                }
                
                mvm.Message = new chat();
                mvm.Message.Receiver = id;
                mvm.Message.Sender = BenimId;

                return View(mvm);
            }
        }

        #endregion Pages

        #region PartialPages
        [HttpPost]
        public ActionResult MessageSend(MessageViewModel newMessage)
        {
            if (newMessage.Message.Message != null)
            {
                int BenimId = Convert.ToInt32(Session["Id"].ToString());

                newMessage.Message.Date = DateTime.Now;
                Session["messages"] = newMessage.Message.Message;
                newMessage.Message.Readable = 0;

                _db.chats.Add(newMessage.Message);
                _db.SaveChanges();

                username receiver = _db.usernames.Where(p => p.Id.Equals(newMessage.Message.Receiver)).SingleOrDefault();

                if (receiver.isOnline == 1)
                {

                }
            }
            Play();
            return RedirectToAction("Index", "Home", new { id = newMessage.Message.Receiver });
        }
        
        public ActionResult UserHeader()
        {
            return PartialView();
        }
        public ActionResult Search()
        {
            return PartialView();
        }
        public ActionResult UserSearch()
        {
            return PartialView();
        }
        public ActionResult SideBar()
        {
            forLast fl = new forLast();

            int BenimId = Convert.ToInt32(Session["Id"].ToString());

            fl.LastList = new List<string> { };
            fl.Lastdate = new List<string> { };
            fl.User = _db.usernames.Where(s => s.Id != BenimId).ToList();


            for (int i = 0; i < fl.User.Count; i++)
            {
                string m = LastMessage(fl.User[i].Id);
                string n = LastDate(fl.User[i].Id);

                if (m == null)
                {
                    (fl.User).Remove(fl.User[i]);
                    i = i - 1;
                }
                else
                {
                    (fl.LastList).Add(m);
                    (fl.Lastdate).Add(n);
                }
            }
            return PartialView(fl);
        }
        public ActionResult UserSideBar()
        {
            int BenimId = Convert.ToInt32(Session["Id"].ToString());
            var users = _db.usernames.Where(s => s.Id != BenimId).ToList();

            return PartialView(users);
        }
        public string WhoIsLastSender(int a)
        {
            int BenimId = Convert.ToInt32(Session["Id"].ToString());

            SqlConnection baglanti = new SqlConnection("Data Source=MIS-STJ;Initial Catalog=columba;Integrated Security=True");
            SqlCommand komut = new SqlCommand();
            baglanti.Open();
            komut.Connection = baglanti;
            komut.CommandText = "select *from chat where Message IS NOT NULL and Date = (select max(Date) from chat where ((Sender = @s AND Receiver = @r) OR(Sender = @r AND Receiver = @s)))";
            komut.Parameters.AddWithValue("@s", BenimId);
            komut.Parameters.AddWithValue("@r", a);
            komut.ExecuteNonQuery();
            SqlDataReader dr = komut.ExecuteReader();

            if (dr.Read())
            {
                string sender = dr["Sender"].ToString();
                dr.Close();
                baglanti.Dispose();
                baglanti.Close();
                return sender;
            }
            return null;
        }
        public string LastMessage(int a)
        {
            int BenimId = Convert.ToInt32(Session["Id"].ToString());

            SqlConnection baglanti = new SqlConnection("Data Source=MIS-STJ;Initial Catalog=columba;Integrated Security=True");
            SqlCommand komut = new SqlCommand();
            baglanti.Open();
            komut.Connection = baglanti;
            komut.CommandText = "select *from chat where Message IS NOT NULL and Date = (select max(Date) from chat where ((Sender = @s AND Receiver = @r) OR(Sender = @r AND Receiver = @s)))";
            komut.Parameters.AddWithValue("@s", BenimId);
            komut.Parameters.AddWithValue("@r", a);
            komut.ExecuteNonQuery();
            SqlDataReader dr = komut.ExecuteReader();

            if (dr.Read())
            {
                string message = dr["Message"].ToString();
                if (message != null)
                {
                    if ((dr["Readable"].ToString() == "0") && (dr["Sender"].ToString() != BenimId.ToString()))
                    {
                        dr.Close();
                        baglanti.Dispose();
                        baglanti.Close();
                        string newone = "You have a new message!";
                        return (newone);
                    }
                    else
                    {
                        dr.Close();
                        baglanti.Dispose();
                        baglanti.Close();
                        return (message);
                    }
                }
                else
                {
                    dr.Close();
                    baglanti.Dispose();
                    baglanti.Close();
                    return null;
                }
            }
            dr.Close();
            baglanti.Dispose();
            baglanti.Close();
            return null;
        }
        public string LastDate(int a)
        {
            int BenimId = Convert.ToInt32(Session["Id"].ToString());

            SqlConnection baglanti = new SqlConnection("Data Source=MIS-STJ;Initial Catalog=columba;Integrated Security=True");
            SqlCommand komut = new SqlCommand();
            baglanti.Open();
            komut.Connection = baglanti;
            komut.CommandText = "select *from chat where Message IS NOT NULL and Date = (select max(Date) from chat where ((Sender = @s AND Receiver = @r) OR(Sender = @r AND Receiver = @s)))";
            komut.Parameters.AddWithValue("@s", BenimId);
            komut.Parameters.AddWithValue("@r", a);
            komut.ExecuteNonQuery();
            SqlDataReader dr = komut.ExecuteReader();

            if (dr.Read())
            {
                string message = dr["Date"].ToString();
                if (message != null)
                {
                    dr.Close();
                    baglanti.Dispose();
                    baglanti.Close();
                    return (message);
                }
                else
                {
                    dr.Close();
                    baglanti.Dispose();
                    baglanti.Close();
                    return null;
                }
            }
            dr.Close();
            baglanti.Dispose();
            baglanti.Close();
            return null;
        }
        public void Play() 
        {
            splayer = new SoundPlayer(@"C:\Projects\Columbam\FirstMVCProject\FirstMVCProject\FirstMVCProject\Content\music\pop.wav");
            splayer.Play();
            return;
        }
        public ActionResult Header()
        {
            int BenimId = Convert.ToInt32(Session["Id"].ToString());
            var users = _db.usernames.Where(s => s.Id.Equals(BenimId)).ToList();

            return PartialView(users);
        }
        public ActionResult Exit()
        {
            int BenimId = Convert.ToInt32(Session["Id"].ToString());

            username login = _db.usernames.Where(p => p.Id.Equals(BenimId)).SingleOrDefault();
            login.LastSeen = DateTime.Now;
            login.isOnline = 0;
            _db.SaveChanges();
            Session["Id"] = null;
            return RedirectToAction("Index", "Home");


        }
        #endregion Partial Pages
    }
}








