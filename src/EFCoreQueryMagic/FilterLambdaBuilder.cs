﻿using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Enums;
using EFCoreQueryMagic.Exceptions;
using EFCoreQueryMagic.Extensions;

namespace EFCoreQueryMagic;

internal static class FilterLambdaBuilder
{
    internal static string BuildLambdaString(FilterKey key)
    {
        return key.TargetPropertyType switch
        {
            not null when key.TargetPropertyType == typeof(string) => BuildStringLambdaString(key),
            not null when key.TargetPropertyType == typeof(char) => BuildCharLambdaString(key),
            not null when key.TargetPropertyType == typeof(char?) => BuildCharLambdaString(key),
            not null when NumericTypes.Contains(key.TargetPropertyType) => BuildNumericLambdaString(key),
            not null when key.TargetPropertyType == typeof(bool) => BuildBoolLambdaString(key),
            not null when key.TargetPropertyType == typeof(bool?) => BuildBoolLambdaString(key),
            not null when key.TargetPropertyType == typeof(Guid) => BuildGuidLambdaString(key),
            not null when key.TargetPropertyType == typeof(Guid?) => BuildGuidLambdaString(key),
            not null when key.TargetPropertyType == typeof(DateTime) => BuildTimeBasedLambdaString(key),
            not null when key.TargetPropertyType == typeof(DateTime?) => BuildTimeBasedLambdaString(key),
            not null when key.TargetPropertyType.IsEnum ||
                          (key.TargetPropertyType.GetGenericArguments().FirstOrDefault()?.IsEnum ?? false)
                => BuildEnumLambdaString(key),
            not null when key.TargetPropertyType is { IsClass: true, IsGenericType: false, IsArray: false }
                => BuildClassLambdaString(key),
            not null when key.TargetPropertyType == typeof(DateOnly) => BuildTimeBasedLambdaString(key),
            not null when key.TargetPropertyType == typeof(DateOnly?) => BuildTimeBasedLambdaString(key),
            not null when key.TargetPropertyType == typeof(TimeOnly) => BuildTimeBasedLambdaString(key),
            not null when key.TargetPropertyType == typeof(TimeOnly?) => BuildTimeBasedLambdaString(key),
            not null when key.TargetPropertyType == typeof(TimeSpan) => BuildTimeBasedLambdaString(key),
            not null when key.TargetPropertyType == typeof(TimeSpan?) => BuildTimeBasedLambdaString(key),
            not null when key.TargetPropertyType == typeof(DateTimeOffset) => BuildTimeBasedLambdaString(key),
            not null when key.TargetPropertyType == typeof(DateTimeOffset?) => BuildTimeBasedLambdaString(key),
            // lists TODO: check for ICollection
            not null when (key.TargetPropertyType == typeof(List<>) ||
                           Nullable.GetUnderlyingType(key.TargetPropertyType) == typeof(List<>)) &&
                          key.TargetPropertyType.IsIEnumerable(typeof(string))
                => BuildListStringLambdaString(key),
            not null when key.TargetPropertyType.IsIEnumerable() && key.TargetPropertyType.GetCollectionType() != typeof(byte) => BuildListLambdaString(key),
            _ => throw new UnsupportedFilterException($"Unsupported type {key.TargetPropertyType}")
        };
    }

    private static string BuildBoolLambdaString(FilterKey key)
    {
        return key.ComparisonType switch
        {
            ComparisonType.Equal => $"{key.TargetPropertyName} == @0[0]",
            ComparisonType.NotEqual => $"{key.TargetPropertyName} != @0[0]",
            ComparisonType.IsTrue => $"{key.TargetPropertyName}",
            ComparisonType.IsFalse => $"{key.TargetPropertyName}",
            _ => throw new ComparisonNotSupportedException($"Unsupported comparison type {key.ComparisonType}")
        };
    }

    private static string BuildClassLambdaString(FilterKey key)
    {
        return key.ComparisonType switch
        {
            ComparisonType.Equal => $"{key.TargetPropertyName}.Id == @{0}[0]",
            ComparisonType.NotEqual => $"{key.TargetPropertyName}.Id != @{0}[0]",
            ComparisonType.In => $"@{0}.Contains({key.TargetPropertyName})",
            ComparisonType.NotIn => $"!@{0}.Contains({key.TargetPropertyName})",
            _ => throw new ComparisonNotSupportedException(
                $"Unsupported comparison type {key.ComparisonType} for type {key.TargetPropertyType}")
        };
    }

    private static string BuildListStringLambdaString(FilterKey key)
    {
        // Check for value types and enums and strings 
        return key.ComparisonType switch
        {
            ComparisonType.Contains => $"{key.TargetPropertyName}.Any(x => x.Contains(@{0}[0]))",
            ComparisonType.NotContains => $"!{key.TargetPropertyName}.Any(x => x.Contains(@{0}[0]))",
            ComparisonType.In => $"y => @{0}.All(x => y.{key.TargetPropertyName}.Contains(x))",
            ComparisonType.NotIn => $"y => !@{0}.All(x => y.{key.TargetPropertyName}.Contains(x))",
            _ => throw new ComparisonNotSupportedException(
                $"Unsupported comparison type {key.ComparisonType} for type {key.TargetPropertyType}")
        };
    }

    private static string BuildListLambdaString(FilterKey key)
    {
        // Check for value types and enums and strings 
        return key.ComparisonType switch
        {
            ComparisonType.Contains => $"{key.TargetPropertyName}.Any(x => @{0}.Contains(x))",
            ComparisonType.NotContains => $"!{key.TargetPropertyName}.Any(x => @{0}.Contains(x))",
            ComparisonType.In => $"y => @{0}.All(x => y.{key.TargetPropertyName}.Contains(x))",
            ComparisonType.NotIn => $"y => !@{0}.All(x => y.{key.TargetPropertyName}.Contains(x))",
            _ => throw new ComparisonNotSupportedException(
                $"Unsupported comparison type {key.ComparisonType} for type {key.TargetPropertyType}")
        };
    }

    private static string BuildGuidLambdaString(FilterKey key)
    {
        return key.ComparisonType switch
        {
            ComparisonType.Equal => $"{key.TargetPropertyName} == @0[0]",
            ComparisonType.NotEqual => $"{key.TargetPropertyName} != @0[0]",
            ComparisonType.In => $"@0.Contains({key.TargetPropertyName})",
            ComparisonType.NotIn => $"!@0.Contains({key.TargetPropertyName})",
            _ => throw new ComparisonNotSupportedException($"Unsupported comparison type {key.ComparisonType}")
        };
    }

    private static string BuildEnumLambdaString(FilterKey key)
    {
        return key.ComparisonType switch
        {
            ComparisonType.Equal => $"{key.TargetPropertyName} == @0[0]",
            ComparisonType.NotEqual => $"{key.TargetPropertyName} != @0[0]",
            ComparisonType.In => $"@0.Contains({key.TargetPropertyName})",
            ComparisonType.NotIn => $"!@0.Contains({key.TargetPropertyName})",
            _ => throw new ComparisonNotSupportedException($"Unsupported comparison type {key.ComparisonType}")
        };
    }

    private static string BuildTimeBasedLambdaString(FilterKey key)
    {
        return key.ComparisonType switch
        {
            ComparisonType.Equal => $"{key.TargetPropertyName} == @0[0]",
            ComparisonType.NotEqual => $"{key.TargetPropertyName} != @0[0]",
            ComparisonType.GreaterThan => $"{key.TargetPropertyName} > @0[0]",
            ComparisonType.GreaterThanOrEqual => $"{key.TargetPropertyName} >= @0[0]",
            ComparisonType.LessThan => $"{key.TargetPropertyName} < @0[0]",
            ComparisonType.LessThanOrEqual => $"{key.TargetPropertyName} <= @0[0]",
            ComparisonType.In => $"@0.Contains({key.TargetPropertyName})",
            ComparisonType.NotIn => $"!@0.Contains({key.TargetPropertyName})",
            ComparisonType.Between => $"{key.TargetPropertyName} >= @0[0] && {key.TargetPropertyName} <= @0[1]",
            _ => throw new ComparisonNotSupportedException(key.ErrorMessage())
        };
    }

    private static string BuildNumericLambdaString(FilterKey key)
    {
        return key.ComparisonType switch
        {
            ComparisonType.Equal => $"{key.TargetPropertyName} == @0[0]",
            ComparisonType.NotEqual => $"{key.TargetPropertyName} != @0[0]",
            ComparisonType.GreaterThan => $"{key.TargetPropertyName} > @0[0]",
            ComparisonType.GreaterThanOrEqual => $"{key.TargetPropertyName} >= @0[0]",
            ComparisonType.LessThan => $"{key.TargetPropertyName} < @0[0]",
            ComparisonType.LessThanOrEqual => $"{key.TargetPropertyName} <= @0[0]",
            ComparisonType.In => $"@0.Contains({key.TargetPropertyName})",
            ComparisonType.NotIn => $"!@0.Contains({key.TargetPropertyName})",
            ComparisonType.Between => $"{key.TargetPropertyName} >= @0[0] && {key.TargetPropertyName} <= @0[1]",
            _ => throw new ComparisonNotSupportedException(key.ErrorMessage())
        };
    }

    private static string BuildStringLambdaString(FilterKey key)
    {
        return key.ComparisonType switch
        {
            ComparisonType.Equal => $"{key.TargetPropertyName}.ToLower() == @0[0]",
            ComparisonType.NotEqual => $"{key.TargetPropertyName}.ToLower() != @0[0]",
            ComparisonType.Contains => $"{key.TargetPropertyName}.ToLower().Contains(@0[0])",
            ComparisonType.StartsWith => $"{key.TargetPropertyName}.ToLower().StartsWith(@0[0])",
            ComparisonType.EndsWith => $"{key.TargetPropertyName}.ToLower().EndsWith(@0[0])",
            ComparisonType.In => $"@0.Contains({key.TargetPropertyName}.ToLower())",
            ComparisonType.NotIn => $"!@0.Contains({key.TargetPropertyName}.ToLower())",
            ComparisonType.IsNotEmpty => $"{key.TargetPropertyName}.Length > 0",
            ComparisonType.IsEmpty => $"{key.TargetPropertyName}.Length == 0",
            ComparisonType.NotContains => $"!{key.TargetPropertyName}.ToLower().Contains(@0[0])",
            ComparisonType.HasCountEqualTo => $"{key.TargetPropertyName}.Count() == @0[0]",
            ComparisonType.HasCountBetween =>
                $"{key.TargetPropertyName}.Count() >= @0[0] && {key.TargetPropertyName}.Count() <= @0[1]",
            _ => throw new ComparisonNotSupportedException()
        };
    }

    private static string BuildCharLambdaString(FilterKey key)
    {
        return key.ComparisonType switch
        {
            ComparisonType.Equal => $"{key.TargetPropertyName} == @0[0]",
            ComparisonType.NotEqual => $"{key.TargetPropertyName} != @0[0]",
            _ => throw new ComparisonNotSupportedException()
        };
    }

    private static readonly List<Type> NumericTypes = new()
    {
        typeof(sbyte),
        typeof(byte),
        typeof(short),
        typeof(ushort),
        typeof(int),
        typeof(uint),
        typeof(long),
        typeof(ulong),
        typeof(float),
        typeof(double),
        typeof(decimal),
        // and nullables
        typeof(sbyte?),
        typeof(byte?),
        typeof(short?),
        typeof(ushort?),
        typeof(int?),
        typeof(uint?),
        typeof(long?),
        typeof(ulong?),
        typeof(float?),
        typeof(double?),
        typeof(decimal?)
    };
}