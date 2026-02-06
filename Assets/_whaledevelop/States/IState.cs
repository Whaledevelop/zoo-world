using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Whaledevelop.States
{
    public interface IState<TStateCode> where TStateCode : Enum
    {
        TStateCode StateCode { get; }

        UniTask EnterAsync(CancellationToken cancellationToken);


        UniTask ExitAsync(CancellationToken cancellationToken);
    }
}
