//using System;
//using System.Diagnostics;
//using System.Management;
//using System.Threading;
//using Microsoft.Data.Sqlite;

//namespace HardwareMonitor
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            // Создаем базу данных и таблицу HardwareUsage


//            // Создаем объекты для измерения параметров
//            var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
//            var gpuCounter = new PerformanceCounter("GPU Engine", "Utilization Percentage", "pid_0_luid_0x00000000_0x0000A1C4_phys_0_eng_3_engtype_Rasterization");
//            var ramCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
//            var diskCounter = new PerformanceCounter("LogicalDisk", "% Free Space", "_Total");


//            while (true)
//            {
//                // Получаем текущее время в формате unix
//                var timeOfLog = DateTimeOffset.Now.ToUnixTimeSeconds();

//                // Получаем загруженность процессора и видеокарты в процентах
//                var ID = cpuCounter.NextValue();
//                //var gpuPercent = gpuCounter.NextValue();

//                // Получаем температуру процессора и видеокарты в градусах Цельсия
//                // Этот код может не работать на некоторых системах, в зависимости от наличия датчиков температуры
//                var cpuTemp = GetCpuTemp();

//                // Получаем загруженность оперативной памяти в процентах
//                var ramPercent = ramCounter.NextValue();

//                // Получаем свободное место на жестком диске в процентах
//                var diskUsage = 100 - diskCounter.NextValue();

//                // Выводим полученные данные на консоль
//                Console.WriteLine($"Time: {timeOfLog}, CPU: {ID:F2}% / {cpuTemp}°C, GPU: - °C, RAM: {ramPercent:F2}%, Disk: {diskUsage:F2}%");

//                // Записываем полученные данные в базу данных


//                // Случайная задержка от 10 до 15 секунд
//                Thread.Sleep(1000); // Преобразуем секунды в миллисекунды
//            }
//        }

//        // Функция для получения температуры процессора в градусах Цельсия
//        // Этот код может не работать на некоторых системах, в зависимости от наличия датчиков температуры
//        static float GetCpuTemp()
//        {
//            var searcher = new ManagementObjectSearcher(@"root\WMI", "SELECT * FROM MSAcpi_ThermalZoneTemperature");
//            foreach (var obj in searcher.Get())
//            {
//                var temp = Convert.ToUInt32(obj["CurrentTemperature"].ToString());
//                return (temp - 2732) / 10.0f; // Преобразуем Кельвины в Цельсии
//            }
//            return 0f; // Возвращаем нуль, если не удалось получить температуру
//        }

//        // Функция для получения температуры видеокарты в градусах Цельсия
//        // Этот код может не работать на некоторых системах, в зависимости от наличия датчиков температуры
//        static float GetGpuTemp()
//        {
//            var searcher = new ManagementObjectSearcher(@"root\WMI", "SELECT * FROM WmiMonitorTemperature");
//            foreach (var obj in searcher.Get())
//            {
//                var temp = Convert.ToUInt32(obj["CurrentTemperature"].ToString());
//                return temp; // Температура уже в Цельсиях
//            }
//            return 0f;
//        }
//    }
//}



using SQLitePCL;
using System;
using System.IO;
using System.Diagnostics;
using System.Management;
using System.Threading;
using Microsoft.Data.Sqlite;

class Program
{
    static void Main(string[] args)
    {
        // Считываем имя директории из аргументов командной строки
        string dirName = "C:\\Users\\User\\Downloads\\work";
        int numb = 0;
        List<string> Dir = new List<string>();
        List<string> Files = new List<string>();
        string DirectoryInfoFile = "";
        string DirectoryInfoDir = "";
        using (var connection = new SqliteConnection("Data Source=DirectoryLog.db"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "CREATE TABLE IF NOT EXISTS DirectoryLog (ID INTEGER, Path REAL, Chenged REAL, TimeOfLog INTEGER, FirstLaunch INTEGER)";
            command.ExecuteNonQuery();
        }

        // Проверяем, существует ли такая директория
        if (Directory.Exists(dirName))
        {
            // Создаем объект DirectoryInfo для работы с директорией
            DirectoryInfo dirInfo = new DirectoryInfo(dirName);

            // Счетчик для ID
            int counter = 1;

            // Флаг для определения первого запуска программы
            bool firstLaunch = true;

            // Переменная для хранения предыдущего размера директории
            long prevSize = 0;

            // Переменные для хранения предыдущего количества файлов и папок в директории
            int prevFileCount = 0;
            int prevDirCount = 0;

            // Бесконечный цикл, пока не нажмем Ctrl+C
            while (true)
            {
                // Получаем текущий размер директории
                long currSize = GetDirectorySize(dirInfo);

                // Получаем текущее количество файлов и папок в директории
                int currFileCount = dirInfo.GetFiles().Length;
                int currDirCount = dirInfo.GetDirectories().Length;
                if(Dir.Count< currDirCount|| Files.Count < currFileCount)
                {
                    foreach (FileInfo file in dirInfo.GetFiles())
                    {
                        if (Files[numb]!=Convert.ToString(file))
                        {

                        }
                        DirectoryInfoFile += " | " + file.Name;
                        Files.Add(file.FullName);
                    }
                    foreach (DirectoryInfo subDir in dirInfo.GetDirectories())
                    {
                        DirectoryInfoDir += " | " + subDir.Name;
                        Dir.Add(subDir.FullName);
                    }
                }
                // Сравниваем текущие и предыдущие значения
                else if (currSize != prevSize || currFileCount != prevFileCount || currDirCount != prevDirCount)
                {
                    // Выводим информацию о директории в консоль
                    Console.WriteLine("ID: {0}", counter);
                    Console.WriteLine("Path: {0}", dirInfo.FullName);
                    Console.WriteLine("DirectoryInfo:");

                    foreach (FileInfo file in dirInfo.GetFiles())
                    {
                        DirectoryInfoFile += " | " + file.Name;
                    }
                    
                    
                    foreach (DirectoryInfo subDir in dirInfo.GetDirectories())
                    {
                        DirectoryInfoDir += " | " + subDir.Name;

                    }

                    string DirectoryInfo = "";
                    DirectoryInfo = "Files: " + DirectoryInfoFile + " | Directories: " + DirectoryInfoDir; 
                    Console.WriteLine(DirectoryInfo);

                    Console.WriteLine("DirectorySize: {0} bytes", currSize);
                    Console.WriteLine("TimeOfLog: {0}", DateTimeOffset.Now.ToUnixTimeSeconds());
                    Console.WriteLine("FirstLaunch: {0}", firstLaunch);


                    using (var connection = new SqliteConnection("Data Source=DirectoryLog.db"))
                    {
                        connection.Open();
                        var command = connection.CreateCommand();
                        command.CommandText = "INSERT INTO DirectoryLog (ID, Path, Chenged, TimeOfLog, FirstLaunch) VALUES (@id, @path, @chenged, @timeOfLog, @firstlaunch)";
                        command.Parameters.AddWithValue("@id", counter);
                        command.Parameters.AddWithValue("@path", dirInfo.FullName);
                        command.Parameters.AddWithValue("@chenged", DirectoryInfo);
                        command.Parameters.AddWithValue("@timeOfLog", DateTimeOffset.Now.ToUnixTimeSeconds());
                        command.Parameters.AddWithValue("@firstlaunch", firstLaunch);
                        command.ExecuteNonQuery();
                    }


                    // Увеличиваем счетчик на 1
                    counter++;

                    // Устанавливаем флаг первого запуска в false
                    firstLaunch = false;

                    // Обновляем предыдущие значения размера, количества файлов и папок директории
                    prevSize = currSize;
                    prevFileCount = currFileCount;
                    prevDirCount = currDirCount;
                    DirectoryInfoFile = "";
                    DirectoryInfoDir = "";
                }

                // Ждем 1 секунду перед следующей итерацией
                System.Threading.Thread.Sleep(1000);
            }
        }
        else
        {
            // Выводим сообщение об ошибке, если директории не существует
            Console.WriteLine("Directory {0} does not exist", dirName);
        }
    }

    // Метод для подсчета размера директории в байтах
    static long GetDirectorySize(DirectoryInfo dirInfo)
    {
        // Инициализируем переменную для хранения размера
        long size = 0;

        // Получаем все файлы в директории и складываем их размеры
        FileInfo[] files = dirInfo.GetFiles();
        foreach (FileInfo file in files)
        {
            size += file.Length;
        }

        // Получаем все поддиректории в директории и рекурсивно вызываем метод для них
        DirectoryInfo[] subDirs = dirInfo.GetDirectories();
        foreach (DirectoryInfo subDir in subDirs)
        {
            size += GetDirectorySize(subDir);
        }

        // Возвращаем размер директории в байтах
        return size;
    }
}