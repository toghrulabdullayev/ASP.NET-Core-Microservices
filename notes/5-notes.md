# 1. Constructor Chaining in C#

## `base(...)`

Calls a constructor in the **parent (base) class**.

```csharp
public class Employee : Person
{
    public Employee(string name)
        : base(name)
    {
    }
}
```

Equivalent Java concept:

```java
public Employee(String name) {
    super(name);
}
```

### When to use

- Initialize the inherited part of an object.
- Required if the base class has no parameterless constructor.

---

## `this(...)`

Calls another constructor in the **same class**.

```csharp
public class Person
{
    public Person()
        : this("Unknown")
    {
    }

    public Person(string name)
    {
        Name = name;
    }
}
```

Equivalent Java concept:

```java
public Person() {
    this("Unknown");
}
```

### When to use

- Reuse initialization logic.
- Avoid duplicate code across constructors.

---

## Syntax Rule

Both `base(...)` and `this(...)` must appear after a colon (`:`):

```csharp
public Person() : this("Unknown")
{
}

public Employee(string name) : base(name)
{
}
```

❌ Invalid:

```csharp
public Person()
{
    this("Unknown"); // Error
}

public Employee(string name)
{
    base(name); // Error
}
```

---

## Execution Order

```csharp
public Child(int x)
    : base(x)
{
    Console.WriteLine("Child");
}
```

Execution:

1. `Base(x)` constructor runs.
2. `Child` constructor body runs.

---

## Important Rules

- A constructor can use **either** `base(...)` or `this(...)`, not both.
- If neither is specified, C# implicitly calls `base()`.
- If the base class does not have a parameterless constructor, you must explicitly use `base(...)`.

```csharp
public Child(int x)
    : base(x)
{
}
```

---

## Quick Java ↔ C# Mapping

| Java             | C#              | Purpose                                |
| ---------------- | --------------- | -------------------------------------- |
| `super(...)`     | `base(...)`     | Call parent constructor                |
| `this(...)`      | `this(...)`     | Call another constructor in same class |
| `super.method()` | `base.Method()` | Call overridden parent method          |
| `this.field`     | `this.field`    | Current object instance                |

---

# 2. ChangeTracker

- Tracks all entities attached to the current `DbContext`.
- Used by EF Core to determine what database operations to execute during `SaveChanges()`.
- Entities become tracked when:
  - Loaded from the database.
  - Added via `Add()`.
  - Attached via `Attach()`, `Update()`, etc.

### Common Entity States

| State     | Meaning                 | SQL Generated |
| --------- | ----------------------- | ------------- |
| Added     | New entity              | INSERT        |
| Modified  | Existing entity changed | UPDATE        |
| Deleted   | Marked for removal      | DELETE        |
| Unchanged | No changes detected     | None          |
| Detached  | Not tracked             | None          |

### In This Code

```csharp
foreach (var entry in ChangeTracker.Entries<EntityBase>())
```

- Gets all tracked entities that inherit from `EntityBase`.

```csharp
entry.State
```

- Returns the current tracking state of the entity.

```csharp
case EntityState.Added:
```

- Runs for newly created entities before they are inserted.
- Sets audit fields such as:
  - `CreatedDate`
  - `CreatedBy`

```csharp
case EntityState.Modified:
```

- Runs for updated entities before they are updated.
- Sets audit fields such as:
  - `LastModifiedDate`
  - `LastModifiedBy`

### Why Override SaveChangesAsync?

To automatically populate audit fields in one place instead of manually setting them throughout the application.

### Summary

`ChangeTracker` is EF Core's internal mechanism for monitoring entity changes and deciding which database operations to perform when `SaveChanges()` is called.

---

# 3. `_orderContext.Set<T>()`

Returns the `DbSet<T>` for the entity type `T`.

Think of it as:

```csharp
_orderContext.Set<Order>()
```

being equivalent to:

```csharp
_orderContext.Orders
```

but generic.

Used when the repository does not know the concrete entity type at compile time. It allows the same repository code to work with `Order`, `Product`, `Customer`, etc. :contentReference[oaicite:0]{index=0}

---

# 4. `Add(entity)`

```csharp
_orderContext.Set<T>().Add(entity);
```

Marks the entity as `Added` in EF Core's Change Tracker.

Nothing is inserted into the database yet.

```text
EntityState = Added
```

When `SaveChangesAsync()` runs, EF generates an `INSERT` statement and persists the entity. :contentReference[oaicite:1]{index=1}

---

# 5. `Remove(entity)`

```csharp
_orderContext.Set<T>().Remove(entity);
```

Marks the entity as `Deleted`.

```text
EntityState = Deleted
```

When `SaveChangesAsync()` runs, EF generates a `DELETE` statement. :contentReference[oaicite:2]{index=2}

---

# 6. `SaveChangesAsync()`

```csharp
await _orderContext.SaveChangesAsync();
```

Persists all tracked changes to the database.

EF inspects the Change Tracker and generates SQL based on entity states:

| State     | SQL Generated |
| --------- | ------------- |
| Added     | INSERT        |
| Modified  | UPDATE        |
| Deleted   | DELETE        |
| Unchanged | Nothing       |

Without `SaveChangesAsync()`, changes only exist in memory. :contentReference[oaicite:3]{index=3}

---

# 7. `AsNoTracking()`

```csharp
_orderContext.Set<T>()
    .AsNoTracking()
```

Disables EF Core change tracking for retrieved entities.

Effects:

- Lower memory usage
- Faster queries
- Returned entities are read-only from EF's perspective
- Later modifications are not automatically detected

Best used for read-only queries. :contentReference[oaicite:4]{index=4}

---

# 8. `ToListAsync()`

```csharp
await query.ToListAsync();
```

Executes the SQL query and materializes all results into a `List<T>`.

Before `ToListAsync()`:

```csharp
IQueryable<T>
```

After `ToListAsync()`:

```csharp
List<T>
```

This is the point where the query is actually sent to the database.

---

# 9. `Where(predicate)`

```csharp
.Where(predicate)
```

Adds a filter to the SQL query.

Example:

```csharp
.Where(x => x.Status == "Completed")
```

becomes roughly:

```sql
WHERE Status = 'Completed'
```

The filtering is performed in the database, not in memory.

---

# 10. `Expression<Func<T, bool>>`

```csharp
Expression<Func<T, bool>>
```

A LINQ expression tree.

Unlike a normal delegate:

```csharp
Func<T, bool>
```

EF can inspect the expression structure and translate it into SQL.

Example:

```csharp
x => x.Price > 100
```

becomes:

```sql
WHERE Price > 100
```

This is what allows LINQ-to-SQL translation.

---

# 11. `FindAsync(id)`

```csharp
await _orderContext.Set<T>().FindAsync(id);
```

Finds an entity by its primary key.

Lookup order:

1. Check Change Tracker first
2. If not found, query the database

Advantages:

- Optimized for primary-key lookups
- Can avoid a database query if already tracked

Returns:

```csharp
T?
```

meaning the entity may not exist. :contentReference[oaicite:5]{index=5}

---

# 12. `_orderContext.Entry(entity)`

```csharp
_orderContext.Entry(entity)
```

Returns an `EntityEntry`.

`EntityEntry` is EF Core's metadata wrapper around a tracked entity.

Allows access to:

- State
- Original values
- Current values
- Tracking information

Example:

```csharp
var entry = _orderContext.Entry(entity);
```

---

# 13. `EntityState.Modified`

```csharp
_orderContext.Entry(entity).State =
    EntityState.Modified;
```

Forces EF Core to treat the entity as modified.

```text
EntityState = Modified
```

When `SaveChangesAsync()` runs, EF generates an `UPDATE`.

Commonly used for disconnected entities (e.g., data received from an API request) where EF was not tracking the object. :contentReference[oaicite:6]{index=6}

---

# 14. Change Tracker

Internal EF Core component that tracks entities and their states.

Possible states:

```text
Added
Modified
Deleted
Unchanged
Detached
```

`SaveChangesAsync()` uses these states to determine which SQL commands should be generated. :contentReference[oaicite:7]{index=7}

---

# 15. Query Lifecycle

```csharp
_orderContext.Set<T>()
    .Where(predicate)
    .AsNoTracking()
    .ToListAsync();
```

Execution flow:

1. Get entity set (`Set<T>()`)
2. Build query (`Where`)
3. Configure tracking (`AsNoTracking`)
4. Execute SQL (`ToListAsync`)
5. Materialize rows into objects
6. Return results

Until `ToListAsync()` is reached, EF is only building the query expression.

---

# 16. Generic Variance and Constraints

## Contravariance (`in`)

```csharp
public interface ICommandHandler<in TCommand>
```

- `in` makes `TCommand` contravariant.
- `TCommand` can only be used as a method parameter (input).
- `TCommand` cannot be used as a return type.
- Used when the interface consumes objects of type `TCommand`.

Example:

```csharp
public interface ICommandHandler<in TCommand>
{
    Task Handle(TCommand command);
}
```

Valid:

```csharp
Task Handle(TCommand command);
```

Invalid:

```csharp
TCommand GetCommand(); // Compile error
```

### Quick Rule

```text
in  = consumes T
out = produces T
```

---

## Generic Constraint (`where`)

```csharp
where TCommand : ICommand
```

- Restricts which types can be used for `TCommand`.
- Only types implementing `ICommand` are allowed.
- Gives compile-time type safety.
- Allows code to assume `TCommand` is always an `ICommand`.

Example:

```csharp
public class CreateOrderCommand : ICommand { }

ICommandHandler<CreateOrderCommand> // ✅
ICommandHandler<string>             // ❌
```

### Quick Rule

```text
where T : Interface
```

= "T must implement Interface"

```text
where T : BaseClass
```

= "T must inherit BaseClass"
