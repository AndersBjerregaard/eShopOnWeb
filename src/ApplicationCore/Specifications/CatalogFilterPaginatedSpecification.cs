using Ardalis.Specification;
using Microsoft.eShopWeb.ApplicationCore.Entities;

namespace Microsoft.eShopWeb.ApplicationCore.Specifications;

public class CatalogFilterPaginatedSpecification : Specification<CatalogItem>
{
    public CatalogFilterPaginatedSpecification(int skip, int take, int? brandId, int? typeId, CatalogItemSorting sort = CatalogItemSorting.None)
        : base()
    {
        if (take == 0)
        {
            take = int.MaxValue;
        }
        base.Query
            .Where(i => (!brandId.HasValue || i.CatalogBrandId == brandId) &&
            (!typeId.HasValue || i.CatalogTypeId == typeId)).OrderBy(SortCatalogItems(sort))
            .Skip(skip).Take(take);

        static System.Linq.Expressions.Expression<System.Func<CatalogItem, object?>> SortCatalogItems(CatalogItemSorting sort)
        {
            switch (sort)
            {
                case CatalogItemSorting.Price:
                    return x => x.Price;
                case CatalogItemSorting.Brand:
                    return x => x.CatalogBrandId;
                case CatalogItemSorting.Name:
                    return x => x.Name;
                default:
                    return x => x.Id;
            }
        }
    }
}
