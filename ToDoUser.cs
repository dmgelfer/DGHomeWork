using System;

namespace DGHomeWork
{
    public class ToDoUser(long telegramUserId, string telegramUserName)
    {
        public long TelegramUserId { get; } = telegramUserId;
        public string TelegramUserName { get; } = telegramUserName;
        public DateTime RegisteredAt { get; } = DateTime.UtcNow;
    }
}
