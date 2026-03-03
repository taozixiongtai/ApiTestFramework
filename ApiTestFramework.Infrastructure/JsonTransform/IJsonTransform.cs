using System;
using System.Collections.Generic;
using System.Text;

namespace ApiTestFramework.Infrastructure.JsonTransform
{
    public interface IJsonTransform
    {

        public string Transform(string json);
    }
}
