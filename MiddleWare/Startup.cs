using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MiddleWare.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiddleWare
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            #region Middleware Açýklama
            /*
               Middleware'ler Request'den Response'a kadar olan Bütün Pipeline'ý yönetirler.

               Middleware'ler zincir mantýðý ile çalýþýr fakat Linq Gibi X'in Çýktýsýný Y'ye, Y'nin Çýktýsýný Z'ye Göndermezler.
               Bunun Yerine X'in Ýçerisinde Y, Y'nin Ýçerisinde Z Çalýþtýrýlýr. Ýsteðin Geldiði ve Cevabýn Döndüðü Tek Nokta Vardýr Oda X'dir.

               Linq Zincir Mantýðý         =>   [] -> [] -> [] -> [] -> [] -> []
               Middleware Zincir Mantýðý   =>   [ -> [ -> [ -> [ -> [ ] <- ] <- ] <- ] <- ]

               Build-in Middlewareler : 
               Run         -> Kendisinden Sonraki Middleware'i Çaðýrmaz.
               Use         -> Kendisinden Sonraki Middleware'i Çaðýrýr.
               Map         -> Spesifik bir endpoint'e gelen Request'lerde çalýþýr.
               MapWhen     -> Bir Requestin Durumunu Koþul Mantýðýyla Sorgulayarak Çalýþmaya Karar Verir.
                */
            #endregion

            #region Run Middleware
            // Run Middleware'i Kendisi çalýþtýktan sonra Bir Sonraki Middleware'i çaðýrmaz. Bundan Dolayý Çalýþmayý Sekteye Uðratabilir.
            //app.Run(async (context) => { await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("Run Middleware'i Çalýþtý. <br />")); });

            #endregion

            #region Use Middleware
            //Run metoduna nazaran, devreye girdikten sonra süreçte sýradaki milddeware'i çaðýrýr. Normal Middleware iþlemi bittikten sonra geride dönüp iþlem yapabilir.
            //app.Use(async (context, next) => // next bir sonraki middleware'i temsil eder.
            //{
            //    context.Response.ContentType = "text/html; charset=UTF-8";

            //    await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("Use Middleware'ine Giriþ Yapýldý <br/>"));

            //    await next.Invoke();   // Sonraki Middleware'i Çaðýrýyoruz.

            //    await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("Use Middleware'inden Çýkýþ Yapýlýyor. <br/>"));
            //});

            //app.Run(async (context) => { await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("Run Middleware'i Çalýþtý. <br/>")); });
            #endregion

            #region Map Middleware
            // Özellikle Bir Endpoint'e gelen isteklerde çalýþtýrmak istediðimiz middleware'leri Map ile tanýmlýyoruz.
            // Ör: Sadece /Home/index'e istek geldiðinde çalýþacak Middleware.
            //app.Map("/home/privacy", builder =>
            //{
            //    builder.Use(async (context, next) =>
            //    {
            //        context.Response.ContentType = "text/html; charset=UTF-8";

            //        await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("Map Middleware'ine Giriþ Yapýldý. Istek /Home/Privacy'e yapýldý. <br/>"));

            //        await next.Invoke();   // Sonraki Middleware'i Çaðýrýyoruz.
            //    });
            //});
            #endregion

            #region MapWhen Middleware
            // Sadece Request'in yapýldýðý path'e göre deðil ayný zamanda Request'in özelliðine görede bizim filtreleme yapmamýzý saðlar.
            //app.MapWhen(c => c.Request.Method == "POST" && c.Request.Path == "/Home/Privacy/15", builder =>
            //{
            //    builder.Use(async (context, next) =>
            //    {
            //        context.Response.ContentType = "text/html; charset=UTF-8";

            //        await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("MapWhen Middleware'ine Giriþ Yapýldý. Istek /Home/Privacy/15'e yapýldý. <br/>"));

            //        await next.Invoke();   // Sonraki Middleware'i Çaðýrýyoruz. 
            //    });
            //});
            #endregion

            #region Custom Middleware
            app.UseHello();
            // Run Middleware'ini buraya koymaktaki maksat Custom Middleware'in GeriDönüþ Verisini Görebilmek. Mantýksal açýdan diðer middleware'lerin çalýþmasý istenen sonucun görülmesini engelleyebiliyor.
            app.Run(async (context) => { await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("Run Middleware'i Çalýþtý. <br />")); });
            #endregion

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }


        // Örnek Middleware Çalýþma Mantýðý.
        // Context -> Ýçerisinde Request'i ve Response'u Barýndýran HttpContext objesini Refere Eder.
        string Middleware1()
        {
            string context = "Hello";
            context = Middleware2(context);
            return context;
        }

        string Middleware2(string context)
        {
            context += "What's ";
            context = Middleware3(context);
            return context;
        }
        string Middleware3(string context)
        {
            context += "Up?";
            return context;
        }
    }
}
