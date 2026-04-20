using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using secondhand_car_backend.Models.Context;
//using secondhand_car_backend.Entities;
using secondhand_car_backend.Models.Context;
using secondhand_car_backend.Models.Entities;
using secondhand_car_backend.Models.Repositories;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace secondhand_car_backend.Models.UnitsOfWork
{
    public sealed class MeigemnUnitOfWork : IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MeigemnUnitOfWork>  _logger;
        private readonly MeigemnDbContext _context;

        // Repositorios
        private MeigemnRepository<IdentityUser>? _usersRepository;
        private MeigemnRepository<CarPart>? _carPartsRepository;
        private MeigemnRepository<PartCriterion>? _partCriterionsRepository;

        // Propiedad de acceso a la base de datos
        public Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade Database => _context.Database;

        // Constructor
        public MeigemnUnitOfWork(MeigemnDbContext context, IServiceProvider serviceProvider, ILogger<MeigemnUnitOfWork> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // --- Repositorios ---
        public MeigemnRepository<IdentityUser> Users => _usersRepository ??= GetRepository<IdentityUser>();
        public MeigemnRepository<CarPart> CarParts => _carPartsRepository ??= GetRepository<CarPart>();
        public MeigemnRepository<PartCriterion> PartCriterions => _partCriterionsRepository ??= GetRepository<PartCriterion>();

        // --- Métodos de Transacción y Guardado ---

        /// <summary>
        /// Inicia una nueva transacción de base de datos especificando el Nivel de Aislamiento.
        /// </summary>

        public Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel)
        {
            // Usamos CancellationToken.None para satisfacer la sobrecarga de tres argumentos requerida por EF Core:
            // BeginTransactionAsync(IsolationLevel, CancellationToken)
            return _context.Database.BeginTransactionAsync(isolationLevel, CancellationToken.None);
        }

        public async Task Complete()
        {
            await _context.SaveChangesAsync();
        }

        // --- Método Auxiliar ---
        private MeigemnRepository<T> GetRepository<T>() where T : class
        {
            return _serviceProvider.GetRequiredService(typeof(MeigemnRepository<T>)) as MeigemnRepository<T>;
        }

        // --- Implementación IDisposable ---
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}