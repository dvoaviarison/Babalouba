using System;
using System.IO;
using System.Xml.Serialization;

namespace Babalouba
{
    [Serializable]
    public class ConfigLoader
    {
        public bool Success = true;
        public string Message = string.Empty;

        public Configuration LoadConfig(string filePath)
        {
            this.Success = true;
            this.Message = string.Empty;
            Configuration configuration = new Configuration();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Configuration));
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            try
            {
                configuration = (Configuration)xmlSerializer.Deserialize((Stream)fileStream);
            }
            catch (Exception ex)
            {
                this.Message = ex.Message;
                this.Success = false;
            }
            finally
            {
                fileStream.Close();
            }
            return configuration;
        }

        public Configuration SaveConfig(Configuration config, string filePath)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Configuration));
            TextWriter textWriter = (TextWriter)new StreamWriter(filePath);
            try
            {
                xmlSerializer.Serialize(textWriter, (object)config);
            }
            catch (Exception ex)
            {
                this.Message = ex.Message;
                this.Success = false;
            }
            finally
            {
                textWriter.Close();
            }
            return config;
        }
    }
}
