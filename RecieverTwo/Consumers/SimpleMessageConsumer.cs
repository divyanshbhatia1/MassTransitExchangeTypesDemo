using MassTransit;
using Shared;
using Shared.Response;
using System;
using System.Threading.Tasks;

namespace RecieverTwo.Consumers
{
	public class SimpleMessageConsumer : IConsumer<SimpleMessage>
	{
		public async Task Consume(ConsumeContext<SimpleMessage> context)
		{
			Console.WriteLine(context.Message.Message);
			await context.RespondAsync(new SimpleMessageResponse
			{
				Response = "Success from Reciever TWO"
			});
		}
	}
}
