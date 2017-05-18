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
An Identity Provider may convert this value to another type, if that is more natural or desireable within the provider's context.

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

## Free identity values
Identity values that are not specifcally bound to an identity provider can be constructed using the `FreeIdentity` classes. 
`FreeIdentity` instances have a `null` Provider reference and therefore cannot influence data type conversion.
The use of free identities should be very limited.
