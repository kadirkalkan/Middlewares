using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiddleWare.Middlewares
{
    public class HelloMiddleware
    {
        RequestDelegate _next;
        public HelloMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Response.ContentType = "text/html; charset=UTF-8";

            await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("Custom HelloMiddleware'ine Giriş Yapıldı <br/>"));

            await _next.Invoke(context);   // Sonraki Middleware'i Çağırıyoruz.

            await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("Custom HelloMiddleware'inden Çıkış Yapılıyor. <br/>"));
        }
    }
}
