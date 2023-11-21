

using codexhere.System;
using Unity.Jobs;

public abstract class JobQueueBuilder : IBaseDisposable {
    public bool IsCompleted => jobHandle.IsCompleted;

    protected JobHandle jobHandle = default;

    public bool Complete() {
        if (!jobHandle.IsCompleted) {
            return false;
        }

        jobHandle.Complete();

        return true;
    }

    public void Cancel() {
        //TODO : Test this actually cancells operating and disposes properly
        _ = Complete();
        Dispose();
    }
}
