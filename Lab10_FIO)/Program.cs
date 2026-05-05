using Lab10_FIO_;
using Serilog;
using System.Text;

namespace Lab10_FIO_
{
    class Program
    {
        private static readonly UserValidator _validator = new UserValidator();

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            ConfigureLogging();


            Console.WriteLine("=== Система регистрации пользователя ===\n");
            Console.WriteLine("Зарезервированные логины: " + string.Join(", ", UserValidator.GetReservedLogins()));
            Console.WriteLine();

            bool continueRunning = true;
            while (continueRunning)
            {
                Console.WriteLine("\n--- Новая регистрация ---");

                // Ввод данных
                Console.Write("Введите логин: ");
                string login = Console.ReadLine() ?? "";

                Console.Write("Введите пароль: ");
                string password = ReadPassword();
                Console.WriteLine();

                Console.Write("Подтвердите пароль: ");
                string passwordConfirm = ReadPassword();
                Console.WriteLine();

                // Маскирование паролей 
                string maskedPassword = PasswordMasker.MaskPassword(password);
                string maskedPasswordConfirm = PasswordMasker.MaskPassword(passwordConfirm);

                // Валидация
                DateTime requestTime = DateTime.Now;
                var result = _validator.ValidateUser(login, password, passwordConfirm);

                if (result.success)
                {
                   
                    Log.Information("Время: {Time} | Логин: {Login} | Пароль: {MaskPwd} | Подтверждение: {MaskConfirm} | Успешная регистрация",
                        requestTime.ToString("yyyy-MM-dd HH:mm:ss"), login, maskedPassword, maskedPasswordConfirm);

                    Console.WriteLine("\n✓ РЕГИСТРАЦИЯ УСПЕШНА!");
                    Console.WriteLine($"Результат: True");
                }
                else
                {
                    
                    Log.Error("Время: {Time} | Логин: {Login} | Пароль: {MaskPwd} | Подтверждение: {MaskConfirm} | Ошибка: {Error}",
                        requestTime.ToString("yyyy-MM-dd HH:mm:ss"), login, maskedPassword, maskedPasswordConfirm, result.message);

                    Console.WriteLine("\n✗ ОШИБКА РЕГИСТРАЦИИ!");
                    Console.WriteLine($"Результат: False");
                    Console.WriteLine($"Причина: {result.message}");
                }

               
                Console.Write("\nПродолжить? (Y/N): ");
                string? choice = Console.ReadLine()?.ToUpper();
                continueRunning = (choice == "Y" || choice == "YES");
            }

            Log.Information("=== Приложение завершено ===");
            Log.CloseAndFlush();

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

       
        private static void ConfigureLogging()
        {
           
            if (!Directory.Exists("logs"))
                Directory.CreateDirectory("logs");

            string template = "{Timestamp:HH:mm:ss} | [{Level:u3}] | {Message:lj}{NewLine}{Exception}";

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(outputTemplate: template)
                .WriteTo.File("logs/log_.txt",
                    outputTemplate: template,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30)
                .CreateLogger();

            Log.Debug("Логирование настроено. Папка logs создана.");
        }

    
        private static string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password[0..^1];
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
            } while (key.Key != ConsoleKey.Enter);

            return password;
        }
    }
}