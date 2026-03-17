using ApiTestFramework.Infrastructure.Domain;

namespace ApiTestFramework.Service.Interface;

public interface IDatabaseService
{
    void InsertData(string tableName, List<DynamicJsonObject> records);
}
