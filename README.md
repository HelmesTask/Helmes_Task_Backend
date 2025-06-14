## TASK
~~~bash
Create database with AppUser and Sectors
Create Sectors with self-reference to parent sectors
Create users with chosen Sectors
Create Apicontrollers to manage AppUser Sectors
- User can save chosen Sectors
- User can load existing Sectors
- Sectors should be loadable in correct hierachy
~~~

## APICONTROLLERS
~~~bash
dotnet aspnet-codegenerator controller -name SectorController -m  App.Domain.Sector  -dc AppDbContext -outDir ApiControllers -api --useAsyncActions -f
dotnet aspnet-codegenerator controller -name AppUserController -m  App.Domain.AppUser  -dc AppDbContext -outDir ApiControllers -api --useAsyncActions -f
~~~
