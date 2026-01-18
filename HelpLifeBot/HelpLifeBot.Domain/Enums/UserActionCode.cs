namespace HelpLifeBot.Domain.Enums
{
    public enum UserActionCode
    {
        /// <summary>
        /// Пользователь ввел произвольный текст (дейстиве не определено)
        /// </summary>
        Default = 0,

        /// <summary>
        /// Пользователь ввел команду /help
        /// </summary>
        Help = 10,
    }
}
