using GraphiQl;
using GraphQL.Server;
using GraphQL.Types;
using GraphQlProject.Data;
using GraphQlProject.Interfaces;
using GraphQlProject.Mutation;
using GraphQlProject.Query;
using GraphQlProject.Schema;
using GraphQlProject.Services;
using GraphQlProject.Type;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO.Compression;
using System.Linq;

namespace GraphQlProject
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
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
                options.Providers.Add<BrotliCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "text/plain", "text/css", "application/javascript", "text/html", "application/xml", "text/xml", "application/json", "text/json" });
            });

            services.AddControllers();
            services.AddTransient<IProduct, ProductServices>();
            services.AddTransient<ProductQuery>();
            services.AddTransient<ProductType>();
            services.AddTransient<ProductMutation>();
            services.AddTransient<ISchema, ProductSchema>();

            services.AddGraphQL(options =>
            {
                options.EnableMetrics = false;
            }).AddSystemTextJson();

            services.AddControllers().AddJsonOptions(options => {
                options.JsonSerializerOptions.IgnoreNullValues = true;
            });

            services.AddMemoryCache();
            services.AddDbContext<GraphQLDbContext>(option => option.UseSqlServer(@"Data Source=DESKTOP-JHPQH6N;Initial Catalog=Desenv;Integrated Security=True"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, GraphQLDbContext dbContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseResponseCompression();

            //app.UseGraphiQl("/graphql");
            //app.UseGraphQL<ISchema>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
