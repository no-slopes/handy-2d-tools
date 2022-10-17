using System.Reflection;
using System.Linq;
using System;
using H2DT.Debugging;

namespace H2DT.Utils
{

    public static class Reflection
    {
        public static void InvokeIfExists(this object evaluatedObject, string methodName)
        {
            if (evaluatedObject == null) return;

            Type type = evaluatedObject.GetType();

            try
            {
                var method = type.GetMethod(methodName);

                if (method != null)
                    method.Invoke(evaluatedObject, null);
            }
            catch (AmbiguousMatchException)
            {
                var method = type.GetMethods().FirstOrDefault(m => m.Name == methodName);

                if (method != null)
                    method.Invoke(evaluatedObject, null);

                Log.Warning($"Multiple {methodName} methods found for {type.Name}. Have in mind that this may confuse the StateMachine.");
            }
        }

        public static MethodInfo HasMethod(this object evaluatedObject, string methodName)
        {
            if (evaluatedObject == null) return null;

            Type type = evaluatedObject.GetType();

            try
            {
                MethodInfo methodInfo = type.GetMethod(methodName);

                if (methodInfo != null)
                    return methodInfo;

                return null;
            }
            catch (AmbiguousMatchException)
            {
                return type.GetMethods().FirstOrDefault(m => m.Name == methodName);
            }
        }
    }
}