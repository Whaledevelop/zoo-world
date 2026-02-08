using System;
using System.Collections.Generic;
using UnityEngine;
using ZooWorld.Models;

namespace ZooWorld.Views
{
    public sealed class AnimalView : MonoBehaviour
    {
        private static readonly Dictionary<Rigidbody, AnimalView> Registry = new Dictionary<Rigidbody, AnimalView>();

        [SerializeField]
        private Rigidbody _rigidbody;

        [SerializeField]
        private Collider _collider;

        private readonly List<IDisposable> _subscriptions = new List<IDisposable>();
        private IAnimalModel _model;
        private IAnimalsModel _animalsModel;
        private IAnimalViewsRegistry _registry;
        private bool _isInitialized;

        public Rigidbody Rigidbody => _rigidbody;
        public Collider Collider => _collider;
        public IAnimalModel Model => _model;

        private void OnDisable()
        {
            if (_rigidbody && Registry.ContainsKey(_rigidbody))
            {
                Registry.Remove(_rigidbody);
            }

            foreach (var subscription in _subscriptions)
            {
                subscription.Dispose();
            }

            _subscriptions.Clear();
        }

        public void Initialize(IAnimalModel model, IAnimalsModel animalsModel, IAnimalViewsRegistry registry)
        {
            _model = model;
            _animalsModel = animalsModel;
            _registry = registry;
            Registry[_rigidbody] = this;
            _registry.Register(_model.Id, this);
            _isInitialized = true;
        }

        public void Deinitialize()
        {
            if (_model != null && _registry != null)
            {
                _registry.Unregister(_model.Id);
            }

            _model = null;
            _animalsModel = null;
            _registry = null;
            _isInitialized = false;
        }

        public void ResetForPool()
        {
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.position = transform.position;
            _rigidbody.rotation = transform.rotation;

            if (_rigidbody.IsSleeping())
            {
                _rigidbody.WakeUp();
            }

            _collider.enabled = true;
        }

        private void FixedUpdate()
        {
            if (!_isInitialized || !_model.IsAlive.CurrentValue)
            {

                return;
            }

            _model.SetPosition(_rigidbody.position);
            _model.SetVelocity(_rigidbody.linearVelocity);
        }

        private void OnCollisionEnter(Collision collision)
        {
            var otherRigidbody = collision.rigidbody;

            if (otherRigidbody == null)
            {

                return;
            }

            if (Registry.TryGetValue(otherRigidbody, out var otherView))
            {
                _animalsModel.ReportCollision(_model, otherView._model);
            }
        }
    }
}