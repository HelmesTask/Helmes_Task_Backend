using System.Net.Http.Json;
using System.Text.Json;
using App.DAL.EF;
using App.DTO.v1_0;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace App.Test;

public class ApiControllerTests: IClassFixture<CustomWebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly ITestOutputHelper _testOutputHelper;

    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly JsonSerializerOptions _camelCaseJsonSerializerOptions = new JsonSerializerOptions()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static readonly Guid TopSectorId = Guid.Parse("0422df2b-ee93-4b0f-a9fc-d156429b946d");
    private static readonly Guid MidSector1Id = Guid.Parse("f11362a7-f7a7-4a58-9a43-bae8b30f1812");
    private static readonly Guid MidSector2Id = Guid.Parse("d45e14ab-c639-4c9c-a982-98ea6fffb9d2");
    
    private static readonly Guid SessionId = Guid.Parse("a53861c8-218d-45dc-ae90-0e1fc355f6b2");

    private static readonly AppUser AppUser = new App.DTO.v1_0.AppUser
    {
        SessionId = SessionId,
        UserName = "testuser2",
        TermsConfirmed = true
    };




    public ApiControllerTests(CustomWebApplicationFactory<Program> factory, ITestOutputHelper testOutputHelper)
    {
        _factory = factory;
        _testOutputHelper = testOutputHelper;
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }
    public async Task DisposeAsync()
    {
    }
    public async Task InitializeAsync()
    {
        var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();        
        
    }
    
    private Sector CreateSector(Guid sectorId, Guid parentSectorId, string sectorName)
    {
        Sector sector = new Sector
        {
            Id = sectorId,
            ParentSectorId = parentSectorId,
            SectorName = sectorName
        };
        return sector;
    }


    [Fact]
    public async Task PostSector1()
    {
        Sector sector1 = CreateSector(TopSectorId, Guid.Empty, "TopSector");
        var url = "/api/v1/Sector/PostSector";
        var data = JsonContent.Create(sector1);
        var response = await _client.PostAsync(url, data);
        Assert.True(response.IsSuccessStatusCode, $"PostSector1 failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    [Fact]
    public async Task PostSector2()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var sector1 = new App.Domain.Sector { Id = TopSectorId, ParentSectorId = null, SectorName = "TopSector" };
            dbContext.Sectors.Add(sector1);
            await dbContext.SaveChangesAsync();
        }

        Sector sector2 = CreateSector(MidSector1Id, TopSectorId, "MidSector");
        var url = "/api/v1/Sector/PostSector";
        var data = JsonContent.Create(sector2);
        var response = await _client.PostAsync(url, data);
        Assert.True(response.IsSuccessStatusCode, $"PostSector2 failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    [Fact]
    public async Task PostSector3()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var sector1 = new App.Domain.Sector { Id = TopSectorId, ParentSectorId = null, SectorName = "TopSector" };
            dbContext.Sectors.Add(sector1);
            await dbContext.SaveChangesAsync();
        }

        Sector sector3 = CreateSector(MidSector2Id, TopSectorId, "MidSector");
        var url = "/api/v1/Sector/PostSector";
        var data = JsonContent.Create(sector3);
        var response = await _client.PostAsync(url, data);
        Assert.True(response.IsSuccessStatusCode, $"PostSector3 failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    [Fact]
    public async Task GetAllSectors()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var sector1 = new App.Domain.Sector { Id = TopSectorId, ParentSectorId = null, SectorName = "TopSector" };
            var sector2 = new App.Domain.Sector { Id = MidSector1Id, ParentSectorId = TopSectorId, SectorName = "MidSector" };
            var sector3 = new App.Domain.Sector { Id = MidSector2Id, ParentSectorId = MidSector1Id, SectorName = "MidSector1" };

            dbContext.Sectors.Add(sector1);
            dbContext.Sectors.Add(sector2);
            dbContext.Sectors.Add(sector3);
            await dbContext.SaveChangesAsync();
        }

        string url = "/api/v1/Sector/GetSectors";
        var response = await _client.GetAsync(url);

        Assert.True(response.IsSuccessStatusCode, $"GetAllSectors failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");

        var responseContent = await response.Content.ReadAsStringAsync();
        var actualSectors = JsonSerializer.Deserialize<List<App.DTO.v1_0.Sector>>(responseContent, _camelCaseJsonSerializerOptions);


        var expectedSectors = new List<App.DTO.v1_0.Sector>
        {
            new App.DTO.v1_0.Sector { Id = TopSectorId, ParentSectorId = null, SectorName = "TopSector" },
            new App.DTO.v1_0.Sector { Id = MidSector1Id, ParentSectorId = TopSectorId, SectorName = "MidSector" },
            new App.DTO.v1_0.Sector { Id = MidSector2Id, ParentSectorId = MidSector1Id, SectorName = "MidSector1" }
        };
        
        //sort them to ensure order and compare object's variables
        var sortedActualSectors = actualSectors.OrderBy(s => s.Id).ToList();
        var sortedExpectedSectors = expectedSectors.OrderBy(s => s.Id).ToList();

        for (int i = 0; i < sortedExpectedSectors.Count; i++)
        {
            Assert.Equal(sortedExpectedSectors[i].Id, sortedActualSectors[i].Id);
            Assert.Equal(sortedExpectedSectors[i].SectorName, sortedActualSectors[i].SectorName);
            Assert.Equal(sortedExpectedSectors[i].ParentSectorId, sortedActualSectors[i].ParentSectorId);
        }
    }

    [Fact]
    public async Task PostUserAndAppUserSectors()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var sector1 = new App.Domain.Sector { Id = TopSectorId, ParentSectorId = null, SectorName = "TopSector" };
            var sector2 = new App.Domain.Sector { Id = MidSector1Id, ParentSectorId = TopSectorId, SectorName = "MidSectorA" };
            var sector3 = new App.Domain.Sector { Id = MidSector2Id, ParentSectorId = TopSectorId, SectorName = "MidSectorB" };
            dbContext.Sectors.AddRange(sector1, sector2, sector3);
            await dbContext.SaveChangesAsync();
        }

        var sectorIdList = new List<Guid>
        {
            TopSectorId,
            MidSector1Id,
            MidSector2Id
        };
        
        var urlAppUserSectors = "/api/v1/AppUserSector/PostAppUserSectors";
        var requestData = new PostAppUserSectorsRequest
        {
            User = AppUser,
            SectorIdsList = sectorIdList
        };
        var dataAppUserSectors = JsonContent.Create(requestData);
        var responseAppUserSectors = await _client.PostAsync(urlAppUserSectors, dataAppUserSectors);

        Assert.True(responseAppUserSectors.IsSuccessStatusCode, $"PostUserAndAppUserSectors failed. Status: {responseAppUserSectors.StatusCode}. Content: {await responseAppUserSectors.Content.ReadAsStringAsync()}");

        var responseContent = await responseAppUserSectors.Content.ReadAsStringAsync();
        var responseSectorIdList = JsonSerializer.Deserialize<List<Guid>>(responseContent);
        Assert.True(sectorIdList.SequenceEqual(responseSectorIdList!), "The sectorIdList does not match the response.");
    }

    [Fact]
    public async Task PostUserAndFalseAppUserSectors()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var sector1 = new App.Domain.Sector { Id = TopSectorId, ParentSectorId = null, SectorName = "TopSector" };
            var sector2 = new App.Domain.Sector { Id = MidSector1Id, ParentSectorId = TopSectorId, SectorName = "MidSector" };
            var sector3 = new App.Domain.Sector { Id = MidSector2Id, ParentSectorId = MidSector1Id, SectorName = "MidSector1" };

            dbContext.Sectors.Add(sector1);
            dbContext.Sectors.Add(sector2);
            dbContext.Sectors.Add(sector3);
            await dbContext.SaveChangesAsync();
        }

        var url = "/api/v1/AppUserSector/PostAppUserSectors";
        var falseSectorIdList = new List<Guid>
        {
            TopSectorId,
            Guid.Empty,
            MidSector2Id
        };

        var correctSectorIdList = new List<Guid>
        {
            TopSectorId,
            MidSector2Id
        };

        var requestData = new PostAppUserSectorsRequest
        {
            User = AppUser,
            SectorIdsList = falseSectorIdList
        };
        var data = JsonContent.Create(requestData);
        var response = await _client.PostAsync(url, data);

        Assert.True(response.IsSuccessStatusCode, $"PostUserAndFalseAppUserSectors failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");

        var responseContent = await response.Content.ReadAsStringAsync();
        var responseSectorIdList = JsonSerializer.Deserialize<List<Guid>>(responseContent);
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(responseSectorIdList));
        Assert.True(correctSectorIdList.SequenceEqual(responseSectorIdList!), "The sectorIdList does not match the response.");
    }

    
    [Fact]
    public async Task GetAllUserAppUserSectors()
    {
        Guid appUserSectorId1 = Guid.Parse("5e64529f-1761-4cf5-90c1-cc306451bea0");
        Guid appUserSectorId2 = Guid.Parse("ef15f6a5-c365-4f54-bfb5-eeb3349989da");
        Guid appUserSectorId3 = Guid.Parse("f520fb92-8030-495c-8e9b-05658aa333bf");
        
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            App.Domain.Sector sector1 = new App.Domain.Sector { Id = TopSectorId, ParentSectorId = null, SectorName = "TopSector" };
            App.Domain.Sector sector2 = new App.Domain.Sector { Id = MidSector1Id, ParentSectorId = TopSectorId, SectorName = "MidSector" };
            App.Domain.Sector sector3 = new App.Domain.Sector { Id = MidSector2Id, ParentSectorId = MidSector1Id, SectorName = "MidSector1" };
            App.Domain.User.AppUser domainAppUser1 = new App.Domain.User.AppUser
            {
                Id = Guid.Parse("eda2026d-9f53-4546-a941-6e516ad18a41"),
                SessionId = AppUser.SessionId,
                UserName = AppUser.UserName,
                TermsConfirmed = AppUser.TermsConfirmed
            };
            App.Domain.User.AppUser domainAppUser2 = new App.Domain.User.AppUser
            {
                Id = Guid.Parse("449d767f-9703-43aa-bae0-26ec03d11563"),
                SessionId = AppUser.SessionId,
                UserName = AppUser.UserName,
                TermsConfirmed = AppUser.TermsConfirmed
            };
            
            App.Domain.User.AppUserSector appUserSector1 = new App.Domain.User.AppUserSector
            {
                Id = appUserSectorId1,
                AppUserId = domainAppUser1.Id,
                SectorId = TopSectorId
            };
            App.Domain.User.AppUserSector appUserSector2 = new App.Domain.User.AppUserSector
            {
                Id = appUserSectorId2,
                AppUserId = domainAppUser1.Id,
                SectorId = MidSector1Id
            };
            App.Domain.User.AppUserSector appUserSector3 = new App.Domain.User.AppUserSector
            {
                Id = appUserSectorId3,
                AppUserId = domainAppUser2.Id,
                SectorId = MidSector2Id
            };

            dbContext.Sectors.Add(sector1);
            dbContext.Sectors.Add(sector2);
            dbContext.Sectors.Add(sector3);
            dbContext.AppUsers.Add(domainAppUser1);
            dbContext.AppUsers.Add(domainAppUser2);
            dbContext.AppUserSectors.Add(appUserSector1);
            dbContext.AppUserSectors.Add(appUserSector2);
            dbContext.AppUserSectors.Add(appUserSector3);
            await dbContext.SaveChangesAsync();
        }
        
        string url = $"/api/v1/AppUserSector/GetAppUserSectors/{SessionId}";
        var response = await _client.GetAsync(url);
        Assert.True(response.IsSuccessStatusCode, $"GetAppUserSectors failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
        var responseContent = await response.Content.ReadAsStringAsync();
        var correctAppUserSectorIdList = new List<Guid>
        {
            TopSectorId,
            MidSector1Id
        };
        
        var responseSectorIdList = JsonSerializer.Deserialize<List<Guid>>(responseContent);
        Assert.True(correctAppUserSectorIdList.SequenceEqual(responseSectorIdList!), "The sectorIdList does not match the response.");

    }
    
    [Fact]
    public async Task PutUserAndAppUserSectors()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            App.Domain.Sector sector1 = new App.Domain.Sector { Id = TopSectorId, ParentSectorId = null, SectorName = "TopSector" };
            App.Domain.Sector sector2 = new App.Domain.Sector { Id = MidSector1Id, ParentSectorId = TopSectorId, SectorName = "MidSector" };
            App.Domain.Sector sector3 = new App.Domain.Sector { Id = MidSector2Id, ParentSectorId = MidSector1Id, SectorName = "MidSector1" };
            App.Domain.User.AppUser domainAppUser = new App.Domain.User.AppUser
            {
                Id = Guid.Parse("eda2026d-9f53-4546-a941-6e516ad18a41"),
                SessionId = AppUser.SessionId,
                UserName = AppUser.UserName,
                TermsConfirmed = AppUser.TermsConfirmed
            };

            dbContext.Sectors.Add(sector1);
            dbContext.Sectors.Add(sector2);
            dbContext.Sectors.Add(sector3);
            dbContext.AppUsers.Add(domainAppUser);
            await dbContext.SaveChangesAsync();
        }
        string url = $"/api/v1/AppUserSector/PutAppUserSectors/{SessionId}";
        var sectorIdList = new List<Guid>
        {
            TopSectorId,
            MidSector1Id,
            MidSector2Id
        };
        var data = JsonContent.Create(sectorIdList);
        var response = await _client.PutAsync(url, data);
        Assert.True(response.IsSuccessStatusCode, $"PutUserAndAppUserSectors failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
        var responseContent = await response.Content.ReadAsStringAsync();
        var responseSectorIdList = JsonSerializer.Deserialize<List<Guid>>(responseContent);
        Assert.True(sectorIdList.SequenceEqual(responseSectorIdList!), "The sectorIdList does not match the response.");
    }

    [Fact]
    public async Task PutUserAndNonExistingAppUserSectors()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var sector1 = new App.Domain.Sector { Id = TopSectorId, ParentSectorId = null, SectorName = "TopSector" };
            var sector3 = new App.Domain.Sector { Id = MidSector2Id, ParentSectorId = TopSectorId, SectorName = "MidSector1" };
            var domainAppUser = new App.Domain.User.AppUser
            {
                Id = Guid.Parse("eda2026d-9f53-4546-a941-6e516ad18a41"),
                SessionId = AppUser.SessionId,
                UserName = AppUser.UserName,
                TermsConfirmed = AppUser.TermsConfirmed
            };

            dbContext.Sectors.Add(sector1);
            //intentionally missing sector
            dbContext.Sectors.Add(sector3);
            dbContext.AppUsers.Add(domainAppUser);
            await dbContext.SaveChangesAsync();
        }
        var url = $"/api/v1/AppUserSector/PutAppUserSectors/{SessionId}";
        var sectorIdList = new List<Guid>
        {
            TopSectorId,
            MidSector1Id,
            MidSector2Id
        };

        var correctIdList = new List<Guid>
        {
            TopSectorId,
            MidSector2Id
        };


        var data = JsonContent.Create(sectorIdList);
        var response = await _client.PutAsync(url, data);
        Assert.True(response.IsSuccessStatusCode, $"PutUserAndNonExistingAppUserSectors failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
        var responseContent = await response.Content.ReadAsStringAsync();
        var responseSectorIdList = JsonSerializer.Deserialize<List<Guid>>(responseContent);
        Assert.True(correctIdList.SequenceEqual(responseSectorIdList!), "The sectorIdList does not match the response.");
    }
}