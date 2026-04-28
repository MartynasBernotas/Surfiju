# Phase 2 — Platform Features: Implementation Plan

## Stack Reference
- .NET 8 Clean Architecture + CQRS (MediatR)
- Blazor Server, MudBlazor 8.5.1
- PostgreSQL + EF Core 9
- ASP.NET Core Identity + JWT, Role-based (Admin / Organizer / Participant)
- FluentValidation 11, AutoMapper 16
- BlazorGoogleMaps (already registered, not yet used)

## Established Patterns (follow for all new code)

- Commands/queries: `DevKnowledgeBase.Application/Commands` and `.../Queries`. Command and handler in separate files. Validators in `.../Validators`.
- Handlers inject `DevDatabaseContext` directly — no repository abstraction.
- DTOs in `DevKnowledgeBase.Domain/Dtos`. UI models in `DevKnowledgeBase.UI/Models`.
- All AutoMapper config lives in the single `MappingProfile.cs`.
- Controllers: `[Authorize(Roles = "...")]` at controller level, `[AllowAnonymous]` on read endpoints.
- UI services: `IXxxService` / `XxxService` in `DevKnowledgeBase.UI/Services`, registered scoped in `UI/Program.cs`.
- Enum storage: `.HasConversion<string>()` in `OnModelCreating` — never store as int.
- Photos: `List<string>?` mapped to PostgreSQL `text[]`.
- UI enum copies: UI layer defines its own copy of domain enums (see `BookingModel.cs`).

## Inter-Step Dependencies

```
2.1 (SurfSpots) ──► 2.2 (CampSurfSpots join table)
                └──► 2.3 (TripStop.ReferenceId soft-refs SurfSpot)
2.4 (GroupBooking) ──► requires Camp + Booking (Phase 1 complete ✓)
2.5 (Guides/Lessons) ──► requires Camp (Phase 1 complete ✓)
```

Steps 2.4 and 2.5 can run in parallel after 2.1. Steps 2.2 and 2.3 must come after 2.1.

## EF Core Migration Strategy

One migration per step, run with:
```
dotnet ef migrations add <MigrationName> \
  --project DevKnowledgeBase/DevKnowledgeBase.Infrastructure \
  --startup-project DevKnowledgeBase/DevKnowledgeBase
```

| Migration Name       | Step | Tables Added                            |
|----------------------|------|-----------------------------------------|
| `AddSurfSpots`       | 2.1  | `SurfSpots`                             |
| `AddCampSurfSpots`   | 2.2  | `CampSurfSpots` (join)                  |
| `AddTripItinerary`   | 2.3  | `Trips`, `TripStops`                    |
| `AddGroupBooking`    | 2.4  | `GroupBookings`, `GroupBookingMembers`  |
| `AddGuidesAndLessons`| 2.5  | `Guides`, `Lessons`, `LessonBookings`   |

---

## Step 2.1 — Surf Spot Database

**Goal:** Admin can create surf spots. Users can browse spots on a map, filtered by region/skill/break type.

### Design Decisions

- **Lat/lng**: Two `double` columns (`Latitude`, `Longitude`) — no PostGIS dependency.
- **Photos**: `List<string>?` → `text[]`, same pattern as `Camp.PhotoUrls`.
- **Enums**: Separate files in `DevKnowledgeBase.Domain/Enums`, stored as strings.
- **Filtering**: `GetSurfSpotsQuery` uses `EF.Functions.Like` on `Location` for region search.
- **Admin-only mutations**: GET endpoints `[AllowAnonymous]`, POST/PUT/DELETE restricted to `Admin`.

### Implementation Order

1. Domain enums: `BreakType`, `SkillLevel`, `CrowdLevel`
2. `SurfSpot` entity
3. `SurfSpotDto`, `CreateSurfSpotDto`, `UpdateSurfSpotDto`
4. Commands: `CreateSurfSpotCommand`, `UpdateSurfSpotCommand`, `DeleteSurfSpotCommand` + handlers + validators
5. Queries: `GetSurfSpotsQuery`, `GetSurfSpotByIdQuery` + handlers
6. `MappingProfile` additions
7. `DevDatabaseContext`: `DbSet<SurfSpot>`, `OnModelCreating` config (enum-as-string, `text[]`)
8. Migration: `AddSurfSpots`
9. `SurfSpotsController`
10. UI: `SurfSpotModel`, `ISurfSpotService` / `SurfSpotService`
11. UI: `SurfSpots.razor` + `SurfSpotDialog.razor`
12. Register service in `UI/Program.cs`, add "Surf Spots" to `NavMenu.razor`

### Files to Create

**Domain:**
- `DevKnowledgeBase.Domain/Enums/BreakType.cs`
- `DevKnowledgeBase.Domain/Enums/SkillLevel.cs`
- `DevKnowledgeBase.Domain/Enums/CrowdLevel.cs`
- `DevKnowledgeBase.Domain/Entities/SurfSpot.cs`
- `DevKnowledgeBase.Domain/Dtos/SurfSpotDto.cs`

**Application:**
- `DevKnowledgeBase.Application/Queries/GetSurfSpotsQuery.cs` + `GetSurfSpotsQueryHandler.cs`
- `DevKnowledgeBase.Application/Queries/GetSurfSpotByIdQuery.cs` + handler
- `DevKnowledgeBase.Application/Commands/CreateSurfSpotCommand.cs` + handler
- `DevKnowledgeBase.Application/Commands/UpdateSurfSpotCommand.cs` + handler
- `DevKnowledgeBase.Application/Commands/DeleteSurfSpotCommand.cs` + handler
- `DevKnowledgeBase.Application/Validators/CreateSurfSpotCommandValidator.cs`

**Infrastructure:**
- Migration: `AddSurfSpots`

**API:**
- `DevKnowledgeBase/Controllers/SurfSpotsController.cs`

**UI:**
- `DevKnowledgeBase.UI/Models/SurfSpotModel.cs`
- `DevKnowledgeBase.UI/Services/ISurfSpotService.cs`
- `DevKnowledgeBase.UI/Services/SurfSpotService.cs`
- `DevKnowledgeBase.UI/Components/Pages/SurfSpots.razor`
- `DevKnowledgeBase.UI/Components/Dialogs/SurfSpotDialog.razor`

### Files to Modify

- `DevKnowledgeBase.Application/Common/MappingProfile.cs` — add `SurfSpot → SurfSpotDto` etc.
- `DevKnowledgeBase.Infrastructure/Data/DevDatabaseContext.cs` — `DbSet<SurfSpot>`, config
- `DevKnowledgeBase.UI/Program.cs` — register `ISurfSpotService`
- `DevKnowledgeBase.UI/Components/Layout/NavMenu.razor` — add "Surf Spots" link
- `DevKnowledgeBase.UI/Components/_Imports.razor` — add `@using GoogleMapsComponents` and `@using GoogleMapsComponents.Maps`

### SurfSpot Entity

```csharp
public class SurfSpot
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;  // human-readable region
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public BreakType BreakType { get; set; }
    public SkillLevel SkillLevel { get; set; }
    public CrowdLevel CrowdLevel { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<string>? Photos { get; set; } = new();
    public ICollection<CampSurfSpot> CampSurfSpots { get; set; } = new List<CampSurfSpot>();

    public SurfSpot() { Id = Guid.NewGuid(); }
}
```

### BlazorGoogleMaps Pattern

```razor
@using GoogleMapsComponents
@using GoogleMapsComponents.Maps

<GoogleMap @ref="_map" Id="surf-spot-map" Options="@_mapOptions" OnAfterInit="OnMapAfterInit" />
```

```csharp
private GoogleMap _map = null!;
private MapOptions _mapOptions = new()
{
    Zoom = 5,
    Center = new LatLngLiteral { Lat = 20, Lng = 0 },
    MapTypeId = MapTypeId.Terrain
};

// MUST be in OnAfterRenderAsync with firstRender guard — not OnInitializedAsync
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (!firstRender) return;
    // map init here
}

private async Task OnMapAfterInit()
{
    foreach (var spot in _spots)
    {
        await Marker.CreateAsync(_map.JsRuntime, new MarkerOptions
        {
            Position = new LatLngLiteral { Lat = spot.Latitude, Lng = spot.Longitude },
            Map = _map.InteropObject,
            Title = spot.Name
        });
    }
}
```

---

## Step 2.2 — Link Camps to Surf Spots

**Goal:** Organizers tag their camp with surf spots. Camp detail page shows tagged spots with map pins.

**Prerequisite:** Step 2.1 complete.

### Design Decisions

- **Join table**: `CampSurfSpot` with composite PK `(CampId, SurfSpotId)` — no surrogate Guid.
- **Update strategy**: `UpdateCampSurfSpotsCommand` clears and replaces all join rows on each save.
- **Camp detail enrichment**: `GetCampByIdQueryHandler` includes `CampSurfSpots.ThenInclude(SurfSpot)`.
- **Gotcha**: `GetAllCampsQueryHandler` uses manual `ToDto()` — must be updated manually (not just via MappingProfile).

### Implementation Order

1. `CampSurfSpot` entity
2. Add `ICollection<CampSurfSpot>` to `Camp` entity + `CampDto` extensions
3. `UpdateCampSurfSpotsCommand` + handler
4. `DevDatabaseContext`: `DbSet<CampSurfSpot>`, composite key config
5. Migration: `AddCampSurfSpots`
6. Update `GetCampByIdQueryHandler` and `GetAllCampsQueryHandler` to include surf spots
7. `CampsController`: `PUT /api/camps/{id}/surf-spots`
8. UI: extend `CampModel`, update `CampDialog.razor`, update `CampDetails.razor`

### Files to Create

- `DevKnowledgeBase.Domain/Entities/CampSurfSpot.cs`
- `DevKnowledgeBase.Application/Commands/UpdateCampSurfSpotsCommand.cs` + handler
- Migration: `AddCampSurfSpots`

### Files to Modify

- `DevKnowledgeBase.Domain/Entities/Camp.cs` — add `ICollection<CampSurfSpot> CampSurfSpots`
- `DevKnowledgeBase.Domain/Dtos/CampDto.cs` — add `List<SurfSpotSummaryDto> SurfSpots`, `List<Guid> SurfSpotIds`
- `DevKnowledgeBase.Domain/Dtos/SurfSpotDto.cs` — add `SurfSpotSummaryDto` (Id, Name, Lat, Lng, BreakType)
- `DevKnowledgeBase.Infrastructure/Data/DevDatabaseContext.cs` — add DbSet, composite key
- `DevKnowledgeBase.Application/Common/MappingProfile.cs` — update Camp → CampDto mapping
- `DevKnowledgeBase.Application/Queries/GetCampByIdQueryHandler.cs` — add includes
- `DevKnowledgeBase.Application/Queries/GetAllCampsQueryHandler.cs` — update manual ToDto()
- `DevKnowledgeBase/Controllers/CampsController.cs` — add new endpoint
- `DevKnowledgeBase.UI/Models/CampModel.cs` — add `List<Guid> SurfSpotIds`, `List<SurfSpotSummaryModel> SurfSpots`
- `DevKnowledgeBase.UI/Components/Dialogs/CampDialog.razor` — MudAutocomplete for surf spots
- `DevKnowledgeBase.UI/Components/Pages/CampDetails.razor` — surf spots section with map

### CampSurfSpot Entity

```csharp
public class CampSurfSpot
{
    public Guid CampId { get; set; }
    public Camp Camp { get; set; } = null!;
    public Guid SurfSpotId { get; set; }
    public SurfSpot SurfSpot { get; set; } = null!;
}
```

---

## Step 2.3 — Trip Itinerary Builder

**Goal:** Users can build a day-by-day travel itinerary, adding surf spots, accommodation, transport, and freeform activity stops.

### Design Decisions

- **`Trip` entity is new** — the old "Trip" was renamed to "Camp" in a migration; no collision.
- **`TripStop.ReferenceId`**: Nullable `Guid?`, soft reference to `SurfSpot.Id` when `StopType == SurfSpot`. No FK constraint — avoids cascades if spots are deleted.
- **Ordering**: `Day` (int) + `OrderIndex` (int per day). `ReorderTripStopsCommand` bulk-updates in one `SaveChangesAsync`.
- **Authorization**: Users see only their own trips. No admin override.

### Implementation Order

1. `StopType` enum, `Trip` and `TripStop` entities
2. DTOs: `TripDto`, `TripStopDto`, `CreateTripDto`, `CreateTripStopDto`, `ReorderStopItem`
3. Queries: `GetUserTripsQuery`, `GetTripByIdQuery` + handlers
4. Commands: `CreateTripCommand`, `AddTripStopCommand`, `ReorderTripStopsCommand`, `DeleteTripCommand`, `DeleteTripStopCommand` + handlers + validators
5. `MappingProfile` additions
6. `DevDatabaseContext` + migration: `AddTripItinerary`
7. `TripsController`
8. UI: `TripModel`, `ITripService` / `TripService`
9. UI: `Trips.razor`, `TripPlanner.razor`, `CreateTripDialog.razor`, `AddTripStopDialog.razor`

### Files to Create

**Domain:**
- `DevKnowledgeBase.Domain/Enums/StopType.cs`
- `DevKnowledgeBase.Domain/Entities/Trip.cs`
- `DevKnowledgeBase.Domain/Entities/TripStop.cs`
- `DevKnowledgeBase.Domain/Dtos/TripDto.cs`

**Application (10 files):**
- `GetUserTripsQuery.cs` + handler
- `GetTripByIdQuery.cs` + handler
- `CreateTripCommand.cs` + handler
- `AddTripStopCommand.cs` + handler
- `ReorderTripStopsCommand.cs` + handler
- `DeleteTripCommand.cs` + handler
- `DeleteTripStopCommand.cs` + handler
- `CreateTripCommandValidator.cs`
- `AddTripStopCommandValidator.cs`

**API:**
- `DevKnowledgeBase/Controllers/TripsController.cs`

**UI:**
- `DevKnowledgeBase.UI/Models/TripModel.cs`
- `DevKnowledgeBase.UI/Services/ITripService.cs` + `TripService.cs`
- `DevKnowledgeBase.UI/Components/Pages/Trips.razor`
- `DevKnowledgeBase.UI/Components/Pages/TripPlanner.razor` — `@page "/trips/{Id:guid}"`
- `DevKnowledgeBase.UI/Components/Dialogs/CreateTripDialog.razor`
- `DevKnowledgeBase.UI/Components/Dialogs/AddTripStopDialog.razor`

### Files to Modify

- `DevKnowledgeBase.Domain/Entities/User.cs` — add `ICollection<Trip> Trips`
- `DevKnowledgeBase.Infrastructure/Data/DevDatabaseContext.cs` — DbSets + config
- `DevKnowledgeBase.Application/Common/MappingProfile.cs`
- `DevKnowledgeBase.UI/Program.cs` — register `ITripService`
- `DevKnowledgeBase.UI/Components/Layout/NavMenu.razor` — add "My Trips" (Authorized)

### Key Entities

```csharp
public class Trip
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Destination { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Notes { get; set; }
    public string UserId { get; set; } = string.Empty;
    public User User { get; set; } = null!;
    public ICollection<TripStop> Stops { get; set; } = new List<TripStop>();

    public Trip() { Id = Guid.NewGuid(); }
}

public class TripStop
{
    public Guid Id { get; set; }
    public Guid TripId { get; set; }
    public Trip Trip { get; set; } = null!;
    public int Day { get; set; }
    public StopType StopType { get; set; }
    public Guid? ReferenceId { get; set; }  // soft ref to SurfSpot.Id
    public string? Notes { get; set; }
    public int OrderIndex { get; set; }

    public TripStop() { Id = Guid.NewGuid(); }
}
```

### UI: TripPlanner.razor

Use `MudTimeline` for day-by-day view. Use `MudDropContainer` / `MudDropZone` (available in MudBlazor 8.5.1) for drag-to-reorder within a day.

---

## Step 2.4 — Group Booking & Cost Splitting

**Goal:** A user creates a group booking for a camp, generates an invite link, friends join and each pays their share independently via Stripe.

### Design Decisions

- **`GroupBooking` is a separate aggregate** — not a subclass of `Booking`. Each member's payment creates a standard `Booking` record. `GroupBookingMember.BookingId` is a nullable FK to `Booking`.
- **Invite token**: `string InviteToken` generated via `Convert.ToBase64String(Guid.NewGuid().ToByteArray())` — URL-safe, stored with unique index.
- **Split modes**: `SplitMode` enum (`Equal = 0, Custom = 1`). Equal split auto-recalculates `AmountOwed` when a member joins.
- **Payment reuse**: `PayGroupShareCommand` calls the existing `IPaymentService.CreatePaymentIntentAsync`.
- **Member status flow**: `Invited → Joined → Paid`.

### Implementation Order

1. Enums: `GroupMemberStatus`, `SplitMode`
2. `GroupBooking`, `GroupBookingMember` entities
3. DTOs
4. Commands: `CreateGroupBookingCommand`, `InviteMemberCommand`, `JoinGroupBookingCommand`, `PayGroupShareCommand` + handlers
5. Queries: `GetGroupBookingQuery`, `GetUserGroupBookingsQuery` + handlers
6. `DevDatabaseContext` — unique index on `InviteToken` is critical
7. Migration: `AddGroupBooking`
8. `GroupBookingsController`
9. UI: `GroupBookingModel`, `IGroupBookingService` / `GroupBookingService`
10. UI: `GroupBooking.razor`, `JoinGroupBooking.razor`, `CreateGroupBookingDialog.razor`

### Files to Create

**Domain:**
- `DevKnowledgeBase.Domain/Enums/GroupMemberStatus.cs`
- `DevKnowledgeBase.Domain/Enums/SplitMode.cs`
- `DevKnowledgeBase.Domain/Entities/GroupBooking.cs`
- `DevKnowledgeBase.Domain/Entities/GroupBookingMember.cs`
- `DevKnowledgeBase.Domain/Dtos/GroupBookingDto.cs`

**Application:**
- `CreateGroupBookingCommand.cs` + handler
- `InviteMemberCommand.cs` + handler
- `JoinGroupBookingCommand.cs` + handler
- `PayGroupShareCommand.cs` + handler (returns `GroupSharePaymentResult(Guid LessonBookingId, string ClientSecret)`)
- `GetGroupBookingQuery.cs` + handler
- `GetUserGroupBookingsQuery.cs` + handler

**API:**
- `DevKnowledgeBase/Controllers/GroupBookingsController.cs`
  - `POST /api/group-bookings`
  - `GET /api/group-bookings`
  - `GET /api/group-bookings/{id}`
  - `POST /api/group-bookings/{id}/invite`
  - `POST /api/group-bookings/join/{token}`
  - `POST /api/group-bookings/{id}/pay`

**UI:**
- `DevKnowledgeBase.UI/Models/GroupBookingModel.cs`
- `DevKnowledgeBase.UI/Services/IGroupBookingService.cs` + `GroupBookingService.cs`
- `DevKnowledgeBase.UI/Components/Pages/GroupBooking.razor` — `@page "/group-booking/{Id:guid}"`
- `DevKnowledgeBase.UI/Components/Pages/JoinGroupBooking.razor` — `@page "/group-booking/join/{Token}"`
- `DevKnowledgeBase.UI/Components/Dialogs/CreateGroupBookingDialog.razor`

### Files to Modify

- `DevKnowledgeBase.Infrastructure/Data/DevDatabaseContext.cs` — DbSets, unique index on `InviteToken`
- `DevKnowledgeBase.Application/Common/MappingProfile.cs`
- `DevKnowledgeBase.UI/Program.cs` — register `IGroupBookingService`
- `DevKnowledgeBase.UI/Components/Pages/CampDetails.razor` — add "Create Group Booking" button
- `DevKnowledgeBase.UI/Components/Pages/User/MyBookings.razor` — add Group Bookings tab

### Key Entities

```csharp
public class GroupBooking
{
    public Guid Id { get; set; }
    public Guid CampId { get; set; }
    public Camp Camp { get; set; } = null!;
    public string CreatedByUserId { get; set; } = string.Empty;
    public User CreatedBy { get; set; } = null!;
    public string InviteToken { get; set; } = string.Empty;
    public SplitMode SplitMode { get; set; }
    public decimal TotalCost { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<GroupBookingMember> Members { get; set; } = new List<GroupBookingMember>();

    public GroupBooking() { Id = Guid.NewGuid(); }
}

public class GroupBookingMember
{
    public Guid Id { get; set; }
    public Guid GroupBookingId { get; set; }
    public GroupBooking GroupBooking { get; set; } = null!;
    public string UserId { get; set; } = string.Empty;
    public User User { get; set; } = null!;
    public GroupMemberStatus Status { get; set; } = GroupMemberStatus.Invited;
    public decimal AmountOwed { get; set; }
    public DateTime? PaidAt { get; set; }
    public Guid? BookingId { get; set; }  // set after payment
    public Booking? Booking { get; set; }

    public GroupBookingMember() { Id = Guid.NewGuid(); }
}
```

---

## Step 2.5 — Guide & Lesson Booking

**Goal:** Admin manages guide profiles and lessons. Users browse guides and book lessons, including inline on camp detail pages.

### Design Decisions

- **`Guide.LinkedUserId`**: Nullable FK to `AspNetUsers` — allows guides to log in, but not required.
- **`Lesson.CampId`**: Nullable FK to `Camp` — lessons can be standalone or tied to a camp.
- **`LessonBookingStatus`**: New enum independent of `BookingStatus` — `Pending`, `Confirmed`, `Cancelled`. Keeps state machines separate.
- **Capacity check**: `BookLessonCommand` counts non-cancelled `LessonBooking` rows — same concurrency pattern as `CreateBookingCommandHandler`.
- **Payment reuse**: Calls existing `IPaymentService.CreatePaymentIntentAsync`, returns `ClientSecret`.

### Implementation Order

1. Enums: `LessonType`, `LessonBookingStatus`
2. `Guide`, `Lesson`, `LessonBooking` entities
3. DTOs: `GuideDto`, `LessonDto`, `LessonBookingDto`, `CreateGuideDto`, `CreateLessonDto`
4. Queries: `GetGuidesQuery`, `GetGuideByIdQuery`, `GetLessonsByCampQuery` + handlers
5. Commands: `CreateGuideCommand`, `UpdateGuideCommand`, `DeleteGuideCommand`, `CreateLessonCommand`, `BookLessonCommand`, `CancelLessonBookingCommand` + handlers + validators
6. `MappingProfile` additions
7. `DevDatabaseContext` + migration: `AddGuidesAndLessons`
8. `GuidesController`, `LessonsController`
9. UI: `GuideModel`, `IGuideService` / `GuideService`
10. UI: `Guides.razor`, `GuideDetails.razor`, `BookLessonDialog.razor`
11. Update `CampDetails.razor` — lessons section

### Files to Create

**Domain:**
- `DevKnowledgeBase.Domain/Enums/LessonType.cs`
- `DevKnowledgeBase.Domain/Enums/LessonBookingStatus.cs`
- `DevKnowledgeBase.Domain/Entities/Guide.cs`
- `DevKnowledgeBase.Domain/Entities/Lesson.cs`
- `DevKnowledgeBase.Domain/Entities/LessonBooking.cs`
- `DevKnowledgeBase.Domain/Dtos/GuideDto.cs`

**Application (12 files):**
- `GetGuidesQuery.cs` + handler
- `GetGuideByIdQuery.cs` + handler
- `GetLessonsByCampQuery.cs` + handler
- `CreateGuideCommand.cs` + handler
- `UpdateGuideCommand.cs` + handler
- `DeleteGuideCommand.cs` + handler
- `CreateLessonCommand.cs` + handler
- `BookLessonCommand.cs` + handler (returns `BookLessonResult(Guid LessonBookingId, string ClientSecret)`)
- `CancelLessonBookingCommand.cs` + handler
- `CreateGuideCommandValidator.cs`
- `BookLessonCommandValidator.cs`

**API:**
- `DevKnowledgeBase/Controllers/GuidesController.cs`
  - `GET /api/guides` — `[AllowAnonymous]`
  - `GET /api/guides/{id}` — `[AllowAnonymous]`
  - `POST /api/guides` — Admin
  - `PUT /api/guides/{id}` — Admin
  - `DELETE /api/guides/{id}` — Admin
  - `POST /api/guides/{id}/lessons` — Admin
  - `GET /api/guides/{id}/lessons` — `[AllowAnonymous]`
- `DevKnowledgeBase/Controllers/LessonsController.cs`
  - `GET /api/lessons?campId={campId}` — `[AllowAnonymous]`
  - `POST /api/lessons/{id}/book` — Authorized
  - `DELETE /api/lessons/bookings/{id}` — Authorized

**UI:**
- `DevKnowledgeBase.UI/Models/GuideModel.cs`
- `DevKnowledgeBase.UI/Services/IGuideService.cs` + `GuideService.cs`
- `DevKnowledgeBase.UI/Components/Pages/Guides.razor` — `@page "/guides"`
- `DevKnowledgeBase.UI/Components/Pages/GuideDetails.razor` — `@page "/guides/{Id:guid}"`
- `DevKnowledgeBase.UI/Components/Dialogs/BookLessonDialog.razor`

### Files to Modify

- `DevKnowledgeBase.Domain/Entities/Camp.cs` — add `ICollection<Lesson> Lessons`
- `DevKnowledgeBase.Infrastructure/Data/DevDatabaseContext.cs` — 3 new DbSets, optional FKs
- `DevKnowledgeBase.Application/Common/MappingProfile.cs`
- `DevKnowledgeBase.UI/Program.cs` — register `IGuideService`
- `DevKnowledgeBase.UI/Components/Layout/NavMenu.razor` — add "Guides" link
- `DevKnowledgeBase.UI/Components/Pages/CampDetails.razor` — add "Lessons at This Camp" section

### Key Entities

```csharp
public class Guide
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string Speciality { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }
    public List<string>? PhotoUrls { get; set; } = new();
    public string? LinkedUserId { get; set; }
    public User? LinkedUser { get; set; }
    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

    public Guide() { Id = Guid.NewGuid(); }
}

public class Lesson
{
    public Guid Id { get; set; }
    public Guid GuideId { get; set; }
    public Guide Guide { get; set; } = null!;
    public Guid? CampId { get; set; }
    public Camp? Camp { get; set; }
    public LessonType LessonType { get; set; }
    public int DurationMinutes { get; set; }
    public decimal Price { get; set; }
    public int MaxParticipants { get; set; }
    public ICollection<LessonBooking> Bookings { get; set; } = new List<LessonBooking>();

    public Lesson() { Id = Guid.NewGuid(); }
}

public class LessonBooking
{
    public Guid Id { get; set; }
    public Guid LessonId { get; set; }
    public Lesson Lesson { get; set; } = null!;
    public string UserId { get; set; } = string.Empty;
    public User User { get; set; } = null!;
    public LessonBookingStatus Status { get; set; } = LessonBookingStatus.Pending;
    public DateTime BookedAt { get; set; }
    public string? PaymentIntentId { get; set; }

    public LessonBooking() { Id = Guid.NewGuid(); }
}
```

---

## Critical Gotchas

1. **`GetAllCampsQueryHandler` uses manual `ToDto()`** — AutoMapper MappingProfile changes alone won't update the list query. Must manually add `SurfSpots = c.CampSurfSpots.Select(...).ToList()` in Step 2.2.

2. **`Camp.UseXminAsConcurrencyToken()`** already configured — adding new navigation properties to `Camp` does not break xmin optimistic concurrency.

3. **BlazorGoogleMaps map init must be in `OnAfterRenderAsync(firstRender)`** — not `OnInitializedAsync`. The map requires the DOM to exist.

4. **`GroupBooking.InviteToken` needs a unique DB index** — configure in `OnModelCreating` via `.HasIndex(gb => gb.InviteToken).IsUnique()`. The handler should also handle the rare collision by retrying token generation.

5. **UI enum copies** — the UI project defines its own enum copies in `Models/` (see `BookingModel.cs`). New status/type enums (`GroupMemberStatus`, `LessonBookingStatus`, `StopType` etc.) must follow the same pattern.

6. **`MappingProfile.cs` is the single AutoMapper config file** — all `CreateMap` calls go here. It will grow; only split if it exceeds ~150 lines and the team agrees.

7. **`PaginatedQueryResult<T>` vs plain `List<T>`** — `GET /api/camps/organizer` returns a `PaginatedQueryResult` but the UI deserializes it as `List<CampModel>`. Do not change this in Phase 2 to avoid regressions.
