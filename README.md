# 🪦 Tombstone

Tombstone is a dead-code analyzer, which will traverse the specifed solution and print the dead code.

Currently, Tombstone will report dead code for:
- Methods
- Properties
- Fields
- Events

## Getting started

To build Tombstone, clone the repository and run the application:

```sh
git clone https://github.com/maxnatamo/Tombstone.git

cd Tombstone/

dotnet run --project src/Tombstone [path-to-solution]
```

## Example

Example `Program.cs` file:
```cs
class Program
{
    public class Service
    {
        public long Property { get; set }
    }

    public void Method()
    {
        
    }

    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
    }
}
```

An example output might be:
```
[16:28:40 ERR] /Example/Program.cs(10,5) Method 'Method()' can be removed, as it's not used.
[16:28:40 ERR] /Example/Program.cs(7,9) Member 'public long Property { get; set }' can be removed, as it's not used.
```

> [!IMPORTANT]
> Executable projects will mark unused public members as dead code, while library projects will not.