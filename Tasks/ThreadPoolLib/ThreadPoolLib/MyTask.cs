using System;
namespace ThreadPoolLib;

internal delegate void Func();

public class ThreadMyTask<TResult> : IMyTask<TResult>, IMyRunnableTask
{
    private MyThreadPool executor;
    private TResult? result;
    private Queue<IMyRunnableTask> queue = new Queue<IMyRunnableTask>();
    private Func<TResult> task;
    private CountdownEvent resultCountdownEvent = new CountdownEvent(1);
    private Exception? thrownException = null;
    private bool canceled = false;

    public bool IsCompleted { get; private set; }
    public TResult? Result
    {
        get
        {
            if (IsCompleted)
            {
                return result;
            }
            resultCountdownEvent.Wait();
            if (canceled)
            {
                throw new Exception("Task canceled because of disposing ThreadPool object!");
            }
            if (thrownException != null)
            {
                throw thrownException;
            }
            return result;
        }
        private set
        {
            result = value;
            IsCompleted = true;
        }
    }

    public ThreadMyTask(Func<TResult> task, MyThreadPool executor)
    {
        this.task = task;
        this.executor = executor;
        this.result = default(TResult?);
    }

    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> task)
    {
        if (canceled)
        {
            throw new Exception("Trying to enqueue to desposed MyThreadPool!");
        }
        Func<TNewResult> funcTask = () => {
            if (Result != null)
            {
                return task(Result);
            }
            else throw new ArgumentNullException("Result is null!"); };
        ThreadMyTask<TNewResult> threadTask = new ThreadMyTask<TNewResult>(funcTask, executor);
        if (IsCompleted)
        {
            executor.EnqueueRunnable(threadTask);
        }
        else
        {
            lock (queue)
            {
                queue.Enqueue(threadTask); 
            }
        }
        return threadTask;
    }

    public void Run()
    {
        try
        {
            Result = task();
        }
        catch (Exception e)
        {
            thrownException = new AggregateException(e);
        }
        resultCountdownEvent.Signal();
        lock (queue)
        {
            while (queue.Count > 0)
            {
                executor.EnqueueRunnable(queue.Dequeue());
            }
        }
    }

    public void Cancel()
    {
        canceled = true;
        resultCountdownEvent.Dispose();
        lock (queue)
        {
            foreach (var task in queue)
            {
                task.Cancel();
            }
        }
    }
}

