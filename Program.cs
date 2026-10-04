using System;
using System.Collections.Generic;
using System.Text;

namespace DGHomeWork
{
    public class Program
    {
        private static string _userName = "Незнакомец";
        private static bool _isSigned = false;

        private const int _taskCountMinLimit = 1;
        private const int _taskCountMaxLimit = 100;
        private const int _taskLengthMinLimit = 1;
        private const int _taskLengthMaxLimit = 100;

        private static int _taskCountLimit = _taskCountMaxLimit;
        private static int _taskLengthLimit = _taskLengthMaxLimit;

        private static string? _currCommand;
        private static string? _currParameter;

        private static BotCommand[] _commands =
        [
                new("/start", "Запуск бота", true),
                new("/help", "Список команд", true),
                new("/info", "Информация о боте", true),
                new("/echo", "Повторяю все, что будет введено после пробела", false),
                new("/addtask", "Добавить задачу в список", false),
                new("/showtasks", "Показать все задачи из списка", false),
                new("/removetask", "Удалить задачу из списка", false),
                new("/exit", "Завершение работы", true),
        ];
        
        private static string? _commandsTxt;

        private static List<string> _tasks = [];

        private static BotBehavior _currentBot = new();

        public static void Main()
        {
            _commandsTxt = GenerateCommandsTxt();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{_currentBot.HelloMsg}{_commandsTxt}");

            do
            {
                try
                {
                    (_currCommand, _currParameter) = SeparateInput(Convert.ToString(GetUserInput()));

                    switch (_currCommand)
                    {
                        case "/start":
                            StartCommand();
                            break;
                        case "/help":
                            HelpCommand();
                            break;
                        case "/info":
                            InfoCommand();
                            break;
                        case "/echo":
                            EchoCommand();
                            break;
                        case "/addtask":
                            AddTaskCommand();
                            break;
                        case "/showtasks":
                            ShowTasksCommand();
                            break;
                        case "/removetask":
                            RemoveTaskCommand();
                            break;
                        case "/exit":
                            ExitCommand();
                            break;
                        default:
                            UnknownCommand();
                            break;
                    }
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Сообщение: {ex.Message}");
                }
                catch (TaskCountLimitException ex)
                {
                    Console.WriteLine($"Сообщение: {ex.Message}");
                }
                catch (TaskLengthLimitException ex)
                {
                    Console.WriteLine($"Сообщение: {ex.Message}");
                }
                catch (DuplicateTaskException ex)
                {
                    Console.WriteLine($"Сообщение: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Произошла непредвиденная ошибка: {ex.GetType().Name}");
                    Console.WriteLine($"Сообщение: {ex.Message}");
                    Console.WriteLine($"StackTrace: {ex.StackTrace}");
                    Console.WriteLine($"InnerException: {ex.InnerException?.Message ?? "нет"}");
                }

            } while (_currCommand != "/exit");
        }

        private static string GenerateCommandsTxt()
        {
            StringBuilder _stringBuilder = new();

            for (int i = 0; i < _commands.Length - 1; i++)
            {
                if (_isSigned || _commands[i].IsAvailableToGuests)
                {
                    _stringBuilder.Append($"{_commands[i].Name} - {_commands[i].Description}\n");
                }
            }
            _stringBuilder.Append($"{_commands[^1].Name} - {_commands[^1].Description}");

            return _stringBuilder.ToString();
        }

        private static string GetUserInput()
        {
            Console.ForegroundColor = ConsoleColor.White;
            var userUnput = Console.ReadLine()!;
            Console.ForegroundColor = ConsoleColor.Green;

            return userUnput;
        }

        private static (string, string) SeparateInput(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return (string.Empty, string.Empty);
            }

            string[] parts = input.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            string command = parts[0].ToLower();
            string parameter = parts.Length > 1 ? parts[1] : string.Empty;

            return (command, parameter);
        }

        private static void StartCommand()
        {
            if (!_isSigned)
            {
                Console.WriteLine($"{_currentBot.UnsignedStartMsg}");
            }
            else
            {
                Console.WriteLine($"{_currentBot.SignedStartMsg}");
            }

            _userName = Convert.ToString(GetUserInput());

            _isSigned = true;
            _commandsTxt = GenerateCommandsTxt();

            SetTaskCountLimit();
            SetTaskLengthLimit();

            HelpCommand();
        }

        private static void HelpCommand()
        {
            Console.WriteLine($"{_userName}{_currentBot.HelpMsg}{_commandsTxt}");
        }

        private static void InfoCommand()
        {
            Console.WriteLine($"{_userName}{_currentBot.InfoMsg}");
        }

        private static void EchoCommand()
        {
            if (!_isSigned)
            {
                Console.WriteLine($"{_userName}{_currentBot.UnsignedErrorMsg}");
            }
            else
            {
                Console.WriteLine(_currParameter);
            }
        }

        private static void AddTaskCommand()
        {
            if (!_isSigned)
            {
                Console.WriteLine($"{_userName}{_currentBot.UnsignedErrorMsg}");
            }
            else
            {
                Console.WriteLine($"{_userName}{_currentBot.TasksAddMsg}");

                string input = Convert.ToString(GetUserInput());
                
                if (_tasks.Count == _taskCountLimit)
                {
                    throw new TaskCountLimitException(_taskCountLimit);
                }
                
                if (input.Length > _taskLengthLimit)
                {
                    throw new TaskLengthLimitException(input.Length, _taskLengthLimit);
                }
                
                foreach (var task in _tasks)
                {
                    if (task == input)
                    {
                        throw new DuplicateTaskException(input);
                    }
                }

                ValidateString(input);

                _tasks.Add(input);

                Console.WriteLine($"{_userName}{_currentBot.TasksAddedMsg} {input}");
            }
        }

        private static void ShowTasksCommand()
        {
            if (!_isSigned)
            {
                Console.WriteLine($"{_userName}{_currentBot.UnsignedErrorMsg}");
            }
            else
            {
                if (_tasks.Count > 0)
                {
                    Console.WriteLine($"{_userName}{_currentBot.TasksListMsg}");

                    for (int i = 0; i < _tasks.Count; i++)
                    {
                        Console.WriteLine($"{i+1}. {_tasks[i]}");
                    }
                }
                else
                {
                    Console.WriteLine($"{_userName}{_currentBot.EmptyTasksListErrorMsg}");
                }
            }
        }

        private static void RemoveTaskCommand()
        {
            if (!_isSigned)
            {
                Console.WriteLine($"{_userName}{_currentBot.UnsignedErrorMsg}");
            }
            else
            {
                ShowTasksCommand();

                if (_tasks.Count > 0)
                {
                    Console.WriteLine($"{_currentBot.TasksRemoveMsg}");
                    do
                    {
                        string? input = Convert.ToString(GetUserInput());

                        if (int.TryParse(input, out int number) && number > 0 && number <= _tasks.Count)
                        {
                            Console.WriteLine($"{_userName}{_currentBot.TasksRemovedMsg} {_tasks[number - 1]}");
                            _tasks.RemoveAt(number - 1);
                            break;
                        }
                        else
                        {
                            Console.WriteLine($"{_userName}{_currentBot.TasksRemoveExistErrorMsg}");
                        }
                    } while (true);
                }
            }
        }

        private static void ExitCommand()
        {
            Console.WriteLine($"{_userName}{_currentBot.ExitMsg}");
        }

        private static void UnknownCommand()
        {
            Console.WriteLine($"{_userName}{_currentBot.UnknownCommandErrorMsg}{_commandsTxt}");
        }

        private static void SetTaskCountLimit()
        {
            while (true)
            {
                Console.WriteLine($"{_currentBot.TaskCountLimitMsg}");
                try
                {
                    ParseAndValidateInt(GetUserInput(), _taskCountMinLimit, _taskCountMaxLimit);
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
                Console.WriteLine($"{_currentBot.TaskLengthLimitMsg}");
                try
                {
                    ParseAndValidateInt(GetUserInput(), _taskLengthMinLimit, _taskLengthMaxLimit);
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

        private static void ValidateString(string? str)
        {
            if (str == null)
            {
                throw new ArgumentException("Строка не может быть null");
            }
                
            if (str.Length == 0)
            {
                throw new ArgumentException("Строка не может быть пустой");
            }

            if (string.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentException("Строка должна содержать символы, помимо пробелов");
            }
        }
    }
}