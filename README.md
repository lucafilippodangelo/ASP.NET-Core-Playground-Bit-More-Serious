# ASP.NET-Core-Playground-Bit-More-Serious

In this project I focused on create an API with REST and dot.net Core. Using of the tool postman.

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
==========================================================
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
==========================================================

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

### Updating a Resource - FULL UPDATE 99999999999999999999999999
==========================================================
now we will do a FULL UPDATE using PUT

start by adding a DTO --> "PointOfInterestForUpdateDto"

//LD STEP18
then we update the controller

  //LD STEP19
    [HttpPut("{cityId}/pointsofinterestLd/{id}")]
    public IActionResult UpdatePointOfInterestLd(int cityId, int id, [FromBody] PointOfInterestForUpdateDto pointOfInterest)
we will accept two parameters and the body