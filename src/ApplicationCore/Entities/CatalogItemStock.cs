using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.ApplicationCore.Entities;

public class CatalogItemStock : BaseEntity, IAggregateRoot
{
    public int Stock { get; private set; }
    public CatalogItem? CatalogItem { get; private set; }
    public int CatalogItemId { get; private set; }

    public CatalogItemStock(int stock,
                            int catalogItemId)
    {
        Stock = stock;
        CatalogItemId = catalogItemId;
    }
}