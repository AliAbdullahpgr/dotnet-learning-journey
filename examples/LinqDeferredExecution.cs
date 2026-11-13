// LINQ deferred execution vs ToList() snapshots.
// Run: dotnet run (as part of a console project) or paste into a C# script runner.
// Shows: a query is only a plan until it is enumerated; ToList() freezes the
// result at that moment — but it copies references, not the objects.
// See also: notes/csharp-linq-basics.md (deferred execution section).
//
// Expected output:
//   Snapshot (ToList before the change): Ayesha, Sara
//   Deferred query (enumerated after the change): Ayesha, Hassan
//   Deferred query (enumerated again): Ayesha, Hassan, Mina

using System;
using System.Collections.Generic;
using System.Linq;

public static class LinqDeferredExecution
{
    public sealed record Student(string Name, int Age);

    public static void Main()
    {
        var students = new List<Student>
        {
            new("Ayesha", 20),
            new("Bilal", 17),
            new("Sara", 22),
        };

        // The query is only a plan here — nothing has run yet.
        var adultNames = students
            .Where(s => s.Age >= 18)
            .Select(s => s.Name);

        // Snapshot taken now: ToList() runs the query and freezes the result.
        var snapshot = adultNames.ToList();

        // Change the source AFTER the query was defined and the snapshot taken.
        students.Add(new("Hassan", 25));
        students.RemoveAll(s => s.Name == "Sara");

        Console.WriteLine("Snapshot (ToList before the change): " + string.Join(", ", snapshot));
        Console.WriteLine("Deferred query (enumerated after the change): " + string.Join(", ", adultNames));

        // Enumerate the same query again after one more change — it re-runs.
        students.Add(new("Mina", 19));
        Console.WriteLine("Deferred query (enumerated again): " + string.Join(", ", adultNames));

        // Note: ToList() copies references, not objects. For a mutable class,
        // changing a property after the snapshot would still show up inside
        // the snapshot — only additions and removals are frozen by ToList().
    }
}
