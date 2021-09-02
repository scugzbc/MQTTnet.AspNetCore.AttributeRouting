// Copyright (c) Atlas Lift Tech Inc. All rights reserved.

using Microsoft.Extensions.DependencyInjection;
using MQTTnet.AspNetCore.AttributeRouting.Routing;
using MQTTnet.Server;
using System.Reflection;
using System.Runtime.CompilerServices;

  
namespace MQTTnet.AspNetCore.AttributeRouting
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMqttControllers(this IServiceCollection services)
        {
            services.AddSingleton(c =>
            {
                // future enhancement: scan for other AppParts, if needed

                var assemblies = new Assembly[] { Assembly.GetEntryAssembly() };

                return MqttRouteTableFactory.Create(assemblies);
            });

            services.AddSingleton<ITypeActivatorCache>(new TypeActivatorCache());
            services.AddSingleton<MqttRouter>();

            return services;
        }

        public static AspNetMqttServerOptionsBuilder WithAttributeRouting(this AspNetMqttServerOptionsBuilder options, bool allowUnmatchedRoutes = false)
        {
            var router = options.ServiceProvider.GetRequiredService<MqttRouter>();
            var interceptor = new MqttServerApplicationMessageInterceptorDelegate(context => router.OnIncomingApplicationMessage(options, context, allowUnmatchedRoutes));

            options.WithApplicationMessageInterceptor(interceptor);

            return options;
        }
    }
}