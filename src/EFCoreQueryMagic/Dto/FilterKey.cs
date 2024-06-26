﻿using EFCoreQueryMagic.Enums;

namespace EFCoreQueryMagic.Dto;

internal record FilterKey
{
    public Type TargetType = null!;
    public string SourcePropertyName = null!;
    public string TargetPropertyName = null!;
    public Type TargetPropertyType = null!;
    public ComparisonType ComparisonType;
    public Type SourceType { get; set; }
    public Type SourcePropertyType { get; set; }

    public string ErrorMessage() => $"Comparison {ComparisonType} not supported for type {TargetType}";
}