using Otus.ToDoList.ConsoleBot;

namespace DGHomeWork
{
    public class Program
    {
        public static void Main()
        {
            var botClient = new ConsoleBotClient();
            var userService = new UserService();
            var toDoService = new ToDoService();
            var handler = new UpdateHandler(userService, toDoService);

            botClient.StartReceiving(handler);
        }
    }
}
