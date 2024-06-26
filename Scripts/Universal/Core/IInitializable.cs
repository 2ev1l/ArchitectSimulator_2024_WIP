namespace Universal.Core
{
    public interface IInitializable
    {
        public void Init();
    }
    public interface IInitializable<T>
    {
        public void Init(T param);
    }
}