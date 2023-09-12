using Microsoft.Extensions.Configuration;
using NLog;
using Sensor.Settings;

namespace Sensor
{
    static class Program
    {
        private static bool _isRunning = true;

        static async Task Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            AppSettings appSettings = new AppSettings
            {
                ServerUrl = configuration["AppSettings:ServerUrl"],
                DeviceId = configuration["AppSettings:DeviceId"],
                IntervalMinutes = double.TryParse(configuration["AppSettings:IntervalMinutes"], out double intervalValue) ? intervalValue : 0.0,
                MaxDataSentCount = int.TryParse(configuration["AppSettings:MaxDataSentCount"], out int maxCountValue) ? maxCountValue : 0
            };

            SensorSettings sensorSettings = new SensorSettings
            {
                Endpoint = configuration["SensorSettings:Endpoint"],
                MinIlluminance = double.TryParse(configuration["SensorSettings:MinIlluminance"], out double minIlluminanceValue) ? minIlluminanceValue : 0.0,
                MaxIlluminance = double.TryParse(configuration["SensorSettings:MaxIlluminance"], out double maxIlluminanceValue) ? maxIlluminanceValue : 0.0
            };

            Console.WriteLine("Sensor is started.");

            do
            {
                Console.WriteLine("Choose the output method or enter 'Q' to exit:");
                Console.WriteLine("1 - Console");
                Console.WriteLine("2 - File (log.txt)");
                Console.WriteLine("3 - Http endpoint");
                Console.WriteLine("Q - Exit");
                Console.Write("Enter the corresponding number and press Enter: ");

                string? userInput = Console.ReadLine();

                NLog.ILogger? logger = null;

                if (userInput != null)
                {
                    switch (userInput.ToLower())
                    {
                        case "1":
                            Console.WriteLine("Output - Console");
                            logger = LogManager.GetLogger("consoleLogger");
                            break;
                        case "2":
                            Console.WriteLine("Output - File");
                            logger = LogManager.GetLogger("fileLogger");
                            break;
                        case "3":
                            Console.WriteLine("Output - Http endpoint");
                            logger = LogManager.GetLogger("httpLogger");

                            var deviceId = appSettings?.DeviceId;
                            GlobalDiagnosticsContext.Set("deviceId", deviceId);

                            break;
                        case "q":
                            Console.WriteLine("Exiting...");
                            _isRunning = false;
                            break;
                        default:
                            Console.WriteLine("Invalid input. Please enter a valid option.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Input is null. Please enter a valid option.");
                }

                Console.WriteLine();

                if (logger != null)
                {
                    var sensor = new LightSensor(appSettings, sensorSettings, logger);
                    await sensor.CollectAndSendDataAsync();
                }
            }
            while (_isRunning);
        }
    }
}