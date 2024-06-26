﻿using Microsoft.EntityFrameworkCore;

namespace EFCoreQueryMagic.Converters;

public class DirectConverter : IConverter<object?, object?>
{
    public DbContext Context { get; set; }

    public object? ConvertTo(object? from)
    {
        return from;
    }

    public object? ConvertFrom(object? to)
    {
        return to;
    }
}