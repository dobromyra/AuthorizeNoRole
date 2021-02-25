﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AppForAuthorize.Models;

namespace AppForAuthorize.Controllers
{
    public class HomeController : Controller
    {
        private BaseNoRoleEntities db = new BaseNoRoleEntities();
        private static string GetMD5Hash(string text)
        {
            using (var hashAlg = MD5.Create()) // Создаем экземпляр класса реализующего алгоритм MD5
            {
                byte[] hash = hashAlg.ComputeHash(Encoding.UTF8.GetBytes(text)); // Хешируем байты строки text
                var builder = new StringBuilder(hash.Length * 2); // Создаем экземпляр StringBuilder. Этот класс предназначен для эффективного конструирования строк
                for (int i = 0; i < hash.Length; i++)
                {
                    builder.Append(hash[i].ToString("X2")); // Добавляем к строке очередной байт в виде строки в 16-й системе счисления
                }
                return builder.ToString(); // Возвращаем значение хеша
            }
        }

        [Authorize]
        public ActionResult Index()
        {
            //var x = Request.IsAuthenticated;
            //var y = User.Identity.Name;
            //var z = User.IsInRole("Admin");
            return View();
        }

        [Authorize]
        public ActionResult Logout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Home");
        }

        [AllowAnonymous]
        public ActionResult AccessError()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            if (Request.IsAuthenticated)
                return RedirectToAction("AccessError");
            else
                return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                string FormHash = GetMD5Hash(model.PassHash);
                var user = db.User.FirstOrDefault(u => u.Login == model.Login && u.PassHash == FormHash);
                if (user == null)
                {
                    ModelState.AddModelError("", "Пользователя с таким логином и паролем не существует");
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(model.Login, false);
                    Session["Menu"] = null;
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                        return Redirect(returnUrl);
                    else
                        return RedirectToAction("Index");
                }
            }
            return View(model);
        }
    }
}