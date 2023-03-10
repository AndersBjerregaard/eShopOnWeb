using System.Collections;
using BlazorShared.Models;
using Microsoft.eShopWeb.ApplicationCore.Specifications;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Services.CatalogViewModelServiceTests;

public class CatalogViewModelClassData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[]
        {
            0, // Page Index
            10, // Items per page
            CatalogItemSorting.None // Sort
        };

        yield return new object[]
        {
            1, // Page Index
            10, // Items per page
            CatalogItemSorting.None // Sort
        };

        yield return new object[]
        {
            0, // Page Index
            10, // Items per page
            CatalogItemSorting.Name // Sort
        };

        yield return new object[]
        {
            1, // Page Index
            10, // Items per page
            CatalogItemSorting.Name // Sort
        };

        yield return new object[]
        {
            0, // Page Index
            10, // Items per page
            CatalogItemSorting.Price // Sort
        };

        yield return new object[]
        {
            1, // Page Index
            10, // Items per page
            CatalogItemSorting.Price // Sort
        };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}