[![Build status](https://ci.appveyor.com/api/projects/status/7jr4rlw130txdvnx?svg=true)](https://ci.appveyor.com/project/jensandresen/clee)
[![Nuget version](https://img.shields.io/nuget/v/Clee.svg?label=latest release)](https://www.nuget.org/packages/clee)

# Clee :: Command line execution engine
Clee is a light weight framework for orchestrating command execution from the command line.


## Installation
Clee is available on [nuget](http://nuget.org/packages/clee) and can be installed with the following command in your package manager console:

````
PM> install-package Clee
````

## Usage
The most common usage is to have a console application that you want to be able to execute with a couple of command line arguments. These arguments should trigger an invokation of a specific functionality within your console application. This functionality is implemented in a class that implements the interface `ICommand<>` that closes an implementation of `ICommandArgument` which will hold all the command line data that you supply at the command line.

Let's try it out...

### Example 1
Let's build a console application that will receive user information from the command line and save it somewhere. Let's imagine that it will be called like this from the command line:

````
.\yourapp.exe add -name "John Doe" -email john@doe.com -dateofbirth 2000-01-01
````

Here you have a command named "add" and you will execute that with the arguments: name, email and "date of birth". You describe the arguments with an implementation of `ICommandArgument` like this:

````csharp
public class AddArguments : ICommandArgument
{
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime DateOfBirth { get; set; }
}
````

Next up you need to create your command as an implementation of `ICommand<>` that closes your `AddArguments` from above - like this:

````csharp
public class AddCommand : ICommand<AddArguments>
{
    public void Execute(AddArguments args)
    {
        /*
            your command logic here! - you have full access to:             
            
            args.Name
            args.Email
            args.DateOfBirth
        */
    }
}
````
All that is left to do is to create an instance of the `CleeEngine` and pass command line arguments to it. 

````csharp
public static void Main(string[] args)
{
    var engine = CleeEngine.CreateDefault();
    engine.Execute(args);
}
````

### The engine
The entry point of the framework is an instance of `CleeEngine` and it's `.Execute(args)` instance method that takes the command line arguments of the application and executed a pre-registered command.

#### Default configuration and default conventions
By using the factory method `CleeEngine.CreateDefault()` you utilize the default configuration and default conventions that are built into the Clee framework. This will scan the calling assembly for `ICommand<>` implementations and register them within the engine. By default `ICommand<>` implementations are registered with a naming convention where the above `AddCommand` class is registered as the `add` command, and can then be executed from the command line by specifing `add` as the first argument to the console executable.

#### Customizations
You can ofcourse customize the engine when initializing it, by using the other factory method like this:

````csharp
var engine = CleeEngine.Create(cfg => 
{
    /* 
        configure the engine by using the extension points:
        
        cfg.Registry(registryCfg => { ...registry customizations... );
        cfg.Mapper(mapperCfg => { ...mapper customizations... );
        cfg.Factory(factoryCfg => { ...factory customizations... );
    */
});
````

The configuration api is fluent and discoverably - so just _dot your way through it_ and you will quickly get the hang of it.

## The future
This is a list of ideas that could be a nice addition to the framework. They may or may not be implemented, only time will tell.

* custom format string on all arguments (specified with the value attribute)
* custom parser (specified with the value attribute)
* custom validation (specified with the value attribute)
* getopt-style short hand (-l) and verbose (--list) arguments
* combine multiple short hand boolean arguments into one (-abc instead of -a -b -c)

## Contributions
I'll be glad to recieve pull request for bugfixes or new features, but please adopt the current style and preferably tdd your implementations.

## Versioning
Clee uses SemVer 2.0.0

## Read more
You can read more at my blog: [http://jensandresen.com/tag/clee](http://jensandresen.com/tag/clee)
