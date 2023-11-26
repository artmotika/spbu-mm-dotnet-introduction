using System;
namespace ThreadPoolLib;

internal interface IMyRunnableTask
{
    void Run();
    void Cancel();
}

