using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace update
{
    public class CheckUpdate
    {
        private static async Task<string> HttpGet(string url)
        {
            RestClient client = new RestClient(url);
            RestRequest request = new RestRequest("", Method.Get);
            RestResponse response = await client.ExecuteAsync(request);
            return response.Content;
        }

        private static async Task<byte[]> HttpGetBytes(string url)
        {
            RestClient client = new RestClient(url);
            RestRequest request = new RestRequest("", Method.Get);
            RestResponse response = await client.ExecuteAsync(request);
            return response.RawBytes;
        }
        private static async Task<string> GetUpdateFileInfo()
        {
            return await HttpGet($"https://{Config.Instance.bucketName}.{Config.Instance.endpoint}/update.json");
        }

        public static async Task<byte[]> GetUpdateFile(string fileName)
        {
            return await HttpGetBytes($"https://{Config.Instance.bucketName}.{Config.Instance.endpoint}/{fileName}");
        }
        private static void EnsureDirectoryExists(string fullPath)
        {
            try
            {
                // 获取文件的目录路径
                string directoryPath = Path.GetDirectoryName(fullPath);

                if (directoryPath == null)
                {
                    throw new ArgumentException("输入路径无效，无法获取目录信息。", nameof(fullPath));
                }
                // 分割路径为每个独立的目录部分
                string[] pathSegments = directoryPath.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);

                // 用于构建逐步递增的路径
                string currentPath = "";

                foreach (string segment in pathSegments)
                {
                    currentPath = Path.Combine(currentPath, segment);

                    if (!Directory.Exists(currentPath))
                    {
                        Directory.CreateDirectory(currentPath);
                        Console.WriteLine($"已创建目录：{currentPath}");
                    }
                    else
                    {
                        Console.WriteLine($"目录已存在：{currentPath}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生错误：{ex.Message}");
            }

        }
        public static async Task<List<string>> GetUpdateFiles()
        {
            List<string> m_UpdateFiles = new List<string>();
            try
            {
                string updateFileInfo = await GetUpdateFileInfo();

                JObject FileContentObj = JsonConvert.DeserializeObject<JObject>(updateFileInfo);

                if (FileContentObj!=null)
                {
                    foreach (var item in FileContentObj)
                    {
                        string fileName = item.Key;
                        EnsureDirectoryExists(fileName);
                        if (!File.Exists(fileName))
                        {
                            m_UpdateFiles.Add(fileName);
                        }
                        else
                        {
                            string crc64Str = item.Value.ToString();
                            ulong crc64 = Crc64Ecma.ComputeFromFile(fileName);
                            if (!crc64Str.Equals(crc64.ToString()))
                            {
                                m_UpdateFiles.Add(fileName);
                            }
                        }
                    }
                }
            }
            catch
            {

            }
            return m_UpdateFiles;

        }
    }
}
