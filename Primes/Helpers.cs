using System.Diagnostics;

namespace Primes
{
    public static class Helpers
    {
        public static void Print(object sender, string message, int stackFrame = 1)
        {
            System.Reflection.MethodBase parentMethod = new StackFrame(stackFrame)?.GetMethod();
            var prefix = string.Empty;
            if (parentMethod != null)
            {
                var parentObject = parentMethod.DeclaringType?.Name;
                prefix = $"{parentObject}.{parentMethod.Name} ";
            }

            Debug.WriteLine($"{sender.GetHashCode(),-12} {prefix}{message}");
        }
    }
}
