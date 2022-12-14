
using System;
using System.Collections.Generic;
using System.Threading;
using H2DT.Management.Levels;
using UnityEngine;
using UnityEngine.Events;

namespace H2DT.Utils
{
    public static class Retry
    {
        public static void Do(Action action, TimeSpan retryInterval, int maxAttemptCount = 3)
        {
            Do<object>(() =>
            {
                action();
                return null;
            }, retryInterval, maxAttemptCount);
        }

        public static T Do<T>(Func<T> action, TimeSpan retryInterval, int maxAttemptCount = 3)
        {
            var exceptions = new List<Exception>();

            for (int attempted = 0; attempted < maxAttemptCount; attempted++)
            {
                try
                {
                    if (attempted > 0)
                    {
                        Thread.Sleep(retryInterval);
                    }

                    return action();
                }
                catch (Exception ex)
                {
                    Debug.Log("Caught");
                    exceptions.Add(ex);
                }
            }

            throw new AggregateException(exceptions);
        }

    }
}