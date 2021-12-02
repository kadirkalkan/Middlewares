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
            #region Middleware A��klama
            /*
               Middleware'ler Request'den Response'a kadar olan B�t�n Pipeline'� y�netirler.

               Middleware'ler zincir mant��� ile �al���r fakat Linq Gibi X'in ��kt�s�n� Y'ye, Y'nin ��kt�s�n� Z'ye G�ndermezler.
               Bunun Yerine X'in ��erisinde Y, Y'nin ��erisinde Z �al��t�r�l�r. �ste�in Geldi�i ve Cevab�n D�nd��� Tek Nokta Vard�r Oda X'dir.

               Linq Zincir Mant���         =>   [] -> [] -> [] -> [] -> [] -> []
               Middleware Zincir Mant���   =>   [ -> [ -> [ -> [ -> [ ] <- ] <- ] <- ] <- ]

               Build-in Middlewareler : 
               Run         -> Kendisinden Sonraki Middleware'i �a��rmaz.
               Use         -> Kendisinden Sonraki Middleware'i �a��r�r.
               Map         -> Spesifik bir endpoint'e gelen Request'lerde �al���r.
               MapWhen     -> Bir Requestin Durumunu Ko�ul Mant���yla Sorgulayarak �al��maya Karar Verir.
                */
            #endregion

            #region Run Middleware
            // Run Middleware'i Kendisi �al��t�ktan sonra Bir Sonraki Middleware'i �a��rmaz. Bundan Dolay� �al��may� Sekteye U�ratabilir.
            //app.Run(async (context) => { await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("Run Middleware'i �al��t�. <br />")); });

            #endregion

            #region Use Middleware
            //Run metoduna nazaran, devreye girdikten sonra s�re�te s�radaki milddeware'i �a��r�r. Normal Middleware i�lemi bittikten sonra geride d�n�p i�lem yapabilir.
            //app.Use(async (context, next) => // next bir sonraki middleware'i temsil eder.
            //{
            //    context.Response.ContentType = "text/html; charset=UTF-8";

            //    await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("Use Middleware'ine Giri� Yap�ld� <br/>"));

            //    await next.Invoke();   // Sonraki Middleware'i �a��r�yoruz.

            //    await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("Use Middleware'inden ��k�� Yap�l�yor. <br/>"));
            //});

            //app.Run(async (context) => { await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("Run Middleware'i �al��t�. <br/>")); });
            #endregion

            #region Map Middleware
            // �zellikle Bir Endpoint'e gelen isteklerde �al��t�rmak istedi�imiz middleware'leri Map ile tan�ml�yoruz.
            // �r: Sadece /Home/index'e istek geldi�inde �al��acak Middleware.
            //app.Map("/home/privacy", builder =>
            //{
            //    builder.Use(async (context, next) =>
            //    {
            //        context.Response.ContentType = "text/html; charset=UTF-8";

            //        await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("Map Middleware'ine Giri� Yap�ld�. Istek /Home/Privacy'e yap�ld�. <br/>"));

            //        await next.Invoke();   // Sonraki Middleware'i �a��r�yoruz.
            //    });
            //});
            #endregion

            #region MapWhen Middleware
            // Sadece Request'in yap�ld��� path'e g�re de�il ayn� zamanda Request'in �zelli�ine g�rede bizim filtreleme yapmam�z� sa�lar.
            //app.MapWhen(c => c.Request.Method == "POST" && c.Request.Path == "/Home/Privacy/15", builder =>
            //{
            //    builder.Use(async (context, next) =>
            //    {
            //        context.Response.ContentType = "text/html; charset=UTF-8";

            //        await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("MapWhen Middleware'ine Giri� Yap�ld�. Istek /Home/Privacy/15'e yap�ld�. <br/>"));

            //        await next.Invoke();   // Sonraki Middleware'i �a��r�yoruz. 
            //    });
            //});
            #endregion

            #region Custom Middleware
            app.UseHello();
            // Run Middleware'ini buraya koymaktaki maksat Custom Middleware'in GeriD�n�� Verisini G�rebilmek. Mant�ksal a��dan di�er middleware'lerin �al��mas� istenen sonucun g�r�lmesini engelleyebiliyor.
            app.Run(async (context) => { await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("Run Middleware'i �al��t�. <br />")); });
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


        // �rnek Middleware �al��ma Mant���.
        // Context -> ��erisinde Request'i ve Response'u Bar�nd�ran HttpContext objesini Refere Eder.
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
