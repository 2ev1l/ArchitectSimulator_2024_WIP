namespace Universal.Events
{
    [System.Serializable]
    public abstract class ExecutableRequest
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        public abstract void Close();
        public virtual void Send()
        {
            RequestController.Instance.TryExecuteRequest(this);
        }
        #endregion methods
    }
}