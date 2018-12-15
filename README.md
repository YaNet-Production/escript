<p align=center>
  <img alt="ESCRIPT" src="https://user-images.githubusercontent.com/25367511/47752398-03692a00-dc9d-11e8-9b91-3f4e91e8ec1f.png">
</p>

## Again No One Needed Project?

With ESCRIPT you can create powerful cross-platform scripts with ESCRIPT syntax (which reminds programming language). Also, you can extend ESCRIPT syntax/platform by commiting in it or creating own .NET plugins (see documentation)

## Download

[Go to Releases page](https://github.com/feel-the-dz3n/escript/releases)

## Cross-platform?

There are two editions of ESCRIPT:
- **Core** (uses .NET Core to work, works on Linux, MacOS and Windows with different architectures)
- **Standard** (uses .NET Framework 4 to work, works only on Windows XP and later and on [ReactOS](https://github.com/reactos/reactos))

ESCRIPT is mainly designed for *.NET Framework*, but code can be compiled with *.NET Core*, because [ESCRIPT Core code](Core) has `IsCore` compiler directive.

## Editing and building ESCRIPT source

#### ESCRIPT Standard
To build ESCRIPT Standard just download ZIP-archive of `master` branch, unpack it and open a project file (`*.sln`) in Visual Studio. Then you can build it using default Visual Studio methods.

#### ESCRIPT Core
Everything is just like in ESCRIPT Standard, but the code is in `Core` folder. Instead of building in Visual Studio you have to use `*.bat` files to build for another platforms.

![image](https://user-images.githubusercontent.com/25367511/50042666-baafd980-006e-11e9-8edf-9e0eb9a05a3b.png)

### Code Synchronization
If you are going to edit ESCRIPT source code, it's recommended to run `Compare Core And Standard Source Files And Replace Them With The Newest.bat` after editing the code. It will check files of both editions for changes and replace them with the newest. But don't forget to use `IsCore` directive. Example:
```csharp
public object CheckIsCore()
{
  #if IsCore
  return true; // if it's ESCRIPT Core project compiler will use this code
  #else
  return false; // if another (Standard) compiler will you use this code
  #endif
}
```
Not all files will be replaced, only which mentioned in `files.txt`:
```01.ico;02.ico;Aero.cs;Cmd.cs;EVariable.cs;FileAssociation.cs;Functions.cs;GlobalVars.cs;Program.cs;Variables.cs;TCPConnection.cs;ConsoleMessageBox.cs;EConsole.cs;InstallScript.es;TemplateScript.es;ExampleFolder.es;InsertIcons.exe;ESCode.cs;UpdateScript.es;API.cs;Functions.es;EMethod.cs```

#### Note
The compilation process is designed to be done on Windows.

## Links

[GitHub](https://github.com/feel-the-dz3n/escript)<br>
[Discord](https://discord.gg/jXcjuqv)<br>
[VK](https://vk.com/dz3n.escript)<br>

## Credits

[Dz3n](https://github.com/feel-the-dz3n)
