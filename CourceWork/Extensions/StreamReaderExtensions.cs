using System;
using Serilog;
using System.IO;
using System.Text;
using System.Threading.Tasks;


namespace CourceWork
{
    public static class StreamReaderExtensions
    {
        
        public static string ReadLineCustom(this StreamReader sr)
        {
            var stringBuilder = new StringBuilder();
            var inputBuff = new char[256];
            sr.Read(inputBuff, 0, inputBuff.Length);
            stringBuilder.Append(inputBuff);
            return stringBuilder.Replace("\0", "").Replace("\n", "").Replace("\r", "").ToString();
        }

        public static string ReadConnectionId(this StreamReader sr)
        {
            const int ID_FIELD = 2;
            var stringBuilder = new StringBuilder();
            var inputBuff = new char[255];
            bool isRecieved = false;
            while (!isRecieved)
            {
                try
                {
                    sr.Read(inputBuff, 0, 255);
                    isRecieved = true;
                }
                catch (IOException ex) 
                {
                    Log.Error("Метод: " + ex.TargetSite);
                    Log.Error("Исключение: " + ex.Message);
                    Log.Error("Трассировка стека: " + ex.Message);
                }
            }
            stringBuilder.Append(inputBuff);
            stringBuilder.Replace("\r\n", "");
            var str = stringBuilder.ToString();
            var stringArray = str.Split(':');
            stringArray = stringArray[ID_FIELD].Split('=');
            return stringArray[1].Replace("\0", "").Replace("\n","").Replace("\r", "");
        }

        public async static Task<string> ReadLineCustomAsync(this StreamReader sr)
        {
            var stringBuilder = new StringBuilder();
            var inputBuff = new char[255];
            await sr.ReadAsync(inputBuff, 0, 255);
            stringBuilder.Append(inputBuff);

            return stringBuilder.Replace("\0", "").Replace("\n", "").Replace("\r", "").ToString();
        }
    }
}
