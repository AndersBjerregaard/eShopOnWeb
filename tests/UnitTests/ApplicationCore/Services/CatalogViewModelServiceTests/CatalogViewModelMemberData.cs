namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Services.CatalogViewModelServiceTests;

public class CatalogViewModelMemberData
{
    public static IEnumerable<object[]> GetCatalogViewModelData()
    {
        List<object[]> objects = new List<object[]>();

        yield return new object[] 
        {
            0,  // page index
            10  // items per page
        };

        yield return new object[] 
        {
            1,  // page index
            10  // items per page
        };
    }
}