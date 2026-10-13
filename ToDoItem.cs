using System;

namespace DGHomeWork
{
    public enum ToDoItemState
    {
        Active,
        Completed
    }

    public class ToDoItem(ToDoUser user, string name)
    {
        public Guid ItemId { get; } = Guid.NewGuid();
        public ToDoUser ItemUser { get; } = user;
        public string ItemName { get; } = name;
        public DateTime CreatedAt { get; } = DateTime.UtcNow;
        public ToDoItemState State { get; private set; } = ToDoItemState.Active;
        public DateTime? StateChangedAt { get; private set; }

        public void Complete()
        {
            State = ToDoItemState.Completed;
            StateChangedAt = DateTime.UtcNow;
        }
        public void Activate()
        {
            State = ToDoItemState.Active;
            StateChangedAt = DateTime.UtcNow;
        }
    }
}
