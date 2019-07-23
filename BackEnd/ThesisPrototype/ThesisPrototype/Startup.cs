using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ThesisPrototype.Models;
using ThesisPrototype.Services.Implementations;
using ThesisPrototype.Services.Interfaces;

namespace ThesisPrototype
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
            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            services.Configure<PublicationDatabaseSettings>(
                Configuration.GetSection(nameof(PublicationDatabaseSettings)));

            services.AddSingleton<IPublicationDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<PublicationDatabaseSettings>>().Value);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            
            services.AddTransient(typeof(ICompressionService), typeof(CompressionService));
            services.AddTransient(typeof(IPublicationService), typeof(PublicationService));

            services.AddTransient(typeof (IGitHubService), typeof(GitHubService));
            services.AddTransient(typeof (ISubversionService), typeof(SubversionService));

            services.AddTransient(typeof (IB2ShareService), typeof(B2ShareService));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseCors("MyPolicy");

            app.UseMvc();
        }
    }
}
