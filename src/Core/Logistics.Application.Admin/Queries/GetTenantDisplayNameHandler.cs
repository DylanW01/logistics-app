﻿namespace Logistics.Application.Admin.Queries;

internal class GetTenantDisplayNameHandler : RequestHandlerBase<GetTenantDisplayNameQuery, ResponseResult<TenantDto>>
{
    private readonly IMainRepository _repository;

    public GetTenantDisplayNameHandler(IMainRepository repository)
    {
        _repository = repository;
    }

    protected override async Task<ResponseResult<TenantDto>> HandleValidated(GetTenantDisplayNameQuery request, CancellationToken cancellationToken)
    {
        var tenantEntity = await _repository.GetAsync<Tenant>(i => i.Id == request.Id || i.Name == request.Name);

        if (tenantEntity == null)
            return ResponseResult<TenantDto>.CreateError("Could not find the specified tenant");

        var tenant = new TenantDto
        {
            Id = tenantEntity.Id,
            Name = tenantEntity.Name,
            DisplayName = tenantEntity.DisplayName
        };
        return ResponseResult<TenantDto>.CreateSuccess(tenant);
    }
}
