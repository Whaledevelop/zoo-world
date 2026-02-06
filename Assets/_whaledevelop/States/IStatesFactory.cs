using System;

namespace Whaledevelop.States
{
    public interface IStatesFactory<TStateCode> where TStateCode : Enum
    {
        bool TryCreateState(TStateCode stateCode, out IState<TStateCode> state);
    }
}
