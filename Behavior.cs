using System;

namespace DGHomeWork
{
    public class BotBehavior
    {
        public string HelloMsg => "Добрый день. Вот перечень доступных команд для неавторизованных пользователей:\n";
        public string UnsignedStartMsg => "Как я могу к вам обращаться?";
        public string SignedStartMsg => "Похоже я не правильно запомнил твое имя. Ок, попробуем еще раз. Как я могу к вам обращаться?";
        public string UnsignedErrorMsg => ", эта команда доступна только авторизованным пользователям";
        public string HelpMsg => ", я еще в процессе обучения, поэтому список доступных команд невелик. Вот их перечень:\n";
        public string InfoMsg => $", я бот v.01.{DateTime.Today:yyyy} создан {DateTime.Today:dd.MM.yyyy}.";
        public string UnknownCommandErrorMsg => ", прошу прощения. Эту команду я еще не выучил. Вот перечень доступных команд:\n";
        public string TasksAddMsg => ", введите новую задачу";
        public string TasksAddedMsg => ", я добавил в список запись";
        public string TasksListMsg => ", вот список актуальных задач:";
        public string TasksRemoveMsg => "Введите номер задачи, которую нужно удалить";
        public string TasksRemoveExistErrorMsg => ", я ожидаю номер существующей задачи. Попробуйте еще раз";
        //public string TasksRemoveHidenErrorMsg => ", эта задача уже была удалена ранее. Попробуйте еще раз";
        public string TasksRemovedMsg => ", я удалил из списка задачу";
        public string EmptyTasksListErrorMsg => ", в моем списке задач нет актуальных записей";

        public string ExitMsg => ", с вами было приятно поработать. Всего доброго.";
    }
}
