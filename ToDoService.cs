using System;
using System.Collections.Generic;
using System.Linq;

namespace DGHomeWork
{
    public class ToDoService : IToDoService
    {
        private const int TaskCountLimit = 10;
        private const int TaskLengthLimit = 20;

        private readonly List<ToDoItem> _tasks = [];
        private readonly object _lock = new();

        private IEnumerable<ToDoItem> UserTasks(long telegramUserId)
        {
            lock (_lock)
            {
                return [.. _tasks.Where(t => t.ItemUser.TelegramUserId == telegramUserId)];
            }
        }

        public IReadOnlyList<ToDoItem> GetAllByUserId(long telegramUserId)
        {
            return [.. UserTasks(telegramUserId)];
        }

        public IReadOnlyList<ToDoItem> GetActiveByUserId(long telegramUserId)
        {
            return [.. UserTasks(telegramUserId).Where(t => t.State == ToDoItemState.Active)];
        }

        public ToDoItem Add(ToDoUser user, string name)
        {
            lock (_lock)
            {
                var userTaskCount = _tasks.Count(t => t.ItemUser.TelegramUserId == user.TelegramUserId);
                if (userTaskCount >= TaskCountLimit)
                {
                    throw new TaskCountLimitException(TaskCountLimit);
                }

                if (name.Length > TaskLengthLimit) throw new TaskLengthLimitException(name.Length, TaskLengthLimit);

                if (_tasks.Any(t => t.ItemUser.TelegramUserId == user.TelegramUserId && t.ItemName == name))
                {
                    throw new DuplicateTaskException(name);
                }

                var newTask = new ToDoItem(user, name);
                _tasks.Add(newTask);
                return newTask;
            }
        }
        
        public void MarkCompleted(ToDoUser user, Guid id)
        {
            lock (_lock)
            {
                var task = _tasks.FirstOrDefault(t => t.ItemId == id)
                    ?? throw new TaskNotFoundException(id);

                if (task.ItemUser.TelegramUserId == user.TelegramUserId)
                {
                    task.Complete();
                }
            }
        }

        public void Delete(ToDoUser user, Guid id)
        {
            lock (_lock)
            {
                var task = _tasks.FirstOrDefault(t => t.ItemId == id)
                    ?? throw new TaskNotFoundException(id);

                if (task.ItemUser.TelegramUserId == user.TelegramUserId)
                {
                    _tasks.Remove(task);
                }
            }
        }
    }
}
