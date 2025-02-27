﻿using Logistics.Application.Common.Behaviours;
using Logistics.Application.Common.Options;
using Logistics.Application.Common.Services;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logistics.Application.Common;

public static class Registrar
{
    public static IServiceCollection AddApplicationLayer(
        this IServiceCollection services,
        IConfiguration configuration,
        string emailConfigSection = "Email",
        string captchaConfigSection = "Captcha")
    {

        var emailSenderOptions = configuration.GetSection(emailConfigSection).Get<EmailSenderOptions>();
        var googleRecaptchaOptions = configuration.GetSection(captchaConfigSection).Get<GoogleRecaptchaOptions>();

        if (emailSenderOptions != null)
        {
            services.AddSingleton(emailSenderOptions);
            services.AddTransient<IEmailSender, EmailSender>();
        }

        if (googleRecaptchaOptions != null)
        {
            services.AddSingleton(googleRecaptchaOptions);
            services.AddScoped<ICaptchaService, GoogleRecaptchaService>();
        }
        
        // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        return services;
    }
}