using Xunit;
using Moq;
using Microsoft.eShopWeb.Web.Services;
using Microsoft.Extensions.Logging;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Specifications;
using Microsoft.eShopWeb.Web.ViewModels;
using Ardalis.Specification;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Services.CatalogViewModelServiceTests;

public class GetSortedCatalogItems
{
    ICatalogViewModelService _catalogViewModelService;
    List<CatalogItem> _mockCatalogItems;
    Random _random;

    public GetSortedCatalogItems()
    {
        var loggerFactory = new Mock<ILoggerFactory>();
        
        var mockLogger = new Mock<ILogger<CatalogViewModelService>>();

        mockLogger.Setup(
            m => m.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<object>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<object, Exception?, string>>()));

        loggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(() => mockLogger.Object);

        // Mock Catalog Items
        _random = new Random(42069);
        
        _mockCatalogItems = new List<CatalogItem>();

        for (int i = 15; i > 0; i--)
        {
            _mockCatalogItems.Add(
            new CatalogItem(_random.Next(),
                            catalogBrandId: 10 * i,
                            _random.NextDouble().ToString(),
                            name: i.ToString(),
                            price: (decimal)i,
                            _random.NextDouble().ToString())
            );
        }

        
        // Mock itemRepository methods
        var itemRepository = new Mock<IRepository<CatalogItem>>();

        itemRepository.Setup(
            i => i.ListAsync(
                It.IsAny<ISpecification<CatalogItem>>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync((ISpecification<CatalogItem> specification, CancellationToken token) => {
            if (!specification.Skip.HasValue || !specification.Take.HasValue) throw new ArgumentNullException();
            return _mockCatalogItems.OrderBy(SortCatalogItems(specification.OrderExpressions))
            .Skip(specification.Skip.Value)
            .Take(specification.Take.Value)
            .ToList();
        });

        // itemRepository.Setup(
        //     i => i.ListAsync(
        //         It.IsAny<ISpecification<CatalogItem>>(),
        //         It.IsAny<CancellationToken>()
        //     )
        // ).ReturnsAsync((ISpecification<CatalogItem> specification, CancellationToken token) => {
        //     return _mockCatalogItems;
        // });

        itemRepository.Setup(
            i => i.CountAsync(
                It.IsAny<ISpecification<CatalogItem>>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(_mockCatalogItems.Count);

        var brandRepository = new Mock<IRepository<CatalogBrand>>();

        brandRepository.Setup(
            i => i.ListAsync(
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(new List<CatalogBrand>());

        var typeRepository = new Mock<IRepository<CatalogType>>();

        typeRepository.Setup(
            i => i.ListAsync(
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(new List<CatalogType>());

        Mock<IUriComposer> uriComposer = new Mock<IUriComposer>();

        uriComposer.Setup(
            i => i.ComposePicUri(
                It.IsAny<string>()
            )
        ).Returns(string.Empty);

        _catalogViewModelService = new CatalogViewModelService(loggerFactory.Object,
                                                               itemRepository.Object,
                                                               brandRepository.Object,
                                                               typeRepository.Object,
                                                               uriComposer.Object);
    }

    private Func<CatalogItem, object> SortCatalogItems(IEnumerable<OrderExpressionInfo<CatalogItem>> orderExpressions)
    {
        // The OrderExpressionInfo<CatalogItem> Contains information about a property
        var y = orderExpressions.First().KeySelector.Body.ToString();
        if (y == "x") return x => x.Id;
        string propertyAsString = y;
        if (y.Contains("Convert")) propertyAsString = y.Split('(')[1].Split(',')[0];
        switch (propertyAsString)
        {
            case "x.Price":
                return x => x.Price;
            case "x.CatalogBrandId":
                return x => x.CatalogBrandId;
            case "x.Name":
                return x => x.Name;
            default:
                return x => x.Id;
        }
    }

    [Fact]
    public async Task GetSortedCatalogItemsNotNull()
    {
        var vm = await _catalogViewModelService.GetCatalogItems(0, 1, brandId: null, typeId: null, CatalogItemSorting.None);

        Assert.NotNull(vm);
        Assert.IsType<CatalogIndexViewModel>(vm);
        Assert.NotNull(vm.CatalogItems);
        Assert.NotEmpty(vm.CatalogItems);
    }

    [Theory, ClassData(typeof(CatalogViewModelClassData))]
    public async Task GetSortedCatalogItemsTest(int pageIndex, int itemsPage, CatalogItemSorting sort)
    {
        // Arrange
        List<CatalogItem> sortedCatalogItems = new List<CatalogItem>();

        switch (sort)
        {
            case CatalogItemSorting.Name:
                sortedCatalogItems = _mockCatalogItems.OrderBy(x => x.Name)
                    .ToList();
                break;
            case CatalogItemSorting.Price:
                sortedCatalogItems = _mockCatalogItems.OrderBy(x => x.Price)
                    .ToList();
                break;
            default:
                sortedCatalogItems = _mockCatalogItems;
                break;
        }

        List<CatalogItemViewModel> sortedViewModelCatalogItems = sortedCatalogItems.Select(i => new CatalogItemViewModel()
            {
                Id = i.Id,
                Name = i.Name,
                PictureUri = string.Empty,
                Price = i.Price
            })
            .Skip(pageIndex * itemsPage).Take(itemsPage).ToList();

        // Act
        var vm = await _catalogViewModelService.GetCatalogItems(pageIndex, itemsPage, brandId: null, typeId: null, sort);

        // Assert
        List<CatalogItemViewModel>? catalogItems = vm.CatalogItems;

        Assert.NotNull(catalogItems);
        Assert.NotEmpty(catalogItems);
        Assert.Equal(sortedViewModelCatalogItems.Select(x => x.Name), catalogItems.Select(x => x.Name));
    }

    [Theory]
    [InlineData(2, 10)]
    [InlineData(10, 10)]
    public async Task GetSortedCatalogItemsFailTest(int pageIndex, int itemsPage)
    {
        // Act
        var vm = await _catalogViewModelService.GetCatalogItems(pageIndex, itemsPage, brandId: null, typeId: null, sort: CatalogItemSorting.None);

        // Assert
        List<CatalogItemViewModel>? catalogItems = vm.CatalogItems;

        Assert.NotNull(catalogItems);
        Assert.Empty(catalogItems);
    }

    [Theory, MemberData(nameof(CatalogViewModelMemberData.GetCatalogViewModelData), MemberType = typeof(CatalogViewModelMemberData))]
    public async Task GetSortedCatalogItemsByBrand(int pageIndex, int itemsPage)
    {
        // Arrange
        List<CatalogItem> sortedCatalogItems = _mockCatalogItems.OrderBy(x => x.CatalogBrandId)
            .ToList();

        List<CatalogItemViewModel> sortedViewModelCatalogItems = sortedCatalogItems.Select(i => new CatalogItemViewModel()
        {
            Id = i.Id,
            Name = i.Name,
            PictureUri = string.Empty,
            Price = i.Price
        })
        .Skip(pageIndex * itemsPage).Take(itemsPage).ToList();

        // Act
        var vm = await _catalogViewModelService.GetCatalogItems(pageIndex, itemsPage, brandId: null, typeId: null, sort: CatalogItemSorting.Brand);

        // Assert
        List<CatalogItemViewModel>? catalogItems = vm.CatalogItems;

        Assert.NotNull(catalogItems);
        Assert.NotEmpty(catalogItems);
        Assert.Equal(sortedViewModelCatalogItems.Select(x => x.Name), catalogItems.Select(x => x.Name));
    }
}