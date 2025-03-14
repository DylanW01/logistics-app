﻿using Logistics.Infrastructure.EF.Interceptors;

namespace Logistics.Infrastructure.EF.Data;

public class TenantDbContext : DbContext
{
    private readonly AuditableEntitySaveChangesInterceptor? _auditableEntitySaveChangesInterceptor;
    private readonly DispatchDomainEventsInterceptor? _dispatchDomainEventsInterceptor;
    private readonly ITenantService? _tenantService;
    private readonly string _connectionString;

    public TenantDbContext(
        TenantDbContextOptions options, 
        ITenantService? tenantService,
        AuditableEntitySaveChangesInterceptor? auditableEntitySaveChangesInterceptor,
        DispatchDomainEventsInterceptor? dispatchDomainEventsInterceptor)
    {
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
        _dispatchDomainEventsInterceptor = dispatchDomainEventsInterceptor;
        _tenantService = tenantService;
        _connectionString = options.ConnectionString ?? ConnectionStrings.LocalDefaultTenant;
    }

    public Tenant? CurrentTenant => _tenantService?.GetTenant();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (_auditableEntitySaveChangesInterceptor is not null)
        {
            options.AddInterceptors(_auditableEntitySaveChangesInterceptor);
        }
        if (_dispatchDomainEventsInterceptor is not null)
        {
            options.AddInterceptors(_dispatchDomainEventsInterceptor);
        }

        if (!options.IsConfigured)
        {
            var connectionString = _tenantService?.GetConnectionString() ?? _connectionString;
            DbContextHelpers.ConfigureSqlServer(connectionString, options);
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<TenantRoleClaim>().ToTable("RoleClaims");

        builder.Entity<TenantRole>(entity =>
        {
            entity.ToTable("Roles");
            entity.HasMany(m => m.Claims)
                .WithOne(m => m.Role)
                .HasForeignKey(m => m.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Employee>(entity =>
        {
            entity.ToTable("Employees");
            entity.HasMany(i => i.Roles)
                .WithMany(i => i.Employees)
                .UsingEntity("EmployeeRoles");
        });

        builder.Entity<Truck>(entity =>
        {
            entity.ToTable("Trucks");

            entity.HasOne(m => m.Driver)
                .WithOne()
                .HasForeignKey<Truck>(m => m.DriverId);

            entity.HasMany(m => m.Loads)
                .WithOne(m => m.AssignedTruck)
                .HasForeignKey(m => m.AssignedTruckId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<Load>(entity =>
        {
            entity.ToTable("Loads");
            //entity.OwnsOne(m => m.SourceAddress);
            //entity.OwnsOne(m => m.DestinationAddress);
            entity.HasIndex(m => m.RefId).IsUnique();

            entity.HasOne(m => m.AssignedDispatcher)
                .WithMany(m => m.DispatchedLoads)
                .HasForeignKey(m => m.AssignedDispatcherId);
                //.OnDelete(DeleteBehavior.SetNull);
            
            entity.HasOne(m => m.AssignedDriver)
                .WithMany(m => m.DeliveredLoads)
                .HasForeignKey(m => m.AssignedDriverId);
                //.OnDelete(DeleteBehavior.SetNull);
        });
    }
}
