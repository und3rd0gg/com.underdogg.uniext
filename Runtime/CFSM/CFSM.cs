using System;
using System.Collections.Generic;

namespace UniExt.CFSM
{
    public class CFSM
    {
        protected IExitableState ActiveState;

        public Dictionary<Type, IExitableState> States;

        public void Construct(Dictionary<Type, IExitableState> states)
        {
            States = states;
        }

        public void Enter<TState>() where TState : class, IState
        {
            IState state = ChangeState<TState>();
            state.Enter();
        }

        public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>
        {
            var state = ChangeState<TState>();
            state.Enter(payload);
        }

        public void Enter<TState, TPayload, TPayload2>(TPayload payload, TPayload2 payload2)
            where TState : class, IPayloadedState<TPayload, TPayload2>
        {
            var state = ChangeState<TState>();
            state.Enter(payload, payload2);
        }

        private TState ChangeState<TState>() where TState : class, IExitableState
        {
            ActiveState?.Exit();

            var state = GetState<TState>();
            ActiveState = state;

            return state;
        }

        private TState GetState<TState>() where TState : class, IExitableState
        {
            return States[typeof(TState)] as TState;
        }
    }
}