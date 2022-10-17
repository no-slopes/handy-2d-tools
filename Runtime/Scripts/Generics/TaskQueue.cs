
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace H2DT.Generics.Threading
{
    public class TaskQueue
    {
        public UnityEvent StartOfTasks = new UnityEvent();
        public UnityEvent EndOfTasks = new UnityEvent();

        private SemaphoreSlim _semaphore;

        public TaskQueue()
        {
            _semaphore = new SemaphoreSlim(1);
        }

        public async Task<T> Enqueue<T>(Func<Task<T>> task)
        {
            T result;

            await _semaphore.WaitAsync();

            try
            {
                result = await task();
            }
            finally
            {
                _semaphore.Release();
            }

            return result;
        }

        public async Task Enqueue(Func<Task> task)
        {
            await _semaphore.WaitAsync();

            try
            {
                await task();
            }
            finally
            {
                _semaphore.Release();
            }

            return;
        }
    }
}