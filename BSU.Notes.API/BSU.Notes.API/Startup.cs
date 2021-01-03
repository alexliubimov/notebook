using BSU.Notes.API.Validators;
using BSU.Notes.Data.DTOs.Note;
using BSU.Notes.Data.DTOs.User;
using BSU.Notes.DataAccess.Dapper;
using BSU.Notes.DataAccess.Interfaces;
using BSU.Notes.Services;
using BSU.Notes.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSU.Notes.API
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
            services.AddMvc(option => option.EnableEndpointRouting = false);
            services.AddControllers();

            services.AddSwaggerDocument(config => 
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "Notes API";
                    document.Info.Contact = new NSwag.OpenApiContact
                    {
                        Name = "Alex Liubimov",
                        Email = "sasha.liubimov@gmail.com",
                        Url = "https://twitter.com/alexliubimov"
                    };
                };
            });

            services.AddScoped<IValidator<CreateUserDto>, CreateUserValidator>();
            services.AddScoped<IValidator<UpdateUserDto>, UpdateUserValidator>();
            services.AddScoped<IValidator<CreateNoteDto>, CreateNoteValidator>();
            services.AddScoped<IValidator<UpdateNoteDto>, UpdateNoteValidator>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<INoteService, NoteService>();

            var connectionString = Configuration.GetConnectionString("ConnectionString");
            services.AddScoped<IUserProvider, UserProvider>(sp => new UserProvider(connectionString));
            services.AddScoped<INoteProvider, NoteProvider>(sp => new NoteProvider(connectionString));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
