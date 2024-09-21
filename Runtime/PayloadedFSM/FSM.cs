using System;
using System.Collections.Generic;

namespace com.underdogg.uniext.Runtime.PayloadedFSM
{
    public sealed class FSM<TStates, TPayload> where TStates : Enum
    {
        private readonly Dictionary<TStates, State<TPayload, TStates>> _statesMap;
        private readonly Transition<TPayload, TStates>[] _anyTransitions;
        
        private State<TPayload, TStates> _currentState;
        private TStates _startState;
        
        private bool _isPaused;
        private bool _isStopped;
        
        protected TPayload Payload;

        public FSM(
            Dictionary<TStates, State<TPayload, TStates>> statesMap,
            Transition<TPayload, TStates>[] anyTransitions,
            TPayload payload, TStates startState
            )
        {
            _statesMap = statesMap;
            Payload = payload;
            _anyTransitions = anyTransitions;
            _startState = startState;
            _isPaused = true;
            _isStopped = true;
        }
        
        public TStates CurrentState { get; private set; }

        public event System.Action<TStates> StateEntered;

        public void Pause() => 
            _isPaused = true;

        public void Stop()
        {
            _isStopped = true;
        }

        public void Play()
        {
            _isPaused = false;
            
            if (_isStopped)
            {
                SetState(_startState);
                _isStopped = false;
                return;
            }
        }

        public void Restart()
        {
            SetState(_startState);
            Play();
        }

        public void SetState(TStates state)
        {
            var nextState = _statesMap[state];

            if (nextState == null)
                throw new ArgumentException("There is no state of type" + state);

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
            if(_isPaused || _isStopped)
                return;
            
            _currentState.OnUpdate();
            CheckAnyTransitions();
        }

        private void CheckAnyTransitions()
        {
            for (var i = 0; i < _anyTransitions.Length; i++)
            {
                _anyTransitions[i].CheckDecision();
            }
        }
    }
}