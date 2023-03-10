using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.eShopWeb.ApplicationCore.Specifications;
namespace Microsoft.eShopWeb.Web.ViewModels;

public class CatalogIndexViewModel
{
    public List<CatalogItemViewModel>? CatalogItems { get; set; }
    public List<SelectListItem>? Brands { get; set; }
    public List<SelectListItem>? Types { get; set; }
    public int? BrandFilterApplied { get; set; }
    public int? TypesFilterApplied { get; set; }
    public CatalogItemSorting? SortingApplied { get; set; }
    public PaginationInfoViewModel? PaginationInfo { get; set; }
}
