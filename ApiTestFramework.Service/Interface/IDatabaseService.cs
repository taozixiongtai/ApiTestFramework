using ApiTestFramework.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiTestFramework.Service.Interface
{
    public interface IDatabaseService
    {
        void InsertData(string tableName, List<DynamicJsonObject> records);
    }
}
