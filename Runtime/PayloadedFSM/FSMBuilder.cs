using System;
using System.Collections.Generic;

namespace UniExt.PayloadedFSM {
    public sealed class FSMBuilder<TStates, TPayload> where TStates : Enum
    {
        private FSM<TStates, TPayload> FSM;
        private TPayload _payload;
        private Dictionary<TStates, State<TPayload, TStates>> _states = new();
        
        private Action _currentStateOnEnterAction;
        private Action _currentStateOnExitAction;
        
        private List<Action<TPayload>> _actions = new();
        
        private List<Transition<TPayload, TStates>> _currentTransitions = new();
        private List<Transition<TPayload, TStates>> _anyTransitions = new();
        private List<Transition<TPayload, TStates>> _allTransitions = new();
        
        private TStates _currentState;
        private TStates _fsmStartState;

        public FSMBuilder(TPayload payload)
        {
            _payload = payload;
        }

        public FSMBuilder<TStates, TPayload> StartBindState(TStates state)
        {
            _currentState = state;
            return this;
        }

        public FSMBuilder<TStates, TPayload> AddStateEnterAction(System.Action onEnterAction)
        {
            _currentStateOnEnterAction = onEnterAction;
            return this;
        }

        public FSMBuilder<TStates, TPayload> AddStateExitAction(System.Action onExitAction)
        {
            _currentStateOnExitAction = onExitAction;
            return this;
        }

        public FSMBuilder<TStates, TPayload> AddAction(Action<TPayload> action)
        {
            action.BindPayload(_payload);
            _actions.Add(action);
            action.Init();
            return this;
        }

        public FSMBuilder<TStates, TPayload> AddAction<TAction>
            (System.Action<TAction> with = null)
            where TAction : Action<TPayload>, new()
        {
            var action = new TAction();
            action.BindPayload(_payload);
            with?.Invoke(action);
            _actions.Add(action);
            action.Init();
            return this;
        }

        public FSMBuilder<TStates, TPayload> EndBindState()
        {
            var state = new State<TPayload, TStates>(
                _payload,
                _actions.ToArray(),
                _currentTransitions.ToArray(),
                _currentStateOnEnterAction,
                _currentStateOnExitAction);
            _states.Add(_currentState, state);

            _actions = new();
            _currentTransitions = new();
            _currentStateOnEnterAction = null;
            _currentStateOnExitAction = null;
            return this;
        }
        
        public FSMBuilder<TStates, TPayload> AddTransition(
            TStates trueDecision, TStates falseDecision, Decision<TPayload> decision
        )
        {
            decision.BindPayload(_payload);
            var transition = new Transition<TPayload, TStates>
                (decision, trueDecision, falseDecision);
            _currentTransitions.Add(transition);
            _allTransitions.Add(transition);
            decision.Init();
            return this;
        }

        public FSMBuilder<TStates, TPayload> AddAnyTransition(
            TStates trueDecision, TStates falseDecision, Decision<TPayload> decision
        )
        {
            decision.BindPayload(_payload);
            var transition = new Transition<TPayload, TStates>
                (decision, trueDecision, falseDecision);
            _allTransitions.Add(transition);
            _anyTransitions.Add(transition);
            decision.Init();
            return this;
        }

        public FSMBuilder<TStates, TPayload> SetStartState(TStates state)
        {
            _fsmStartState = state;
            return this;
        }

        private void Reset()
        {
            FSM = null;
            _payload = default;
            _states = new();
            _actions = new();
            _currentStateOnEnterAction = null;
            _currentStateOnExitAction = null;
            _currentTransitions = new();
            _allTransitions = new();
            _anyTransitions = new();
            _currentState = default;
            _fsmStartState = default;
        }

        public FSM<TStates, TPayload> Build()
        {
            var fsm = new FSM<TStates, TPayload>(
                _states, _anyTransitions.ToArray(), _payload, _fsmStartState
            );

            foreach (var transition in _allTransitions)
            {
                transition.BindFSM(fsm);
            }

            Reset();
            return fsm;
        }
    }
}