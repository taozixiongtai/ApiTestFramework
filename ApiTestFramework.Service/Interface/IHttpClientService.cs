namespace ApiTestFramework.Service.Interface;

public interface IHttpClientService
{
    Task<T> GetAsync<T>(string url);
    Task<T> PostAsync<T>(string url, object body);
    Task<T> PutAsync<T>(string url, object body);
    Task<T> DeleteAsync<T>(string url);
    Task<string> GetStringAsync(string url);
    Task<string> PostStringAsync(string url, object body);
    Task<string> PutStringAsync(string url, object body);
    Task<string> DeleteStringAsync(string url);
}
