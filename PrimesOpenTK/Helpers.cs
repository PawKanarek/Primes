using System;
using System.Diagnostics;
using System.Threading;

namespace PrimesOpenTK
{
    public static class Helpers
    {
        public static void Print(object sender, string message = null, int stackFrame = 1)
        {
            System.Reflection.MethodBase parentMethod = new StackFrame(stackFrame)?.GetMethod();
            var prefix = string.Empty;
            if (parentMethod != null)
            {
                var parentObject = parentMethod.DeclaringType?.Name;
                prefix = $"{parentObject}.{parentMethod.Name} ";
            }

            Console.WriteLine($"{DateTimeOffset.Now.ToString("HH:mm:ss.FFF"),-12} T:{Thread.CurrentThread.ManagedThreadId,-3}{sender.GetHashCode(),-12} {prefix}{message}");
        }
    }
}
