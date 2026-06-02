namespace StudentManagementSystem.Interfaces;

/// <summary>
/// Interface for entities that support keyword-based searching.
/// </summary>
/// <typeparam name="T">Entity type to search over.</typeparam>
public interface ISearchable<T>
{
    /// <summary>Searches across multiple fields for the given keyword.</summary>
    IEnumerable<T> Search(string keyword);

    /// <summary>Filters the collection using a predicate.</summary>
    IEnumerable<T> Filter(Func<T, bool> predicate);
}
