using System;
using System.Threading;

namespace ThreadPoolLib;

public class MyThreadPool : IDisposable
{
    private Queue<IMyRunnableTask> queue = new Queue<IMyRunnableTask>();
    private Thread[] pool;
    private EventWaitHandle taskWaitHandle = new(false, EventResetMode.AutoReset);
    private uint numWaitingThreads = 0;
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    private CancellationToken cancellationToken;

    private uint NumThreads { get; init; }

    public MyThreadPool(uint numThreads)
    {
        cancellationToken = cancellationTokenSource.Token;
        Array.Resize(ref pool, (int) numThreads);
        if (numThreads < 1) throw new ArgumentException("Number threads can't be lower than 1!");
        NumThreads = numThreads;
        for (var i = 0; i < numThreads; i++)
        {
            Thread thread = new Thread(() => RunThreadWorker(cancellationToken));
            thread.Start();
            pool[i] = thread;
        }
    }

    private void RunThreadWorker(CancellationToken cancellationToken)
    {
        WaitHandle[] waitHandles = { cancellationToken.WaitHandle, taskWaitHandle };    
        while (!cancellationToken.IsCancellationRequested)
        {
            IMyRunnableTask? task = null;
            lock (queue)
            {
                if (queue.Count > 0)
                {
                    task = queue.Dequeue();
                }
            }
            if (task == null)
            {
                Interlocked.Increment(ref numWaitingThreads);
                WaitHandle.WaitAny(waitHandles);
                Interlocked.Decrement(ref numWaitingThreads);
            }
            else
            {
                task.Run();
            } 
        }

    }

    public IMyTask<TResult> Enqueue<TResult>(Func<TResult> task)
    {
        var threadTask = new ThreadMyTask<TResult>(task, this);
        EnqueueRunnable(threadTask);
        return threadTask;
    }

    internal void EnqueueRunnable(IMyRunnableTask task)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            throw new Exception("Trying to enqueue to desposed MyThreadPool!");
        }
        else
        {
            lock (queue)
            {
                queue.Enqueue(task);
                if (numWaitingThreads > 0)
                {
                    taskWaitHandle.Set();
                }
            }
        }
    }

    public void Dispose()
    {   
        cancellationTokenSource.Cancel();
        lock(queue)
        {
            foreach (var task in queue)
            {
                task.Cancel();
            }
        }
        foreach (var thread in pool)
        {
            thread.Join();
        }
    }
}
