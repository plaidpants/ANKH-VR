/* 
 * Native dll invocation helper by Francis R. Griffiths-Keam
 * http://runningdimensions.com/blog/?p=5
 * Feel free to use this code as you please.
 */

using System;
using System.Runtime.InteropServices;
using UnityEngine;

public static class Native
{
    public enum PosixDlopenFlags : int
    {
        Lazy = 0x00001,
        Now = 0x00002,
        Lazy_Global = 0x00100 | Lazy,
        Now_Global = 0x00100 | Now
    }

    public static T Invoke<T, T2>(IntPtr library, params object[] pars)
    {
#if PLATFORM_ANDROID && !UNITY_EDITOR
        IntPtr funcPtr = dlsym(library, typeof(T2).Name);
#else
        IntPtr funcPtr = GetProcAddress(library, typeof(T2).Name);
#endif
        if (funcPtr == IntPtr.Zero)
        {
            Debug.LogWarning("Could not gain reference to method address.");
            return default(T);
        }


#if PLATFORM_ANDROID && !UNITY_EDITOR
        var func = Marshal.GetDelegateForFunctionPointer(dlsym(library, typeof(T2).Name), typeof(T2));
#else
        var func = Marshal.GetDelegateForFunctionPointer(GetProcAddress(library, typeof(T2).Name), typeof(T2));
#endif
        return (T)func.DynamicInvoke(pars);
    }

    public static void Invoke<T>(IntPtr library, params object[] pars)
    {
#if PLATFORM_ANDROID && !UNITY_EDITOR
        IntPtr funcPtr = dlsym(library, typeof(T).Name);
#else
        IntPtr funcPtr = GetProcAddress(library, typeof(T).Name);
#endif
        if (funcPtr == IntPtr.Zero)
        {
            Debug.LogWarning("Could not gain reference to method address.");
            return;
        }

        var func = Marshal.GetDelegateForFunctionPointer(funcPtr, typeof(T));
        func.DynamicInvoke(pars);
    }

#if PLATFORM_ANDROID && !UNITY_EDITOR
    [DllImport("libdl.so")]
    public static extern IntPtr dlopen(string filename, int flags);

    [DllImport("libdl.so")]
    public static extern IntPtr dlsym(IntPtr handle, string symbol);

    [DllImport("libdl.so")]
    public static extern int dlclose(IntPtr handle);
#else
    [DllImport("kernel32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool FreeLibrary(IntPtr hModule);

    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern IntPtr LoadLibrary(string lpFileName);

    [DllImport("kernel32")]
    public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);
#endif
}