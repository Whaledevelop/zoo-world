using System;
using System.Collections.Generic;
using R3;
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

        public Rigidbody Rigidbody => _rigidbody;
        public Collider Collider => _collider;
        public IAnimalModel Model => _model;

        private void OnEnable()
        {
            Registry[_rigidbody] = this;
        }

        private void OnDisable()
        {
            Registry.Remove(_rigidbody);

            foreach (var subscription in _subscriptions)
            {
                subscription.Dispose();
            }

            _subscriptions.Clear();
        }

        public void Initialize(IAnimalModel model, IAnimalsModel animalsModel)
        {
            _model = model;
            _animalsModel = animalsModel;

            _subscriptions.Add(_model.Position.Subscribe(position =>
            {
                _rigidbody.position = position;
            }));
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