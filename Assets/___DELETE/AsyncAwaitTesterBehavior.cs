using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace codexhere {
    [ExecuteAlways]
    public class AsyncAwaitTesterBehavior : MonoBehaviour {
        public bool Reload = false;

        async void Update() {
            if (!Reload) {
                return;
            }

            Reload = false;
            transform.rotation = Quaternion.Euler(0, 0, 0);

            int value;
            Stopwatch timer = new();
            Stopwatch subTimer = new();
            timer.Start();
            subTimer.Start();
            Debug.Log("Starting Job Sequence");

            // // Simple Loop Test
            // Debug.Log("Starting Simple Loop (App will be frozen)...");
            // await Task.Delay(250);
            // subTimer.Restart();
            // value = SimpleLoop();
            // Debug.LogFormat("Simple Loop > Time {0} | Value: {1}", subTimer.ElapsedMilliseconds, value);

            // await Task.Delay(3000);

            // // SimpleAwaitYieldLoop Test
            // Debug.Log("Starting SimpleAwaitYieldLoop (App should be responsive)...");
            // subTimer.Restart();
            // value = await SimpleAwaitYieldLoop();
            // Debug.LogFormat("SimpleAwaitYieldLoop > Time {0} | Value: {1}", subTimer.ElapsedMilliseconds, value);

            // Task.Run() Loop Test
            Debug.Log("Starting Task.Run() Loop (App should be responsive)...");
            subTimer.Restart();
            value = await TaskRunLoop();
            Debug.LogFormat("Task.Run() Loop > Time {0} | Value: {1}", subTimer.ElapsedMilliseconds, value);

            Debug.LogFormat("Ending Job Sequence > Time {0}", timer.ElapsedMilliseconds);
            timer.Stop();
        }

        int SimpleLoop() {
            int val = 0;
            Vector3 rotateVec = new(1, 1);

            for (int i = 0; i <= 5_000_000; i++) {
                transform.Rotate(rotateVec, Space.Self);
                val = i;
            }

            return val;
        }

        async Task<int> SimpleAwaitYieldLoop() {
            int val = 0;
            Vector3 rotateVec = new(1, 1);

            for (int i = 0; i <= 100; i++) {
                transform.Rotate(rotateVec, Space.Self);
                val = i;
                await Task.Yield();
            }

            return val;
        }

        Task<int> TaskRunLoop() {
            var task = Task.Run(() => {
                int val = 0;
                Vector3 rotateVec = new(1, 1);

                for (int i = 0; i <= 1_000_000_000; i++) {
                    val = i;
                }

                return val;
            });

            _ = task.ConfigureAwait(false);


            return task;
        }
    }
}
