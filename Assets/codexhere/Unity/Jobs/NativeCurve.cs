using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEngine;
using static Unity.Mathematics.math;

namespace codexhere.Unity.Jobs {

    public struct NativeCurve : IDisposable {
        public bool IsCreated => values.IsCreated;

        private NativeArray<float> values;
        private WrapMode preWrapMode;
        private WrapMode postWrapMode;

        private void InitializeValues(int count) {
            if (values.IsCreated)
                values.Dispose();

            values = new NativeArray<float>(count, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
        }

        public void Update(AnimationCurve curve, int resolution) {
            if (curve == null) {
                throw new NullReferenceException("AnimationCurve is null");
            }

            preWrapMode = curve.preWrapMode;
            postWrapMode = curve.postWrapMode;

            if (!values.IsCreated || values.Length != resolution) {
                InitializeValues(resolution);
            }

            for (int i = 0; i < resolution; i++) {
                values[i] = curve.Evaluate((float)i / (float)resolution);
            }
        }

        public float Evaluate(float t) {
            var count = values.Length;

            if (count == 1)
                return values[0];

            if (t < 0f) {
                switch (preWrapMode) {
                    default:
                        return values[0];
                    case WrapMode.Loop:
                        t = 1f - (abs(t) % 1f);
                        break;
                    case WrapMode.PingPong:
                        t = Pingpong(t, 1f);
                        break;
                }
            } else if (t > 1f) {
                switch (postWrapMode) {
                    default:
                        return values[count - 1];
                    case WrapMode.Loop:
                        t %= 1f;
                        break;
                    case WrapMode.PingPong:
                        t = Pingpong(t, 1f);
                        break;
                }
            }

            var it = t * (count - 1);
            var lower = (int)it;
            var upper = lower + 1;
            if (upper >= count)
                upper = count - 1;

            return lerp(values[lower], values[upper], it - lower);
        }

        public void Dispose() {
            if (values.IsCreated)
                values.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float Repeat(float t, float length) {
            return clamp(t - floor(t / length) * length, 0, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float Pingpong(float t, float length) {
            t = Repeat(t, length * 2f);
            return length - abs(t - length);
        }
    }

}
