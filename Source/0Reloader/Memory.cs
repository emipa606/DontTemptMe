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
            memory = writeBytes(memory, [72, 184]);
            memory = writeLong(memory, destination);
            memory = writeBytes(memory, [255, 224]);
        }
        else
        {
            memory = writeByte(memory, 104);
            memory = writeInt(memory, (int)destination);
            memory = writeByte(memory, 195);
        }

        return memory;
    }

    private static RuntimeMethodHandle getRuntimeMethodHandle(MethodBase method)
    {
        if (method is not DynamicMethod)
        {
            return method.MethodHandle;
        }

        const BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic;
        var method2 = typeof(DynamicMethod).GetMethod("GetMethodDescriptor", bindingAttr);
        if ((object)method2 != null)
        {
            return (RuntimeMethodHandle)method2.Invoke(method, []);
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
        var runtimeMethodHandle = getRuntimeMethodHandle(method);
        RuntimeHelpers.PrepareMethod(runtimeMethodHandle);
        return runtimeMethodHandle.GetFunctionPointer().ToInt64();
    }

    private static unsafe long writeByte(long memory, byte value)
    {
        var ptr = (byte*)memory;
        *ptr = value;
        return memory + 1;
    }

    private static long writeBytes(long memory, byte[] values)
    {
        foreach (var value in values)
        {
            memory = writeByte(memory, value);
        }

        return memory;
    }

    private static unsafe long writeInt(long memory, int value)
    {
        var ptr = (int*)memory;
        *ptr = value;
        return memory + 4;
    }

    private static unsafe long writeLong(long memory, long value)
    {
        var ptr = (long*)memory;
        *ptr = value;
        return memory + 8;
    }
}