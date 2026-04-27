# Phase 1 — Core Booking Flow: Architecture Plan

## Context

The Surfiju Blazor Server app has a working but incomplete booking system. The current `Booking` entity has no status tracking, `CancelBookingCommand` hard-deletes records, there is no payment integration, no concurrency protection on capacity, FluentValidation is not wired to the MediatR pipeline, and organizers have no dashboard to manage bookings. This plan adds a full booking lifecycle with Stripe payments, email notifications, and organizer/user dashboards.

---

## Pre-Flight Fixes (Do First)

These must be done before any Phase 1 feature work.

### Fix 1 — Wire FluentValidation to MediatR Pipeline

**File to create:** `Application/Behaviours/ValidationBehavior.cs`

```csharp
public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (!validators.Any()) return await next();
        var ctx = new ValidationContext<TRequest>(request);
        var failures = validators.SelectMany(v => v.Validate(ctx).Errors).Where(f => f != null).ToList();
        if (failures.Count != 0) throw new ValidationException(failures);
        return await next();
    }
}
```

**File to modify:** `Program.cs` — add after `builder.Services.AddMediatR(...)`:
```csharp
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
```

### Fix 2 — Global Exception Handler

**File to create:** `DevKnowledgeBase.API/Middleware/GlobalExceptionMiddleware.cs`

Map `ValidationException` → 400, `UnauthorizedAccessException` → 401, `KeyNotFoundException` → 404, `DbUpdateConcurrencyException` → 409, everything else → 500. Never expose `ex.Message` for 500s.

**File to modify:** `Program.cs` — add `app.UseMiddleware<GlobalExceptionMiddleware>();` before `app.UseRouting()`.

### Fix 3 — CurrentParticipants Mapping Bug

**File to modify:** `Application/Common/MappingProfile.cs`

Change:
```csharp
.ForMember(dest => dest.CurrentParticipants, opt => opt.MapFrom(src => src.Members.Count))
```
To:
```csharp
.ForMember(dest => dest.CurrentParticipants, opt => opt.MapFrom(src => src.Bookings.Sum(b => b.Participants)))
```

Also ensure `Camp` entity includes `ICollection<Booking> Bookings` navigation property and EF config loads it.

### Fix 4 — Remove Wrong SqlServer Package

**File to modify:** `DevKnowledgeBase.API.csproj` and `DevKnowledgeBase.Infrastructure.csproj` — remove `Microsoft.EntityFrameworkCore.SqlServer`. Keep only `Npgsql.EntityFrameworkCore.PostgreSQL`.

---

## Step 1.1 — Booking State Machine

### New Files

**`Domain/Enums/BookingStatus.cs`**
```csharp
public enum BookingStatus
{
    Pending = 0,      // created, awaiting payment
    Confirmed = 1,    // payment succeeded
    Cancelled = 2,    // cancelled before confirmed OR within policy
    Completed = 3,    // camp end date passed
    Refunded = 4      // cancelled after confirmed, refund issued
}
```

### Modified Files

**`Domain/Entities/Booking.cs`** — Add:
- `BookingStatus Status` property (default `Pending`)
- `string? PaymentIntentId` property
- `string? CancellationReason` property
- `DateTime? CancelledAt` property
- Domain method: `void Confirm(string paymentIntentId)`
- Domain method: `void Cancel(string reason)` — sets `Cancelled` or `Refunded` based on current status
- Domain method: `void Complete()`

**`Infrastructure/Data/DevDatabaseContext.cs`** — Add in `OnModelCreating`:
```csharp
modelBuilder.Entity<Booking>()
    .Property(b => b.Status)
    .HasDefaultValue(BookingStatus.Pending)
    .HasConversion<string>();
```

### New Commands

**`Application/Commands/ConfirmBookingCommand.cs`** + handler:
- Input: `Guid BookingId`, `string PaymentIntentId`
- Handler: Load booking, call `booking.Confirm(paymentIntentId)`, save, publish `BookingConfirmedNotification`

**`Application/Commands/CompleteBookingCommand.cs`** + handler:
- Input: `Guid BookingId`
- Handler: Load booking, call `booking.Complete()`, save, publish `BookingCompletedNotification`

**`Application/Commands/CancelBookingCommand.cs`** — REPLACE hard delete with:
- Load booking, call `booking.Cancel(reason)`, save, publish `BookingCancelledNotification`
- Do NOT delete the record

### Migration

```
dotnet ef migrations add AddBookingStatusAndPayment --project DevKnowledgeBase.Infrastructure --startup-project DevKnowledgeBase.API
```

---

## Step 1.2 — Concurrency-Safe Capacity Management

### Concurrency Token (No Migration Needed)

**`Infrastructure/Data/DevDatabaseContext.cs`** — Add in `OnModelCreating` for `Camp`:
```csharp
modelBuilder.Entity<Camp>().UseXminAsConcurrencyToken();
```

PostgreSQL `xmin` is a system column — no schema migration required.

### Updated CreateBookingCommandHandler

**`Application/Commands/CreateBookingCommandHandler.cs`** — Wrap booking creation in retry loop for `DbUpdateConcurrencyException`:

```csharp
// Pseudocode
var camp = await _db.Camps.FindAsync(request.CampId, ct); // EF tracks xmin
var booked = await _db.Bookings
    .Where(b => b.CampId == request.CampId && b.Status != BookingStatus.Cancelled)
    .SumAsync(b => b.Participants, ct);
if (booked + request.Participants > camp.MaxParticipants)
    throw new InvalidOperationException("Not enough spots available.");
_db.Bookings.Add(newBooking);
// SaveChangesAsync will throw DbUpdateConcurrencyException if xmin changed
await _db.SaveChangesAsync(ct);
```

The `GlobalExceptionMiddleware` maps `DbUpdateConcurrencyException` → 409 so the client can retry.

### New Query

**`Application/Queries/GetCampAvailabilityQuery.cs`** + handler:
- Input: `Guid CampId`
- Returns: `CampAvailabilityDto { int MaxParticipants, int BookedParticipants, int AvailableSpots, bool IsAvailable }`
- Query: `SUM(Participants) WHERE CampId = x AND Status != Cancelled`

**`Controllers/CampsController.cs`** — Add endpoint:
```
GET /api/camps/{id}/availability
```

---

## Step 1.3 — Stripe Payment Integration

### Packages

Add to `DevKnowledgeBase.Infrastructure.csproj`:
```
<PackageReference Include="Stripe.net" Version="46.*" />
```

Add to `DevKnowledgeBase.UI.csproj`:
```
<PackageReference Include="Stripe.net" Version="46.*" />
```

### Configuration

`appsettings.json`:
```json
"Stripe": {
  "PublishableKey": "",
  "SecretKey": "",
  "WebhookSecret": ""
}
```

### New Interface + Implementation

**`Application/Interfaces/IPaymentService.cs`**:
```csharp
public interface IPaymentService
{
    Task<string> CreatePaymentIntentAsync(decimal amount, string currency, Guid bookingId);
    Task<bool> RefundPaymentAsync(string paymentIntentId, decimal amount);
}
```

**`Infrastructure/Services/StripePaymentService.cs`** — Implements `IPaymentService` using `Stripe.PaymentIntentService`. Sets metadata `bookingId` on the intent.

**`Program.cs`** — Add:
```csharp
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];
builder.Services.AddScoped<IPaymentService, StripePaymentService>();
```

### Payment Flow (Blazor UI)

1. User clicks "Book Now" in `CampDetails.razor` sidebar
2. POST `/api/bookings` → creates booking with `Status = Pending`, returns `BookingId` + calls `IPaymentService.CreatePaymentIntentAsync` → returns `ClientSecret`
3. Blazor component calls JS interop: `Stripe.js confirmCardPayment(clientSecret)`
4. On JS success: POST `/api/bookings/{id}/confirm` → `ConfirmBookingCommand`
5. On JS failure: display error, booking stays `Pending` (cleanup via background job or webhook)

**New file:** `wwwroot/stripe-interop.js`
```js
window.stripeInterop = {
    confirmPayment: async (clientSecret, cardElement) => {
        const stripe = Stripe(window.stripePublishableKey);
        return await stripe.confirmCardPayment(clientSecret, { payment_method: { card: cardElement } });
    }
};
```

**New Blazor component:** `Components/Pages/Checkout.razor` (`/checkout/{bookingId}`)
- Uses `IJSRuntime` to call `stripeInterop.confirmPayment`
- Shows Stripe card element via JS interop
- On success: navigates to `/my-bookings` with success message

### Webhook Controller

**`Controllers/StripeWebhookController.cs`**:
```csharp
[ApiController, Route("api/stripe/webhook")]
public class StripeWebhookController(IMediator mediator, IConfiguration config) : ControllerBase
{
    [HttpPost, AllowAnonymous]
    public async Task<IActionResult> Handle()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var stripeEvent = EventUtility.ConstructEvent(json,
            Request.Headers["Stripe-Signature"], config["Stripe:WebhookSecret"]);

        if (stripeEvent.Type == Events.PaymentIntentSucceeded)
        {
            var intent = (PaymentIntent)stripeEvent.Data.Object;
            var bookingId = Guid.Parse(intent.Metadata["bookingId"]);
            await mediator.Send(new ConfirmBookingCommand(bookingId, intent.Id));
        }
        return Ok();
    }
}
```

---

## Step 1.4 — Email Notifications

### New Notification Records

**`Application/Notifications/BookingConfirmedNotification.cs`**:
```csharp
public record BookingConfirmedNotification(Guid BookingId, string UserEmail, string UserName, string CampName, DateTime StartDate, int Participants, decimal TotalPrice) : INotification;
```

Similarly: `BookingCancelledNotification`, `BookingCompletedNotification`, `BookingCreatedNotification`.

### Handlers

**`Application/Notifications/Handlers/BookingConfirmedEmailHandler.cs`**:
```csharp
public class BookingConfirmedEmailHandler(IEmailService emailService) : INotificationHandler<BookingConfirmedNotification>
{
    public async Task Handle(BookingConfirmedNotification n, CancellationToken ct)
    {
        var body = $"""
            <h2>Booking Confirmed!</h2>
            <p>Hi {n.UserName},</p>
            <p>Your booking for <strong>{n.CampName}</strong> starting {n.StartDate:MMMM dd, yyyy} is confirmed.</p>
            <p>Participants: {n.Participants} | Total: €{n.TotalPrice:F2}</p>
            """;
        await emailService.SendEmailAsync(n.UserEmail, "Your Surfiju Booking is Confirmed", body);
    }
}
```

Similarly for: `BookingCancelledEmailHandler`, `BookingCompletedEmailHandler`, `BookingCreatedEmailHandler`.

### Publish Notifications from Handlers

In each command handler (Confirm, Cancel, Complete, Create), publish via:
```csharp
await _mediator.Publish(new BookingConfirmedNotification(...), ct);
```

MediatR resolves `INotificationHandler<>` via DI automatically since `AddMediatR` scans the assembly.

---

## Step 1.5 — Organizer Dashboard

### New Queries

**`Application/Queries/GetOrganizerBookingsQuery.cs`** + handler:
- Input: `Guid? CampId` (optional filter), `BookingStatus? Status`
- Returns: `List<OrganizerBookingDto>`
- `OrganizerBookingDto`: `BookingId`, `CampName`, `UserName`, `UserEmail`, `Participants`, `TotalPrice`, `Status`, `BookingDate`, `PaymentIntentId`
- Handler filters by `OrganizerId` via camp ownership

**`Application/Queries/GetCampBookingSummaryQuery.cs`** + handler:
- Input: `Guid? CampId`
- Returns: `BookingSummaryDto { TotalRevenue, TotalBookings, ConfirmedBookings, PendingBookings, CancelledBookings }`

### New API Endpoints

**`Controllers/BookingsController.cs`** — Add:
```
GET /api/organizer/bookings?campId={}&status={}   → GetOrganizerBookingsQuery
GET /api/organizer/bookings/summary               → GetCampBookingSummaryQuery
POST /api/bookings/{id}/confirm                   → ConfirmBookingCommand (Admin/Organizer)
POST /api/bookings/{id}/cancel                    → CancelBookingCommand (Admin/Organizer/Owner)
```

### New UI Page

**`Components/Pages/OrganizerDashboard.razor`** (`/organizer/bookings`):
- `[Authorize(Roles = "Admin, Organizer")]`
- Top: 4 stat cards (Total Revenue, Total Bookings, Confirmed, Pending) via `GetCampBookingSummaryQuery`
- CampId filter dropdown + Status filter chips
- `MudTable` with columns: Camp, User, Dates, Participants, Total, Status chip, Actions
- Confirm button (green) for Pending bookings
- Cancel button (red) with `ConfirmDialog`
- Snackbar feedback

Add link to organizer dashboard in `TopNavMenu.razor` for users with Organizer/Admin role using `<AuthorizeView Roles="Admin, Organizer">`.

---

## Step 1.6 — User Booking History

### Updated DTO

**`Application/DTOs/BookingDto.cs`** — Add:
- `BookingStatus Status`
- `string StatusLabel` (computed)
- `string? PaymentIntentId`
- `bool CanCancel` — true if `Status == Pending || (Status == Confirmed && StartDate > DateTime.UtcNow.AddHours(48))`

### Updated Query

**`Application/Queries/GetUserBookingsQuery.cs`** + handler — Add:
- Optional `BookingStatus? Status` filter parameter
- Return full `BookingDto` including new fields

### Updated UI

**`Components/Pages/User/MyBookings.razor`** — Add:
- Status filter chips at top (All / Pending / Confirmed / Cancelled / Completed)
- Status column with color-coded `MudChip` (Pending=Warning, Confirmed=Success, Cancelled=Error, Completed=Info)
- Cancel button per row (visible only if `dto.CanCancel`)
- Cancel calls `DELETE /api/bookings/{id}` (which now soft-cancels)
- Snackbar feedback on cancel

---

## Implementation Order

1. Pre-flight fixes (ValidationBehavior, GlobalExceptionMiddleware, CurrentParticipants mapping, remove SqlServer package)
2. Step 1.1 — BookingStatus enum + entity changes + new commands + migration
3. Step 1.2 — xmin concurrency token + fix CreateBooking handler + GetCampAvailabilityQuery
4. Step 1.4 — Email notifications (no external dependency, easy to add)
5. Step 1.3 — Stripe integration (requires Stripe account config; add package, service, webhook, Blazor checkout)
6. Step 1.5 — Organizer dashboard (queries + API endpoints + UI)
7. Step 1.6 — User booking history updates (DTO + query filter + UI)

---

## Critical Files to Modify

| File | Change |
|------|--------|
| `Domain/Entities/Booking.cs` | Add Status, PaymentIntentId, domain methods |
| `Domain/Entities/Camp.cs` | Add Bookings navigation, xmin token in EF config |
| `Application/Common/MappingProfile.cs` | Fix CurrentParticipants calculation |
| `Application/Commands/CancelBookingCommandHandler.cs` | Soft cancel instead of delete |
| `Application/Commands/CreateBookingCommandHandler.cs` | Add concurrency retry, Status = Pending |
| `Infrastructure/Data/DevDatabaseContext.cs` | xmin token for Camp, Booking status config |
| `Controllers/BookingsController.cs` | Add organizer endpoints, confirm endpoint |
| `Components/Pages/User/MyBookings.razor` | Status filter + cancel with policy check |
| `Program.cs` | ValidationBehavior, GlobalExceptionMiddleware, Stripe config, IPaymentService |

## New Files to Create

| File | Purpose |
|------|---------|
| `Domain/Enums/BookingStatus.cs` | Enum definition |
| `Application/Behaviours/ValidationBehavior.cs` | MediatR pipeline validation |
| `API/Middleware/GlobalExceptionMiddleware.cs` | Centralized error handling |
| `Application/Interfaces/IPaymentService.cs` | Payment abstraction |
| `Infrastructure/Services/StripePaymentService.cs` | Stripe implementation |
| `Controllers/StripeWebhookController.cs` | Stripe webhook handler |
| `Application/Commands/ConfirmBookingCommand.cs` + handler | Confirm after payment |
| `Application/Commands/CompleteBookingCommand.cs` + handler | Mark completed |
| `Application/Queries/GetCampAvailabilityQuery.cs` + handler | Concurrency-safe availability |
| `Application/Queries/GetOrganizerBookingsQuery.cs` + handler | Organizer booking list |
| `Application/Queries/GetCampBookingSummaryQuery.cs` + handler | Revenue/stats summary |
| `Application/Notifications/Booking*Notification.cs` (×4) | MediatR notification records |
| `Application/Notifications/Handlers/*EmailHandler.cs` (×4) | Email dispatch handlers |
| `Components/Pages/Checkout.razor` | Stripe card payment UI |
| `Components/Pages/OrganizerDashboard.razor` | Organizer booking management |
| `wwwroot/stripe-interop.js` | JS interop for Stripe.js |

---

## Verification

1. **Unit**: Create unit tests for `Booking` domain methods (state transitions — invalid transitions must throw)
2. **Integration**: Test `CreateBookingCommandHandler` with two concurrent requests exceeding capacity — one must get 409
3. **Stripe**: Use Stripe CLI `stripe listen --forward-to localhost:PORT/api/stripe/webhook` to test webhook locally
4. **Email**: Use MailHog or similar local SMTP server to verify email templates render correctly
5. **UI flow**: Book a camp end-to-end: browse → details → checkout → confirm → appears in MyBookings with Confirmed status
6. **Organizer**: Log in as Organizer → see booking in dashboard → confirm/cancel → verify status update in user's MyBookings
7. **Cancel policy**: Attempt to cancel a confirmed booking within 48h of start date — verify button is disabled
