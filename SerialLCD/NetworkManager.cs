using System;
using System.Net.Sockets;

namespace SerialLCD.Managers
{
    /// <summary>
    /// Singleton класс для управления сетевыми соединениями
    /// </summary>
    public sealed class NetworkManager
    {
        private static NetworkManager _instance = null;
        private static readonly object _lock = new object();
        
        private string _ipAddress = "192.168.0.104";
        private int _port = 81;
        private TcpClient _client;
        private NetworkStream _stream;
        private bool _isConnected = false;
        private DateTime _lastConnectionAttempt = DateTime.MinValue;
        private int _reconnectDelay = 500; // Уменьшаем начальную задержку

        // Приватный конструктор для Singleton
        private NetworkManager() { }

        /// <summary>
        /// Получить единственный экземпляр NetworkManager
        /// </summary>
        public static NetworkManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new NetworkManager();
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Настройка параметров подключения
        /// </summary>
        public void Configure(string ipAddress, int port)
        {
            _ipAddress = ipAddress;
            _port = port;
        }

        /// <summary>
        /// Подключение к устройству
        /// </summary>
        public bool Connect()
        {
            try
            {
                if (_isConnected)
                {
                    Disconnect();
                }

                _client = new TcpClient();
                // Устанавливаем таймауты для более стабильного соединения
                _client.ReceiveTimeout = 5000;
                _client.SendTimeout = 5000;
                
                _client.Connect(_ipAddress, _port);
                _stream = _client.GetStream();
                _isConnected = true;
                
                Console.WriteLine($"Подключено к устройству на {_ipAddress}:{_port}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка подключения: {ex.Message}");
                _isConnected = false;
                return false;
            }
        }

        /// <summary>
        /// Отключение от устройства
        /// </summary>
        public void Disconnect()
        {
            try
            {
                _stream?.Close();
                _client?.Close();
                _isConnected = false;
                Console.WriteLine("Отключено от устройства.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при отключении: {ex.Message}");
            }
            finally
            {
                _stream = null;
                _client = null;
            }
        }

        /// <summary>
        /// Отправка данных на устройство
        /// </summary>
        public bool SendData(byte[] data)
        {
            if (data == null)
            {
                Console.WriteLine("Ошибка: Данные не могут быть null.");
                return false;
            }

            if (!_isConnected || _stream == null)
            {
                Console.WriteLine("Ошибка: Нет соединения с устройством.");
                return false;
            }

            try
            {
                _stream.Write(data, 0, data.Length);
                _stream.Flush();
                Console.WriteLine($"Успешно отправлено {data.Length} байт.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при отправке данных: {ex.Message}");
                // При ошибке отправки помечаем соединение как разорванное
                _isConnected = false;
                return false;
            }
        }

        /// <summary>
        /// Попытка переподключения при разрыве соединения
        /// </summary>
        public bool TryReconnect()
        {
            if (_isConnected)
                return true;

            // Проверяем, прошло ли достаточно времени с последней попытки
            var timeSinceLastAttempt = DateTime.Now - _lastConnectionAttempt;
            if (timeSinceLastAttempt.TotalMilliseconds < _reconnectDelay)
            {
                Console.WriteLine($"Ожидание {_reconnectDelay}мс перед переподключением...");
                return false;
            }

            Console.WriteLine("Попытка переподключения...");
            _lastConnectionAttempt = DateTime.Now;
            
            bool success = Connect();
            
            if (success)
            {
                _reconnectDelay = 500; // Сбрасываем задержку при успешном подключении
            }
            else
            {
                // Увеличиваем задержку при неудаче (максимум 3 секунды)
                _reconnectDelay = Math.Min(_reconnectDelay * 2, 3000);
                Console.WriteLine($"Следующая попытка через {_reconnectDelay}мс");
            }
            
            return success;
        }

        /// <summary>
        /// Проверка состояния соединения
        /// </summary>
        public bool CheckConnection()
        {
            if (!_isConnected || _client == null || _stream == null)
                return false;

            try
            {
                // Проверяем, что сокет все еще подключен
                return _client.Connected;
            }
            catch
            {
                _isConnected = false;
                return false;
            }
        }

        /// <summary>
        /// Проверка состояния подключения
        /// </summary>
        public bool IsConnected => _isConnected;

        /// <summary>
        /// Получение информации о подключении
        /// </summary>
        public string ConnectionInfo => $"IP: {_ipAddress}, Port: {_port}, Connected: {_isConnected}";
    }
}
