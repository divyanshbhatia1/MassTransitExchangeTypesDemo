using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.Response;
using System.Threading.Tasks;

namespace Sender.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SendMessageController : ControllerBase
	{
		private readonly IPublishEndpoint _publishEndpoint;
		private readonly IRequestClient<SimpleMessage> _simpleMessageRequestClient;
		private readonly ISendEndpointProvider _sendEndpointProvider;

		public SendMessageController(IPublishEndpoint publishEndpoint, 
			IRequestClient<SimpleMessage> simpleMessageRequestClient, 
			ISendEndpointProvider sendEndpointProvider)
		{
			_publishEndpoint = publishEndpoint;
			this._simpleMessageRequestClient = simpleMessageRequestClient;
			this._sendEndpointProvider = sendEndpointProvider;
		}

		[HttpPost("PublishMessage")]
		public async Task<IActionResult> PublishMessage(string message)
		{
			var simpleMessage = new SimpleMessage
			{ Message = message };

			await _publishEndpoint.Publish(simpleMessage);

			return Ok();
		}

		[HttpPost("PublishMessageToONE")]
		public async Task<IActionResult> PublishMessageToONE(string message)
		{
			var endpoint = await _sendEndpointProvider.GetSendEndpoint(new System.Uri("exchange:simple-message-exchange"));

			var simpleMessage = new SimpleMessage
			{ Message = message };

			await endpoint.Send(simpleMessage, x => x.SetRoutingKey("One"));

			return Ok();
		}

		[HttpPost("PublishMessageToTWO")]
		public async Task<IActionResult> PublishMessageToTWO(string message)
		{
			var simpleMessage = new SimpleMessage
			{ Message = message };

			await _publishEndpoint.Publish(simpleMessage);

			return Ok();
		}

		[HttpPost("PostMessage")]
		public async Task<IActionResult> PostMessage(string message)
		{
			var endpoint = await _sendEndpointProvider.GetSendEndpoint(new System.Uri("queue:simple-message-queue"));

			var simpleMessage = new SimpleMessage
			{ Message = message };

			await endpoint.Send(simpleMessage);

			return Ok();
		}

		[HttpPost("PostMessageToQueue")]
		public async Task<IActionResult> PostMessageToQueue(string queue, string message)
		{
			var endpoint = await _sendEndpointProvider.GetSendEndpoint(new System.Uri($"queue:{queue}")); //Will create queue if it doesn't exist

			var simpleMessage = new SimpleMessage
			{ Message = message };

			await endpoint.Send(simpleMessage);

			return Ok();
		}


		[HttpPost("PostMessageAndGetResponse")]
		public async Task<IActionResult> PostMessageAndGetResponse(string message)
		{
			var simpleMessage = new SimpleMessage
			{ Message = message };

			Response<SimpleMessageResponse> response = await _simpleMessageRequestClient.GetResponse<SimpleMessageResponse>(simpleMessage);

			return Ok(response.Message);
		}
	}
}
