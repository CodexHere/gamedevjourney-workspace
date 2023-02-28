

using codexhere.System;
using Unity.Jobs;

public abstract class JobQueueBuilder : IBaseDisposable {
    public bool IsCompleted => jobHandle.IsCompleted;

    protected JobHandle jobHandle = default;

    public void Complete() {
        if (!jobHandle.IsCompleted) {
            return;
        }

        jobHandle.Complete();
    }

    public void Cancel() {
        Complete();
        Dispose();
    }
}
