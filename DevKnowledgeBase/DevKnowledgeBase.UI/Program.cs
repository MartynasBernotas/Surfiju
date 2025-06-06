using DevKnowledgeBase.UI.Components;
using DevKnowledgeBase.UI.Models;
using DevKnowledgeBase.UI.Services;
using FluentValidation;
using GoogleMapsComponents;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<CustomAuthStateProvider>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<INoteService, NoteService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICampService, CampService>();
builder.Services.AddScoped<IBookingService, BookingService>();

builder.Services.AddScoped<LoadingService>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireOrganizerRole", policy => policy.RequireRole("Organizer"));
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/login";
            options.LogoutPath = "/logout";
            options.AccessDeniedPath = "/";
        });

builder.Services.AddTransient<AuthenticationDelegatingHandler>(); //"https://localhost:7046/"
builder.Services.AddTransient<RequestLoadingDelegatingHandler>();

builder.Services.AddHttpClient("API", httpClient =>
{
    httpClient.BaseAddress = new Uri("https://localhost:7046/");
})
.AddHttpMessageHandler<AuthenticationDelegatingHandler>()
.AddHttpMessageHandler<RequestLoadingDelegatingHandler>();

builder.Services.AddHttpClient<GoogleLocationService>();

builder.Services.AddCircuitServicesAccessor();

builder.Services.AddMudServices();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterModelValidator>();
builder.Services.AddBlazorGoogleMaps("AIzaSyAAVP_llxal1IGKKGkVX76V-IIx8Vur2F8");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
