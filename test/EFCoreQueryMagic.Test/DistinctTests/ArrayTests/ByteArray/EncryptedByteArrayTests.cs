using EFCoreQueryMagic.Converters;
using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Extensions;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.Infrastructure;
using FluentAssertions;
using Pandatech.Crypto;

namespace EFCoreQueryMagic.Test.DistinctTests.ArrayTests.ByteArray;

[Collection("Database collection")]
public class EncryptedByteArrayTests(DatabaseFixture fixture)
{
    private readonly TestDbContext _context = fixture.Context;
    private readonly Aes256 _aes256 = fixture.Aes256;
    
    [Fact]
    public void TestDistinctColumnValuesAsync()
    {
        var set = _context.Customers;

        EncryptedConverter.Aes256 = _aes256;
        var converter = new EncryptedConverter();

        var query = set
            .Select(x => x.FirstName).ToList()
            .Select(x => converter.ConvertFrom(x) as object)
            .Skip(0).Take(20).ToList();
        
        var qString = new GetDataRequest();

        var result = set.DistinctColumnValuesAsync(qString.Filters, nameof(CustomerFilter.FirstName), 20, 1).Result;
        
        query.Should().Equal(result.Values);
    }
    
    [Fact]
    public void TestDistinctColumnValues()
    {
        var set = _context.Customers;

        EncryptedConverter.Aes256 = _aes256;
        var converter = new EncryptedConverter();

        var query = set
            .Select(x => x.FirstName).ToList()
            .Select(x => converter.ConvertFrom(x) as object)
            .Skip(0).Take(20).ToList();
        
        var qString = new GetDataRequest();

        var result = set.DistinctColumnValues(qString.Filters, nameof(CustomerFilter.FirstName), 20, 1);
        
        query.Should().Equal(result.Values);
    }
}