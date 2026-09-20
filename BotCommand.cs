namespace DGHomeWork
{
    public class BotCommand(string name, string description, bool isAvailableToGuests)
    {
        public string Name { get; init; } = name;
        public string Description { get; init; } = description;
        public bool IsAvailableToGuests { get; init; } = isAvailableToGuests;
    }
}
