namespace UniExt.FSM
{
    public interface IState : IExitableState
    {
        void Enter();
    }

    public interface IPayloadedState<TPayload> : IExitableState
    {
        void Enter(TPayload payload);
    }

    public interface IPayloadedState<TPayload, TPayload2> : IExitableState
    {
        void Enter(TPayload payload, TPayload2 payload2);
    }

    public interface IPayloadedState<TPayload, TPayload2, TPayload3> : IExitableState
    {
        void Enter(TPayload payload, TPayload2 payload2, TPayload3 payload3);
    }

    public interface IPayloadedState<TPayload, TPayload2, TPayload3, TPayload4> : IExitableState
    {
        void Enter(TPayload payload, TPayload2 payload2, TPayload3 payload3, TPayload4 payload4);
    }

    public interface IExitableState
    {
        FSM StateMachine { get; }

        void Exit();
    }
}