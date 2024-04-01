namespace Universal.Events
{
    /// <summary>
    /// Don't use it if your execution order depends on this message
    /// </summary>
    public interface IFixedUpdateSender : IMessageSender
    {
        /// <summary>
        /// You must unsubscribe and subscribe by your own. Works as Unity original message
        /// </summary>
        public void FixedUpdateMessage();
    }
}