using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Dto.Public;
using EFCoreQueryMagic.Enums;
using EFCoreQueryMagic.Extensions;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.FilterTests.SingleTypes;
using EFCoreQueryMagic.Test.Infrastructure;
using FluentAssertions;

namespace EFCoreQueryMagic.Test.FilterTests.ListTests;

[Collection("Database collection")]
public class ListStringTest(DatabaseFixture fixture) : ITypedTests<string>
{
    private readonly TestDbContext _context = fixture.Context;

    [Fact]
    public void TestEmptyValues()
    {
        var set = _context.Items;

        var query = set
            .Where(x => false).ToList();

        var request = new FilterQuery
        {
            PropertyName = nameof(ItemFilter.ListString),
            ComparisonType = ComparisonType.Contains,
            Values = []
        };

        var qString = new MagicQuery([request], null);

        var result = set.FilterAndOrder(qString.ToString()).ToList();

        query.Should().Equal(result);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    public void TestNotNullable(string value)
    {
        var set = _context.Items;

        var query = set
            .Where(x => x.ListString.Contains(value))
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        var request = new FilterQuery
        {
            PropertyName = nameof(ItemFilter.ListString),
            ComparisonType = ComparisonType.Contains,
            Values = [value]
        };

        var qString = new MagicQuery([request], null);

        var result = set.FilterAndOrder(qString.ToString()).ToList();
        
        query.Select(x=>x.Id).OrderBy(x=>x)
            .Should()
            .Equal(result.Select(x=>x.Id).OrderBy(x=>x));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("5")]
    public void TestNullable(string? value)
    {
        var set = _context.Items;

        var query = set
            .Where(x => x.ListStringNullable.Contains(value))
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        var request = new FilterQuery
        {
            PropertyName = nameof(ItemFilter.ListStringNullable),
            ComparisonType = ComparisonType.Contains,
            Values = [value]
        };

        var qString = new MagicQuery([request], null);

        var result = set.FilterAndOrder(qString.ToString()).ToList();
        
        query.Select(x=>x.Id).OrderBy(x=>x)
            .Should()
            .Equal(result.Select(x=>x.Id).OrderBy(x=>x));
    }

    [Fact]
    public void TestNotNullableWithNullableValue()
    {
        var set = _context.Items;

        var query = set.Where(x => x.ListString == null);

        var request = new FilterQuery
        {
            PropertyName = nameof(ItemFilter.ListString),
            ComparisonType = ComparisonType.Contains,
            Values = [null]
        };

        var qString = new MagicQuery([request], null);

        var result = set.FilterAndOrder(qString.ToString()).ToList();

        query.Should().Equal(result);
    }

    public void TestEqual(string value)
    {
        throw new NotImplementedException();
    }

    public void TestNotEqual(string value)
    {
        throw new NotImplementedException();
    }

    public void TestGreaterThan(string value)
    {
        throw new NotImplementedException();
    }

    public void TestGreaterThanOrEqual(string value)
    {
        throw new NotImplementedException();
    }

    public void TestLessThan(string value)
    {
        throw new NotImplementedException();
    }

    public void TestLessThanOrEqual(string value)
    {
        throw new NotImplementedException();
    }

    public void TestContains(string value)
    {
        throw new NotImplementedException();
    }

    public void TestStartsWith(string value)
    {
        throw new NotImplementedException();
    }

    public void TestEndsWith(string value)
    {
        throw new NotImplementedException();
    }

    public void TestIn(string value)
    {
        throw new NotImplementedException();
    }

    public void TestNotIn(string value)
    {
        throw new NotImplementedException();
    }

    public void TestIsNotEmpty(string value)
    {
        throw new NotImplementedException();
    }

    public void TestIsEmpty(string value)
    {
        throw new NotImplementedException();
    }

    public void TestBetween(string value)
    {
        throw new NotImplementedException();
    }

    public void TestNotContains(string value)
    {
        throw new NotImplementedException();
    }

    public void TestHasCountEqualTo(string value)
    {
        throw new NotImplementedException();
    }

    public void TestHasCountBetween(string value)
    {
        throw new NotImplementedException();
    }

    public void TestIsTrue(string value)
    {
        throw new NotImplementedException();
    }

    public void TestIsFalse(string value)
    {
        throw new NotImplementedException();
    }
}