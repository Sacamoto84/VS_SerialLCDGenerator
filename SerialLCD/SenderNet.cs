using System;
using System.Net.Sockets;
using SerialLCD.Managers;

namespace SerialLCD
    {
        /// <summary>
        /// Класс для отправки данных через сеть с использованием Singleton паттерна
        /// </summary>
        internal class SenderNet
        {
            private static SenderNet _instance = null;
            private static readonly object _lock = new object();
            
            private NetworkManager _networkManager;
            
            // Приватный конструктор для Singleton
            private SenderNet()
            {
                _networkManager = NetworkManager.Instance;
            }

            /// <summary>
            /// Получить единственный экземпляр SenderNet
            /// </summary>
            public static SenderNet Instance
            {
                get
                {
                    if (_instance == null)
                    {
                        lock (_lock)
                        {
                            if (_instance == null)
                                _instance = new SenderNet();
                        }
                    }
                    return _instance;
                }
            }

            /// <summary>
            /// Настройка параметров подключения
            /// </summary>
            public void Configure(string ip = "192.168.0.100", int portNumber = 81)
            {
                _networkManager.Configure(ip, portNumber);
            }

            /// <summary>
            /// Подключение к ESP32
            /// </summary>
            public bool Connect()
            {
                return _networkManager.Connect();
            }

            /// <summary>
            /// Отключение от ESP32
            /// </summary>
            public void Disconnect()
            {
                _networkManager.Disconnect();
            }

            /// <summary>
            /// Отправка массива из 1024 байт на ESP32
            /// </summary>
            public bool SendByteArray(byte[] data)
            {
                if (data == null || data.Length != 1024)
                {
                    Console.WriteLine("Ошибка: Данные должны быть массивом из 1024 байт.");
                    return false;
                }

                return _networkManager.SendData(data);
            }

            /// <summary>
            /// Проверка состояния подключения
            /// </summary>
            public bool IsConnected => _networkManager.IsConnected;

            /// <summary>
            /// Получение информации о подключении
            /// </summary>
            public string ConnectionInfo => _networkManager.ConnectionInfo;

            /// <summary>
            /// Пример: создание и отправка тестового массива из 1024 байт
            /// </summary>
            public void SendSampleData()
            {
                // Создаем тестовый массив (например, заполненный значениями от 0 до 255)
                byte[] sampleData = new byte[1024];
                for (int i = 0; i < sampleData.Length; i++)
                {
                    sampleData[i] = (byte)(i % 256);
                }

                if (Connect())
                {
                    SendByteArray(sampleData);
                    Disconnect();
                }
            }
        }
}
