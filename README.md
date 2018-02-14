# ASP.NET-Core-Playground-Bit-More-Serious

In this project I focused on:

## Main Topics
- setting of "Program.cs" and "Startup.cs"
- Core request pipeline and middleware
- Routing
  - How to implement calls
  - Adding DTO, ENTITY classes and then call it
- http request-responce
  - The importance of Status Codes
  - Returning correct status codes
  - Returning child resources
  - Formatters and Content Negotiation
- Manipulating Resources
  - Create Resource
- Validating Input
  - Data Annotations
- Updating a Resource
  - Full Update
  - Partial Update
  - json patch document
- Inversion of Control and Dependency Injection
  - Inversion Of Control
  - Dependency Injection
    - Logger
- Implementing and using a CUSTOM SERVICE(in Startup.cs)
  - in this case we will send an email
- .net core and CONFIGURATION FILES 
  - Safe configuration of CONNECTION STRING depend on the environment
- Seeding the database with data
  - (by) Extension Method
- Repository Pattern
  - linq
    - addsingleton
    - addscoped
    - addtransient
- Returning data from repository 
- Automapper

## Implementation Description

"Program.cs" is the very first method executed when we run a .core application, it cointain the server configuration etc...
 - startup.cs -> "ConfigureServices": is where we configure dependency injection
 - startup.cs -> "Configure": is the method responsible to manage all the coming http requests.

# Core request pipeline and middleware

//LD STEP1
- from "startup.cs" in the middleware I can call straight some method of classes for example to seed database.

- "IHostingEnvironment" make possible have access to informations related to the hosting environment




