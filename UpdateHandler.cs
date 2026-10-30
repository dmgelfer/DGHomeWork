using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using System;
using System.Text;

namespace DGHomeWork
{
    public class UpdateHandler(IUserService userService, IToDoService toDoService) : IUpdateHandler
    {
        private readonly IUserService _userService = userService;
        private readonly IToDoService _toDoService = toDoService;

        private readonly BotCommand[] _commands =
        [
                new("/start", "Запуск бота", true),
                new("/help", "Список команд", true),
                new("/info", "Информация о боте", true),
                new("/addtask", "Добавить задачу в список", false),
                new("/showtasks", "Показать только активные задачи из списка", false),
                new("/showalltasks", "Показать все задачи из списка", false),
                new("/completetask", "Отметить задачу из списка выполненной", false),
                new("/removetask", "Удалить задачу из списка", false),
                new("/exit", "Завершение работы", true),
        ];

        public void HandleUpdateAsync(ITelegramBotClient botClient, Update update)
        {
            (var currCommand, var currParameter) = SeparateInput(update.Message.Text);

            switch (currCommand)
            {
                case "/start":
                    StartCommand(botClient, update);
                    break;
                case "/help":
                    HelpCommand(botClient, update);
                    break;
                case "/info":
                    InfoCommand(botClient, update);
                    break;
                case "/addtask":
                    AddTaskCommand(botClient, update, currParameter);
                    break;
                case "/showtasks":
                    ShowTasksCommand(botClient, update);
                    break;
                case "/showalltasks":
                    ShowAllTasksCommand(botClient, update);
                    break;
                case "/completetask":
                    CompleteTaskCommand(botClient, update, currParameter);
                    break;
                case "/removetask":
                    RemoveTaskCommand(botClient, update, currParameter);
                    break;
                case "/exit":
                    ExitCommand();
                    break;
                default:
                    UnknownCommand(botClient, update);
                    break;
            }
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

        private string GenerateCommandsTxt(bool isSigned)
        {
            StringBuilder stringBuilder = new();

            for (int i = 0; i < _commands.Length - 1; i++)
            {
                if (isSigned || _commands[i].IsAvailableToGuests)
                {
                    stringBuilder.Append($"{_commands[i].Name} - {_commands[i].Description}\n");
                }
            }
            stringBuilder.Append($"{_commands[^1].Name} - {_commands[^1].Description}");

            return stringBuilder.ToString();
        }

        private void StartCommand(ITelegramBotClient botClient, Update update)
        {
            var telegramUserId = update.Message.From.Id;
            var user = _userService.GetUser(telegramUserId);
            var userName = update.Message.From.Username ?? "Незнакомец";

            if (user == null)
            {
                _userService.RegisterUser(telegramUserId, userName);
                botClient.SendMessage(update.Message.Chat, $"Зарегистрировал пользователя'{userName}'.");
            }
            else
            {
                botClient.SendMessage(update.Message.Chat, $"{userName} уже зарегистрирован.");
            }
            
            HelpCommand(botClient, update);
        }

        private void HelpCommand(ITelegramBotClient botClient, Update update)
        {
            var telegramUserId = update.Message.From.Id;
            var user = _userService.GetUser(telegramUserId);
            var userName = update.Message.From.Username ?? "Незнакомец";

            botClient.SendMessage(update.Message.Chat, $"{userName}, вот перечень доступных команд:\n{GenerateCommandsTxt(user != null)}");
        }

        private void InfoCommand(ITelegramBotClient botClient, Update update)
        {
            var telegramUserId = update.Message.From.Id;
            var user = _userService.GetUser(telegramUserId);
            var userName = update.Message.From.Username ?? "Незнакомец";

            botClient.SendMessage(update.Message.Chat, $"{userName}, я бот v.01.{DateTime.Today:yyyy} создан {DateTime.Today:dd.MM.yyyy}.");
        }

        private void UnknownCommand(ITelegramBotClient botClient, Update update)
        {
            var telegramUserId = update.Message.From.Id;
            var user = _userService.GetUser(telegramUserId);
            var userName = update.Message.From.Username ?? "Незнакомец";

            botClient.SendMessage(update.Message.Chat, $"{userName}, прошу прощения. Эту команду я еще не выучил.\nВот перечень доступных команд:\n{GenerateCommandsTxt(user != null)}");
        }

        private void AddTaskCommand(ITelegramBotClient botClient, Update update, string currTask)
        {
            var telegramUserId = update.Message.From.Id;
            var user = _userService.GetUser(telegramUserId);
            var userName = update.Message.From.Username ?? "Незнакомец";

            if (user != null)
            {
                if (!string.IsNullOrWhiteSpace(currTask))
                {
                    _toDoService.Add(user, currTask);
                    botClient.SendMessage(update.Message.Chat, $"{userName}, я добавил задачу '{currTask}'");
                }
                else
                {
                    botClient.SendMessage(update.Message.Chat, $"{userName}, я не могу добавить задачу без описания");
                }
            }
            else
            {
                UnknownCommand(botClient, update);
            }
        }

        private void ShowTasksCommand(ITelegramBotClient botClient, Update update)
        {
            var telegramUserId = update.Message.From.Id;
            var user = _userService.GetUser(telegramUserId);
            var userName = user?.TelegramUserName ?? "Незнакомец";

            if (user != null)
            {
                StringBuilder stringBuilder = new();
                var tasks = _toDoService.GetActiveByUserId(user.Id);

                if (tasks.Count > 0)
                {
                    for (int i = 0; i < tasks.Count; i++)
                    {
                        stringBuilder.Append($"\n{i + 1}. {tasks[i].ItemName} - {tasks[i].CreatedAt} - {tasks[i].ItemId}");
                    }
                    
                    botClient.SendMessage(update.Message.Chat, $"{userName}, вот перечень ваших активных задач{stringBuilder}");
                }
                else
                {
                    botClient.SendMessage(update.Message.Chat, $"{userName}, у вас в списке нет актуальных задач.");
                }
            }
            else
            {
                UnknownCommand(botClient, update);
            }
        }

        private void ShowAllTasksCommand(ITelegramBotClient botClient, Update update)
        {
            var telegramUserId = update.Message.From.Id;
            var user = _userService.GetUser(telegramUserId);
            var userName = user?.TelegramUserName ?? "Незнакомец";

            if (user != null)
            {
                StringBuilder stringBuilder = new();
                var tasks = _toDoService.GetAllByUserId(user.Id);

                if (tasks.Count > 0)
                {
                    for (int i = 0; i < tasks.Count; i++)
                    {
                        stringBuilder.Append($"\n{i + 1}. ({tasks[i].State}) {tasks[i].ItemName} - {tasks[i].CreatedAt} - {tasks[i].ItemId}");
                    }

                    botClient.SendMessage(update.Message.Chat, $"{userName}, вот перечень всех ваших задач{stringBuilder}");
                }
                else
                {
                    botClient.SendMessage(update.Message.Chat, $"{userName}, у вас в списке нет никаких задач.");
                }
            }
            else
            {
                UnknownCommand(botClient, update);
            }
        }
        
        private void CompleteTaskCommand(ITelegramBotClient botClient, Update update, string taskID)
        {
            var userId = update.Message.From.Id;
            var user = _userService.GetUser(userId);
            var userName = update.Message.From.Username ?? "Незнакомец";

            if (user != null)
            {
                if (Guid.TryParse(taskID, out Guid itemIdToComplete))
                {
                    _toDoService.MarkCompleted(itemIdToComplete);
                    botClient.SendMessage(update.Message.Chat, $"{userName}, я пометил задачу '{taskID}' как выполненную");
                }
                else
                {
                    botClient.SendMessage(update.Message.Chat, $"{userName}, задача с таким ID не существует или ID указан некорректно");
                }
            }
            else
            {
                UnknownCommand(botClient, update);
            }
        }

        private void RemoveTaskCommand(ITelegramBotClient botClient, Update update, string taskID)
        {
            var userId = update.Message.From.Id;
            var user = _userService.GetUser(userId);
            var userName = update.Message.From.Username ?? "Незнакомец";

            if (user != null)
            {
                if (Guid.TryParse(taskID, out Guid itemIdToComplete))
                {
                    _toDoService.Delete(itemIdToComplete);
                    botClient.SendMessage(update.Message.Chat, $"{userName}, я удалил задачу '{taskID}'");
                }
                else
                {
                    botClient.SendMessage(update.Message.Chat, $"{userName}, задача с таким ID не существует или ID указан некорректно");
                }
            }
            else
            {
                UnknownCommand(botClient, update);
            }
        }

        private void ExitCommand()
        {
            Environment.Exit(0);
        }
    }
}