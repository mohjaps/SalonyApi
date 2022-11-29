
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Salony
{
    public class SwaggerConfigration : IConfigureOptions<SwaggerGenOptions>
    {
        public void Configure(SwaggerGenOptions c)
        {
            //c.SwaggerDoc("CustomerAPI", new OpenApiInfo { Title = "Customer API", Version = "v1" });
            c.SwaggerDoc("WebApi", new OpenApiInfo { Title = "Site API", Version = "v1" });
            c.SwaggerDoc("MobileApi", new OpenApiInfo { Title = "Mobile API", Version = "v1" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme."
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}

                    }
                });

            // Set the comments path for the Swagger JSON and UI from xml file.
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        }
    }

    public class SwaggerUIConfiguration : IConfigureOptions<SwaggerUIOptions>
    {
        public void Configure(SwaggerUIOptions options)
        {
            //options.DocExpansion(DocExpansion.None);
            //options.SwaggerEndpoint("/swagger/CustomerAPI/swagger.json", "Customer API");
            //options.SwaggerEndpoint("/swagger/DelegetAPI/swagger.json", "Deleget API V1");
            options.SwaggerEndpoint("/swagger/MobileApi/swagger.json", "Mobile API V1");
            options.SwaggerEndpoint("/swagger/WebApi/swagger.json", "Web API V1");

        }
    }
}
