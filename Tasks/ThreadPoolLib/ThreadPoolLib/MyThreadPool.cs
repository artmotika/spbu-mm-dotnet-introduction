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
        if (numThreads < 1) throw new ArgumentException("Number threads can't be lower than 1!");
        NumThreads = numThreads;
        Array.Resize(ref pool, (int)numThreads);
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
            WaitHandle.WaitAny(waitHandles);
            Interlocked.Increment(ref numWaitingThreads);
            IMyRunnableTask? task = null;
            lock (queue)
            {
                queue.TryDequeue(out task);
                Interlocked.Decrement(ref numWaitingThreads);
                /*
                 * Нужно если taskWaitHandle.Set(); выполнится при добавлении
                 * тасков несколько раз подряд, тем самым не разблокирует 
                 * нужное кол-во потоков, и чтобы не будить потоков больше,
                 * чем нам нужно, проверяется, что кол-во потоков, которые ждут
                 * таску из очереди меньше кол-ва тасок в очереди.
                 */
                if (numWaitingThreads < queue.Count)
                {
                    taskWaitHandle.Set();
                }
            }
            if (task != null)
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
                taskWaitHandle.Set();
            }
        }
    }

    public void Dispose()
    {   
        cancellationTokenSource.Cancel();
        lock (queue)
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
