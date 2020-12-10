using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Carcharoth
{
    public class CookiesClass
    {
        public void WriteCookie(string name, string value)
        {
            var cookie = new HttpCookie(name, value);
            cookie.Expires = DateTime.Now.AddDays(1);
            HttpContext.Current.Response.Cookies.Set(cookie);
        }
        public string ReadCookie(string name)
        {
            if (HttpContext.Current.Response.Cookies.AllKeys.Contains(name))
            {
                var cookie = HttpContext.Current.Response.Cookies[name];
                return cookie.Value;
            }

            if (HttpContext.Current.Request.Cookies.AllKeys.Contains(name))
            {
                var cookie = HttpContext.Current.Request.Cookies[name];
                return cookie.Value;
            }
            return null;
        }
        public void ChangeCookie(string name,string value)
        {
            if (HttpContext.Current.Response.Cookies.AllKeys.Contains(name))
            {
                var cookie = HttpContext.Current.Response.Cookies[name];
                cookie.Expires = DateTime.Now.AddDays(-1);
            }

            if (HttpContext.Current.Request.Cookies.AllKeys.Contains(name))
            {
                var cookie = HttpContext.Current.Request.Cookies[name];
                cookie.Expires = DateTime.Now.AddDays(-1);
            }
            WriteCookie(name,value);
        }
    }
}