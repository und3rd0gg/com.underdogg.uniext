using System;
using System.Collections.Generic;

namespace com.underdogg.uniext.Runtime.CFSM
{
    public sealed class CFSM
    {
        private IExitableState _activeState;
        private readonly Dictionary<Type, IExitableState> _states = new();

        public IReadOnlyDictionary<Type, IExitableState> States => _states;

        public void Construct(Dictionary<Type, IExitableState> states)
        {
            if (states == null)
                throw new ArgumentNullException(nameof(states));

            _states.Clear();
            foreach (var pair in states)
            {
                var stateType = pair.Key;
                var stateInstance = pair.Value;

                if (stateType == null)
                    throw new ArgumentException("State type cannot be null.", nameof(states));

                if (stateInstance == null)
                    throw new ArgumentException($"State instance for '{stateType.Name}' is null.", nameof(states));

                _states[stateType] = stateInstance;
            }

            _activeState = null;
        }

        public void Enter<TState>() where TState : class, IState
        {
            var state = ChangeState<TState>();
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

        public void Enter<TState, TPayload, TPayload2, TPayload3>(
            TPayload payload,
            TPayload2 payload2,
            TPayload3 payload3)
            where TState : class, IPayloadedState<TPayload, TPayload2, TPayload3>
        {
            var state = ChangeState<TState>();
            state.Enter(payload, payload2, payload3);
        }

        public void Enter<TState, TPayload, TPayload2, TPayload3, TPayload4>(
            TPayload payload,
            TPayload2 payload2,
            TPayload3 payload3,
            TPayload4 payload4)
            where TState : class, IPayloadedState<TPayload, TPayload2, TPayload3, TPayload4>
        {
            var state = ChangeState<TState>();
            state.Enter(payload, payload2, payload3, payload4);
        }

        private TState ChangeState<TState>() where TState : class, IExitableState
        {
            _activeState?.Exit();

            var state = GetState<TState>();
            _activeState = state;

            return state;
        }

        private TState GetState<TState>() where TState : class, IExitableState
        {
            if (_states.Count == 0)
                throw new InvalidOperationException("CFSM is not constructed. Call Construct(...) before Enter(...).");

            if (!_states.TryGetValue(typeof(TState), out var state))
                throw new KeyNotFoundException($"CFSM does not contain state '{typeof(TState).Name}'.");

            if (state is not TState typedState)
                throw new InvalidCastException($"State '{typeof(TState).Name}' has incompatible runtime type '{state.GetType().Name}'.");

            return typedState;
        }
    }
}
