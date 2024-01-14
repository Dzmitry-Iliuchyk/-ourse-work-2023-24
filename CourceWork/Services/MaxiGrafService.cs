using Serilog;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace CourceWork.Services
{
    public class MaxiGrafService
    {
        private IPAddress? host;
        private int port;

        public NetworkStream networkStream;
        public StreamReader streamReader;
        public StreamWriter streamWriter;
        private TcpClient? client;

        public IPAddress Host { get => host; set => host = value; }
        public int Port { get => port; set => port = value; }
        public const string apiKey = "WwB0hlUI5o78N8P7Y815aiyTgN81P5U8";

        public string Group { get; set; }
        public string prefix;
        public string connectionId;

        public string markingTime;
        public string loadedFile;

        public bool isDetailReady = false;
        public bool isGrafAvailableForMarking = false;

        CancellationTokenSource cancelTokenSource;
        public CancellationToken token;

        Task setListenerMode;

        public async Task<bool> TryConnectAsync()
        {
            bool isConnected = false;
            client = new TcpClient();
                try
                {
                    await client.ConnectAsync(Host, Port);
                    isConnected = client.Connected;
                    Log.Information("успех");
                    OnConnectToMaxiGraph();
                }
                catch (SocketException)
                {
                    Log.Error("Не удалось подключиться к сокету удалённого хоста");
                    isConnected = false;
                }
                catch (Exception ex)
                {
                    Log.Error("Метод: " + ex.TargetSite);
                    Log.Error("Исключение: " + ex.Message);
                    Log.Error("Трассировка стека: " + ex.StackTrace);
                    isConnected = false;
                }
            return isConnected;
        }
        private void OnConnectToMaxiGraph()
        {
            Log.Information("Статус подключения: " + client?.Connected);
            try
            {
                networkStream = client.GetStream(); // получаем поток
            }
            catch (ObjectDisposedException ex)
            {
                Log.Error("Метод: " + ex.TargetSite);
                Log.Error("Клиент закрыл подключение к удалённому хосту");
            }
            catch (InvalidOperationException ex)
            {
                Log.Error("Метод: " + ex.TargetSite);
                Log.Error("Отсутствует подключение к удалённому хосту");
            }

            streamReader = new StreamReader(networkStream);
            streamWriter = new StreamWriter(networkStream);

            //Отпрaвляем ApiKey

            streamWriter.WriteLine(apiKey);
            streamWriter.Flush();
            connectionId = streamReader.ReadConnectionId();
            prefix = $"{apiKey}|{connectionId}|";
            Log.Information($"API Key: {apiKey}\n ConnectionID: {connectionId}");

            streamWriter.WriteLine("Api Tcp Prefix Length=" + prefix.Length);
            streamWriter.Flush();
            Log.Information("API TCP Prefix Length = " + prefix.Length + "bytes");

            loadedFile = SendCommandAndReceiveAnswer(prefix + "GetFileName");
            markingTime = GetMarkingTime();
            Group = GetObject();
        }
        public string GetMarkingTime()
        {
            var response = SendCommandAndReceiveAnswer(prefix + "Get Marking Time");
            Log.Information("GetMarkingTime = " + response);
            return response;
        }
        public string GetObject()
        {
            var response = SendCommandAndReceiveAnswer(prefix + "GetObjects=");
            Log.Information("Главная группа: " + response.Split('|')[0]);
            return response.Split('|')[0];
        }
        public void ShiftZ(float z)
        {
            Log.Information("ShiftZ Answer ="
                + SendCommandAndReceiveAnswer(prefix + "MoveAxis=Z" + z.ToString()));
        }
        public void MoveToY(float y)
        {
            SendCommandAndReceiveAnswer(prefix + $"SetNewValue={Group}.PosOfAnchorY=" + y);
        }
        public void MoveToX(float x)
        {
            SendCommandAndReceiveAnswer(prefix + $"SetNewValue={Group}.PosOfAnchorX=" + x);
        }
        public string GetCoordinateZ()
        {
            var response = SendCommandAndReceiveAnswer(prefix + "GetCurrentAxesPosition=Z");
            Log.Information("Axis Z = " + response);
            return response;
        }
        public string GetCoordinateX()
        {
            SendCommandAndReceiveAnswer(prefix + $"GetValue={Group}.PosOfAnchorX=");
            string response = streamReader.ReadLineCustom();
            Log.Information("Axis X = " + response);
            return response;
        }
        public string GetCoordinateY()
        {
            SendCommandAndReceiveAnswer(prefix + $"GetValue={Group}.PosOfAnchorY=");
            string response = streamReader.ReadLineCustom();
            Log.Information("Axis Y = " + response);
            return response;
        }
        public void ShowDetail()
        {
            var response = SendCommandAndReceiveAnswer(prefix + "Show Rectangular Joystick");
            if (int.Parse(response) == 0)
            { Log.Information("Активирован джойстик"); }
            else
                Log.Information("Не удалось активировать джойстик");
        }
        public void StartMarking()
        {
            isGrafAvailableForMarking = false;
            string command = "Start mark";
            Log.Information("Отправлена комманда: " + command);
            streamWriter.WriteLine(prefix + command);
            streamWriter.Flush();
            var MarkingResult = streamReader.ReadLineCustom();

            if (MarkingResult == "MarkingCompletedSuccessfully")
            {
                Log.Information($"Маркировка успешно завершена");
                isDetailReady = false;
                isGrafAvailableForMarking = true;
                return;
            }
            else
            {
                Log.Error($"Ошибка: {MarkingResult}");
                isGrafAvailableForMarking = true;
                isDetailReady = false;
                Shell.Current.DisplayAlert("Error!", "Что-то пошло не так", "Ok");
                return;
            }
        }
        private Task SetListenerMode()
        {
            while (!token.IsCancellationRequested)
            {
                //IsDetailReady = Convert.ToBoolean(RoboReader);
                if (isDetailReady && isGrafAvailableForMarking)
                {
                    StartMarking();
                }
            }
            return Task.CompletedTask;
        }
        private string SendCommandAndReceiveAnswer(string command)
        {
            Log.Information($"Executing command: {command}");
            streamWriter.WriteLine(command);
            streamWriter.Flush();
            return streamReader.ReadLineCustom();
        }
        public void StartButton_Click()
        {
            cancelTokenSource = new CancellationTokenSource();
            token = cancelTokenSource.Token;
            this.isGrafAvailableForMarking = true;
            setListenerMode = new Task(() => this.SetListenerMode(), token);
            setListenerMode.Start();
        }
        public void button_Stop_Click()
        {
            this.streamWriter.WriteLine(this.prefix + "Stop");
            this.streamWriter.Flush();
            //Console.WriteLine(controller.streamReader.ReadLineCustomAsync());
            this.cancelTokenSource.Cancel();
            Console.WriteLine($"Статус потока: {setListenerMode.Status}");
            this.cancelTokenSource.Dispose();
            Console.WriteLine($"Статус потока: {setListenerMode.Status}");
            this.setListenerMode.Dispose();
            Console.WriteLine($"Статус потока: {setListenerMode.Status}");
        }
        public async Task<string> SelectFileProcess(FileResult pathLeFile, BackgroundWorker worker)
        {
            
            int packageDelay = 500;
            double percent = 0;
            //var inBuff = new byte[256];
            string command = "This is a LE file";
            //inBuff = Encoding.UTF8.GetBytes(prefix + command);
            //await networkStream.WriteAsync(inBuff, 0, inBuff.Length);
            await streamWriter.WriteLineAsync(prefix + command);
            await streamWriter.FlushAsync();
            Log.Information($"Отправлена команда: {command}");
            Stream stream = await pathLeFile.OpenReadAsync();
            BinaryReader reader = new BinaryReader(stream);

            byte[] dataPrefix = Encoding.UTF8.GetBytes(prefix);
            byte[] UB = dataPrefix;
            int i = 0;
            int countbytes = 0;
            //Log.Information("PREFIX BYTES==>" + BitConverter.ToString(dataPrefix, 0, dataPrefix.Length));

            while (reader.BaseStream.Position < reader.BaseStream.Length - (256))
            {
                i++;
                // var inputBuff = new byte[256];
                var inputBuff = reader.ReadBytes(256);
                UB = dataPrefix;
                Array.Resize(ref UB, UB.Length + inputBuff.Length);
                Array.Copy(inputBuff, 0, UB, UB.Length - inputBuff.Length, inputBuff.Length);
                await networkStream.WriteAsync(UB, 0, UB.Length);
                percent = 256.0 / reader.BaseStream.Length * 100 * i;
                worker.ReportProgress((int)percent);

                //Log.Information(reader.BaseStream.Position + "==>" + BitConverter.ToString(UB, 0, UB.Length));

                countbytes += 256;
                await Task.Delay(packageDelay);
            }

            int len = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
            if (len > 0)
            {
                //var inputBuff = new byte[len];
                var inputBuff = reader.ReadBytes(len);
                UB = dataPrefix;
                Array.Resize(ref UB, UB.Length + inputBuff.Length);
                Array.Copy(inputBuff, 0, UB, UB.Length - inputBuff.Length, inputBuff.Length);
                await networkStream.WriteAsync(UB, 0, UB.Length);
                //Log.Information(reader.BaseStream.Position + "==>" + BitConverter.ToString(UB, 0, UB.Length));

                countbytes += len;
                await Task.Delay(packageDelay);
            }

            Log.Information("Отправлено байтов = " + countbytes);
            Log.Information("Общее количество байтов = " + reader.BaseStream.Length);
            command = "This is the end of file";
            await streamWriter.WriteLineAsync(prefix + command);
            await streamWriter.FlushAsync();
            Log.Information($"Отправлена команда: {command}");
            //var inpuBuff = Encoding.UTF8.GetBytes(command);
            //await networkStream.WriteAsync(inpuBuff, 0, inpuBuff.Length);
            //networkStream.Flush();
            reader.Close();
            await Task.Delay(200);

            //Считываем ответ
            var response = await streamReader.ReadLineCustomAsync();
            Log.Information(response);
            if (!response.Contains("LE success"))
            {
                Log.Information("Ошибка чтения файла MaxiGraf'ом:" + pathLeFile.FullPath);
                await Task.Delay(50);
                await Shell.Current.DisplayAlert("Error", "Ошибка чтения файла MaxiGraf'ом:" + pathLeFile.FullPath,"Ok");
            }
            worker.ReportProgress((int)percent);
            Log.Information($"Открыт файл: {pathLeFile}");
            markingTime = GetMarkingTime();
            await Task.Delay(50);
            
            Group = GetObject();
            return pathLeFile.FileName;
        }
        public void SetDetailsReady()
        {
            isDetailReady = true;
        }
    }
}