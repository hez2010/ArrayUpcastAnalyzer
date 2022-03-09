using System.Text;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Semantics;

static void Traverse(AstNode node)
{
    if (node is VariableDeclarationStatement
        {
            Type: ComposedType
            {
                ArraySpecifiers: { Count: > 0 } specifiers,
                Annotations: not null
            } type,
            Variables: { Count: > 0 } variables
        })
    {
        var target = type.Annotations.SelectMany(i => i is TypeResolveResult ti ? new[] { ti } : Array.Empty<TypeResolveResult>()).OrderByDescending(i => i.Type.FullName.Length).First();
        var stloc = variables.First().Annotation<StLoc>();
        if (stloc is { Value: LdLoc or NewArr or Block { Kind: BlockKind.ArrayInitializer, Children.Count: > 0 } })
        {
            var value = stloc.Value;
            var stack = new Stack<EntityDeclaration>();
            var cur = node.Parent;
            while (cur is not null)
            {
                if (cur is EntityDeclaration ed) stack.Push(ed);
                cur = cur.Parent;
            }
            var sigSb = new StringBuilder();
            while (stack.TryPop(out var e))
            {
                sigSb.Append('.');
                sigSb.Append(e.Name);
            }
            if (sigSb.Length > 0) sigSb.Remove(0, 1);

            switch (value)
            {
                case LdLoc { Variable: ILVariable variable }:
                    {
                        if (target.Type.ReflectionName != variable.Type.ReflectionName)
                        {
                            Console.WriteLine($"Possible array upcast {target.Type.ReflectionName} <- {variable.Type.ReflectionName} at {sigSb}: {node}");
                        }
                        break;
                    }
                case NewArr { Type: var elemType }:
                    {
                        var sb = new StringBuilder();
                        sb.Append('[');
                        sb.Append(',', specifiers.Last().Dimensions - 1);
                        sb.Append(']');
                        if (target.Type.ReflectionName != $"{elemType.ReflectionName}{sb}")
                        {
                            Console.WriteLine($"Possible array upcast {target.Type.ReflectionName} <- {elemType.ReflectionName}{sb} at {sigSb}: {node}");
                        }
                        break;
                    }
                case Block { Children: var children }
                    when children[0] is StLoc { Children: { Count: > 0 } innerChildren } &&
                        innerChildren[0] is NewArr { Type: var elemType }:
                    {
                        var sb = new StringBuilder();
                        sb.Append('[');
                        sb.Append(',', specifiers.Last().Dimensions - 1);
                        sb.Append(']');
                        if (target.Type.ReflectionName != $"{elemType.ReflectionName}{sb}")
                        {
                            Console.WriteLine($"Possible array upcast {target.Type.ReflectionName} <- {elemType.ReflectionName}{sb} at {sigSb}: {node}");
                        }
                        break;
                    }
            }
        }
    }

    foreach (var child in node.Children)
    {
        Traverse(child);
    }
}

foreach (var arg in args)
{
    var dir = Path.GetDirectoryName(arg);
    if (dir is null) continue;
    foreach (var file in Directory.GetFiles(dir, Path.GetFileName(arg)))
    {
        var fileName = Path.GetFileName(file);
        Console.WriteLine($"Analysis for {fileName}:");
        try
        {
            var decompiler = new CSharpDecompiler(file, new DecompilerSettings(LanguageVersion.CSharp1));
            var syntaxTree = decompiler.DecompileWholeModuleAsSingleFile();
            Traverse(syntaxTree);
        }
        catch (PEFileNotSupportedException)
        {
            Console.WriteLine("Skip due to not appliable: no managed code.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Skip due to exception: {ex.Message}");
        }
        finally
        {
            Console.WriteLine($"End of analysis.");
            Console.WriteLine();
        }
    }
}