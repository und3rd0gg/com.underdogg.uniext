using System;

namespace com.underdogg.uniext.Runtime.PayloadedFSM
{
    public sealed class Transition<TPayload, TStates> where TStates : Enum
    {
        private FSM<TStates, TPayload> _fsm;
        private readonly Decision<TPayload> _decision;
        private readonly TStates _trueDecision;
        private readonly TStates _falseDecision;

        internal TStates TrueDecision => _trueDecision;
        internal TStates FalseDecision => _falseDecision;

        public Transition(Decision<TPayload> decision, TStates trueDecision, TStates falseDecision)
        {
            _decision = decision ?? throw new ArgumentNullException(nameof(decision));
            _trueDecision = trueDecision;
            _falseDecision = falseDecision;
        }

        public void BindFSM(FSM<TStates, TPayload> fsm) =>
            _fsm = fsm ?? throw new ArgumentNullException(nameof(fsm));

        public void CheckDecision()
        {
            if (_fsm == null)
                throw new InvalidOperationException("Transition is not bound to FSM. Call BindFSM(...) before running transitions.");

            var decision = _decision.Decide();

            if (decision == null)
                return;

            if (decision.Value)
                _fsm.SetState(_trueDecision);
            else
                _fsm.SetState(_falseDecision);
        }
    }
}
