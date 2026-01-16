using System.Text;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NotebookTherapy.Application.Features.Orders.Commands;
using NotebookTherapy.Infrastructure.Services;
using Stripe;

namespace NotebookTherapy.API.Controllers;

[ApiController]
[Route("api/webhooks/stripe")]
public class StripeWebhookController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly StripeSettings _settings;

    public StripeWebhookController(IMediator mediator, IOptions<StripeSettings> settings)
    {
        _mediator = mediator;
        _settings = settings.Value;
    }

    [HttpPost]
    public async Task<IActionResult> Handle()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var signatureHeader = Request.Headers["Stripe-Signature"];

        Event stripeEvent;
        try
        {
            stripeEvent = EventUtility.ConstructEvent(json, signatureHeader, _settings.WebhookSecret);
        }
        catch (StripeException)
        {
            return BadRequest();
        }

        switch (stripeEvent.Type)
        {
            case Events.PaymentIntentSucceeded:
                {
                    var intent = stripeEvent.Data.Object as PaymentIntent;
                    if (intent != null)
                    {
                        await _mediator.Send(new UpdatePaymentStatusCommand(intent.Id, "Succeeded", "Stripe"));
                    }
                    break;
                }
            case Events.PaymentIntentPaymentFailed:
                {
                    var intent = stripeEvent.Data.Object as PaymentIntent;
                    if (intent != null)
                    {
                        await _mediator.Send(new UpdatePaymentStatusCommand(intent.Id, "Failed", "Stripe"));
                    }
                    break;
                }
            case Events.PaymentIntentCanceled:
                {
                    var intent = stripeEvent.Data.Object as PaymentIntent;
                    if (intent != null)
                    {
                        await _mediator.Send(new UpdatePaymentStatusCommand(intent.Id, "Cancelled", "Stripe"));
                    }
                    break;
                }
            case Events.ChargeRefunded:
                {
                    var charge = stripeEvent.Data.Object as Charge;
                    if (charge?.PaymentIntentId != null)
                    {
                        await _mediator.Send(new UpdatePaymentStatusCommand(charge.PaymentIntentId, "Refunded", "Stripe"));
                    }
                    break;
                }
            default:
                if (stripeEvent.Type.StartsWith("payment_intent."))
                {
                    var intent = stripeEvent.Data.Object as PaymentIntent;
                    if (intent != null)
                    {
                        await _mediator.Send(new UpdatePaymentStatusCommand(intent.Id, intent.Status, "Stripe"));
                    }
                }
                break;
        }

        return Ok();
    }
}
