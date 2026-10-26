using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Reloader;

public static class Memory
{
    public static void WriteJump(long memory, long destination)
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

    private static long writeByte(long memory, byte value)
    {
        Marshal.WriteByte(new IntPtr(memory), value);
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

    private static long writeInt(long memory, int value)
    {
        Marshal.WriteInt32(new IntPtr(memory), value);
        return memory + 4;
    }

    private static long writeLong(long memory, long value)
    {
        Marshal.WriteInt64(new IntPtr(memory), value);
        return memory + 8;
    }
}