using System;

namespace com.underdogg.uniext.Runtime.PayloadedFSM {
    public sealed class State<TPayload, TStates> where TStates : Enum
    {
        private readonly Action<TPayload>[] _actions;
        private readonly Transition<TPayload, TStates>[] _transitions;

        private readonly System.Action _onEnterAction;
        private readonly System.Action _onExitAction;

        public State(Action<TPayload>[] actions,
            Transition<TPayload, TStates>[] transitions,
            System.Action onEnterAction,
            System.Action onExitAction)
        {
            _actions = actions ?? Array.Empty<Action<TPayload>>();
            _transitions = transitions ?? Array.Empty<Transition<TPayload, TStates>>();
            _onEnterAction = onEnterAction;
            _onExitAction = onExitAction;
        }

        public void OnEnter()
        {
            for (var i = 0; i < _actions.Length; i++)
            {
                _actions[i].Enter();
            }

            _onEnterAction?.Invoke();
        }

        public void OnExit()
        {
            _onExitAction?.Invoke();
        }

        public void OnUpdate()
        {
            for (var i = 0; i < _actions.Length; i++)
            {
                _actions[i].Act();
            }

            for (var i = 0; i < _transitions.Length; i++)
            {
                _transitions[i].CheckDecision();
            }
        }
    }
}
