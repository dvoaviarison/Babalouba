using System;
using System.IO;
using System.Xml.Serialization;

namespace Babalouba
{
    [Serializable]
    public class ThemeLoader
    {
        public bool Success = true;
        public string Message = string.Empty;

        public Theme LoadTheme(string filePath)
        {
            this.Success = true;
            this.Message = string.Empty;
            Theme theme = new Theme();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Theme));
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            try
            {
                theme = (Theme)xmlSerializer.Deserialize((Stream)fileStream);
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
            return theme;
        }

        public Theme SaveTheme(Theme theme, string filePath)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Theme));
            TextWriter textWriter = (TextWriter)new StreamWriter(filePath);
            try
            {
                xmlSerializer.Serialize(textWriter, (object)theme);
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
            return theme;
        }
    }
}
