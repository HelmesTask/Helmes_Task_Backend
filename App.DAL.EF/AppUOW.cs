using App.Contracts.DAL;
using App.Contracts.DAL.Repositories;
using App.DAL.EF.Repositories;
using AutoMapper;
using Base.DAL.EF;

namespace App.DAL.EF;

public class AppUOW : BaseUnitOfWork<AppDbContext>, IAppUnitOfWork
{
    private readonly IMapper _mapper;

    public AppUOW(AppDbContext dbContext, IMapper mapper) : base(dbContext)
    {
        _mapper = mapper;

    }

    private IAppUserRepository? _appUserRepository;
    public IAppUserRepository AppUserRepository =>
        _appUserRepository ??= new AppUserRepository(UowDbContext, _mapper);

    private ISectorRepository? _sectorRepository;
    
    public ISectorRepository SectorRepository =>
        _sectorRepository ??= new SectorRepository(UowDbContext, _mapper);
    
    private IAppUserSectorRepository? _appUserSectorRepository;
    public IAppUserSectorRepository AppUserSectorRepository =>
        _appUserSectorRepository ??= new AppUserSectorRepository(UowDbContext, _mapper);
}