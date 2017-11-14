# Changelog #


__v1.4.0__

Added support for Nullable properties on `ICommandArguments` implementations (e.g. `public Guid? NullableGuid { get; set; }`).

__v1.3.0__

Changed behavior for dealing with unhandled exceptions. They are now caught and an application exit code is now returned to the console. Old behavior can be introduced by accessing the settings configuration when configuring the engine.

__v1.2.0__

Command names can now be specified with the CommandAttribute annotation on the execute method or the entire command implementation class.
Added extension point for overriding default command naming convention used when registering command implementation types.
Added setting for combining system commands list and custom commands list when displayed from the help command.

__v1.1.1__

Fixed issue with list command not outputting command descriptions.

__v1.1.0__

Added help command (executed by invoking _help_ from the command line).
Added command descriptions by annotating commands on method or class level.

__v1.0.0__

First release of the framework containing basic functionality for command execution.

