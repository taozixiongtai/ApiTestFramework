using ApiTestFramework.Infrastructure.Exceptions;

namespace ApiTestFramework.Infrastructure.Helper
{
    public static class FileHelper
    {

        /// <summary>
        /// 读取指定路径下所有 JSON 文件
        /// </summary>
        /// <param name="path">目录路径</param>
        /// <param name="includeSubDirectories">是否包含子目录</param>
        /// <returns>key=文件路径, value=json内容</returns>
        public static Dictionary<string, string> ReadAllJsonFiles(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new BusinessException("目录不能为空");

            if (!Directory.Exists(path))
                throw new BusinessException($"目录不存在: {path}");

            var result = new Dictionary<string, string>();

            var files = Directory.GetFiles(path, "*.json");

            foreach (var file in files)
            {
                var content = File.ReadAllText(file);
                result[file] = content;
            }

            return result;
        }


    }
}
