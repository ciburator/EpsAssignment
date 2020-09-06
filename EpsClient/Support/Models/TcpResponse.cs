namespace Support.Models
{
    using Common;

    public class TcpResponse
    {
        public ActionState State;
        public string Message;

        public TcpResponse(
            string message
            , ActionState state = ActionState.Continue)
        {
            this.State = state;
            this.Message = message;
        }

        public TcpResponse(ActionState state = ActionState.Continue)
        {
            this.State = state;
            this.Message = string.Empty;
        }
    }
}
