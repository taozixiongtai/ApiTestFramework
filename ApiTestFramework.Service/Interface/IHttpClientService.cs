using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiTestFramework.Service.Interface
{
    public interface IHttpClientService
    {
        Task<T> GetAsync<T>(string url);
        Task<T> PostAsync<T>(string url, object body);
        Task<T> PutAsync<T>(string url, object body);
        Task<T> DeleteAsync<T>(string url);
    }
}
