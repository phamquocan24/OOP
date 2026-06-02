namespace StudentManagementSystem.Interfaces;

/// <summary>
/// Generic CRUD repository interface.
/// Demonstrates the use of generics and interface abstraction.
/// </summary>
/// <typeparam name="T">Entity type managed by this repository.</typeparam>
public interface IRepository<T>
{
    /// <summary>Returns all entities.</summary>
    IEnumerable<T> GetAll();

    /// <summary>Finds a single entity by its primary key string.</summary>
    T? GetById(string id);

    /// <summary>Adds a new entity. Returns true on success.</summary>
    bool Add(T entity);

    /// <summary>Updates an existing entity. Returns true if found and updated.</summary>
    bool Update(T entity);

    /// <summary>Removes an entity by its id. Returns true if found and removed.</summary>
    bool Delete(string id);

    /// <summary>Returns total number of stored entities.</summary>
    int Count { get; }
}
