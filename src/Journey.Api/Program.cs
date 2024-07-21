using Journey.Api;
using Journey.Api.Filters;
using Journey.Api.Jobs;
using Journey.Application.UseCases.Activities.Complete;
using Journey.Application.UseCases.Activities.Delete;
using Journey.Application.UseCases.Activities.GetAllByTripId;
using Journey.Application.UseCases.Activities.Register;
using Journey.Application.UseCases.Trips.Delete;
using Journey.Application.UseCases.Trips.GetAll;
using Journey.Application.UseCases.Trips.GetById;
using Journey.Application.UseCases.Trips.Register;
using Journey.Application.UseCases.Users.AuthenticateUser;
using Journey.Application.UseCases.Users.GetAll;
using Journey.Application.UseCases.Users.Register;
using Journey.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Quartz;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<JourneyDbContext>(options =>
    options.UseSqlite(connectionString));

var key = Encoding.ASCII.GetBytes(Settings.Secret);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddMvc(config => config.Filters.Add(typeof(ExceptionFilter)));

var jobKey = JobKey.Create(nameof(CheckTripsJob));
builder.Services.AddQuartz(opt =>
{
    opt.AddJob<CheckTripsJob>(jobKey).AddTrigger(trigger => trigger.StartNow()
    .ForJob(jobKey)
    .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(00,00)));
    
});
builder.Services.AddQuartzHostedService(opt =>
{
    opt.WaitForJobsToComplete = true;
});

builder.Services.AddTransient<GetAllTripsUseCase>();
builder.Services.AddTransient<GetTripByIdUseCase>();
builder.Services.AddTransient<RegisterTripUseCase>();
builder.Services.AddTransient<DeleteTripByIdUseCase>();

builder.Services.AddTransient<GetAllUsersUseCase>();
builder.Services.AddTransient<AuthenticateUserUseCase>();
builder.Services.AddTransient<RegisterUserUseCase>();

builder.Services.AddTransient<GetAllActivitiesByTripIdUseCase>();
builder.Services.AddTransient<RegisterActivityForTripUseCase>();
builder.Services.AddTransient<DeleteActivityForTripUseCase>();
builder.Services.AddTransient<CompleteActivityForTripUseCase>();

var app = builder.Build();

var schedulerFactory = app.Services.GetRequiredService<ISchedulerFactory>();
var scheduler = await schedulerFactory.GetScheduler();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
