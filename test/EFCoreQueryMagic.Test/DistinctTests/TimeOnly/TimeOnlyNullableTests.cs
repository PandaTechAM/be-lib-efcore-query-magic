using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Extensions;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.Infrastructure;
using FluentAssertions;

namespace EFCoreQueryMagic.Test.DistinctTests.TimeOnly;

[Collection("Database collection")]
public class TimeOnlyNullableTests(DatabaseFixture fixture)
{
    private readonly TestDbContext _context = fixture.Context;
    
    [Fact]
    public void TestDistinctColumnValuesAsync()
    {
        var set = _context.Items;

        var query = set
            .Select(x => x.TimeOnlyNullable as object)
            .Distinct().OrderByDescending(x => x).ThenBy(x => x)
            .Skip(0).Take(20).ToList();
        
        var qString = new GetDataRequest();

        var result = set.DistinctColumnValuesAsync(qString.Filters, nameof(ItemFilter.TimeOnlyNullable), 20, 1).Result;
        
        query.Should().Equal(result.Values);
    }
    
    [Fact]
    public void TestDistinctColumnValues()
    {
        var set = _context.Items;

        var query = set
            .Select(x => x.TimeOnlyNullable as object)
            .Distinct().OrderByDescending(x => x).ThenBy(x => x)
            .Skip(0).Take(20).ToList();
        
        var qString = new GetDataRequest();

        var result = set.DistinctColumnValues(qString.Filters, nameof(ItemFilter.TimeOnlyNullable), 20, 1);
        
        query.Should().Equal(result.Values);
    }
}