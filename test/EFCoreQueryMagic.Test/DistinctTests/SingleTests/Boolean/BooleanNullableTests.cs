using EFCoreQueryMagic.Dto.Public;
using EFCoreQueryMagic.Extensions;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.Infrastructure;
using FluentAssertions;

namespace EFCoreQueryMagic.Test.DistinctTests.SingleTests.Boolean;

[Collection("Database collection")]
public class BooleanNullableTests(DatabaseFixture fixture)
{
    private readonly TestDbContext _context = fixture.Context;

    [Fact]
    public async Task TestDistinctColumnValuesAsync()
    {
        var set = _context.Orders;

        var query = set
            .Select(x => x.Returned as object)
            .Distinct()
            .OrderBy(x => x == null ? 0 : 1)
            .ThenBy(x => x)
            .Skip(0).Take(20).ToList();
        
        var request = new ColumnDistinctValueQueryRequest
        {
            Page = 1,
            PageSize = 20,
            ColumnName = nameof(OrderFilter.Returned)
        };

        var result = await set.ColumnDistinctValuesAsync(request);
        
        query.Should().Equal(result.Values);
    }
}