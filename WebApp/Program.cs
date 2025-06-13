using App.Contracts.DAL;
using App.DAL.EF;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddScoped<IAppUnitOfWork, AppUOW>();


builder.Services.AddAutoMapper(
    typeof(App.DAL.EF.AutoMapperProfile),
    typeof(AutoMapperProfile)
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();