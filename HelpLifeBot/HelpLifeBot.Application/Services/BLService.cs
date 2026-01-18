using HelpLifeBot.Bindings.Suno;
using HelpLifeBot.Bindings.Telegram;
using HelpLifeBot.Domain;
using HelpLifeBot.Domain.Enums;
using MentKit.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HelpLifeBot
{
    public interface IBLService
    {
        Task ProcessTheMessageAsync(WebhookMessage message, CancellationToken cancellationToken);
    }

    public class BLService : IBLService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserActionRepository _userActionRepository;
        private readonly AppDbContext _appContext;
        private readonly IReadModelQueryExecutor _queryExecutor;
        private readonly ITgService _tgService;
        private readonly ILogger<BLService> _logger;

        public BLService(IUserRepository userRepository, IUserActionRepository userActionRepository, AppDbContext appContext,
            IReadModelQueryExecutor queryExecutor, ITgService tgService, ILogger<BLService> logger)
        {
            _userRepository = userRepository;
            _userActionRepository = userActionRepository;
            _appContext = appContext;
            _queryExecutor = queryExecutor;
            _tgService = tgService;
            _logger = logger;
        }

        public async Task ProcessTheMessageAsync(WebhookMessage message, CancellationToken cancellationToken)
        {
            if (message.Chat.Id != message.From.Id)
            {
                _logger.LogWarning($"Несоотвествие ID чата ({message.Chat.Id}) и ID пользователя ({message.From.Id}");
                return;
            }

            await SaveUserAsync(message.From, cancellationToken);

            switch (message.Text)
            {
                case "/help":
                    await ProcessHelpAsync(message.From.Id);
                    return;
                default:
                    await ProcessDefaultAsync(message.From.Id, message.Text);
                    return;
            }
        }

        #region processing the messages


        private async Task ProcessHelpAsync(long userId)
        {
            await AddUserActionAsync(userId, UserActionCode.Help);
            await _tgService.SendMessageAsync(userId,
                "Я помогу тебе во всём!!" +
                "\n" +
                "\nМожешь задавать разные вопросы о жизни прямо в БОТа, например:" +
                "\n" +
                "\nКак перейти через дорогу?" +
                "\nКак вызвать лифт?" +
                "\nКак хорошо учиться?");
        }

        private async Task ProcessDefaultAsync(long userId, string? text)
        {
            var lastUserAction = await GetLastUserActionAsync(userId);
            if (lastUserAction == null)
            {
                await AddUserActionAsync(userId, UserActionCode.Default, text);
                await SendMessageIfJustTextAsync(userId, text);
                return;
            }

            var actionCode = Enum.Parse<UserActionCode>(lastUserAction.ActionCode);
            switch (actionCode)
            {
                default:
                    await AddUserActionAsync(userId, UserActionCode.Default, text);
                    await SendMessageIfJustTextAsync(userId, text);
                    break;
            }
        }

        private async Task SendMessageIfJustTextAsync(long userId, string? text)
        {
            if (text == "Как перейти через дорогу?")
            {
                await _tgService.SendMessageAsync(userId,
                    "Подходишь, смотришь налево, направо и переходишь!");
                return;
            }

            if (text == "Как вызвать лифт?")
            {
                await _tgService.SendMessageAsync(userId,
                    "Просто нажми на кнопку вызова лифта!");
                return;
            }

            if (text == "Как хорошо учиться?")
            {
                await _tgService.SendMessageAsync(userId,
                    "Не ленись, делай уроки!");
                return;
            }

            var answers = new string[]
            {
                "Ну тут без доната не разобрать!",
                "Ничего не понятно но очень интересно!",
                "Странный вопрос!",
                "Интересный вопрос!"
            };

            await _tgService.SendMessageAsync(userId,
                answers[new Random().Next(0, answers.Length)]);
        }

        #endregion


        #region user's methods

        private async Task SaveUserAsync(WebhookFromChat messageFrom, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FindAsync(messageFrom.Id, cancellationToken);

            if (user == null)
            {
                user = new User
                {
                    UserId = messageFrom.Id,
                    UserName = messageFrom.Username,
                    FirstName = messageFrom.FirstName,
                    LastName = messageFrom.LastName,
                    IsBot = messageFrom.IsBot,
                    IsPremium = messageFrom.IsPremium,
                };

                await _userRepository.SaveAsync(user);
            }

            if (user.UserId != messageFrom.Id)
            {
                _logger.LogError($"Несоотвествие ID пользователя в БД ({user.UserId}) и ID пользователя в сообщении ({messageFrom.Id}");
                return;
            }

            if (user.UserId == messageFrom.Id && user.UserName == messageFrom.Username
                && user.FirstName == messageFrom.FirstName && user.LastName == messageFrom.LastName
                && user.IsBot == messageFrom.IsBot && user.IsPremium == messageFrom.IsPremium)
                return;

            user.UserName = messageFrom.Username;
            user.FirstName = messageFrom.FirstName;
            user.LastName = messageFrom.LastName;
            user.IsBot = messageFrom.IsBot;
            user.IsPremium = messageFrom.IsPremium;
            user.UpdatedOn = DateTime.UtcNow;

            await _userRepository.SaveAsync(user);
        }

        #endregion


        #region user action's methods

        private async Task<UserAction> AddUserActionAsync(long userId, UserActionCode actionCode, string? actionDetails = null)
        {
            var userAction = new UserAction
            {
                UserId = userId,
                ActionCode = $"{actionCode}",
                ActionDetails = actionDetails
            };

            await _userActionRepository.SaveAsync(userAction);

            return userAction;
        }

        private async Task<UserAction?> GetLastUserActionAsync(long userId)
        {
            var lastUserAction = await _queryExecutor.FirstOrDefaultAsync(
                _appContext.UserActions
                    .Where(x => x.UserId == userId)
                    .OrderByDescending(x => x.UserActionId));

            return lastUserAction;
        }

        #endregion
    }
}
