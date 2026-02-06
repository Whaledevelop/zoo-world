using System;
using System.Collections.Generic;

namespace Whaledevelop.States
{
    public abstract class StatesFactory<TStateCode> : IStatesFactory<TStateCode> where TStateCode : Enum
    {
        private static readonly Dictionary<TStateCode, Func<IState<TStateCode>>> Map = new();

        protected static void Register<TState>(TStateCode code) where TState : IState<TStateCode>, new()
        {
            Map[code] = () => new TState();
        }

        public bool TryCreateState(TStateCode stateCode, out IState<TStateCode> state)
        {
            if (!Map.TryGetValue(stateCode, out var createFunc))
            {
                state = null;

                return false;
            }

            state = createFunc.Invoke();

            return state != null;
        }
    }
}
