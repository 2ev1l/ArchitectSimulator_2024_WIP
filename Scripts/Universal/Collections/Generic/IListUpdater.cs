namespace Universal.Collections.Generic
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">Used for data update</typeparam>
    public interface IListUpdater<T>
    {
        public void OnListUpdate(T param);
    }
}