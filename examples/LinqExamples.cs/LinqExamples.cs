// LINQ — filtering, grouping and aggregation over a small sample list.
// Run: dotnet run (as part of a console project) or paste into a C# script runner.

using System;
using System.Collections.Generic;
using System.Linq;

public static class LinqExamples
{
    // A small sample record used by the queries below.
    public sealed record Course(string Title, string Category, int Hours, double Rating);

    public static void Main()
    {
        var courses = new List<Course>
        {
            new("C# Fundamentals", "C#", 12, 4.8),
            new("ASP.NET Core Web APIs", "ASP.NET Core", 18, 4.9),
            new("Entity Framework Core", "EF Core", 10, 4.7),
            new("SQL Server Basics", "SQL Server", 8, 4.5),
            new("LINQ in Depth", "C#", 6, 4.6),
            new("Minimal APIs", "ASP.NET Core", 5, 4.4),
        };

        // 1. Filtering: highly rated courses, ordered by rating.
        var topRated = courses
            .Where(c => c.Rating >= 4.7)
            .OrderByDescending(c => c.Rating)
            .Select(c => c.Title);

        Console.WriteLine("Top rated courses:");
        foreach (string title in topRated)
        {
            Console.WriteLine($"- {title}");
        }

        // 2. Grouping: total learning hours per category.
        var hoursByCategory = courses
            .GroupBy(c => c.Category)
            .Select(g => new { Category = g.Key, TotalHours = g.Sum(c => c.Hours) })
            .OrderBy(x => x.Category);

        Console.WriteLine("\nHours per category:");
        foreach (var group in hoursByCategory)
        {
            Console.WriteLine($"- {group.Category}: {group.TotalHours}h");
        }

        // 3. Aggregation: overall stats across all courses.
        int totalHours = courses.Sum(c => c.Hours);
        double averageRating = courses.Average(c => c.Rating);
        Course longest = courses.OrderByDescending(c => c.Hours).First();

        Console.WriteLine($"\nTotal hours: {totalHours}");
        Console.WriteLine($"Average rating: {averageRating:F2}");
        Console.WriteLine($"Longest course: {longest.Title} ({longest.Hours}h)");
    }
}
