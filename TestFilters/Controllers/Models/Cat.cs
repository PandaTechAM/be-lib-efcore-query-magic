﻿using Microsoft.EntityFrameworkCore;

namespace TestFilters.Controllers.Models;

[PrimaryKey(nameof(Id))]
public class Cat
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public int Age { get; set; }

    public override string ToString() => $"Cat {Name} {Age} {Id}";
    
}