using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Reloader;

public static class Memory
{
    public static long WriteJump(long memory, long destination)
    {
        if (IntPtr.Size == 8)
        {
            memory = WriteBytes(memory, new byte[] { 72, 184 });
            memory = WriteLong(memory, destination);
            memory = WriteBytes(memory, new byte[] { 255, 224 });
        }
        else
        {
            memory = WriteByte(memory, 104);
            memory = WriteInt(memory, (int)destination);
            memory = WriteByte(memory, 195);
        }

        return memory;
    }

    private static RuntimeMethodHandle GetRuntimeMethodHandle(MethodBase method)
    {
        if (method is not DynamicMethod)
        {
            return method.MethodHandle;
        }

        var bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic;
        var method2 = typeof(DynamicMethod).GetMethod("GetMethodDescriptor", bindingAttr);
        if ((object)method2 != null)
        {
            return (RuntimeMethodHandle)method2.Invoke(method, Array.Empty<object>());
        }

        var field = typeof(DynamicMethod).GetField("m_method", bindingAttr);
        if ((object)field != null)
        {
            return (RuntimeMethodHandle)field.GetValue(method);
        }

        return (RuntimeMethodHandle)typeof(DynamicMethod).GetField("mhandle", bindingAttr)?.GetValue(method)!;
    }

    public static long GetMethodStart(MethodBase method)
    {
        var runtimeMethodHandle = GetRuntimeMethodHandle(method);
        RuntimeHelpers.PrepareMethod(runtimeMethodHandle);
        return runtimeMethodHandle.GetFunctionPointer().ToInt64();
    }

    public static unsafe long WriteByte(long memory, byte value)
    {
        var ptr = (byte*)memory;
        *ptr = value;
        return memory + 1;
    }

    public static long WriteBytes(long memory, byte[] values)
    {
        foreach (var value in values)
        {
            memory = WriteByte(memory, value);
        }

        return memory;
    }

    public static unsafe long WriteInt(long memory, int value)
    {
        var ptr = (int*)memory;
        *ptr = value;
        return memory + 4;
    }

    public static unsafe long WriteLong(long memory, long value)
    {
        var ptr = (long*)memory;
        *ptr = value;
        return memory + 8;
    }
}