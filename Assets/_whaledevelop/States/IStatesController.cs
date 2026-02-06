using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;

namespace Whaledevelop.States
{
    public interface IStatesController<T> where T : Enum
    {
        ReadOnlyReactiveProperty<T> StateCode { get; }
        
        T PrevStateCode { get; }

        event Action<T> OnBeforeStateEnter;

        event Action<T> OnAfterStateEnter;

        UniTask EnterStateAsync(T stateCode, CancellationToken cancellationToken = default);
    }
}
