using System;
using UnityEngine;

namespace Whaledevelop
{
    public abstract class SyncLifetime :
        IInitializable,
        IReleasable,
        ILifetime
    {
        [NonSerialized]
        private bool _initialized;

        bool IInitializable.Initialized => _initialized;

        void IInitializable.Initialize()
        {
            if (_initialized)
            {
                Debug.Log("Already Initialized");

                return;
            }

            OnInitialize();

            _initialized = true;
        }

        void IReleasable.Release()
        {
            if (!_initialized)
            {
                Debug.Log("Not initialized");

                return;
            }

            OnRelease();

            _initialized = false;
        }

        protected virtual void OnInitialize()
        {
        }

        protected virtual void OnRelease()
        {
        }
    }
}