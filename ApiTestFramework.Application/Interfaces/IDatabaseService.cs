using ApiTestFramework.Domain.Entities;

namespace ApiTestFramework.Application.Interfaces;

public interface IDatabaseService
{
    void InsertData(string tableName, List<DynamicJsonObject> records);
}
