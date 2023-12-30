namespace UniExt.PayloadedFSM {
    public abstract class Decision<TPayload>
    {
        protected TPayload Payload;

        public void BindPayload(TPayload payload)
        {
            Payload = payload;
        }

        public virtual void Init() { }

        public abstract bool? Decide();
    }
}