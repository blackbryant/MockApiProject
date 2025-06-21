using System;
using Microsoft.AspNetCore.HttpOverrides;

namespace mockAPI.Middleware;

public static class ForwardedHeadersMiddlewareExtensions
{

    public static IApplicationBuilder UseForwardedHeaders(this IApplicationBuilder builder)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        var options = new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        };

        options.KnownNetworks.Clear();
        options.KnownProxies.Clear();

        return builder.UseForwardedHeaders(options);
    }

    public static IServiceCollection AddForwardedHeaders(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        //清除內建的受信任網段（例如：127.0.0.1/8）。
        //清除預設的受信任 Proxy 清單。
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });

        return services;
    }






}
