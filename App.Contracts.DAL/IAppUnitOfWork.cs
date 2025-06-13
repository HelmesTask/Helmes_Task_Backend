using App.Contracts.DAL.Repositories;
using Base.Contracts.DAL;

namespace App.Contracts.DAL;

public interface IAppUnitOfWork : IUnitOfWork
{
    IAppUserRepository AppUserRepository { get; }
    ISectorRepository SectorRepository { get; }
}