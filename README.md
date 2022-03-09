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

For example:

```
dotnet run -c Release -- path/to/project/bin/net6.0/Release/*.dll
```

## Sample usage
```
dotnet run -c Release -- System.Private.CoreLib.dll
```

Output:

```
Analysis for System.Private.CoreLib.dll:
Possible array upcast System.Type[] <- System.RuntimeType[] at RuntimeTypeHandle.SatisfiesConstraints: Type[] inHandles = typeContext;

Possible array upcast System.Type[] <- System.RuntimeType[] at RuntimeType.GetInterfaces: Type[] array = interfaceList;

Possible array upcast System.Object[] <- System.Reflection.MemberInfo[] at RuntimeType.GetMembers: object[] array2 = array;

Possible array upcast System.Reflection.MemberInfo[] <- System.Reflection.MethodBase[] at RuntimeType.GetMember: MemberInfo[] array2 = new MethodBase[num];

Possible array upcast System.Object[] <- System.Reflection.MemberInfo[] at RuntimeType.GetMember: object[] array4 = array3;

Possible array upcast System.Type[] <- System.RuntimeType[] at RuntimeType.MakeGenericType: Type[] inst = array;

Possible array upcast System.Object[] <- System.String[] at RuntimeType.IsEnumDefined: object[] array2 = array;

Possible array upcast System.Reflection.MethodBase[] <- System.Reflection.MethodInfo[] at RuntimeType.InvokeMember: MethodBase[] match = array4;

Possible array upcast System.Object[] <- System.String[] at Type.IsEnumDefined: object[] array = enumNames;

Possible array upcast System.Object[] <- System.Reflection.MethodInfo[] at ResourceReader.InitializeBinaryFormatter: object[] parameters = new MethodInfo[1] { method };

Possible array upcast System.Type[] <- System.RuntimeType[] at RuntimePropertyInfo.ToString: Type[] parameterTypes = arguments;

End of analysis.
```