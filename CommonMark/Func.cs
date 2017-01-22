using System;

namespace CommonMark
{
#if v2_0
    /// <summary>An alternative to <c>System.Func</c> which is not present in .NET 2.0.</summary>
    public delegate TResult Func<out TResult>();

    /// <summary>An alternative to <c>System.Func</c> which is not present in .NET 2.0.</summary>
    public delegate TResult Func<in T, out TResult>(T arg);

    /// <summary>An alternative to <c>System.Action</c> which is not present in .NET 2.0.</summary>
    public delegate void Action<in T1, in T2, in T3>(T1 arg1, T2 arg2, T3 arg3);
#endif
}
