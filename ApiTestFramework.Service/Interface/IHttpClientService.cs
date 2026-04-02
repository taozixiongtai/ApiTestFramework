namespace ApiTestFramework.Service.Interface;

/// <summary>
/// HTTP 客户端服务接口，定义了发送 HTTP 请求的方法
/// </summary>
public interface IHttpClientService
{
    /// <summary>
    /// 发送 GET 请求并返回反序列化的对象
    /// </summary>
    /// <typeparam name="T">返回对象的类型</typeparam>
    /// <param name="url">请求 URL</param>
    /// <returns>反序列化后的对象</returns>
    Task<T> GetAsync<T>(string url);

    /// <summary>
    /// 发送 POST 请求并返回反序列化的对象
    /// </summary>
    /// <typeparam name="T">返回对象的类型</typeparam>
    /// <param name="url">请求 URL</param>
    /// <param name="body">请求体对象</param>
    /// <returns>反序列化后的对象</returns>
    Task<T> PostAsync<T>(string url, object body);

    /// <summary>
    /// 发送 PUT 请求并返回反序列化的对象
    /// </summary>
    /// <typeparam name="T">返回对象的类型</typeparam>
    /// <param name="url">请求 URL</param>
    /// <param name="body">请求体对象</param>
    /// <returns>反序列化后的对象</returns>
    Task<T> PutAsync<T>(string url, object body);

    /// <summary>
    /// 发送 DELETE 请求并返回反序列化的对象
    /// </summary>
    /// <typeparam name="T">返回对象的类型</typeparam>
    /// <param name="url">请求 URL</param>
    /// <returns>反序列化后的对象</returns>
    Task<T> DeleteAsync<T>(string url);

    /// <summary>
    /// 发送 GET 请求并返回字符串
    /// </summary>
    /// <param name="url">请求 URL</param>
    /// <returns>响应字符串</returns>
    Task<string> GetStringAsync(string url);

    /// <summary>
    /// 发送 POST 请求并返回字符串
    /// </summary>
    /// <param name="url">请求 URL</param>
    /// <param name="body">请求体对象</param>
    /// <returns>响应字符串</returns>
    Task<string> PostStringAsync(string url, object body);

    /// <summary>
    /// 发送 PUT 请求并返回字符串
    /// </summary>
    /// <param name="url">请求 URL</param>
    /// <param name="body">请求体对象</param>
    /// <returns>响应字符串</returns>
    Task<string> PutStringAsync(string url, object body);

    /// <summary>
    /// 发送 DELETE 请求并返回字符串
    /// </summary>
    /// <param name="url">请求 URL</param>
    /// <returns>响应字符串</returns>
    Task<string> DeleteStringAsync(string url);

    /// <summary>
    /// 发送 PATCH 请求并返回字符串
    /// </summary>
    /// <param name="url">请求 URL</param>
    /// <param name="body">请求体对象</param>
    /// <returns>响应字符串</returns>
    Task<string> PatchStringAsync(string url, object body);
}
