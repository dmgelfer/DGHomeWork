using System;
using System.Collections.Generic;
using System.Text;

namespace DGHomeWork
{
    public class Program
    {
        public static void Main()
        {
            string? _userName = "Незнакомец";
            bool _isSigned = false;
            string? _currCommand;
            string? _currParameter;

            BotCommand[] _commands =
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
            string? _commandsTxt;
                       
            List<BotTask> _tasks = [];
            int _taskCount = 0;

            BotBehavior _currentBot = new();
            
            StringBuilder _stringBuilder = new();
            for (int i = 0; i < _commands.Length - 1; i++)
            {
                if (_commands[i].IsAvailableToGuests)
                {
                    _stringBuilder.Append($"{_commands[i].Name} - {_commands[i].Description}\n");
                }
            }
            _stringBuilder.Append($"{_commands[^1].Name} - {_commands[^1].Description}");
            _commandsTxt = _stringBuilder.ToString();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{_currentBot.HelloMsg}{_commandsTxt}");

            do
            {
                Console.ForegroundColor = ConsoleColor.White;
                (_currCommand, _currParameter) = ParseCommand(Convert.ToString(Console.ReadLine()));

                Console.ForegroundColor = ConsoleColor.Green;
                switch (_currCommand)
                {
                    case "/start":
                        if (!_isSigned)
                        {
                            Console.WriteLine($"{_currentBot.UnsignedStartMsg}");
                        }
                        else
                        {
                            Console.WriteLine($"{_currentBot.SignedStartMsg}");
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                        _userName = Convert.ToString(Console.ReadLine());
                        Console.ForegroundColor = ConsoleColor.Green;

                        _isSigned = true;

                        _stringBuilder = new();
                        for (int i = 0; i < _commands.Length-1; i++)
                        {
                            _stringBuilder.Append($"{_commands[i].Name} - {_commands[i].Description}\n");
                        }
                        _stringBuilder.Append($"{_commands[^1].Name} - {_commands[^1].Description}");
                        _commandsTxt = _stringBuilder.ToString(); 

                        Console.WriteLine($"{_userName}{_currentBot.HelpMsg}{_commandsTxt}");
                        break;
                    case "/help":
                        Console.WriteLine($"{_userName}{_currentBot.HelpMsg}{_commandsTxt}");
                        break;
                    case "/info":
                        Console.WriteLine($"{_userName}{_currentBot.InfoMsg}");
                        break;
                    case "/echo":
                        if (!_isSigned)
                        {
                            Console.WriteLine($"{_userName}{_currentBot.UnsignedErrorMsg}");
                        }
                        else
                        {
                            Console.WriteLine(_currParameter);
                        }
                        break;
                    case "/addtask":
                        if (!_isSigned)
                        {
                            Console.WriteLine($"{_userName}{_currentBot.UnsignedErrorMsg}");
                        }
                        else
                        {
                            Console.WriteLine($"{_userName}{_currentBot.TasksAddMsg}");
                            Console.ForegroundColor = ConsoleColor.White;
                            string? input = Convert.ToString(Console.ReadLine());
                            Console.ForegroundColor = ConsoleColor.Green;

                            _taskCount++;
                            _tasks.Add(new(_taskCount, input, true));
                            Console.WriteLine($"{_userName}{_currentBot.TasksAddedMsg} {input}");
                        }
                        break;
                    case "/showtasks":
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
                                    if (_tasks[i].IsVisible)
                                    {
                                        Console.WriteLine($"{_tasks[i].ID}. {_tasks[i].Description}");
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine($"{_userName}{_currentBot.EmptyTasksListErrorMsg}");
                            }
                        }
                        break;
                    case "/removetask":
                        if (!_isSigned)
                        {
                            Console.WriteLine($"{_userName}{_currentBot.UnsignedErrorMsg}");
                        }
                        else
                        {
                            int visibleTasksCount = 0;

                            for (int i = 0; i < _tasks.Count; i++)
                            {
                                if (_tasks[i].IsVisible)
                                {
                                    visibleTasksCount++;
                                }
                            }

                            if (visibleTasksCount > 0)
                            {
                                Console.WriteLine($"{_userName}{_currentBot.TasksRemoveMsg}");
                                for (int i = 0; i < _tasks.Count; i++)
                                {
                                    if (_tasks[i].IsVisible)
                                    {
                                        Console.WriteLine($"{_tasks[i].ID}. {_tasks[i].Description}");
                                    }
                                }

                                Console.ForegroundColor = ConsoleColor.White;
                                int taskID = -1;

                                do
                                {
                                    string? input = Convert.ToString(Console.ReadLine());

                                    if (int.TryParse(input, out int number) && number > 0 && number <= _tasks.Count)
                                    {
                                        taskID = number - 1;
                                        Console.ForegroundColor = ConsoleColor.Green;

                                        if (_tasks[taskID].IsVisible == true)
                                        {
                                            Console.WriteLine($"{_userName}{_currentBot.TasksRemovedMsg} {taskID + 1} {_tasks[taskID].Description}");
                                            _tasks[taskID].IsVisible = false;
                                        }
                                        else
                                        {
                                            Console.WriteLine($"{_userName}{_currentBot.TasksRemoveHidenErrorMsg}");
                                            taskID = -1;
                                        }
                                        Console.ForegroundColor = ConsoleColor.White;
                                    }
                                    else
                                    {
                                        Console.ForegroundColor = ConsoleColor.Green;
                                        Console.WriteLine($"{_userName}{_currentBot.TasksRemoveExistErrorMsg}");
                                        Console.ForegroundColor = ConsoleColor.White;
                                    }
                                } while (taskID == -1);
                            }
                            else
                            {
                                Console.WriteLine($"{_userName}{_currentBot.EmptyTasksListErrorMsg}");
                            } 
                        }
                        break;
                    case "/exit":
                        Console.WriteLine($"{_userName}{_currentBot.ExitMsg}");
                        break;
                    default:
                        Console.WriteLine($"{_userName}{_currentBot.UnknownCommandErrorMsg}{_commandsTxt}");
                        break;
                }

            } while (_currCommand != "/exit");
        }

        private static (string, string) ParseCommand(string? input)
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
    }
}