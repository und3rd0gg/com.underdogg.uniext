using System;
using System.Collections.Generic;

namespace com.underdogg.uniext.Runtime.PayloadedFSM
{
    public sealed class FSMBuilder<TStates, TPayload> where TStates : Enum
    {
        private readonly TPayload _payload;
        private readonly Dictionary<TStates, State<TPayload, TStates>> _states = new();
        private readonly List<Transition<TPayload, TStates>> _anyTransitions = new();
        private readonly List<Transition<TPayload, TStates>> _allTransitions = new();

        private Action _currentStateOnEnterAction;
        private Action _currentStateOnExitAction;
        private List<Action<TPayload>> _actions = new();
        private List<Transition<TPayload, TStates>> _currentTransitions = new();

        private TStates _currentState;
        private TStates _fsmStartState;
        private bool _isBindingState;
        private bool _hasStartState;
        private bool _isBuilt;

        public FSMBuilder(TPayload payload)
        {
            _payload = payload;
        }

        public FSMBuilder<TStates, TPayload> StartBindState(TStates state)
        {
            ThrowIfBuilt();

            if (_isBindingState)
                throw new InvalidOperationException("A state is already being configured. Call EndBindState() before starting another state.");

            if (_states.ContainsKey(state))
                throw new InvalidOperationException($"State '{state}' is already configured.");

            _currentState = state;
            _isBindingState = true;
            return this;
        }

        public FSMBuilder<TStates, TPayload> AddStateEnterAction(Action onEnterAction)
        {
            EnsureBindingState();
            _currentStateOnEnterAction = onEnterAction;
            return this;
        }

        public FSMBuilder<TStates, TPayload> AddStateExitAction(Action onExitAction)
        {
            EnsureBindingState();
            _currentStateOnExitAction = onExitAction;
            return this;
        }

        public FSMBuilder<TStates, TPayload> AddAction(Action<TPayload> action)
        {
            EnsureBindingState();

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            action.BindPayload(_payload);
            _actions.Add(action);
            action.Init();
            return this;
        }

        public FSMBuilder<TStates, TPayload> AddAction<TAction>(System.Action<TAction> with = null)
            where TAction : Action<TPayload>, new()
        {
            EnsureBindingState();

            var action = new TAction();
            action.BindPayload(_payload);
            with?.Invoke(action);
            _actions.Add(action);
            action.Init();
            return this;
        }

        public FSMBuilder<TStates, TPayload> EndBindState()
        {
            EnsureBindingState();

            var state = new State<TPayload, TStates>(
                _actions.ToArray(),
                _currentTransitions.ToArray(),
                _currentStateOnEnterAction,
                _currentStateOnExitAction);

            _states.Add(_currentState, state);
            ResetCurrentStateConfiguration();
            return this;
        }

        public FSMBuilder<TStates, TPayload> AddTransition(TStates trueDecision, TStates falseDecision, Decision<TPayload> decision)
        {
            EnsureBindingState();

            if (decision == null)
                throw new ArgumentNullException(nameof(decision));

            decision.BindPayload(_payload);
            decision.Init();

            var transition = new Transition<TPayload, TStates>(decision, trueDecision, falseDecision);
            _currentTransitions.Add(transition);
            _allTransitions.Add(transition);
            return this;
        }

        public FSMBuilder<TStates, TPayload> AddAnyTransition(TStates trueDecision, TStates falseDecision, Decision<TPayload> decision)
        {
            ThrowIfBuilt();

            if (decision == null)
                throw new ArgumentNullException(nameof(decision));

            decision.BindPayload(_payload);
            decision.Init();

            var transition = new Transition<TPayload, TStates>(decision, trueDecision, falseDecision);
            _allTransitions.Add(transition);
            _anyTransitions.Add(transition);
            return this;
        }

        public FSMBuilder<TStates, TPayload> SetStartState(TStates state)
        {
            ThrowIfBuilt();

            _fsmStartState = state;
            _hasStartState = true;
            return this;
        }

        public FSM<TStates, TPayload> Build()
        {
            ThrowIfBuilt();

            if (_isBindingState)
                throw new InvalidOperationException("Cannot build FSM while a state is being configured. Call EndBindState().");

            if (_states.Count == 0)
                throw new InvalidOperationException("Cannot build FSM without any states.");

            if (!_hasStartState)
                throw new InvalidOperationException("Start state is not configured. Call SetStartState(...).");

            if (!_states.ContainsKey(_fsmStartState))
                throw new InvalidOperationException($"Start state '{_fsmStartState}' is not configured.");

            ValidateTransitionTargets();

            var fsm = new FSM<TStates, TPayload>(
                new Dictionary<TStates, State<TPayload, TStates>>(_states),
                _anyTransitions.ToArray(),
                _payload,
                _fsmStartState);

            for (var i = 0; i < _allTransitions.Count; i++)
            {
                _allTransitions[i].BindFSM(fsm);
            }

            _isBuilt = true;
            return fsm;
        }

        private void ValidateTransitionTargets()
        {
            for (var i = 0; i < _allTransitions.Count; i++)
            {
                var transition = _allTransitions[i];
                if (!_states.ContainsKey(transition.TrueDecision))
                    throw new InvalidOperationException($"Transition target state '{transition.TrueDecision}' is not configured.");

                if (!_states.ContainsKey(transition.FalseDecision))
                    throw new InvalidOperationException($"Transition target state '{transition.FalseDecision}' is not configured.");
            }
        }

        private void EnsureBindingState()
        {
            ThrowIfBuilt();

            if (!_isBindingState)
                throw new InvalidOperationException("No active state binding. Call StartBindState(...) first.");
        }

        private void ThrowIfBuilt()
        {
            if (_isBuilt)
                throw new InvalidOperationException("FSMBuilder can only build once. Create a new builder instance for another FSM.");
        }

        private void ResetCurrentStateConfiguration()
        {
            _actions = new List<Action<TPayload>>();
            _currentTransitions = new List<Transition<TPayload, TStates>>();
            _currentStateOnEnterAction = null;
            _currentStateOnExitAction = null;
            _isBindingState = false;
        }
    }
}
