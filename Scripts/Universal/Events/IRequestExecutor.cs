namespace Universal.Events
{
    public interface IRequestExecutor
    {
        /// <summary>
        /// To receive messages, you should manually subscribe/unsubscribe to <see cref="RequestController"/>. <br></br>
        /// Also you should invoke <see cref="ExecutableRequest.Close"/> on your own
        /// </summary>
        public bool TryExecuteRequest(ExecutableRequest request);
    }
}