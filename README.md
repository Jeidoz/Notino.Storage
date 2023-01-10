# Notino.Storage
## Requested Assignment Description
Origin description located [here](https://delabin.com/kauxv5bq).

Create production-ready ASP.NET Core service app that provides API for storage and retrive documents in different formats

- [x] The documents are send as a payload of POST request in JSON format and could be modified via PUT verb.
- [x] The service is able to return the stored documents in different format, such as XML, MessagePack, etc.
- [x] It must be easy to add support for new formats.
- [x] It must be easy to add different underlying storage, like cloud, HDD, InMemory, etc.
- [x] Assume that the load of this service will be very high (mostly reading).
- [ ] Demonstrate ability to write unit tests.
- [x] The document has mandatory field id, tags and data as shown bellow. The schema of the data field can be arbitrary.

## Brief Summary About Done Work
### Chosen Document Schema
The assignment states that the document must have the mandatory fields id, tags, and data. The data field can have any scheme that I decide. In this project, the "document" scheme is represented by the Document and DocumentDTO classes in the Core project. The data field contains a nested object with fields that describe some package or third-party project.

Example of such a document:
```
{
    "id": "demoDoc",
    "tags": ["important", ".net"],
    "data": {
        "author": "Stanislav Khavruk",
        "projectName": "Test Project",
        "Description": "Some desc"
    }
}
```
### Solution Architecture
Because the assignment requires the ability to be easily added to and extended, I decided to implement the project using the classic N-Layer Architecture as recommended by [Microsoft](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures).
This is also the first time I have used 3rd-party packages from Ardalis (whose author and examples I discovered on the Microsoft website under the References section). I was primarily inspired by the [Clean Architecture template](https://github.com/ardalis/cleanarchitecture).

In this project, I used the following packages from Ardalis:
- Ardalis.Result - A Result pattern implementation for services (I found this to be a well-designed class for passing results, data, status, errors, and validation errors from a service to a controller)
- Ardalis.Specification - Provides a basic Specification class for use in creating queries that work with Repository types (This package adds a generic IRepositoryBase boilerplate and ready-to-use queries & specifications that convert LINQ requests to named functions for repository methods. In a complex project, this should make the code more readable and easier to maintain and test.)

The solution is divided into three projects:

- Core (Entities and Services): Contains database entities, interfaces, the main Documents service, and the project's module for Autofac.
- Infrastructure (Interface implementations and EF): Contains the DbContext, the EfRepository implementation, configurations for EF, and the project's module for Autofac. Migrations can also be included in this project.
- Web (ASP.NET WebAPI): Contains all controllers, settings files, DB seeding, dependency injections, and other related items

For the ability to easily add or change the underlying storage, I decided to use Entity Framework. Its configuration allows us to easily change the provider (from InMemory to SQLite/SQL server/AzureDb/InFiles and so on) by modifying only 2-3 lines in the Web/Project.cs file:
```
// Here you can change storage for EF Core
string? connectionString = builder.Configuration.GetConnectionString("Sqlite");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));
// options.UseInMemory()
// options.UseSqlServer()
// ...
```

For "easy to add support for new formats" I decided to use ASP.NET functionality - output formatters and allow ASP.NET to decide by itself returning Content-Type based on Accept header.
Such formatters can be easily added in Web/Project.cs:
```
// Here you can add any output formatters
builder.Services.AddMvcCore()
    .AddMvcOptions(options =>
    {
        // JSON Serializer enabled by default
        options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
        options.OutputFormatters.Add(new MessagePackOutputFormatter(ContractlessStandardResolver.Options));
    });
builder.Services.AddOutputFormattersService();
```

Additionally, I implemented a separate service for the validation of supported Accept headers - OutputFormattersService.cs. Its main task is to check if the passed Accept header is supported by the ASP.NET application after launch.
```
var requestedResponseContentType = Request.Headers[HeaderNames.Accept].ToString();
if (!_formattersService.HasSupportOf(requestedResponseContentType))
{
    return BadRequest(...);
}
```

With this method, it's easy to include custom or pre-made formatters to change the format of the document that's being returned.
![image](https://user-images.githubusercontent.com/23425714/211588351-48e35e8a-2278-4530-acf2-6125ca6d7322.png)
![image](https://user-images.githubusercontent.com/23425714/211588392-7ae06512-0037-4ee3-a113-8d42a60e5561.png)
![image](https://user-images.githubusercontent.com/23425714/211588415-b62e392b-fad8-4551-b37b-d97639467d39.png)

### Interface Segregation Principle and Tests
The Interface Segregation Principle, one of the SOLID principles, allows for easy replacement of implementations for database Repository classes in Core and Infrastructure projects, as well as the ability to use mocked or fake services in Unit tests.

While I am familiar with general techniques for writing Unit tests, I believe that this solution does not have enough functionality that would benefit from being covered by Unit tests. Instead, I would prefer to use Integration and Functional tests for this Web project. Unit testing would only be useful for testing a single service and 3rd-party repository, which I do not see as being necessary.

In addition, I have implemented exception handling and validation in the service to ensure proper functionality.

However, due to personal circumstances, I have not been able to include XML documentation for the default Swagger configuration. As a result, the project can currently only be used for viewing request routes and POST body schemas for requests.

### "Assume that the load of this service will be very high (mostly reading)."
I have limited experience in designing high-load services, so my project only includes a few performance enhancements:

- Asynchrony: almost all code, including controllers, services, and repositories, is asynchronous.
- Lazy loading of document entities to check for their existence.
If I had the option to use raw Entity Framework (EF) without a repository implementation to allow for the choice of storage outside of EF, I would have attempted additional optimizations such as using AsNoTracking() calls or raw SQL queries/procedures.
