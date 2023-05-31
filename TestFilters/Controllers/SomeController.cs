using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PandaTech.IEnumerableFilters;
using PandaTech.IEnumerableFilters.Dto;

namespace TestFilters.Controllers;

[ApiController]
[Route("[controller]")]
public class SomeController : ControllerBase
{
    private readonly Context _context;
    private readonly FilterProvider _filterProvider;

    public SomeController(Context context)
    {
        _context = context;
        _filterProvider = new FilterProvider();
        _filterProvider.MapApiToContext(typeof(Person), typeof(Person));
    }

    [HttpPost("[action]")]
    public IActionResult PopulateDb()
    {
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();

        var start = DateTime.Now;

        const int count = 10_000;
        Console.Clear();

        var optionBuilder = new DbContextOptionsBuilder<Context>();
        optionBuilder.UseNpgsql("Server=127.0.0.1;Database=xyz;Username=postgres;Password=example");

        var tasks = new List<Task>();
        var catId = 1;
        var context = new Context(optionBuilder.Options);
        for (var i = 1; i <= count; i++)
        {
            var catCount = Random.Shared.Next(0, 4);
            var person = new Person
            {
                Id = i,
                Name = NameProvider.GetRandomName(),
                Age = Random.Shared.Next(15, 90),
                Cats = new List<Cat>(),
                Address = NameProvider.GetRandomAddress(),
                Email = "test@TEST.am",
                Money = Random.Shared.NextDouble() * 100000,
                Phone = "+37412345678",
                Surname = NameProvider.GetRandomName(),
                BirthDate = new DateTime(2000, 1, 1).AddDays(Random.Shared.Next(0, 10000)).ToUniversalTime(),
                IsHappy = Random.Shared.Next(0, 1) == 1,
                IsMarried = Random.Shared.Next(0, 3) == 0,
                IsWorking = Random.Shared.Next(0, 5) != 1
            };

            for (var j = 0; j < catCount; j++)
            {
                person.Cats.Add(new Cat
                {
                    Id = catId++,
                    Name = NameProvider.GetRandomName(),
                    Age = Random.Shared.Next(1, 20),
                });
            }

            context.Add(person);

            if (i % 100 != 0) continue;
            {
                Console.SetCursorPosition(0, 0);
                Console.WriteLine(i);
                //context = new Context(optionBuilder.Options);
            }
        }

        tasks.Add(context.SaveChangesAsync());

        Task.WaitAll(tasks.ToArray());

        Console.WriteLine(DateTime.Now - start);

        return Ok();
    }


    [HttpPost($"[action]")]
    public IActionResult AddPhrase(string phrase)
    {
        _context.Phrases.Add(new Phrase { Text = phrase });
        _context.SaveChanges();
        return Ok();
    }

    [HttpGet($"[action]")]
    public List<Phrase> GetPhrases()
    {
        return _context.Phrases.ToList();
    }

    [HttpGet("GetFilters")]
    public List<FilterInfo> GetFilters(string tableName)
    {
        return _filterProvider.GetFilters(tableName);
    }

    [HttpGet("GetTables")]
    public List<string> GetTables()
    {
        return _filterProvider.GetTables();
    }

    [HttpGet("DistinctColumnValues")]
    public List<string> DistinctColumnValues([FromQuery] string filtersString, string tableName ,string columnName, int page,
        int pageSize)
    {
        var request = JsonSerializer.Deserialize<GetDataRequest>(filtersString);

        if (request == null)
            return new List<string>();
        return _context.Persons.DistinctColumnValues(request.Filters, columnName, page: page,
            pageSize: pageSize,
            totalCount: out _);
    }

    [HttpPost("FilterDto")]
    public string FilterDto()
    {
        var request = new GetDataRequest()
        {
            Aggregates = new List<AggregateDto>()
            {
                new()
                {
                    AggregateType = AggregateType.Max,
                    PropertyName = "Age"
                }
            },
            Filters = new List<FilterDto>(),
        };

        return request.ToString();
    }

    [HttpGet("GetPersons")]
    public FilteredDataResult<Person> GetPersons([FromQuery] string? filtersString, int page, int pageSize)
    {
        var now = DateTime.Now;
        var filters = JsonSerializer.Deserialize<GetDataRequest>(filtersString ?? "");

        if (filters == null)
        {
            return new FilteredDataResult<Person>();
        }

        var query = _context.Persons.ApplyFilters(filters.Filters).ApplyOrdering(filters.Order);

        var response = new FilteredDataResult<Person>
        {
            Data = query.Include(p => p.Cats).Skip((page - 1) * pageSize).Take(pageSize)
                .ToList(),
            TotalCount = query.Count(),
            Aggregates = query.GetAggregates(filters.Aggregates ?? new List<AggregateDto>())
        };
        return response;
    }
}

public class Phrase
{
    public string Text { get; set; } = null!;
    [Key]
    public int Id { get; set; }
}