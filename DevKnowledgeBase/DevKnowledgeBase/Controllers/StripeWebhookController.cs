using DevKnowledgeBase.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace DevKnowledgeBase.API.Controllers;

[ApiController]
[Route("api/stripe/webhook")]
public class StripeWebhookController(IMediator mediator, IConfiguration config) : ControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> Handle()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var stripeEvent = EventUtility.ConstructEvent(
            json,
            Request.Headers["Stripe-Signature"],
            config["Stripe:WebhookSecret"]);

        if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded)
        {
            var intent = (PaymentIntent)stripeEvent.Data.Object;
            var bookingId = Guid.Parse(intent.Metadata["bookingId"]);
            await mediator.Send(new ConfirmBookingCommand(bookingId, intent.Id));
        }

        return Ok();
    }
}
