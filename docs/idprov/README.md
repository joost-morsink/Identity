# Identity Providers
Identity Providers are objects that are bound to a certain context.
Within that context it fulfills the role of creating, and translating identity values.

## Interface
The interface definition is as follows:

```csharp
interface IIdentityProvider : IEqualityComparer<IIdentity>
{
    IIdentity Create<K>(Type forType, K value);
    IIdentity<T> Create<T, K>(K value);
    bool SupportsNewIdentities { get; }
    IIdentity New(Type forType, object entity);
    IIdentity<T> New<T>(T entity);
    IIdentity Translate(IIdentity id);
    IIdentity<T> Translate<T>(IIdentity<T> id);
    IDataConverter GetConverter(Type t, bool incoming);
}
```

The type `T` is always the entity type the identity value is a reference for. 
The type `K` is an input type for the underlying value.
An Identity Provider may convert this value to another type, if that is more natural or desirable within the provider's context.

### Create
The create methods create identity values for a certain type, with a certain underlying value.
It is up to the provider to determine how to construct such values. 
Often it is the most natural representation for the identity provider's context.
For instance, in SQL, an auto increment integer primary key may convert the value of type `K` to an `int`.

If the type is known at compile time, the method can return an `IIdentity<T>`, otherwise a general `IIdentity` is returned.
It is still required to have the type known at runtime to construct the identity value.
While the static type is `IIdentity`, at runtime there still it should always be an instance of `IIdentity<T>` where `typeof(T)` equals the parameter `forType`.

### New
The new methods create new identity values to use for new entities.
It is up to the provider to determine whether and how to construct such values.
The `SupportsNewIdentities` should indicate whether the identity provider is able to produce new identity values.
If it is `false`, an `InvalidOperationException` should bw thrown on invocation of a `New` method.
A storage layer must be able to provide new identity values for new entities, but an API layer should probably let the backend storage layer handle the generation of new identities.

When an entity has an identity value that has more semantics than just being a key, the key is often determined by the domain layer. 
In this case the `New` functions are not necessary to determine new identity values, although an implementation can be provided by using the `entity` parameter.

### Translate
The translate methods are able to convert an identity value that belongs to another identity provider to one that belongs to itself.
It might use a DataConverter for the conversion, and it might use the DataConverter obtained from the other identity provider.
Because of the import/export nature of this call there is a slight asymmetry in this call. 

### GetConverter
An identity provider is able to assist in the conversion of underlying values for other identity providers by giving them access to an `IDataConverter`. 
This method provider a best suited instance based on the specified usage parameters.
For each type and direction a different instance could be returned. 

### IEqualityComparer<IIdentity>
Every identity provider should be able to compare two identity values for equality.
Equality comparison is divided into three categories:
* Regular value equality. 
  The most common case, if the underlying values of two identities equal, then so do the identities themselves.
  Sometimes the right hand side may need to be converted to the type of the left hand side before the values are compared.
* Null values.
  Null values are not equal to regular values, but *are* equal to other null values.
* Late values.
  Late values are identity values that are yet to be determined by some storage layer. 
  Late values are not equal to regular values or null values.
  Late values are also not equal to other late values, unless they are the *same* instance (reference equality).
  The reason behind this is that two different late identity values should ultimately resolve to two different actual identity values.

Late values should only be used if there is no way to determine an identity value beforehand.
For instance, an existing SQL database with an auto identity column might need to use late identities.
However, an SQL database that uses sequences might not need to use late identities.

## AbstractIdentityProvider
A convenient base class for an identity provider is the `AbstractIdentityProvider` class.
It provides default (overridable) implementations for the whole `IIdentityProvider` interface.
It delegates actual identity value creation to two protected methods and identity value generation to another two methods:

```csharp
IIdentityCreator GetCreator(Type type);
IIdentityCreator<T> GetCreator<T>();
IIdentityGenerator GetGenerator(Type type);
IIdentityGenerator GetGenerator<T>();
```

It also contains default equality logic and translation logic. 

### ReflectedIdentityProvider

#### Creation
This base class uses reflection to find two kinds of methods on the derived class:

* `IIdentity<T> MethodName<K>(K value)`
  * For any `T`, but explicitly specified.
* `I MethodName(parameters...)`
  * For any `I` that implements some `IIdentity<T>`
  * The arity of the returned identity value should match the arity of the method
  * The input value is converted using the default converter to a `ValueTuple` and split into input parameters.

In both cases the method should be attributed with `[Creator]`. 
These methods convert underlying values to concrete identity values.
For instance, a database table 'Person' with autoincrement primary key, may be modeled by the identity provider as follows:

```csharp
public class DatabaseIdentityProvider : ReflectedIdentityProvider 
{
    public Identity<Person, int> PersonId(int x)
        => new Identity<Person, int>(this, x);
}
```

Note that the return type could also have been `IIdentity<Person>`, it depends on what contract the implementor desires.
The constraint is that a method needs to be present that returns some `IIdentity<Person>` implementing type.

The `ReflectedIdentityProvider` default implementation takes care of calls to `Create<Person, K>(K value)` and `Create<K>(Type type, K value` (where `type == typeof(Person)`) , by converting the `K` valued parameter to `int` and passing it to the `PersonId` method.
The `Translate` methods build on these `Create` methods.

#### Generation
Generation of new and unique identity values may be supported by deriving classes. 
The base class uses reflection to find methods with the `IIdentity<T> MethodName(T)` signature and attributed with `[Generator]`.
These methods should provide the caller with new and unique identity values for supported types.

### GenericIdentityProvider
This utility class provides an implementation for `IIdentityProvider` that is able to generate identity values for _any_ type.
It is parameterized on the underlying type, which is used for _all_ entity types.
This makes it very easy to use when keytypes of some storage layer are uniform, but it makes it very difficult to use with domain models containing identity value types of different arities. 

All that is necessary to create an identity value with this class is instantiate it and call the Create method:
```csharp
new GenericIdentityProvider<int>().Create<Person, int>(42);
```

The constructor takes an optional `IDataConverter` instance. 
If nothing or null is passed the `DataConverter.Default` instance is used.

### PathIdentityProvider
The `PathIdentityProvider` is a base class for API identity providers.
It allows for the definition of mappings between entity types and path templates. 

#### Path parsing
Next to an `IIdentityProvider` implementation, it contains methods to 'parse' a path string to an identity value.
It can also achieve this through the intermediate `IIdentity<object>` type.
The following parse signatures are implemented:

```csharp
IIdentity Parse(string path, bool nullOnFailure = false);
IIdentity<T> Parse<T>(string path);
IIdentity Parse(IIdentity<object> objectId, bool nullOnFailure);
IIdentity<T> Parse<T>(IIdentity<object>);
```

Because **any** string can always be an identity value for an object, any string can always be converted to an `IIdentity<object>`. 
When the path should be parsed using a specific type's template, the resulting match can either succeed (returning an `Identity<T>`) or it can fail (in which case an `Identity<object>` can be returned or a `null` value, depending on the static return type and the `nullOnFailure` parameter).

#### Path (re-)construction
Identity values that are known to the provider should be convertible back to their paths using the configured templates.
Two method are implemented to do this:

```csharp
IIdentity<object> ToGeneralIdentity(IIdentity id);
string ToPath(IIdentity id);
```

A general `IIdentity` value contains enough information to convert it to an `IIdentity<object>`, so there are no generic variants of these methods. 
The `Value` of the `IIdentity<object>` is the path, but these methods return `null` if the entity type is not known to the provider.

#### Path templates
Path templates can be used to match actual path against a pattern. 
Paths are a series of strings separated by a separator (default '/'), with optionally wildcards ('*'). 
The general rule is that either a string is a literal string, which needs to be matched or it is a wildcard.
Wildcards match _any_ string and will produce the value for consumption into an identity value.
For instance, a path template `/api/person/*/detail/*` will match a path `/api/person/1/detail/1` because all parts match the template. 
By configuring the provider to produce an identity value of type `IIdentity<Person,Detail>` it will produce a binary identity value with value `("1","1")`.

Paths can be made case sensitive or case insensitive at the provider level. 
Case sensitivity influences the applicability of some DataConverters from and to byte arrays. 
A hex converter could be case insensitive, but a base-64 converter is always case sensitive.
A `PathIdentityProvider` implementation should be configured to respect the semantics of the underlying values it provides.

Not all paths in a path system refer to actual entity types.
This means there are some 0-ary paths that need to be mapped as well. 
The trick is to define a (dummy) class to represent that resource and configure it with a path without wildcards.
Matching these paths will create 1-ary identity values with an empty string as the underlying value.

### Other implementations
Other, more specific, implementations of the `AbstractIdentityProvider` or `IIdentityProvider` can be made. 
However it will need to take a dependency on an external library, making an implementation in the main library impossible.

Some pointers to take into account are:
* Make a general implementation for a certain technology. 
  For example, one for SQL Server, one for MongoDB, one for any other data storage system.
* Derive your domain specific variant from the general one.
* Try to generate new identity values within the identity provider (and not in the backend storage layer itself).
  For example when an auto identity column is implemented on the backend storage layer, the sequencing of inserts matters, which impedes scalability.
  Allocating multiple identity values from a sequence object is preferred, but when this is not possible, you should use `LateIdentity` values.
  Using uniquely generated identity values like GUIDs or Mongo's ObjectIds is even better, as this does not require any roundtrips to the backend.

## Free identity values
Identity values that are not specifically bound to an identity provider can be constructed using the `FreeIdentity` classes. 
`FreeIdentity` instances have a `null` Provider reference and therefore cannot influence data type conversion.
The use of free identities should be very limited.
