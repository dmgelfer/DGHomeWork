using System;
using System.Collections.Generic;
using System.Linq;

namespace DGHomeWork
{
    public class ToDoService (int taskCountLimit, int taskLengthLimit): IToDoService
    {
        public int TaskCountLimit { get; } = taskCountLimit;
        public int TaskLengthLimit { get; } = taskLengthLimit;

        private readonly List<ToDoItem> _tasks = [];
        private readonly object _lock = new();

        private IEnumerable<ToDoItem> UserTasks(Guid userId)
        {
            lock (_lock)
            {
                return [.. _tasks.Where(t => t.ItemUser.Id == userId)];
            }
        }

        public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
        {
            return [.. UserTasks(userId)];
        }

        public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
        {
            return [.. UserTasks(userId).Where(t => t.State == ToDoItemState.Active)];
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
        
        public void MarkCompleted(Guid id)
        {
            lock (_lock)
            {
                var task = _tasks.FirstOrDefault(t => t.ItemId == id)
                    ?? throw new TaskNotFoundException(id);

                task.Complete();
            }
        }

        public void Delete(Guid id)
        {
            lock (_lock)
            {
                var task = _tasks.FirstOrDefault(t => t.ItemId == id)
                    ?? throw new TaskNotFoundException(id);

                _tasks.Remove(task);
            }
        }
    }
}
