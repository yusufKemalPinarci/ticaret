using System.Text;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NotebookTherapy.Application.Features.Orders.Commands;
using Stripe;
using NotebookTherapy.Infrastructure.Services;

namespace NotebookTherapy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly StripeSettings _settings;

    public PaymentsController(IMediator mediator, IOptions<StripeSettings> settings)
    {
        _mediator = mediator;
        _settings = settings.Value;
    }

    [AllowAnonymous]
    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body, Encoding.UTF8).ReadToEndAsync();
        var signatureHeader = Request.Headers["Stripe-Signature"].FirstOrDefault();
        if (string.IsNullOrEmpty(signatureHeader)) return BadRequest();

        Event stripeEvent;
        try
        {
            stripeEvent = EventUtility.ConstructEvent(json, signatureHeader, _settings.WebhookSecret);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

        if (stripeEvent.Data?.Object is PaymentIntent intent)
        {
            var status = stripeEvent.Type switch
            {
                Events.PaymentIntentSucceeded => "Succeeded",
                Events.PaymentIntentPaymentFailed => "Failed",
                Events.PaymentIntentRequiresAction => "RequiresAction",
                Events.PaymentIntentProcessing => "Processing",
                _ => null
            };

            if (status != null)
            {
                await _mediator.Send(new UpdatePaymentStatusCommand(intent.Id, status, "Stripe"));
            }
        }

        return Ok();
    }
}