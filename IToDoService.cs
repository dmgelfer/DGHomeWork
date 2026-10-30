using System;
using System.Collections.Generic;

namespace DGHomeWork
{
    public interface IToDoService
    {
        IReadOnlyList<ToDoItem> GetAllByUserId(long telegramUserId);
        //Возвращает ToDoItem для UserId со статусом Active
        IReadOnlyList<ToDoItem> GetActiveByUserId(long telegramUserId);
        ToDoItem Add(ToDoUser user, string name);

        //добавил доп проверку по user, чтобы нельзя было отредактировать чужие записи
        void MarkCompleted(ToDoUser user, Guid id);
        void Delete(ToDoUser user, Guid id);
    }
}
