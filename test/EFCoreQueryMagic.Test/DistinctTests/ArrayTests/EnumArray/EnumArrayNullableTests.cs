using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Enums;
using EFCoreQueryMagic.Extensions;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.Enums;
using EFCoreQueryMagic.Test.Infrastructure;
using FluentAssertions;

namespace EFCoreQueryMagic.Test.DistinctTests.ArrayTests.EnumArray;

[Collection("Database collection")]
public class EnumArrayNullableTests(DatabaseFixture fixture)
{
    private readonly TestDbContext _context = fixture.Context;

    [Fact]
    public void TestDistinctColumnValuesAsync()
    {
        var set = _context.Customers;

        var query = set
            .Select(x => x.Statuses)
            .AsEnumerable()
            .SelectMany(x => x ?? [])
            .Select(x => x as object).ToList()
            .Distinct()
            .ToList();

        var list = new List<object?>();
        
        var nullable = set.Select(x => x.Statuses)
            .Any(x => x == null);
        if (nullable)
        {
            list.Add(null);
            list.AddRange(query);
        }
        
        var qString = new GetDataRequest();

        var result = set.DistinctColumnValuesAsync(qString.Filters, nameof(CustomerFilter.Statuses), 20, 1).Result;

        list.Should().Equal(result.Values);
    }

    [Fact]
    public void TestDistinctColumnValuesAsync_String()
    {
        var set = _context.Customers;

        var query = set
            .Where(x => x.Statuses.Contains(CustomerStatus.Active))
            .Select(x => x.Statuses)
            .AsEnumerable()
            .SelectMany(x => x ?? [])
            .Select(x => x as object).ToList()
            .OrderBy(x => (int)x)
            .Distinct()
            .ToList();

        var qString = GetDataRequest.FromString(new GetDataRequest
        {
            Filters =
            [
                new FilterDto
                {
                    Values = [CustomerStatus.Active.ToString()],
                    ComparisonType = ComparisonType.In,
                    PropertyName = nameof(CustomerFilter.Statuses)
                }
            ]
        }.ToString());

        var result = set.DistinctColumnValues(qString.Filters, nameof(CustomerFilter.Statuses), 20, 1);

        query.Should().Equal(result.Values);
    }

    [Fact]
    public void TestDistinctColumnValuesAsync_Number()
    {
        var set = _context.Customers;

        var query = set
            .Where(x => x.Statuses.Contains(CustomerStatus.Active))
            .Select(x => x.Statuses)
            .AsEnumerable()
            .SelectMany(x => x ?? [])
            .Select(x => x as object).ToList()
            .OrderBy(x => (int)x)
            .Distinct()
            .ToList();

        var qString = GetDataRequest.FromString(new GetDataRequest
        {
            Filters =
            [
                new FilterDto
                {
                    Values = [(int)CustomerStatus.Active],
                    ComparisonType = ComparisonType.In,
                    PropertyName = nameof(CustomerFilter.Statuses)
                }
            ]
        }.ToString());

        var result = set.DistinctColumnValues(qString.Filters, nameof(CustomerFilter.Statuses), 20, 1);

        query.Should().Equal(result.Values);
    }


    [Fact]
    public void TestDistinctColumnValues()
    {
        var set = _context.Customers;

        var query = set
            .Select(x => x.Statuses)
            .AsEnumerable()
            .SelectMany(x => x ?? [])
            .Select(x => x as object).ToList()
            .Distinct()
            .ToList();

        var list = new List<object?>();
        
        var nullable = set.Select(x => x.Statuses)
            .Any(x => x == null);
        if (nullable)
        {
            list.Add(null);
            list.AddRange(query);
        }

        var qString = new GetDataRequest();

        var result = set.DistinctColumnValues(qString.Filters, nameof(CustomerFilter.Statuses), 20, 1);

        list.Should().Equal(result.Values);
    }

    [Fact]
    public void TestDistinctColumnValues_String()
    {
        var set = _context.Customers;

        var query = set
            .Where(x => x.Statuses.Contains(CustomerStatus.Active))
            .Select(x => x.Statuses)
            .AsEnumerable()
            .SelectMany(x => x ?? [])
            .Select(x => x as object).ToList()
            .OrderBy(x => (int)x)
            .Distinct()
            .ToList();

        var qString = GetDataRequest.FromString(new GetDataRequest
        {
            Filters =
            [
                new FilterDto
                {
                    Values = [CustomerStatus.Active.ToString()],
                    ComparisonType = ComparisonType.In,
                    PropertyName = nameof(CustomerFilter.Statuses)
                }
            ]
        }.ToString());

        var result = set.DistinctColumnValues(qString.Filters, nameof(CustomerFilter.Statuses), 20, 1);

        query.Should().Equal(result.Values);
    }

    [Fact]
    public void TestDistinctColumnValues_Number()
    {
        var set = _context.Customers;

        var query = set
            .Where(x => x.Statuses.Contains(CustomerStatus.Active))
            .Select(x => x.Statuses)
            .AsEnumerable()
            .SelectMany(x => x ?? [])
            .Select(x => x as object).ToList()
            .OrderBy(x => (int)x)
            .Distinct()
            .ToList();

        var qString = GetDataRequest.FromString(new GetDataRequest
        {
            Filters =
            [
                new FilterDto
                {
                    Values = [(int)CustomerStatus.Active],
                    ComparisonType = ComparisonType.In,
                    PropertyName = nameof(CustomerFilter.Statuses)
                }
            ]
        }.ToString());

        var result = set.DistinctColumnValues(qString.Filters, nameof(CustomerFilter.Statuses), 20, 1);

        query.Should().Equal(result.Values);
    }
}