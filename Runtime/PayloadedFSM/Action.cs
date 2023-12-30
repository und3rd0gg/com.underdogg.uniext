namespace UniExt.PayloadedFSM {
    public abstract class Action<TPayload>
    {
        protected TPayload Payload;

        public void BindPayload(TPayload payload) =>
            Payload = payload;

        public virtual void Init() { }
        
        public virtual void Enter() { }

        public abstract void Act();
    }
}