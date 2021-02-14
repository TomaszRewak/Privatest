# Privatest

Privatest is a code analyzer that adds instance-level accessibility mode to C#.

### Setup

To set it up simply install the `Privatest` Nuget package in your project:

```
dotnet add package Privatest
```

### Usage

To assign an instance-level accessibility mode to a member of a class, decorate it with the `[This]` attribute.

```csharp
public class Class
{
   [This] private int _field;

   public void Method(Class other)
   {
      _field = 10; // OK

      other._field = 10; // ERROR
    //^^^^^^^^^^^^
    //`_field` is inaccessible due to its protection level. It can only be accessed by the instance to which it belongs (through the `this` reference).
   }
}
```

You can apply the `[This]` attribute on fields, methods and properties (including property getters and setters).

The `[This]` attribute can only be applied on private members.

```csharp
public class Class
{
   [This] public int Property { get; set; }
                   //^^^^^^^^
                   //[This] attribute can be applied only on private members, but was applied on 'Public' member 'Property'
}
```

The `[This]` attribute can also be used to limit the accessibility of a member to a specific scope only. It can be useful when working with backing fields, which should be accessed only by the associated properties.

```csharp
public class Class
{
   [This(nameof(FrontProperty))] private int _backField;
   public int FrontProperty
   {
      get => _backField;
      set => _backField = value;
   }

   public void Method(Class other)
   {
      _backField = 2;
    //^^^^^^^^^^
    //`_backField` is inaccessible due to its protection level. It can only be accessed in `FrontProperty` (but is used in `Method`).
   }
}
```

The scope can be limited to any method or property of a class. It can by specified either by using the `nameof` operator or as a plain string value.

### How does it work?

Privatest is a C# code analyzer that performs accessibility checks during the compilation stage only (using Roslyn hooks). It does not introduce any additional checks in the compiled code and therefore does not impact its performance.

## Contributions

I am open to PRs as well as github issues.
