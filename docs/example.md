# Example Web Api
The Biz.Morsink.Identity.Test.WebApplication project contains a very simple example of an readonly Api using the concepts of the Identity library.
It models a very basic blogging website.
In no way does it intend to display any best practice nor guidelines on how to model or implement a blogging site. 
It only serves to display how to use the library in this example scenario.

The Api implementation is a two layered design: An api layer and a backend storage layer. 
In the example no effort was made to explicitly separate the two layers into different assemblies.
In a real scenario it _is_ advised to separate the two layers into different assemblies.
Because the logical separation is present, separating the code into multiple assemblies should be a breeze.

The Api layer is implemented by the middleware component `IdentityBasedResourceMiddleware`. 
The storage layer is implemented by two classes implementing the repository pattern for four domain types.
It is based on simple in memory collections of objects.

## Domain
The domain types are:
* User
* Blog
* BlogEntry
* Comment

The domain's identity values are all based on semantic properties of the entities.
It is therefore possible to construct an identity provider for the domain.

### User
The `User` class represents a user/login object with a username, a fullname and a password. 
It is not encouraged to store plaintext passwords in a storage layer for any reason. 
The example only contains a SHA-1 hash for a username salted password.

The `User` class contains an `Id` property of type `IIdentity<User>`. 
The underlying value is a string and equals the `Name` property.

### Blog
The `Blog` class represents a blog with a name, title and owner.
The `Name` property serves as an identifier for the `Blog` and equals the underlying value of the `Id` property.

The `Owner` property is of type `IIdentity<User>` and references a `User` object.

### BlogEntry
The `BlogEntry` class represents an entry in a `Blog`.
The `Id` property contains an identity value of arity 2. 
The parent identity value references the `Blog` the `BlogEntry` is an entry in.
The `Title` property is supposed to be the second part of the identity value.
The `Id` property's type is therefore `IIdentity<Blog, BlogEntry>`.

### Comment
The `Comment` class represents a comment on a `BlogEntry`. 
The `Id` property contains an identity value of arity 3.
2 are accounted for in the parent's identity value of `BlogEntry` and the last is an ordinal number for the comment, _within the context of the parent_.
The `Order` property equals this last part of the identity value.

A proof of concept lies in this class, as the identity value's underlying type differs between identity providers. 
Because the key composition is actually based on domain knowledge, it is possible to derive the canonical type of `(string, string, int)` for the underlying value.
However, this knowledge does not need to be present in the identity providers; they can choose their own representations.

## Storage layer
The storage layer is modeled using the repository pattern.
The example does not use an actual storage layer or storage layer technology, but just a readonly in-memory collection of objects.

The repository interface is specified in the `IRead<T>` interface:

```csharp
interface IRead<T> where T : class 
{
    T Read(IIdentity<T> id);
}
```
The `T : class` constraint allows returning `null` if an entity is not found.

Four implementations exist on two classes:

```csharp
class UserRepository 
    : IRead<User> 
{ ... }
class BlogRepository 
    : IRead<Blog>
    , IRead<BlogEntry>
    , IRead<Comment>
{ ... }
```

### Identity provider
The storage layer uses a `BackendIdentityProvider` as an identity provider. 
It is a domain specific implementation of the `ReflectedIdentityProvider` class.

It uses the following underlying types:

| Entity type | Underlying identity value type |
| ----------- | ------------------------------ |
| `User`      | `string`                       |
| `Blog`      | `string`                       |
| `BlogEntry` | `(string, string)`             |
| `Comment`   | `(string, string, int)`        |



## Api layer
The Api layer consists of an ASP.Net Core middleware component in combination with a custom `PathIdentityProvider` derived class called `ApiIdentityProvider`.
This example does _not_ use MVC, Razor or any other related technology to produce an HTTP response. 
Instead this example should be viewed as a way of substituting such a component.
It does build on ASP.Net Core for HTTP request pipelining. 

The example replies with an HTML page containing links to all the resource when a request to the root path is made.

### Middleware
The ASP.Net Core middleware component responsible for using the Identity library to produce HTTP responses is called `IdentityBasedResourceMiddleware`, because it exposes resources based on identity values.

The component terminates the HTTP request pipeline as it does not call any components after it.

The basic steps it does to determine what the output should be is:
* Use `ApiIdentityProvider` to parse the request path and try to match it to an entity type.
* If the resulting identity value's `ForType` property is `typeof(object)` it successfully parsed the path, but it did not match any entity type. 
  The request results in a 404 - Not Found.
* If it does match an entity type, we have a value of type `IIdentity<T>` for some known type `T` and we can lookup a service of `IRead<T>`.
* If it cannot find the service `IRead<T>`, something is wrong and the current information cannot be requested. 
  The request results in a 400 - Bad Request.
* If it can find the service, the `Read` method can be called with the parsed identity value.
* The services either returns a null, which results in a 404 - Not Found, or it returns an entity of type `T`.
  The object of type `T` is converted to JSON using the Newtonsoft.Json library and one custom `JsonConverter`.
  In this case the response status is 200 - Ok.

'Looking up' the `IRead<T>` is an implementation of the 'Service Locator' (anti-)pattern.
It is this library's author's opinion that it is a **pattern** in certain type of generic or general-purpose (framework) components, and an **anti-pattern** in actual implementation components.

A lot of information can be found on why and how to avoid this pattern. 
However, in our case the need for dynamic information to route the request to the correct implementation, and the fact that it is a 'framework'-component justifies the use of this pattern. 

Another way of doing this would be to introduce a component registry for type/service mappings and using that as a service to find other services. 
However this just duplicates the registration functionality of high quality IoC containers, and an additional mechanism should be constructed to register service implementations into the registry.
This basically is the Service Locator pattern outside of your IoC container.

The custom `JsonConverter` mentioned is the `IdentityConverter` that converts an identity value, using the `ApiIdentityProvider`, to the path where it can be found.
This way _any_ identity value can be converted to a string property that makes sense within the context of a Web Api.

### ApiIdentityProvider
The `ApiIdentityProvider` is derived from the `PathIdentityProvider` and maps the following paths:

| Entity type | Path                    | Underlying identity valuetype |
| ----------- | ----------------------- | ----------------------------- |
| `User`      | /user/\*                | `string`                      |
| `Blog`      | /blog/\*                | `string`                      |
| `BlogEntry` | /blog/\*/\*             | `(string, string)`            |
| `Comment`   | /blog/\*/\*/comments/\* | `(string, string, string)`    |

Note that there is a type discrepancy between the underlying value for the `Comment` type. 
Within the `ApiIdentityProvider` it is `(string, string, string)`, while in the `BackendIdentityProvider` it is `(string, string, int)`. 
However, the Identity library relies on the DataConvert library to take care of that, without any effort from the developer.

## Example
* A request is made for `/blog/Tech/Lorem/comments/2`. 
* It is parsed to an `IIdentity<Comment>` with value `("Tech", "Lorem", "2")`.
* An `IRead<Comment>` implementation is found on `BlogRepository`
* The `BlogRepository` class tries to find a comment with the id.
  For comparison it converts the identity value to `("Tech", "Lorem", 2)`.
* The Id property of the found object is `("Tech", "Lorem", 2)`, provided by the `BackendIdentityProvider`.
* The `Id` property is converted to an identity of the `ApiIdentityProvider`.
  Now the conversion back to `("Tech", "Lorem", "2")` is done.
* The comment identity is converted to the path `"/blog/Tech/Lorem/comments/2"` by using type's template `"/blog/*/*/comments/*"`.
* Likewise for the User property:
  * A comment is found by user with identity `"Joost"`, provided by the `BackendIdentityProvider`.
  * This user is converted by the `IdentityConverter` to an identity of the `ApiIdentityProvider`. 
    No conversion is done, although that fact is checked at runtime.
  * The user identity `"Joost"` is converted to the path `"/user/Joost"` by using the type's template `"/user/*"`.

