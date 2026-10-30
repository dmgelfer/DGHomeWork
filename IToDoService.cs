using System;
using System.Collections.Generic;

namespace DGHomeWork
{
    public interface IToDoService
    {
        IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId);
        //Возвращает ToDoItem для UserId со статусом Active
        IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId);
        ToDoItem Add(ToDoUser user, string name);

        //добавил доп проверку по user, чтобы нельзя было отредактировать чужие записи
        void MarkCompleted(Guid id);
        void Delete(Guid id);
    }
}
