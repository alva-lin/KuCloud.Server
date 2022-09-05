using KuCloud.Infrastructure.Filters;

using Microsoft.AspNetCore.Mvc.Filters;

namespace KuCloud.Infrastructure.Extensions;

public static class FilterExtension
{
    public static FilterCollection AddResponseWrapperFilter(this FilterCollection filters)
    {
        filters.Add<ResponseWrapperFilter>();
        return filters;
    }

    public static FilterCollection AddModelValidFilter(this FilterCollection filter)
    {
        filter.Add<ModelValidFilter>();
        return filter;
    }
}
