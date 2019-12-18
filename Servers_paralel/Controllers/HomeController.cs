using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Net;
using Servers_paralel.Models;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Servers_paralel.Controllers
{
    public class HomeController : Controller
    {
        public static bool IsFirst = true;
        UserContext context = new UserContext();

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = null;
                using (UserContext db = new UserContext())
                {
                    user = db.Users.FirstOrDefault(u => u.Email == model.Email);
                }
                if (user == null)
                {
                    using (UserContext db = new UserContext())
                    {
                        db.Users.Add(new User { Email = model.Email, Password = model.Password, Age = model.Age, RoleId = 2 });
                        db.SaveChanges();

                        user = db.Users.Where(u => u.Email == model.Email && u.Password == model.Password).FirstOrDefault();
                    }
                    if (user != null)
                    {
                        FormsAuthentication.SetAuthCookie(model.Email, true);//Створює квиток перевірки справжності для зазначеного імені користувача і додає його до колекції файлів Cookie відповіді або до URL-адресу
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "User already exist!");
                }
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User user = null;
                using (UserContext db = new UserContext())
                {
                    user = db.Users.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);
                }
                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(model.Email, true);//Створює квиток перевірки справжності для зазначеного імені користувача і додає його до колекції файлів Cookie відповіді або до URL-адресу
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "User is not real!");
                }
            }
            return View(model);
        }

        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public ActionResult MakeTask()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MakeTask(Info model)
        {
            Info info = new Info();
            if (Int32.Parse(model.bytes) < 60)
            {
                info.bytes = model.bytes;
                info.IsDone = false;
                info.IsPaused = false;

                byte[] data = Encoding.Unicode.GetBytes(model.bytes);
                string address = "127.0.0.1"; // адрес сервера 
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                int port;

                int howlongsleep = Int32.Parse(model.bytes);
                if (IsFirst)
                {
                    port = 8005; // порт сервера    
                    info.WhichServer = 1;
                }
                else
                {
                    port = 8006;
                    info.WhichServer = 2;
                }
                IsFirst = !IsFirst;

                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);

                socket.Connect(ipPoint);
                socket.Send(data);

                socket.Shutdown(SocketShutdown.Both);
                socket.Close();


                using (UserContext db = new UserContext())
                {
                    db.Infos.Add(info);
                    db.SaveChanges();
                }

                Task task = Task.Factory.StartNew(() => todo(howlongsleep, info));

                return RedirectToAction("Index");
            }
            else
                return RedirectToAction("Problems");
        }

        public void todo(int howlong, Info inf)
        {
            for (int i = 0; i < howlong; i++)
            {
                if (!inf.IsPaused)
                {
                    using (UserContext db = new UserContext())
                    {
                        db.Infos.Find(inf.Id).Percentage = ((Convert.ToDouble(i) / Convert.ToDouble(howlong)) * 100);
                        db.SaveChanges();
                    }
                    Thread.Sleep(1000);
                }
                else
                    break;
            }
            if (!inf.IsPaused)
            {
                using (UserContext db = new UserContext())
                {
                    db.Infos.Find(inf.Id).Percentage = 100;
                    db.Infos.Find(inf.Id).IsDone = true;
                    db.SaveChanges();
                }
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public ActionResult ShowTasks()
        {
            IEnumerable<Info> phonesPerPages = context.Infos;
            return View(phonesPerPages);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public ActionResult Pause(int id)
        {
            using (UserContext db = new UserContext())
            {
                db.Infos.Find(id).IsPaused = true;
                todo(1, db.Infos.Find(id));
                db.SaveChanges();
            }
            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public ActionResult Problems()
        {
            return View();
        }
    }
}
