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
It delegates actual identity value creation to two protected methods:

```csharp
IIdentityCreator GetCreator(Type type);
IIdentityCreator<T> GetCreator<T>();
```

However it does not do anything useful as-is.

### ReflectedIdentityProvider
This base class uses reflection to find two kinds of methods on the derived class:

* `IIdentity<T> MethodName<K>(K value)`
  * For any `T`, but explicitly specified.
* `I MethodName(parameters...)`
  * For any `I` that implements some `IIdentity<T>`
  * The arity of the returned identity value should match the arity of the method
  * The input value is converted using the default converter to a `ValueTuple` and split into input parameters.

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