# ASP.NET-Core-Playground-Bit-More-Serious

In this project I focused on create an API with REST and dot.net Core. Using postman and automapper tools.

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

### Core request pipeline and middleware

//LD STEP1
- from "startup.cs" in the middleware I can call straight some method of classes for example to seed database.

- "IHostingEnvironment" make possible have access to informations related to the hosting environment

### Routing and Http protocol

it can be:
- "convention based"
- "attribute based" (for API this is recomended): attributes at controller and action level, URI is MATCHED to a specific action of a controller

Attributes:
- GET --> HttpGet
- POST --> HttpPost (to create and store)
- PUT --> HttpPut (to update a resource)
- PATCH -->HttpPatch (to partial update a resource)
- DELETE --> HttpDelete (to delete a resource)

### Basic REST requests

- //LD STEP3 example of a basic get request

below examples of Http Request for [Route("api/cities")] controller action attribute
- [Route("ld")] --> called by: http://localhost:1029/api/cities/ld
- [HttpGet("api/ld")] --> called by: http://localhost:1029/api/cities/api/ld

### Adding DTO, ENTITY classes and then call it

- //LD STEP4
adding "CityDto.cs" under the MODEL folder

- //LD STEP5
adding "City.cs" under the ENTITY folder

- //LD STEP6
adding "CitiesDataStore" to create some dummy data

- //LD STEP7
  - return a city by id
    - http://localhost:1029/api/cities/ld/1
      - notice that if i request an object id that doesn't exist, the returned value is "null", but the HTTP CODE is "200 OK".

- this return is fixed by //LD STEP8

### Status Codes

After doing an "HttpRequest", the "HttpResponce" cointain a "status code"
- LEVEL 100--> information level
- LEVEL 200 --> success level
  - 200 OK --> success GET
  - 201 --> success CREATION
  - 204 --> success request that don't return anything, like DELETE
- LEVEL 300 --> used for redirection
- LEVEL 400 --> CLIENT ERROR
  - 400 bad request --> the sent request to the server is wrong(for example a json request can't be parsed)
  - 401 unauthorized --> when there is not or invalid authentication provided
  - 403 forbitten --> right authentication but not authorization to the resource.
  - 404 not found --> the request doesn't exist
  - 409 conflict
- LEVEL 500 --> server error

- //LD STEP8 an example of how return a "not found"

- //LD STEP9 there is a way to return friendly status sentences in request done by browser, I need to set in "startup.cs" --> app.UseStatusCodePages();

//LD STEP10 get a list of points of interest

//LD STEP11 - //LD STEP12 get a specific point of interest

NOTE:
as we see by using "CitiesDataStore" also if I request for a "city", if the city has related data like "point of interest, that data will be returned automatically", BUT with ENTITY FRAMEWORK NO!!! related data is returned on demand.

### Serializer Settings

by default the serialization is in JSON but we can configure that in order to don't get a CAMEL SINTAX for the responce --> //LD STEP12

### Formatters and Content Negotiation

in dot.net core is possible select the best representation for a given responce when there are multiple representations available.

Usually the client use the "accept header" of the request to "specify" the media type of the return:
- application/json
- application/xml

DOT.NET CORE support:
- Output Formatter
- Input Formatter

dotnet.core by default handle a JSON request and respond with json, but I can specify the header. 
For instance
- KEY: Accept, VALUE: application/json
- KEY: Accept, VALUE: application/xml

I have to act in the method "ConfigureServices" and do the setting of

//LD STEP2
services.AddMvc().AddMvcOptions(o => o.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter()));

### Manipulating Resources - Create a resource

NOTE:
//LD STEP13 no "Id" has to be part of DTO used for creation, for this reason I create a NEW DTO without "Id"

//LD STEP14 now I create an "HttpPost" Request, we will get a "PointOfInterest" in json and we will deserialize it in a format compatible with the dto

a LINQ way to get the MAX of a related collection:
//LD get the new max for the id, because in the DTO that we call there is not "Id"
        var MaxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(RelatedInstance => RelatedInstance.PointsOfInterest).Max(poi => poi.Id);

IMPORTANT: in this way we don't manage the situation where multiple users ask for an id at the same time

//LD STEP15 allow to return a responce "201" (created) with location header, the location header will contain the URL where the new "PointOfInterest" created can be found. In my case the "GetPointOfInterestLd" at //LD STEP12 tat I already named.

   return CreatedAtRoute("GetPointOfInterestLd",new { cityId= cityId , Id=FinalPointOfInterest.Id }, FinalPointOfInterest);
now I have to SUBMIT A POST of a body content with an HEADER JSON
I have to select "BODY" --> "RAW" --> then on the right select "json".

{

    "name": "Central Park",
    "description": "The most visited urban park in the United States."
}

This is the call to GET what just posted

http://localhost:1029/api/cities/3/pointsofinterestLd/7

### Validating Input - Data Annotations

I will use data annotations to check if the model is valid

- //LD STEP16 and if I do this post

http://localhost:1029/api/cities/3/pointsofinterestLd/

With this body

    {
"name": "Eiffel Tower",
"description": "Eiffel Tower"
  }

the returned message is //LD STEP17, so I'm manually updating and using "ModelState" errors. 

- //LD STEP18 It could be possible use the default MVC "validations errors"

### Updating a Resource - full update

tsting the use of "put"

start by adding a dto --> "PointOfInterestForUpdateDto"

- //LD STEP18 update of the controller

- //LD STEP19
    
    [HttpPut("{cityId}/pointsofinterestLd/{id}")]
    public IActionResult UpdatePointOfInterestLd(int cityId, int id, [FromBody] PointOfInterestForUpdateDto pointOfInterest)

### Updating a Resource - partial update

- //LD STEP20 to do that I will use a "json patch document" that has defined a list of partial operations to do partial update

- //LD STEP21
  - so apply the received "patchDoc" to the record that is already in database.
  - if an error happen we update the "ModelState"

        patchDoc.ApplyTo(pointOfInterestToPatch, ModelState);

example

    [
  {
      "op":"replace",
      "path":"/name",
      "value":"luchino"
  }
]

- //LD STEP22 it is possible to use "TryValidateModel" to check if the DTO model is still valid after that I apply the received "json patch document". Of course I do that after execute "patchDoc.ApplyTo"

### Inversion of Control and Dependency Injection

Definition: "Inversion of control" is a pattern. It delegates the function of selecting a concrete implementation type for a class dependencies to an external component.

Definition: "Dependency Injection"(it is how we implement) is a specialization of the "Inversion of control pattern". The "dependency injection" pattern uses an object, the "container" to initialize the object and provide the required dependencies to the object.
Is the "Container" that inject the dipendencies for our class.

### Dependency Injection - injecting and using a "logger"

- //LD STEP22 to add a "logger", act in "startup.cs" --> "Configure" method

- //LD STEP23 then configure the controller

I can log info in try-catch statements and then BUILD A STATUS CODE TO RETURN

//LD STEP24
catch (Exception ex)
      {

          _logger.LogCritical($"Exception while getting points of interest for city with id {cityId}.", ex);
          return StatusCode(500, "A problem happened while handling your request.");
      }

### Implementing and using a "custom service"

GOAL: I'm going to send an email each time an item is deleted

- //LD STEP25 add the class "LocalMailService" and all the related classes and methods
  
- "Startup.cs" --> "Configure Services", I look to the methods that allow to "Register Custom Services" depend on the lifetime of the service.
  - services.addTransient(created each time they are requested, used for lightweight stateless services) maybe static classes
  - services.addScoped (created once per request)
  - services.addSingleton (created first time they are requested)

### .net core and "configuration files"

- //LD STEP27 that's how to separate the informations

  "mailSettings": {
    "mailToAddress": "info@lucadangelo.it",
    "mailFromAddress": "sviluppo.dangelo@gmail.com"
},

and how to call those

  private string _mailTo = Startup.Configuration["mailSettings:mailToAddress"];

### Core Entity Framework - crossplatform version of entity framework

- //LD STEP28 set up the classes and the "Data Annotation" to interact with the database by ORM

### seeding the database with data

I will use an EXTENSION METHOD to call in "Startup.cs" class

- //LD STEP29 "this CityInfoContext context" is useful to say to the compiler that "EnsureSeedDataForContext" extend "CityInfoContext"

public static void EnsureSeedDataForContext(this CityInfoContext context)

so in //LD STEP29 I have created an extension method, to execute it I have to add the CONTEXT to the "Configure" method of the "Startup.cs" class, then call it

//LD STEP29
cityInfoContext.EnsureSeedDataForContext();

### Repository Pattern

- no duplication
- no error prone
- better stability of the consuming class
- persistence ignorant: switch the persist technology

start by adding an interface to the repository "ICityInfoRepository", is a contract

- //LD STEP30 add the real implementation "CityInfoRepository"

- //LD STEP31 the "ToList()" ensure that the query is executed at that time

return _context.Cities.OrderBy(c => c.Name).ToList();

I give the optional possibility to include the related information about "PointOfInterest" to the cities, using "include"

- //LD STEP32 - (LINQ usage)

- //LD STEP33 regist the repository
  - "addscoped": for the lifetime of a single web request, the same instance will be returned to that request.
  If I ask for IGreetingService twice in a single request (even from different components like our Controller and View), the exact same instance will be returned. On the next request however, a brand new instance will be constructed and used instead. Individual requests have their own dependencies that are alive for the duration of that request!
  - addtransient: each time somebody asks for an IGreetingService, they will get a brand new instance of GreetingService.
  - addsingleton: create one and only one instance of IGreetingService, and any components wanting an IGreetingService will use the same shared instance.

http://dotnetliberty.com/index.php/2015/10/15/asp-net-5-mvc6-dependency-injection-in-6-steps/

### Returning data from repository - automapper

Istart reading from repository, but I have to map to dto, could do that by a "for", but in this case I use "mapper"

- //LD STEP34 use query string in the request to call

- //LD STEP8 http://localhost:1029/api/cities/1?includePointOfInterest=true

### automapper - mapping between entity and dto

"AutoMaper" configuration in "Startup.cs"

- //LD STEP35 to "map", "AutoMapper" use a named convention based approach, so will map the attributes that match.