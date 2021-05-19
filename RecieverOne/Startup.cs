using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MassTransit;
using RecieverOne.Consumers;
using Shared;
using System;
using MassTransit.Definition;
using RabbitMQ.Client;

namespace RecieverOne
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
			//services.AddMassTransit(config =>
			//{
			//	//config.AddConsumer<SimpleMessageConsumer>();

			//	config.UsingRabbitMq((context, cfg) =>
			//	{
			//		cfg.ReceiveEndpoint("simple-message-queue", c =>
			//		{
			//			c.ConfigureConsumeTopology = false;
			//			c.PrefetchCount = 20;
			//			c.Bind("simple-message-exchange", b =>
			//			{
			//				b.ExchangeType = ExchangeType.Direct;
			//				b.RoutingKey = "One";
			//			});

			//			//c.ConfigureConsumer<SimpleMessageConsumer>(context);
			//		});
			//	});
			//});

			services.AddMassTransit(config =>
			{
				config.AddConsumer<SimpleMessageConsumer>();

				config.UsingRabbitMq((context, cfg) =>
				{
					cfg.ReceiveEndpoint("simple-message-first-queue", c =>
					{
						c.ConfigureConsumer<SimpleMessageConsumer>(context);
					});
				});
			});

			services.AddMassTransitHostedService();

			services.AddControllers();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

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
