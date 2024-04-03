using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using RimWorld;
using Verse;

namespace Reloader;

internal class Reloader : Mod
{
    public static readonly BindingFlags allBindings =
        BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
        BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.SetField |
        BindingFlags.GetProperty | BindingFlags.SetProperty;

    private readonly string assemblyPath;

    private readonly ModContentPack content;
    private readonly Dictionary<string, MethodInfo> reloadableMethods = new Dictionary<string, MethodInfo>();

    public Reloader(ModContentPack content) : base(content)
    {
        this.content = content;
        LongEventHandler.QueueLongEvent(CacheExstingMethods, "CacheExstingMethods", false, null);
        var path = Path.Combine(content.RootDir, VersionControl.CurrentVersionStringWithoutBuild);
        assemblyPath = Path.Combine(path, "Assemblies");
        var fileSystemWatcher = new FileSystemWatcher();
        fileSystemWatcher.Path = assemblyPath;
        fileSystemWatcher.Filter = "*.dll";
        fileSystemWatcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.DirectoryName |
                                         NotifyFilters.FileName | NotifyFilters.LastWrite;

        fileSystemWatcher.Created += Value;
        fileSystemWatcher.Changed += Value;
        fileSystemWatcher.EnableRaisingEvents = true;
        return;

        void Value(object sender, FileSystemEventArgs args)
        {
            var fullPath = args.FullPath;
            if (!ExcludedDLLs(fullPath))
            {
                LoadPath(fullPath);
            }
        }
    }

    private void CacheExstingMethods()
    {
        AppDomain.CurrentDomain.GetAssemblies().Where(delegate(Assembly assembly)
            {
                var fullName = assembly.FullName;
                fullName = fullName.Split(',')[0];
                var path = Path.Combine(assemblyPath, $"{fullName}.dll");
                return File.Exists(path) && !ExcludedDLLs(path);
            }).ToList()
            .ForEach(delegate(Assembly assembly)
            {
                Log.Warning($"Reloader: analyzing {assembly.FullName}");
                assembly.GetTypes().ToList().ForEach(delegate(Type type)
                {
                    type.GetMethods(allBindings).ToList().ForEach(delegate(MethodInfo method)
                    {
                        if (!method.TryGetAttribute<ReloadMethod>(out var _))
                        {
                            return;
                        }

                        var text = $"{method.DeclaringType?.FullName}.{method.Name}";
                        reloadableMethods[text] = method;
                        _ = method.DeclaringType;
                        Log.Warning($"Reloader: found reloadable method {text}");
                    });
                });
            });
    }

    private void LoadPath(string path)
    {
        Assembly.Load(File.ReadAllBytes(path)).GetTypes().ToList()
            .ForEach(delegate(Type type)
            {
                type.GetMethods(allBindings).ToList().ForEach(delegate(MethodInfo newMethod)
                {
                    if (!newMethod.TryGetAttribute<ReloadMethod>(out var _))
                    {
                        return;
                    }

                    var text = $"{newMethod.DeclaringType?.FullName}.{newMethod.Name}";
                    Log.Warning($"Reloader: patching {text}");
                    var methodInfo = reloadableMethods[text];
                    if ((object)methodInfo != null)
                    {
                        var methodStart = Memory.GetMethodStart(methodInfo);
                        var methodStart2 = Memory.GetMethodStart(newMethod);
                        Memory.WriteJump(methodStart, methodStart2);
                    }
                    else
                    {
                        Log.Warning("Reloader: original missing");
                    }
                });
            });
    }

    private bool ExcludedDLLs(string path)
    {
        return path.EndsWith("0Reloader.dll");
    }
}