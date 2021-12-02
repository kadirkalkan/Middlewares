using Microsoft.AspNetCore.Builder;
using MiddleWare.Middlewares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiddleWare.Extensions
{
    public static class Extension
    {
        public static IApplicationBuilder UseHello(this IApplicationBuilder app)
        {
            return app.UseMiddleware<HelloMiddleware>();
        }

    }
}
