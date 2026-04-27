# 🏄 Surf Camp Platform — Production Roadmap

## Stack Reference
- **Backend:** .NET 8, ASP.NET Core Web API, Clean Architecture + CQRS (MediatR)
- **Frontend:** Blazor Server, MudBlazor 8.5.1
- **Database:** PostgreSQL + EF Core 9
- **Auth:** ASP.NET Core Identity + JWT, Role-based (Admin / Organizer / User)
- **Validation:** FluentValidation 11
- **Mapping:** AutoMapper 16
- **Maps:** BlazorGoogleMaps (Places API)
- **Photo storage:** API-served from disk (to be migrated)

---

## How to Use This Plan

- Work **one step at a time** — each step is a self-contained Claude session goal
- Before starting a step, **paste relevant files** into Claude (entity, DbContext, existing commands)
- Mark steps with `[x]` as you complete them
- Each step follows the pattern: **Domain → Application → API → UI**

---

## Phase 1 — Core Booking Flow
> **Goal:** A user can discover a camp, book it, and pay. An organizer can manage incoming bookings. Nothing else matters until this works end-to-end.

---

### Step 1.1 — Booking State Machine
- [ ] Define `BookingStatus` enum: `Pending`, `Confirmed`, `Cancelled`, `Completed`, `Refunded`
- [ ] Add state transition rules in domain (e.g. only `Pending` can move to `Confirmed`)
- [ ] Create `Booking` entity with all required fields (UserId, CampId, Spots, TotalPrice, Status, CreatedAt)
- [ ] Add `BookingConfiguration` for EF Core
- [ ] Migration + apply to DB

**Commands to build:**
- `CreateBookingCommand` — user initiates a booking
- `ConfirmBookingCommand` — organizer confirms
- `CancelBookingCommand` — user or organizer cancels
- `CompleteBookingCommand` — post-trip completion (admin/system)

**Validation rules:**
- Camp must be published and not full
- User cannot double-book the same camp
- Cancellation rules (e.g. no cancel within 24h)

---

### Step 1.2 — Capacity Management
- [ ] Add `Capacity` and `BookedSpots` to Camp entity
- [ ] Enforce capacity on booking creation (concurrency-safe — use EF row version or pessimistic lock)
- [ ] Query: `GetCampAvailabilityQuery` — returns remaining spots
- [ ] Display availability on camp detail page in UI

---

### Step 1.3 — Stripe Payment Integration
- [ ] Install `Stripe.net` NuGet package
- [ ] Add Stripe config (API keys via `appsettings` / env vars — never hardcode)
- [ ] Create `PaymentService` in Infrastructure layer
- [ ] Implement two flows:
  - **Deposit** — partial upfront payment
  - **Full payment** — pay total at booking
- [ ] Create `CreatePaymentIntentCommand` — returns `clientSecret` to UI
- [ ] Handle Stripe webhook endpoint (`/api/webhooks/stripe`)
  - Events to handle: `payment_intent.succeeded`, `payment_intent.payment_failed`
  - Update booking status based on payment outcome
- [ ] Store `StripePaymentIntentId` on Booking entity
- [ ] UI: integrate Stripe.js or Stripe Elements in Blazor (JSInterop)

---

### Step 1.4 — Email Notifications
> Reuse existing email infrastructure from auth flow

- [ ] Create email templates for:
  - Booking received (to user)
  - Booking confirmed (to user)
  - Booking cancelled (to user + organizer)
  - New booking alert (to organizer)
  - Payment received (to user)
- [ ] Trigger emails from MediatR pipeline via domain events or post-command handlers
- [ ] Consider using `INotificationHandler<>` for decoupled email dispatch

---

### Step 1.5 — Organizer Dashboard
- [ ] `GetOrganizerBookingsQuery` — paginated, filterable by status/camp
- [ ] `GetCampBookingSummaryQuery` — total revenue, spots filled, pending count
- [ ] UI: Organizer dashboard page
  - Booking list with status badges
  - Confirm / Cancel actions per booking
  - Revenue summary card per camp
  - Camp capacity progress bar

---

### Step 1.6 — User Booking History
- [ ] `GetMyBookingsQuery` — returns user's bookings with camp details
- [ ] UI: "My Trips" page
  - Active bookings
  - Past trips
  - Cancel option (within policy)
  - Payment status indicator

---

## Phase 2 — Platform Features
> **Goal:** Build the features that differentiate this from a generic booking app.

---

### Step 2.1 — Surf Spot Database
- [ ] Create `SurfSpot` entity: Name, Location (lat/lng), BreakType (Beach/Reef/Point), SkillLevel, CrowdLevel, Description, Photos
- [ ] Seed with initial spots or allow Admin to add/edit spots
- [ ] `GetSurfSpotsQuery` — filterable by region, skill level, break type
- [ ] UI: Surf Spot browser page with map (BlazorGoogleMaps)
  - Spot cards with key info
  - Clickable map markers
  - Filter sidebar

---

### Step 2.2 — Link Camps to Surf Spots
- [ ] Add `CampSurfSpots` join table (many-to-many)
- [ ] Organizer can tag their camp with surf spots during creation/edit
- [ ] Display tagged spots on camp detail page with map pins

---

### Step 2.3 — Trip Itinerary Builder
- [ ] Create `Trip` entity (owned by User): Name, Destination, StartDate, EndDate, Notes
- [ ] Create `TripStop` entity: Day, Type (SurfSpot/Accommodation/Transport/Activity), ReferenceId, Notes, OrderIndex
- [ ] Commands: `CreateTripCommand`, `AddTripStopCommand`, `ReorderTripStopsCommand`, `DeleteTripCommand`
- [ ] UI: Itinerary builder page
  - Day-by-day drag-and-drop timeline (MudBlazor list or custom)
  - Add stops from surf spot database, booked camps, or freeform
  - Print/export view (PDF or share link)

---

### Step 2.4 — Group Booking & Cost Splitting
- [ ] Add `GroupBooking` concept — one booking with multiple participants
- [ ] Invite flow: organizer creates group booking → generates invite link → friends join
- [ ] `GroupBookingMember` entity: UserId, Status (Invited/Joined/Paid), AmountOwed, PaidAt
- [ ] Cost split calculation: equal split or custom per member
- [ ] Each member pays their share independently via Stripe
- [ ] UI:
  - Group booking creation with invite link generation
  - Member payment status tracker
  - "Pay my share" button per member

---

### Step 2.5 — Guide & Lesson Booking
- [ ] Create `Guide` entity: Name, Bio, Speciality, HourlyRate, Photos, LinkedUserId (optional)
- [ ] Create `Lesson` entity: GuideId, CampId (optional), Type (Private/Group), Duration, Price, MaxParticipants
- [ ] Allow Organizers to attach guides/lessons to their camps
- [ ] Allow Users to book standalone lessons (independent of a camp)
- [ ] Commands: `BookLessonCommand`, `CancelLessonBookingCommand`
- [ ] UI: Guide profiles page, lesson booking flow inline on camp detail

---

## Phase 3 — Social & Coordination
> **Goal:** Make the platform sticky. Users should want to return and interact.

---

### Step 3.1 — Real-time Group Chat (SignalR)
- [ ] Create `ChatHub` in API using SignalR
- [ ] `ChatMessage` entity: TripId or BookingId, SenderId, Content, SentAt
- [ ] Message persistence to PostgreSQL
- [ ] Auth: only trip/booking members can join the hub group
- [ ] UI: Chat panel component (sidebar or page)
  - Message list with sender avatars
  - Real-time updates
  - Unread badge indicator

---

### Step 3.2 — Reviews & Ratings
- [ ] `Review` entity: CampId or GuideId, UserId, Rating (1–5), Comment, CreatedAt
- [ ] Only users with a `Completed` booking can leave a review
- [ ] `GetCampReviewsQuery`, `SubmitReviewCommand`
- [ ] Aggregate rating stored (or calculated) on Camp entity
- [ ] UI:
  - Star rating + comment form on completed booking
  - Review list on camp detail page
  - Average rating display on camp cards

---

### Step 3.3 — Notifications Center
- [ ] In-app notification system (not just email)
- [ ] `Notification` entity: UserId, Type, Message, IsRead, CreatedAt, Link
- [ ] Trigger notifications for: booking updates, chat messages, payment status, review requests
- [ ] UI: Notification bell in nav with unread count, dropdown list

---

## Phase 4 — Production Readiness
> **Goal:** The app is stable, secure, observable, and deployable.

---

### Step 4.1 — Photo Storage Migration
- [ ] Migrate from disk to **Azure Blob Storage** or **AWS S3**
- [ ] Create `IFileStorageService` abstraction in Application layer
- [ ] Implement `AzureBlobStorageService` (or S3) in Infrastructure
- [ ] Upload returns a public URL stored in DB (not a file ID)
- [ ] Migrate existing photos to blob storage
- [ ] Update all photo serving — remove `/api/files/photos/{id}` endpoint

---

### Step 4.2 — Error Handling & Logging
- [ ] Add global exception handling middleware in API
- [ ] Standardise API error responses (ProblemDetails RFC 7807)
- [ ] Install **Serilog** with structured logging
- [ ] Sinks: Console (dev) + File + optionally Seq or Application Insights (prod)
- [ ] Log all unhandled exceptions, payment events, auth failures
- [ ] UI: User-friendly error pages (404, 500, unauthorised)

---

### Step 4.3 — Security Hardening
- [ ] Remove all hardcoded URLs — move to environment config
- [ ] CORS policy locked to known origins only
- [ ] Rate limiting on auth and payment endpoints (`AspNetCoreRateLimit`)
- [ ] HTTPS enforced in all environments
- [ ] Secrets in environment variables (never in `appsettings.json` for prod)
- [ ] Add input sanitisation where user-generated content is rendered as HTML
- [ ] Review all endpoints for missing `[Authorize]` guards

---

### Step 4.4 — Performance & Resilience
- [ ] Add pagination to all list queries (cursor or offset)
- [ ] Add caching for read-heavy data (surf spots, camp listings) — `IMemoryCache` or Redis
- [ ] Add `Polly` retry/circuit-breaker for Stripe and external API calls
- [ ] EF Core query audit — check for N+1 queries using logging
- [ ] Add DB indexes for common query patterns (UserId, CampId, Status)

---

### Step 4.5 — Dockerise
- [ ] Write `Dockerfile` for API project
- [ ] Write `Dockerfile` for Blazor UI project
- [ ] Write `docker-compose.yml` — API + UI + PostgreSQL
- [ ] Verify full local stack runs via Docker
- [ ] Add `.dockerignore`

---

### Step 4.6 — CI/CD Pipeline (GitHub Actions)
- [ ] `build.yml` — restore, build, run tests on every push/PR
- [ ] `deploy.yml` — build Docker images, push to registry, deploy on merge to `main`
- [ ] Secrets managed via GitHub Secrets
- [ ] Run EF Core migrations as part of deploy (or via migration job)

---

### Step 4.7 — Cloud Deployment
- [ ] Choose hosting: **Railway** (simplest) / **Render** / **Azure App Service**
- [ ] Choose managed PostgreSQL: Railway / **Supabase** / Azure Database for PostgreSQL
- [ ] Configure environment variables in hosting dashboard
- [ ] Set up custom domain + SSL
- [ ] Smoke test full flow on prod after first deploy

---

## Phase 5 — Monetisation
> **Goal:** The platform generates revenue.

---

### Step 5.1 — Commission System
- [ ] Define commission rate (e.g. 10% per booking)
- [ ] Calculate platform fee at booking creation
- [ ] Store `PlatformFee` and `OrganizerPayout` on Booking
- [ ] Admin dashboard: total revenue, commission earned, payout status

---

### Step 5.2 — Stripe Connect (Organizer Payouts)
- [ ] Organizers onboard via Stripe Connect (Express accounts)
- [ ] Platform charges user → holds funds → releases to organizer minus commission
- [ ] `OrganizerPayoutService` — trigger payouts post-completion
- [ ] UI: Organizer stripe connect onboarding flow, payout history page

---

### Step 5.3 — Subscription Tiers (Optional)
- [ ] Define tiers: Free (limited camps), Pro (unlimited + analytics + priority listing)
- [ ] Stripe Subscriptions for recurring billing
- [ ] Gate features by subscription tier in both API (`[Authorize(Policy=...)]`) and UI (`<AuthorizeView>`)
- [ ] Admin: subscription management, upgrade/downgrade

---

## Appendix — Claude Session Template

When starting a new step, paste this into Claude:

```
I'm working on Step X.X — [Step Name] of my surf camp platform.

Stack: .NET 8 Clean Architecture + CQRS (MediatR), Blazor Server, PostgreSQL + EF Core 9, MudBlazor

Here are the relevant files:
[paste Domain entity / DbContext / existing command / UI page]

Goal for this session: [what you want built]
```

---

*Last updated: Phase 1 in progress*
