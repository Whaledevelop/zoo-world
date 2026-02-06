using System;
using Cysharp.Threading.Tasks;

namespace Whaledevelop.States
{
    public static class StatesControllerExtensions
    {
        public static void EnterState<TStateCode>(this IStatesController<TStateCode> statesController, TStateCode stateCode) where TStateCode : Enum
        {
            statesController.EnterStateAsync(stateCode).Forget();
        }
    }
}
