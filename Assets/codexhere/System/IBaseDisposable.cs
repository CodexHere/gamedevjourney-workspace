using System;
using UnityEngine;

namespace codexhere.System {
    public abstract class IBaseDisposable : IDisposable {
        protected bool isDisposed;
        protected IDisposable[] disposableItems;

        ~IBaseDisposable() => Dispose(false);

        public void Dispose() {
            Debug.Log("Dispose() called");
            if (isDisposed) {
                return;
            }

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing) {
            Debug.Log($"Dispose({disposing}) called");

            if (isDisposed || !disposing) {
                return;
            }

            foreach (var disposable in disposableItems) {
                disposable.Dispose();
            }

            isDisposed = true;
        }
    }
}