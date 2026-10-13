using System;

namespace DGHomeWork
{
    public class ToDoUser(string name)
    {
        public Guid UserId { get; } = Guid.NewGuid();
        public string TelegramUserName { get; private set; } = name;
        public DateTime RegisteredAt { get; private set; }

        public void SetNewName(string name)
        {
            TelegramUserName = name;
            RegisteredAt = DateTime.UtcNow;
        }
    }
}
