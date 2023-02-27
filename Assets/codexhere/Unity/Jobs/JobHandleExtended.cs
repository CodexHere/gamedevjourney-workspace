// https://forum.unity.com/threads/from-docs-it-is-not-recommended-to-complete-a-job-immediately.515181/#post-5464704

using Unity.Jobs;

namespace codexhere.Unity.Jobs {

    public enum JobHandleStatus {
        Running,
        AwaitingCompletion,
        Completed
    }

    public struct JobHandleExtended {
        public JobHandle handle;
        private JobHandleStatus status;

        public bool CanComplete => JobHandleStatus.AwaitingCompletion == Status;

        public JobHandleExtended(JobHandle handle) : this() =>
            //by default status is Running
            this.handle = handle;

        public JobHandleStatus Status {
            get {
                if (status == JobHandleStatus.Running && handle.IsCompleted) {
                    status = JobHandleStatus.AwaitingCompletion;
                }

                return status;
            }
        }

        public void Complete() {
            handle.Complete();
            status = JobHandleStatus.Completed;
        }

        public static implicit operator JobHandle(JobHandleExtended extended) {
            return extended.handle;
        }

        public static implicit operator JobHandleExtended(JobHandle handle) {
            return new JobHandleExtended(handle);
        }
    }

}