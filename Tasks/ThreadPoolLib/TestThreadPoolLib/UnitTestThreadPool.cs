using System;
using ThreadPoolLib;

namespace TestThreadPoolLib;

[TestClass]
public class UnitTestThreadPool
{
    private int GetNthFibonacci_Rec(int n)
    {
        if ((n == 0) || (n == 1))
        {
            return n;
        }
        else
            return GetNthFibonacci_Rec(n - 1) + GetNthFibonacci_Rec(n - 2);
    }

    [TestMethod]
    public void TestThreadPoolWith4ThreadsAndManyTasksAnd6ContinueWith()
    {
        var tp = new MyThreadPool(4);
        int n = 20;
        int[] resultsExpected = { 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233, 377, 610, 987, 1597, 2584, 4181, 6765 };
        string[] continueResultsExpected = { "-14", "-18", "-20", "-221964123", "-8", "-8", "-8", "1", "1", "1", "1", "1,9",
            "1,9", "10", "104,5", "1156", "1159", "1190", "12", "125", "1258", "12649337", "12853,5", "138736", "142129",
            "142506", "143260", "15,2", "166375", "169", "169,1", "17443132", "17480761", "17484942", "17493304", "18", "182",
            "1875,3", "19440", "2", "2", "20736", "208", "20880", "21168", "2197", "226981000", "24,7", "25", "252", "2530",
            "2536036", "2550409", "2552006", "2555200", "27", "273,59999999999997", "2985984", "3,8", "30", "3025",
            "3034,2999999999997", "3080", "3190", "364101813", "366610", "372100", "372710", "373930", "39,9", "39304",
            "4", "4", "4", "40", "441", "442,7", "45704340", "45765225", "45771990", "45785520", "462", "4909,599999999999",
            "5,699999999999999", "504", "512", "52", "52192", "53582633", "54289", "54522", "54988", "6", "64", "64,6",
            "6653800", "6677056", "6679640", "6684808", "704969", "7120", "716,3", "72", "72617709", "73643520", "7921",
            "7943,9", "8", "8010", "8188", "850", "88", "9", "9,5", "9261", "961504803", "965286", "974169", "975156", "977130" };
        int[] results = new int[n];
        string[] continueResults = new string[n * 6];
        List<IMyTask<int>> tasks = new List<IMyTask<int>>(n);
        List<IMyTask<string>> continueTasks = new List<IMyTask<string>>(n*6);
        for (int i = 1; i <= n; i++)
        {
            int number = i;
            tasks.Add(tp.Enqueue(() => GetNthFibonacci_Rec(number)));
        }

        foreach (var t in tasks)
        {
            var continue_t = t.ContinueWith<string>((int n) => (n * 1.9).ToString());
            continueTasks.Add(continue_t);
            continue_t = t.ContinueWith<string>((int n) => (n * n).ToString());
            continueTasks.Add(continue_t);
            continue_t = t.ContinueWith<string>((int n) => (n * n + n).ToString());
            continueTasks.Add(continue_t);
            continue_t = t.ContinueWith<string>((int n) => (n * n + 3*n).ToString());
            continueTasks.Add(continue_t);
            continue_t = t.ContinueWith<string>((int n) => (n * n * n).ToString());
            continueTasks.Add(continue_t);
            continue_t = t.ContinueWith<string>((int n) => (n * n - n * 9).ToString());
            continueTasks.Add(continue_t);
        }

        int idx = 0;
        foreach (var t in tasks)
        {
            results[idx] = t.Result;
            idx++;
        }

        idx = 0;
        foreach (var t in continueTasks)
        {
            continueResults[idx] = t.Result;
            idx++;
        }
        tp.Dispose();
        Array.Sort(results);
        Array.Sort(continueResults);
        for (int i = 0; i < n; i ++)
        {
            Assert.AreEqual(resultsExpected[i], results[i]);
        }
        for (int i = 0; i < n*6; i++)
        {
            Assert.AreEqual(continueResultsExpected[i], continueResults[i]);
        }
    }

    [TestMethod]
    public void TestThreadPoolWith1ThreadAndManyTasksAnd2ContinueWith()
    {
        var tp = new MyThreadPool(1);
        int n = 10;
        int[] resultsExpected = { 1, 1, 2, 3, 5, 8, 13, 21, 34, 55 };
        string[] continueResultsExpected = { "1", "1", "1,9", "1,9", "104,5",
            "1156", "15,2", "169", "24,7", "25", "3,8", "3025", "39,9", "4",
            "441", "5,699999999999999", "64", "64,6", "9", "9,5" };
        int[] results = new int[n];
        string[] continueResults = new string[n * 2];
        List<IMyTask<int>> tasks = new List<IMyTask<int>>();
        List<IMyTask<string>> continueTasks = new List<IMyTask<string>>();
        for (int i = 1; i <= n; i++)
        {
            int number = i;
            tasks.Add(tp.Enqueue(() => GetNthFibonacci_Rec(number)));
        }

        foreach (var t in tasks)
        {
            var continue_t = t.ContinueWith<string>((int n) => (n * 1.9).ToString());
            continueTasks.Add(continue_t);
            continue_t = t.ContinueWith<string>((int n) => (n * n).ToString());
            continueTasks.Add(continue_t);
        }

        int idx = 0;
        foreach (var t in tasks)
        {
            results[idx] = t.Result;
            idx++;
        }

        idx = 0;
        foreach (var t in continueTasks)
        {
            continueResults[idx] = t.Result;
            idx++;
        }
        tp.Dispose();
        Array.Sort(results);
        Array.Sort(continueResults);
        for (int i = 0; i < n; i++)
        {
            Assert.AreEqual(resultsExpected[i], results[i]);
        }
        for (int i = 0; i < n * 2; i++)
        {
            Assert.AreEqual(continueResultsExpected[i], continueResults[i]);
        }
    }

    [TestMethod]
    public void TestThreadPoolWith10ThreadsAndALittleTasks()
    {
        var tp = new MyThreadPool(10);
        int n = 2;
        int[] resultsExpected = { 1, 1 };
        int[] results = new int[n];
        List<IMyTask<int>> tasks = new List<IMyTask<int>>(); 
        for (int i = 1; i <= n; i++)
        {
            int number = i;
            tasks.Add(tp.Enqueue(() => GetNthFibonacci_Rec(number)));
        }

        int idx = 0;
        foreach (var t in tasks)
        {
            results[idx] = t.Result;
            idx++;
        }
        tp.Dispose();
        Array.Sort(results); 
        for (int i = 0; i < n; i++)
        {
            Assert.AreEqual(resultsExpected[i], results[i]);
        }
    }

    [TestMethod]
    public void TestThreadPoolWith10ThreadsAnd1TaskAnd4ContinueWith()
    {
        var tp = new MyThreadPool(10);
        int n = 1;
        int[] resultsExpected = { 1 };
        string[] continueResultsExpected = { "-18", "1", "1", "1,9" };
        int[] results = new int[n];
        string[] continueResults = new string[n * 4];
        List<IMyTask<int>> tasks = new List<IMyTask<int>>();
        List<IMyTask<string>> continueTasks = new List<IMyTask<string>>();
        for (int i = 1; i <= n; i++)
        {
            int number = i;
            tasks.Add(tp.Enqueue(() => GetNthFibonacci_Rec(number)));
        }

        foreach (var t in tasks)
        {
            var continue_t = t.ContinueWith<string>((int n) => (n * 1.9).ToString());
            continueTasks.Add(continue_t);
            continue_t = t.ContinueWith<string>((int n) => (n * n).ToString());
            continueTasks.Add(continue_t);
            continue_t = t.ContinueWith<string>((int n) => (n * n * n).ToString());
            continueTasks.Add(continue_t);
            continue_t = t.ContinueWith<string>((int n) => (n * n - 19).ToString());
            continueTasks.Add(continue_t);
        }

        int idx = 0;
        foreach (var t in tasks)
        {
            results[idx] = t.Result;
            idx++;
        }

        idx = 0;
        foreach (var t in continueTasks)
        {
            continueResults[idx] = t.Result;
            idx++;
        }
        tp.Dispose();
        Array.Sort(results);
        Array.Sort(continueResults);
        for (int i = 0; i < n; i++)
        {
            Assert.AreEqual(resultsExpected[i], results[i]);
        }
        for (int i = 0; i < n * 2; i++)
        {
            Assert.AreEqual(continueResultsExpected[i], continueResults[i]);
        }
    }

    [TestMethod]
    public void TestThreadPoolWithTaskWithException()
    {
        var tp = new MyThreadPool(10);
        Exception exception = new ArgumentException();
        IMyTask<bool> task = tp.Enqueue<bool>(() => throw exception);
        AggregateException aggregateException = Assert.ThrowsException<AggregateException>(() => task.Result);
        Assert.AreSame(exception, aggregateException.InnerException);
        tp.Dispose();
    }

    [TestMethod]
    public void TestThreadPoolWithTaskWithExceptionAndContinueWith()
    {
        var tp = new MyThreadPool(10);
        Exception exception = new ArgumentException();
        IMyTask<bool> task = tp.Enqueue<bool>(() => throw exception);
        IMyTask<bool> continueTask = task.ContinueWith<bool>((b) => !b);
        AggregateException aggregateException = Assert.ThrowsException<AggregateException>(() => task.Result);
        AggregateException aggregateExceptionContinue = Assert.ThrowsException<AggregateException>(() => continueTask.Result);        
        Assert.AreSame(exception, aggregateException.InnerException);
        Assert.AreSame(exception, aggregateExceptionContinue?.InnerException?.InnerException);
        tp.Dispose();
    }

    [TestMethod]
    public void TestThreadPoolWithTaskAndContinueWithWithException()
    {
        var tp = new MyThreadPool(10);
        IMyTask<bool> task;
        IMyTask<bool> continueTask;
        Exception exception = new ArgumentException();
        task = tp.Enqueue<bool>(() => true);
        continueTask = task.ContinueWith<bool>((b) => throw exception);
        AggregateException aggregateExceptionContinue = Assert.ThrowsException<AggregateException>(() => continueTask.Result);
        Assert.AreSame(exception, aggregateExceptionContinue.InnerException);
        tp.Dispose();
    }

    [TestMethod]
    public void TestThreadPoolWithTaskAndContinueWithIsWaitingResultOfTask()
    {
        var tp = new MyThreadPool(10);
        IMyTask<bool> task;
        IMyTask<string> continueTask;
        task = tp.Enqueue(() =>
        {
            Thread.Sleep(1000);
            return true;
        });
        continueTask = task.ContinueWith((b) => (!b).ToString());
        Assert.AreEqual("False", continueTask.Result);
        tp.Dispose();
    }

    [TestMethod]
    public void TestThreadPoolWith4ThreadsWith4TasksParallelCheck()
    {
        var tp = new MyThreadPool(4);
        IMyTask<bool> task1;
        IMyTask<bool> task2;
        IMyTask<bool> task3;
        IMyTask<bool> task4;
        task1 = tp.Enqueue(() =>
        {
            Thread.Sleep(1000);
            return true;
        });
        task2 = tp.Enqueue(() =>
        {
            Thread.Sleep(1000);
            return true;
        });
        task3 = tp.Enqueue(() =>
        {
            Thread.Sleep(1000);
            return true;
        });
        task4 = tp.Enqueue(() =>
        {
            Thread.Sleep(1000);
            return true;
        });

        Assert.IsFalse(task1.IsCompleted);
        Assert.IsFalse(task2.IsCompleted);
        Assert.IsFalse(task3.IsCompleted);
        Assert.IsFalse(task4.IsCompleted);
        Thread.Sleep(500);
        Assert.IsFalse(task1.IsCompleted);
        Assert.IsFalse(task2.IsCompleted);
        Assert.IsFalse(task3.IsCompleted);
        Assert.IsFalse(task4.IsCompleted);
        Thread.Sleep(700);
        Assert.IsTrue(task1.IsCompleted);
        Assert.IsTrue(task2.IsCompleted);
        Assert.IsTrue(task3.IsCompleted);
        Assert.IsTrue(task4.IsCompleted);
        tp.Dispose();
    }

    [TestMethod]
    public void TestThreadPoolRunningTasksMustCompletedAfterDispose()
    {
        var tp = new MyThreadPool(4);
        IMyTask<bool> task1;
        IMyTask<bool> task2;
        IMyTask<bool> task3;
        IMyTask<bool> task4;
        IMyTask<bool> task5;
        task1 = tp.Enqueue(() =>
        {
            Thread.Sleep(2000);
            return true;
        });
        task2 = tp.Enqueue(() =>
        {
            Thread.Sleep(2000);
            return true;
        });
        task3 = tp.Enqueue(() =>
        {
            Thread.Sleep(2000);
            return true;
        });
        task4 = tp.Enqueue(() =>
        {
            Thread.Sleep(2000);
            return true;
        });
        task5 = tp.Enqueue(() =>
        {
            Thread.Sleep(2000);
            return true;
        });

        Assert.IsFalse(task1.IsCompleted);
        Assert.IsFalse(task2.IsCompleted);
        Assert.IsFalse(task3.IsCompleted);
        Assert.IsFalse(task4.IsCompleted);
        Assert.IsFalse(task5.IsCompleted);
        Thread.Sleep(500);
        tp.Dispose();
        Assert.IsTrue(task1.IsCompleted);
        Assert.IsTrue(task2.IsCompleted);
        Assert.IsTrue(task3.IsCompleted);
        Assert.IsTrue(task4.IsCompleted);
        Assert.IsFalse(task5.IsCompleted);
    }

    [TestMethod]
    public void TestThreadPoolRunningTasksMustCompletedAfterDisposeCheckContinueWith()
    {
        var tp = new MyThreadPool(10);
        IMyTask<bool> task = tp.Enqueue(() =>
        {
            Thread.Sleep(1000);
            return true;
        });
        Thread.Sleep(500);
        tp.Dispose();
        Assert.AreEqual(true, task.Result);
        Assert.ThrowsException<Exception>(() => task.ContinueWith(b => !b).Result);
    }

    [TestMethod]
    public void TestThreadPoolAfterDisposeWhenAddingTask()
    {
        var tp = new MyThreadPool(10);
        tp.Dispose();
        Assert.ThrowsException<Exception>(() => tp.Enqueue(() =>
        {
            return true;
        }));
    }   
}
