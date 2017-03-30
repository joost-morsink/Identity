# Overview
In domain driven design a very important type of object is the _entity_.
An _entity_ can be uniquely identified by some value. 
The attribute/property that identifies some entity is called a primary key, and its value we will call an _identity value_.
Often the type of this value depends partly on the domain model (domain key), partly on some system that stores the entity (technical key).

## Identity Values {#idval}

> **Definition:** An identity value is at least a type and some value that uniquely defines an entity instance of that type.

This library tries to unify the generic concept of an identity value, making apparent the structure of the identity value.
Deriving from the definition, the following minimal interface can be stated for identity values:

```csharp
interface IIdentity
{
    Type ForType { get; }
    object Value { get; }
}
```

The following aspects come into play when dealing generically with identity values:
* The system which provides and consumes these values.
* The arity of identity values, either implicit or explicit.
* Conversion of between different representations without loss of information.
* The value itself should be an immutable value.

In domain classes, it will be important to make the properties containing identities typesafe.
Therefore we will create an extension on `IIdentity` to indicate the identity value's corresponding type. The implementation of the `ForType` property should return the generic parameter.

```csharp
interface IIdentity<T> : IIdentity 
{
}
```

Some notational convention will prove to be convenient:
> **Convention:** We indicate the identity value x for some type T as `T(x)`

## Identity Providers {#idprov}
An Identity Provider is an object that produces identity values for certain types. 
It is responsible for providing enough information for the context in which it is used.
Part of an identity value is knowing where it came from, so we'll extend the minimal interface:

```csharp
interface IIdentity
{
    IIdentityProvider Provider { get; }
    Type ForType { get; }
    object Value { get; }
}
interface IIdentityProvider 
{
    IIdentity Translate(IIdentity id);
}
```

Every Identity Provider can determine its own way of providing identity values.  
The `Translate` function can be used to translate externally defined identity values to 'this' Identity Provider.


### Example
For example, let Person be a class representing people in the domain layer. 
Its interface will look something like this:

```csharp
class Person 
{
    IIdentity<Person> Id { get; }
    string FirstName { get; }
    string LastName { get; }
    DateTime BirthDate { get; }
    // etc.
} 
```

Let Person be an SQL table storing the information in these objects.
Let it contain some integer (identity) column for a primary key.
The fact that the database stores this column as an integer value is not relevant for the domain model, just that it's _some_ value.
It is the Identity Provider's responsibility to use the actual value to retrieve the object from the database, or to create an identity value based on the database value.

Let there be some Web API that exposes this table to the world. 
The Web API should be an Identity Provider as well:

Let's say some request will ask for a resource at `/api/person/1`. 
This means we have a generic identity value for _some_ `Object`, containing the value `/api/person/1`.
In other words `api/person/1` is converted to the identity value `Object("/api/person/1")`.

The Web API Identity Provider is responsible for determining the `/api/person/` prefix belongs to identity values of the `Person` class. 
This Identity Provider is able to convert `Object("/api/person/1")` to `Person("1")`
Note that because of the string character of URIs, any value in an identity will always be a substring of the URI, and hence `String`-valued.

When the identity value is passed to the storage layer's Identity Provider it will be converted from a `Person("1")` to a `Person(1)`.
The storage layer can then determine it will need to query the database:

```sql
select [Id], [FirstName], [LastName], [BirthDate] from [Person] where [Id] = 1
```

This retrieves a row with type `(int, string, string, DateTime)`, which needs to be converted by some ORM layer to an instance of the `Person` class.
The int will be converted to a `Person(1)` by the storage layer's Identity Provider. 
After the `Person` instance is passed to the Web API, the identity value is converted back into `Object("/api/person/1")`. 
When the Web API serializes the object, the id property will contain a URI pointing to the location the object was retrieved from:

```json
{
    "id": "http://api.morsink.biz/api/person/1",
    "firstName": "Joost",
    "lastName": "Morsink"
    ...
}
```

The Web API identity provider is responsible for knowing where it is hosted and deriving a base URI.
This base URI is used in conjunction with the identity value to create a full URI. 

Conversion between strings and integers is a well understood domain.

## Arity {#arity}
An entity may have a multi-ary identity value, because of multiple reasons:
* The storage layer may require it.
* The domain layer may define it.
* The same type of entity may be stored in multiple different systems. 

> **Convention:** A multiary identity value x,y,... of a type T will be indicated by `T(x, y, ...)`.

As long as the arity is the same, and the identity value's component values correspond to the same entities everything is fine. But an identity value may not always be considered to have the same structure by all of the systems dealing with these values.

A parent-child relationship between two entity types may allow an identity value of arity 2. 
This means the value identifying the child only makes sense within the context of a certain parent. 
Both identifying components contribute to the total arity of the identity value.

In some other context the child may be referred to, without knowing about the higher arity.
Something has to be done to convert between identity values of different arities _without_ losing information.

The Identity Providers should be able to take on the role of converting this information into their own subdomain.
This could be done as easily as using a separator in a string: `T(1,2)` could be transformed to `T("1-2")`, changing the arity from 2 to 1 (without losing information).
Likewise the `T("1-2")` can be transformed back to `T("1","2")` if the 'separator' is known.
From there `T("1","2")` is easily converted to `T(1,2)`.
Of course the problem arises that Identity Providers on both sides might need to know about this separator. 

By using the `Biz.Morsink.DataConvert` library, two `IDataConverter`s can be defined per Identity Provider (or even per defined type within the Identity Provider). 
One should be responsible for incoming identity values, and one for outgoing identity values.
When converting an identity value, both the `IDataConverter`s can be tried. 

Arity 3 adds another layer of complexity when it is split in two, it adds the question whether to split it 2-1 or 1-2.
In other words, it is quite straightforward to convert from `T(1,2,3)` to `T("1-2-3")` and back, but where do `T("1-2","3")` and `T("1","2-3")` fit in?
One can imagine it gets exponentially more complex as the arity gets higher.

This leads us to divide the information in an identity value in:
* Inner domain component values, contributing to the _intrinsic_ arity.
  These component values have identifying meaning within the domain and within the context they are infrastructurally placed.
  One could say these are _local_ identity values.
* Infrastructural component values, contributing only to the _total_ arity.
  These component values are used to identify the system the entities belong to.
  Routing can be based on these values, and these values may be stripped for consumption in a local system after routing.

For multi-ary identity values we will define the following two terms:
> **Definition:** The parent of an identity value of T is an identity value of U, where the arity of U is one less than the arity of T. 
> The identity value of T also denotes the identity value of U.
 
> **Definition:** The head component value of an identity value of T with parent identity value of U is the component value that is not contained in the indentity value of U.

What this means can be seen in the following table:

| Identity value | Head component value | Parent identity value |
| -------------- | -------------------- | --------------------- |
| `T(x,y,z)`     | `z`                  | `U(x,y)`              |
| `U(x,y)`       | `y`                  | `V(x)`                |
| `V(x)`         | `x`                  | ?                     |

At the position of the question mark lies an important mathematical principle.
The unit identity, which denotes no object of any type, is needed for mathematical closure.
It is the only identity value with arity 0.

```csharp
interface IMultiaryIdentity<T, P> : IIdentity<T>
    where P : IIdentity
{
    P Parent { get; }
    object ComponentValue { get; }
}
```

Using this definition we can define the following multiary identity value interfaces:

```csharp
interface IIdentity<T, U> 
    : IMultiaryIdentity<U, IIdentity<T>> 
{ }
interface IIdentity<T, U, V>
    : IMultiaryIdentity<V, IIdentity<T, U>>
{ }
// et cetera.
```

## References {#refs}

### Intrinsic component values
When an identity value is multi-ary it also contains multiple identity values (for other types).
For instance for a 3-ary identity value `T(x,y,z)`, there exists some type U for which `T(x,y,z)` also denotes `U(x,y)`.
The relationship between `T` and `U` is defined by the domain model.
The same holds for a relationship between `U` and some `V` for which `U(x,y)` also denotes `V(x)`.
Transitively `T(x,y,z)` also denotes `V(x)`

Now the identity value is said to be a _reference_ for `T`, `U` and `V`. 
This is expressed by the following interface part:

```csharp
interface IIdentity
{
    // ...
    IEnumerable<IIdentity> Identities;
}
```

It is of course not possible to implement a generic reference interface multiple times because of error message CS0695. 
We have to work with the limits of the language and platform here.

### Infrastructural component values
Infrastructural or non-intrinsic component values are not part of the identity value within the domain model. The `Provider` property is some indication of who is the current owner of the identity value, but not necessarily where it can be found.

When a service has multiple locations to possibly find an entity, a system identifier might be necessary to determine where to look for the entity. 
It depends on whether the identity domains overlap or not.

Given a domain class `Person` with unary identity value, and a service layer with backends A and B.
Given a `Person` can either be registered in backend system A or in backend system B. 
The identity value can be expressed as a unary string starting with `"A"` or starting with `"B"`.
Based on this prefix the identity provider in the service layer can determine how to route the request.
For consumers of the service the prefix is meaningless, but the prefix _is_ part of the intrinsic identity.
The service layer is able to distill this information from the identity value, allowing for a _local_ identity part in a system with some identifier (A or B). 
This effectively alters the arity from 1 to 2, routing the identity value resolves it back to 1, which is then eligible for consumption in the backend layer.

When the responsibility of systems is properly segregated, these kinds of 'tricks' are of course not necessary.
However, in practice, when integrating with legacy systems or when migrating between old and new versions of systems they can prove to be very useful tricks.

### Self references
This section is deliberately left blank, because it is not known at this time how to model these kinds of references. 
This placeholder functions as a reminder.
