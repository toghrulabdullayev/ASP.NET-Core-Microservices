# 1 `await using`

Used for objects that implement `IAsyncDisposable`.

```csharp
await using var connection = new NpgsqlConnection(connectionString);
```

Equivalent to:

```csharp
var connection = new NpgsqlConnection(connectionString);

try
{
    // Use connection
}
finally
{
    await connection.DisposeAsync();
}
```

### Why?

A database connection holds resources such as:

- Network sockets
- Buffers
- Connection pool entries

When the scope ends, `DisposeAsync()` is automatically called to clean up the connection and usually return it to the connection pool.

### `using` vs `await using`

| Syntax        | Calls            |
| ------------- | ---------------- |
| `using`       | `Dispose()`      |
| `await using` | `DisposeAsync()` |

### Example

```csharp
public async Task<Coupon> GetDiscount(string productName)
{
    await using var connection = new NpgsqlConnection(_connectionString);

    return await connection.QueryFirstOrDefaultAsync<Coupon>(
        "SELECT * FROM Coupon WHERE ProductName = @ProductName",
        new { ProductName = productName }
    );
}
```

When the method exits, the connection is automatically disposed asynchronously.

---

# 2 Why Each Method Creates a New Database Connection (Dapper + Npgsql)

**File Path:**  
`Discount.Infrastructure/Repositories/DiscountRepository.cs`

## Key Idea

A new `NpgsqlConnection` is created per method because database connections are **not meant to be long-lived or shared globally**. Instead, they are **lightweight, pooled, and short-lived resources**.

---

## 1. Connection Pooling (Main Reason)

Although you create a new `NpgsqlConnection` each time, you are **not actually creating a new physical TCP connection every time**.

Npgsql uses **connection pooling by default**:

- `new NpgsqlConnection(...)` → creates a _logical connection object_
- `Open()` → borrows a _physical connection from the pool_
- `Dispose()` / `await using` → returns it back to the pool

So:

> Creating a new connection per method is cheap and expected because pooling handles reuse internally.

:contentReference[oaicite:0]{index=0}

---

## 2. Why NOT use a single global connection?

### ❌ Thread safety issues

- `NpgsqlConnection` is **NOT thread-safe**
- Multiple concurrent requests (e.g., web API calls) would conflict if sharing one connection

---

### ❌ Connection corruption risk

A single shared connection can break because:

- one query is still executing
- another method tries to reuse it
- state leaks between operations (transactions, commands, etc.)

---

### ❌ Lifetime & fault tolerance problems

- If the connection drops, everything breaks globally
- Hard to recover without restarting the app or recreating the connection

---

## 3. Short-lived connections are the best practice

Standard ADO.NET / Dapper pattern:

> Open connection → execute query → dispose immediately

This is recommended because:

- connections are **expensive to open physically but cheap via pooling**
- keeps operations isolated and safe
- works naturally with DI and web request lifetimes

:contentReference[oaicite:1]{index=1}

---

## 4. Performance reality (important misconception)

People often think:

> “Creating connections repeatedly is expensive”

In reality:

- Pooling avoids real connection cost most of the time
- Open/Close is just “check-out / return to pool”
- This is why frequent creation is safe and expected

---

## 5. Why this pattern fits your repository

Each method in your repository:

- performs a single DB operation
- does not need shared state
- is independent of other methods

So this pattern:

```csharp
await using var connection = new NpgsqlConnection(_connectionString);
```

---

# 3 `using static` in C# (gRPC context)

## What it is

`using static` allows you to import **static members and nested types** of a class so you can use them without repeating the full class name.

```csharp
using static Discount.Grpc.Protos.DiscountProtoService;
```

---

## What it does

Instead of writing the full nested type path:

```csharp
public class DiscountService
    : Discount.Grpc.Protos.DiscountProtoService.DiscountProtoServiceBase
{
}
```

You can write:

```csharp
public class DiscountService : DiscountProtoServiceBase
{
}
```

---

## What gets imported

`using static` imports:

- Static methods
- Static fields
- Static properties
- Nested types (important in gRPC generated code)

It does NOT import:

- Instance members
- Namespaces
- Objects

---

## Why it is used in gRPC

In gRPC, `.proto` files generate C# code like:

```csharp
DiscountProtoService.DiscountProtoServiceBase
```

So `using static` allows you to access:

```csharp
DiscountProtoServiceBase
```

directly, without repeating the full type path.

---

## In your case

```csharp
public class DiscountService : DiscountProtoServiceBase { }
```

This works because:

- `DiscountProtoServiceBase` is a **nested type inside `DiscountProtoService`**
- `using static` brings that nested type into the current scope

---

## When to use it

✔ Useful for:

- gRPC generated base classes
- `System.Math`
- Static helper utilities

---

## When NOT to use it

❌ Avoid when:

- It reduces code clarity
- Multiple static imports cause ambiguity
- In large systems where explicit naming is preferred

---

## Mental model

Think of it as:

> “Bring static members and nested types into scope so I don’t need to prefix them with the class name.”
