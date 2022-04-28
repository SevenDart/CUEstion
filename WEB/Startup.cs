using System.Linq;
using BLL.Implementations;
using BLL.Interfaces;
using BLL.ModelsDTO;
using DAL.EF;
using DAL.Entities;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace WEB
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		private IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();
			
			services.AddDbContext<ApplicationContext>(
				options =>
					options.UseSqlServer(@"Server=DESKTOP-KH4PKN3;Database=CUEstionDB;Trusted_Connection=True;TrustServerCertificate=true;")
					);
			
			services.AddScoped<IMarkManagerService, MarkManagerService>();
			services.AddScoped<IUserManagerService, UserManagerService>();
			services.AddScoped<ITagManagerService, TagManagerService>();
			services.AddScoped<ICommentManagerService, CommentManagerService>();
			services.AddScoped<IAnswerManagerService, AnswerManagerService>();
			services.AddScoped<IQuestionManagerService, QuestionManagerService>();
			services.AddScoped<IWorkspaceRoleManagerService, WorkspaceRoleManagerService>();
			services.AddScoped<IWorkspaceManagerService, WorkspaceManagerService>();

			TypeAdapterConfig<Question, QuestionDto>
				.NewConfig()
				.Map(dest => dest.Tags, src => src.Tags.Select(t => t.Name));

			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
			{
				options.RequireHttpsMetadata = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = AuthOptions.ISSUER,
                    ValidateAudience = true,
                    ValidAudience = AuthOptions.AUDIENCE,
                    ValidateLifetime = true,
                    IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                    ValidateIssuerSigningKey = true
                };
			});

			services.AddCors(
				c =>
				c.AddPolicy(
					"defaultPolicy",
					options => 
									options
									.AllowAnyHeader()
									.AllowAnyMethod()
									.AllowAnyOrigin()
					)
				);
			services.AddSwaggerGen();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI();
			}
			else
			{
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseCors("defaultPolicy");

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
