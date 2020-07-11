using System;
using System.IO;
using System.Xml.Serialization;

namespace Babalouba
{
    [Serializable]
    public class PlayListLoader
    {
        public bool Success = true;
        public string Message = string.Empty;

        public PlaylistCollection LoadPlayLists(string filePath)
        {
            this.Success = true;
            this.Message = string.Empty;
            PlaylistCollection bbl = new PlaylistCollection();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(PlaylistCollection));
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            try
            {
                bbl = (PlaylistCollection)xmlSerializer.Deserialize((Stream)fileStream);
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
            return bbl;
        }

        public PlaylistCollection SavePlayLists(PlaylistCollection playList, string filePath)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(PlaylistCollection));
            TextWriter textWriter = (TextWriter)new StreamWriter(filePath);
            try
            {
                xmlSerializer.Serialize(textWriter, (object)playList);
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
            return playList;
        }
    }
}
