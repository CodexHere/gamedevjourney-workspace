

using codexhere.System;
using codexhere.Unity.Jobs;

public abstract class JobQueueBuilder : IBaseDisposable {
    protected JobHandleExtended jobHandle = default;

    public void Complete() {
        if (!jobHandle.CanComplete) {
            return;
        }

        jobHandle.Complete();
    }

    public void Cancel() {
        Complete();
        Dispose();
    }
}
