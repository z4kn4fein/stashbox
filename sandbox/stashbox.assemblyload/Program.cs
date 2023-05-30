using Stashbox;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;

IStashboxContainer container = new StashboxContainer();

LoadAndUnload(container, out var weakContext);

for (var i = 0; weakContext.TryGetTarget(out _) && i < 10; i++)
{
    GC.Collect();
    GC.WaitForPendingFinalizers();
}

Console.WriteLine(weakContext.TryGetTarget(out _));

[MethodImpl(MethodImplOptions.NoInlining)]
static void LoadAndUnload(IStashboxContainer container, out WeakReference<AssemblyLoadContext> weakContext)
{
    var context = new AssemblyLoadContext(name: "context", isCollectible: true);
    weakContext = new WeakReference<AssemblyLoadContext>(context, trackResurrection: true);
    var assembly =
        context.LoadFromAssemblyPath(Path.Combine(Path.GetDirectoryName(container.GetType().Assembly.Location) ?? throw new NullReferenceException(),
            "TestAssembly.dll"));

    container.RegisterAssembly(assembly);

    var instances = assembly.GetExportedTypes().Where(t => !t.IsGenericType).Select(exportedType => container.Resolve(exportedType)).ToList();

    Console.WriteLine(instances.Count);
    Console.WriteLine(container.GetRegistrationDiagnostics().Count());

    context.Unload();
}