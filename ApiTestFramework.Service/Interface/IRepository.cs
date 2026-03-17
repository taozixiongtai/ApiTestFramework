namespace ApiTestFramework.Service.Interface;

public interface IRepository<T> where T : class, new()
{
    Task<T> GetAsync();
    Task SaveAsync(T entity);
    Task ResetAsync();
}
