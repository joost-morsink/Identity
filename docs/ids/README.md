# Identity Value implementations
The `IIdentity` can be implemented freely, but a few default implementations are provided by the library:
* `Identity<...>` models identity values with arity up to and including 5.
  All component values are generically typed, so these classes can only be used when the actual underlying type is known.
* `LateIdentity<T, K>` models a unary identity value that does not have a value yet (it needs to be inserted into some backend).
* `FreeIdentity<T, K>` models an identity provider free identity value for a certain entity type and for a certain underlying value type.
* `DecoratedIdentity<D, I, T>` models an identity value decorated by another identity value.
  This one can be used when identity value are to be regarded within a certain context.
* `IdentityWithSystem<T, I>` is like the `DecoratedIdentity` but specifically for the `SystemIdentity`.
* `SystemIdentity` is a specific identity value implementation for the non-instantiable class `Sys`.

### Identity
The `Identity<...>` classes are to be used in most cases and support arities of 1 through 5.
For arities 2 and higher they implement the `IMultiaryIdentity` interface.

The unary identity value is constructed by passing an `IIdentityProvider` instance and a underlying component value.
Multiary identity values support the same constructor, only with multiple component value parameters:

```csharp
new Identity<A, int>(prov, 1);
new Identity<A, B, int, int>(prov, 1, 2);
new Identity<A, B, C, int, int, int>(prov, 1, 2, 3);
// ...
```

Multiary identity values also support construction based on a parent Identity value and a 'last' identity component value:

```csharp
new Identity<A, B, C, int, int, int>(
        new Identity<A, B, int, int>(prov, 1, 2),
    3);
new Identity<A, B, C, int, int, int>(
        new Identity<A, B, int, int>(
                new Identity<A, int>(prov, 1),
            2),
    3);
```

### LateIdentity
A `LateIdentity` is a placeholder identity value, that will resolve to an actual underlying value at some point.
Until that time the instance is only equal to itself, based on the assumption that a mechanism issuing underlying identity values will always issue unique ones.
This way, equality should be preserved after resolving the `LateIdentity`.

The classes `LateIdentity<T>` and `LateIdentity<T, K>` are implementations of `ILateIdentity` and `ILateIdentity<K>` respectively.

### FreeIdentity
A `FreeIdentity` is some identity value that is not bound to a specific identity provider. 
Any type and any underlying type can be used to construct these identities, but there is of course no guarantee that any identity provider can actually translate them. 
Also, there is no mechanism to indicate data conversion strategies, so that's up to the importing identity provider.

### DecoratedIdentity
A `DecoratedIdentity` serves to provide identity context to an existing identity value. 
It implements a `Map` method to transfer the same context onto another identity value.
This context does not amount to the arity of the identity value.
(It is a decoration and not a part of the actual identity value.)

### IdentityWithSystem
An `IdentityWithSystem` provides a system context to an identity value. 
The same could also be done by using a `DecoratedIdentity`.

### SystemIdentity
This class takes a string in its constructor and serves to indicate subsystems within a system landscape. 