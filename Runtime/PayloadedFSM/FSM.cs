using System;
using System.Collections.Generic;

namespace com.underdogg.uniext.Runtime.PayloadedFSM
{
    public sealed class FSM<TStates, TPayload> where TStates : Enum
    {
        private readonly Dictionary<TStates, State<TPayload, TStates>> _statesMap;
        private readonly Transition<TPayload, TStates>[] _anyTransitions;
        private readonly TStates _startState;

        private State<TPayload, TStates> _currentState;
        private bool _isPaused;
        private bool _isStopped;

        public FSM(
            Dictionary<TStates, State<TPayload, TStates>> statesMap,
            Transition<TPayload, TStates>[] anyTransitions,
            TPayload payload,
            TStates startState)
        {
            if (statesMap == null)
                throw new ArgumentNullException(nameof(statesMap));

            if (anyTransitions == null)
                throw new ArgumentNullException(nameof(anyTransitions));

            if (statesMap.Count == 0)
                throw new ArgumentException("FSM requires at least one state.", nameof(statesMap));

            if (!statesMap.ContainsKey(startState))
                throw new ArgumentException($"FSM start state '{startState}' is not present in state map.", nameof(startState));

            _statesMap = statesMap;
            _anyTransitions = anyTransitions;
            _startState = startState;
            _isPaused = true;
            _isStopped = true;

            _ = payload;
        }

        public TStates CurrentState { get; private set; }
        public bool IsPaused => _isPaused;
        public bool IsStopped => _isStopped;

        public event System.Action<TStates> StateEntered;

        public void Pause() =>
            _isPaused = true;

        public void Stop()
        {
            if (_isStopped)
                return;

            _currentState?.OnExit();
            _isStopped = true;
            _isPaused = true;
        }

        public void Play()
        {
            _isPaused = false;

            if (_isStopped)
            {
                SetState(_startState);
                _isStopped = false;
            }
        }

        public void Restart()
        {
            SetState(_startState);
            Play();
        }

        public void SetState(TStates state)
        {
            if (!_statesMap.TryGetValue(state, out var nextState))
                throw new ArgumentException($"There is no state of type '{state}'.", nameof(state));

            if (nextState == null)
                throw new InvalidOperationException($"State '{state}' is null.");

            if (nextState == _currentState)
                return;

            _currentState?.OnExit();
            _currentState = nextState;
            StateEntered?.Invoke(state);
            _currentState.OnEnter();
            CurrentState = state;
        }

        public void OnUpdate()
        {
            if (_isPaused || _isStopped)
                return;

            _currentState.OnUpdate();
            CheckAnyTransitions();
        }

        private void CheckAnyTransitions()
        {
            for (var i = 0; i < _anyTransitions.Length; i++)
            {
                var transition = _anyTransitions[i];
                if (transition == null)
                    continue;

                transition.CheckDecision();
            }
        }
    }
}
