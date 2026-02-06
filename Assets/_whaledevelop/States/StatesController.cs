using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using QuizRPG.Utility;
using R3;
using VContainer;

namespace Whaledevelop.States
{
    public abstract class StatesController<TStateCode> : InitializableAsync, IStatesController<TStateCode> where TStateCode : Enum
    {
        private ReactiveProperty<TStateCode> _currentStateCode;

        private CancellationTokenSource _cancellationTokenSource;

        public ReadOnlyReactiveProperty<TStateCode> StateCode => _currentStateCode.ToReadOnlyReactiveProperty();
        public TStateCode PrevStateCode { get; private set; }

        public event Action<TStateCode> OnBeforeStateEnter;

        public event Action<TStateCode> OnAfterStateEnter;

        private IState<TStateCode> _currentState;

        private IObjectResolver _objectResolver;

        private readonly IStatesFactory<TStateCode> _statesFactory;

        protected StatesController(IStatesFactory<TStateCode> statesFactory)
        {
            _statesFactory = statesFactory;
        }

        [Inject]
        public void Construct(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
        }

        protected override UniTask OnInitializeAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _currentStateCode = new ReactiveProperty<TStateCode>();

            return base.OnInitializeAsync(cancellationToken);
        }

        protected override UniTask OnReleaseAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource?.CancelAndDispose();

            return base.OnReleaseAsync(cancellationToken);
        }

        public virtual async UniTask EnterStateAsync(TStateCode stateCode, CancellationToken cancellationToken = default)
        {
            OnBeforeStateEnter?.Invoke(stateCode);
            
            CancellationTokenSource linkedCts = null;
            var token = cancellationToken.CanBeCanceled
                ? (linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken)).Token
                : _cancellationTokenSource.Token;

            if (_currentState != null)
            {
                await _currentState.ExitAsync(token);
            }
            
            try
            {
                if (_statesFactory.TryCreateState(stateCode, out var state))
                {
                    SetState(stateCode, state);
                    _objectResolver.Inject(state);
                    await state.EnterAsync(token);
                }
                else
                {
                    SetState(stateCode, null);
                }
            }
            finally
            {
                linkedCts?.Dispose();
                OnAfterStateEnter?.Invoke(stateCode);
            }
        }
        
        private void SetState(TStateCode stateCode, IState<TStateCode> state)
        {
            _currentState = state;
            PrevStateCode = _currentStateCode.Value;
            _currentStateCode.Value = stateCode;
        }
    }
}
