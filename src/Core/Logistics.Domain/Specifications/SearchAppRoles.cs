﻿namespace Logistics.Domain.Specifications;

public class SearchAppRoles : BaseSpecification<AppRole>
{
    public SearchAppRoles(string? search)
    {
        if (string.IsNullOrEmpty(search))
            return;
        
        Criteria = i =>
            (!string.IsNullOrEmpty(i.Name) &&
             i.Name.Contains(search)) ||

            (!string.IsNullOrEmpty(i.DisplayName) &&
             i.DisplayName.Contains(search));
    }
}