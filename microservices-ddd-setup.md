### 1. Create the WebAPI Project

This creates the entry point of your service.

```bash
dotnet new webapi -n Catalog.API -o Services/Catalog/Catalog.API
```

- **`-n`**: Sets the project name.
- **`-o`**: Sets the **output directory** (creates the physical `Services/Catalog` folders automatically).

### 2. Create the DDD Class Libraries

These represent your core logic, data, and interfaces.

```bash
# Create the Application Layer
dotnet new classlib -n Catalog.Application -o Services/Catalog/Catalog.Application

# Create the Infrastructure Layer
dotnet new classlib -n Catalog.Infrastructure -o Services/Catalog/Catalog.Infrastructure

# Create the Domain Layer (The core)
dotnet new classlib -n Catalog.Domain -o Services/Catalog/Catalog.Domain
```

- **`classlib`**: A standard library that doesn't run on its own but is used by the API.

### 3. Create and Configure the Solution File (`.slnx`)

The `.slnx` file is the "map" that tells VS Code how to display the folders.

```bash
# Create the solution file in the root folder
dotnet new slnx -n Ecommerce
```

### 4. Add Projects to the Solution with Folders

This command automatically updates the `.slnx` XML with the correct `<Folder>` and `<Project>` tags to match your screenshot.

```bash
# Add API to the "Services/Catalog" solution folder
dotnet sln add Services/Catalog/Catalog.API/Catalog.API.csproj

# Add Application to the same folder
dotnet sln add Services/Catalog/Catalog.Application/Catalog.Application.csproj

# Add Infrastructure to the same folder
dotnet sln add Services/Catalog/Catalog.Infrastructure/Catalog.Infrastructure.csproj
```

```xml
<Solution>
  <Folder Name="/Services/Catalog/">
    <Project Path="Services/Catalog/Catalog.API/Catalog.API.csproj" />
    <Project Path="Services/Catalog/Catalog.Application/Catalog.Application.csproj" />
    <Project Path="Services/Catalog/Catalog.Infrastructure/Catalog.Infrastructure.csproj" />
    <Project Path="Services/Catalog/Catalog.Domain/Catalog.Domain.csproj" />
  </Folder>
</Solution>
```

### 5. Reference the Libraries in the WebAPI

This allows the API project to "see" and use the code inside your class libraries.

```bash
# Link Application to the API
dotnet add Services/Catalog/Catalog.API/Catalog.API.csproj reference Services/Catalog/Catalog.Application/Catalog.Application.csproj

# Link Infrastructure to the API
dotnet add Services/Catalog/Catalog.API/Catalog.API.csproj reference Services/Catalog/Catalog.Infrastructure/Catalog.Infrastructure.csproj
```

- **`dotnet add reference`**: Updates the `.csproj` file of the API so it can call classes/methods from the libraries.

---

### Pro-Tip: The "DDD Chain" References

In a true DDD architecture, you also need to link the libraries to each other so code can flow inward:

1.  **Application needs Domain:**
    `dotnet add Services/Catalog/Catalog.Application reference Services/Catalog/Catalog.Domain`
2.  **Infrastructure needs Application:**
    `dotnet add Services/Catalog/Catalog.Infrastructure reference Services/Catalog/Catalog.Application`
