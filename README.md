# Array Upcast Analyzer
An analyzer to detect array upcasts in your assembly.

## Build
```
dotnet build -c Release
```

## Run
```
dotnet run -c Release -- <path/to/assembly>...
```

Wildcard in file name is supported.

## Example usage
```
dotnet run -c Release -- path/to/project/bin/net6.0/Release/*.dll
```
