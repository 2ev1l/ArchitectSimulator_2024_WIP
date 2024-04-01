namespace Universal.Collections.Generic.Filters
{
    public interface ISmartFilter<T>
    {
        public VirtualFilter VirtualFilter { get; }
        /// <summary>
        /// Invoke this before <see cref="FilterItem(T)"/> in controller (not in loop)
        /// </summary>
        public void UpdateFilterData();
        /// <summary>
        /// <see cref="UpdateFilterData"/> should invokes before this method in controller
        /// </summary>
        /// <param name="item"></param>
        public bool FilterItem(T item);
    }
}