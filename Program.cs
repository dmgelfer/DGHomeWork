using Otus.ToDoList.ConsoleBot;
using System;

namespace DGHomeWork
{
    public class Program
    {
        private const int _taskCountMinLimit = 1;
        private const int _taskCountMaxLimit = 100;
        private const int _taskLengthMinLimit = 1;
        private const int _taskLengthMaxLimit = 100;

        private static int _taskCountLimit;
        private static int _taskLengthLimit;

        public static void Main()
        {
            while (true)
            {
                try
                {
                    Console.ForegroundColor = ConsoleColor.Green;

                    SetTaskCountLimit();
                    SetTaskLengthLimit();

                    Console.ForegroundColor = ConsoleColor.White;

                    var botClient = new ConsoleBotClient();
                    var userService = new UserService();
                    var toDoService = new ToDoService(_taskCountLimit, _taskLengthLimit);
                    var handler = new UpdateHandler(userService, toDoService);

                    botClient.StartReceiving(handler);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Произошла непредвиденная ошибка: {ex.GetType().Name}");
                }
            }
        }

        private static string GetUserInput()
        {
            Console.ForegroundColor = ConsoleColor.White;
            var userUnput = Console.ReadLine()!;
            Console.ForegroundColor = ConsoleColor.Green;

            return userUnput;
        }

        private static void SetTaskCountLimit()
        {
            while (true)
            {
                Console.WriteLine($"Введите максимальное количество задач, доступное одному пользователю");
                try
                {
                    _taskCountLimit = ParseAndValidateInt(GetUserInput(), _taskCountMinLimit, _taskCountMaxLimit);
                    return;
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Исключение: {ex.Message}");
                }
            }
        }

        private static void SetTaskLengthLimit()
        {
            while (true)
            {
                Console.WriteLine($"Введите максимальное количество символов для описания одной задачи");
                try
                {
                    _taskLengthLimit = ParseAndValidateInt(GetUserInput(), _taskLengthMinLimit, _taskLengthMaxLimit);
                    return;
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Исключение: {ex.Message}");
                }
            }
        }

        private static int ParseAndValidateInt(string? str, int min, int max)
        {
            if (int.TryParse(str, out int number)
                        && number >= min
                        && number <= max)
            {
                return number;
            }

            throw new ArgumentException($"Это должно быть число от {min} до {max}");
        }
    }
}
