namespace GameServ.Core
{
    public interface IPoolable
    {
        void PrepareForReuse();
    }
}