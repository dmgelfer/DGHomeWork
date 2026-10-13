using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DGHomeWork
{
    public class Program
    {
        private static ToDoUser _user = new("Незнакомец");
        private static bool _isSigned = false;

        private const int _taskCountMinLimit = 1;
        private const int _taskCountMaxLimit = 100;
        private const int _taskLengthMinLimit = 1;
        private const int _taskLengthMaxLimit = 100;

        private static int _taskCountLimit;
        private static int _taskLengthLimit;

        private static int _activeTasksCount = 0;

        private static string? _currCommand;
        private static string? _currParameter;

        private static BotCommand[] _commands =
        [
                new("/start", "Запуск бота", true),
                new("/help", "Список команд", true),
                new("/info", "Информация о боте", true),
                new("/echo", "Повторяю все, что будет введено после пробела", false),
                new("/addtask", "Добавить задачу в список", false),
                new("/showtasks", "Показать только активные задачи из списка", false),
                new("/showalltasks", "Показать все задачи из списка", false),
                new("/completetask", "Отметить задачу из списка выполненной", false),
                new("/removetask", "Удалить задачу из списка", false),
                new("/exit", "Завершение работы", true),
        ];
        
        private static string? _commandsTxt;

        private static List<ToDoItem> _tasks = [];

        private static BotBehavior _currentBot = new();

        public static void Main()
        {
            
            Console.ForegroundColor = ConsoleColor.Green;

            SetTaskCountLimit();
            SetTaskLengthLimit();

            _commandsTxt = GenerateCommandsTxt();
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
                        case "/showalltasks":
                            ShowAllTasksCommand();
                            break;
                        case "/completetask":
                            CompleteTaskCommand();
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

            _user.SetNewName(Convert.ToString(GetUserInput()));

            _isSigned = true;
            _commandsTxt = GenerateCommandsTxt();

            HelpCommand();
        }

        private static void HelpCommand()
        {
            Console.WriteLine($"{_user.TelegramUserName}{_currentBot.HelpMsg}{_commandsTxt}");
        }

        private static void InfoCommand()
        {
            Console.WriteLine($"{_user.TelegramUserName}{_currentBot.InfoMsg}");
        }

        private static void EchoCommand()
        {
            if (!_isSigned)
            {
                Console.WriteLine($"{_user.TelegramUserName}{_currentBot.UnsignedErrorMsg}");
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
                Console.WriteLine($"{_user.TelegramUserName}{_currentBot.UnsignedErrorMsg}");
            }
            else
            {
                Console.WriteLine($"{_user.TelegramUserName}{_currentBot.TasksAddMsg}");

                string input = Convert.ToString(GetUserInput());

                ValidateString(input);

                if (_tasks.Count >= _taskCountLimit)
                {
                    throw new TaskCountLimitException(_taskCountLimit);
                }
                
                if (input.Length > _taskLengthLimit)
                {
                    throw new TaskLengthLimitException(input.Length, _taskLengthLimit);
                }

                foreach (var task in _tasks)
                {
                    if (task.ItemName == input)
                    {
                        throw new DuplicateTaskException(input);
                    }
                }

                _tasks.Add(new ToDoItem(_user, input));
                _activeTasksCount++;

                Console.WriteLine($"{_user.TelegramUserName}{_currentBot.TasksAddedMsg} {input}");
            }
        }

        private static void ShowTasksCommand()
        {
            if (!_isSigned)
            {
                Console.WriteLine($"{_user.TelegramUserName}{_currentBot.UnsignedErrorMsg}");
            }
            else
            {
                if (_activeTasksCount > 0)
                {
                    Console.WriteLine($"{_user.TelegramUserName}{_currentBot.TasksListMsg}");

                    for (int i = 0; i < _tasks.Count; i++)
                    {
                        if (_tasks[i].State == ToDoItemState.Active)
                        {
                            Console.WriteLine($"{i + 1}. {_tasks[i].ItemName} - {_tasks[i].CreatedAt} - {_tasks[i].ItemId}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"{_user.TelegramUserName}{_currentBot.EmptyTasksListErrorMsg}");
                }
            }
        }

        private static void ShowAllTasksCommand()
        {
            if (!_isSigned)
            {
                Console.WriteLine($"{_user.TelegramUserName}{_currentBot.UnsignedErrorMsg}");
            }
            else
            {
                if (_tasks.Count > 0)
                {
                    Console.WriteLine($"{_user.TelegramUserName}{_currentBot.TasksListMsg}");

                    for (int i = 0; i < _tasks.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. ({_tasks[i].State}) {_tasks[i].ItemName} - {_tasks[i].CreatedAt} - {_tasks[i].ItemId}");
                    }
                }
                else
                {
                    Console.WriteLine($"{_user.TelegramUserName}{_currentBot.EmptyTasksListErrorMsg}");
                }
            }
        }

        private static void CompleteTaskCommand()
        {
            if (!_isSigned)
            {
                Console.WriteLine($"{_user.TelegramUserName}{_currentBot.UnsignedErrorMsg}");
            }
            else
            {
                if (Guid.TryParse(_currParameter, out Guid itemIdToComplete))
                {
                    foreach (var task in _tasks)
                    {
                        if (task.ItemId == itemIdToComplete)
                        {
                            Console.WriteLine($"{_user.TelegramUserName}{_currentBot.TasksCompletedMsg} {task.ItemName}");
                            task.Complete();
                            _activeTasksCount--;
                        }
                    }
                }
            }
        }

        private static void RemoveTaskCommand()
        {
            if (!_isSigned)
            {
                Console.WriteLine($"{_user.TelegramUserName}{_currentBot.UnsignedErrorMsg}");
            }
            else
            {
                ShowAllTasksCommand();

                if (_tasks.Count > 0)
                {
                    Console.WriteLine($"{_currentBot.TasksRemoveMsg}");
                    do
                    {
                        string? input = Convert.ToString(GetUserInput());

                        if (int.TryParse(input, out int number) && number > 0 && number <= _tasks.Count)
                        {
                            Console.WriteLine($"{_user.TelegramUserName}{_currentBot.TasksRemovedMsg} {_tasks[number - 1].ItemName}");
                            _tasks.RemoveAt(number - 1);

                            _activeTasksCount = 0;
                            foreach (var task in _tasks)
                            { 
                                if (task.State == ToDoItemState.Active)
                                {
                                    _activeTasksCount++;
                                }
                            }
                            break;
                        }
                        else
                        {
                            Console.WriteLine($"{_user.TelegramUserName}{_currentBot.TasksRemoveExistErrorMsg}");
                        }
                    } while (true);
                }
            }
        }

        private static void ExitCommand()
        {
            Console.WriteLine($"{_user.TelegramUserName}{_currentBot.ExitMsg}");
        }

        private static void UnknownCommand()
        {
            Console.WriteLine($"{_user.TelegramUserName}{_currentBot.UnknownCommandErrorMsg}{_commandsTxt}");
        }

        private static void SetTaskCountLimit()
        {
            while (true)
            {
                Console.WriteLine($"{_currentBot.TaskCountLimitMsg}");
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
                Console.WriteLine($"{_currentBot.TaskLengthLimitMsg}");
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