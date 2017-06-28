using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace BirthdayBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Hi, I am bday bot");
            context.Wait(MessageReceivedAsync);
        }

        private int GetDaysToBirthday(string birthday)
        {
            string[] bdayParts = birthday.Split('-');

            int bMonth = Convert.ToInt32(bdayParts[1]);
            int bDay = Convert.ToInt32(bdayParts[0]);

            DateTime today = DateTime.Today;
            DateTime next = new DateTime(today.Year, bMonth, bDay);

            if (next < today) next = next.AddYears(1);

            return (next - today).Days;
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var msg = await argument;

            string birthday = String.Empty;
            bool expectingBirthDayMessage = false;

            context.UserData.TryGetValue("Birthday", out birthday);
            context.UserData.TryGetValue("expectingBirthDayMessage", out expectingBirthDayMessage);

            if (expectingBirthDayMessage)
            {
                birthday = msg.Text;
                if (!string.IsNullOrEmpty(birthday))
                {
                    await context.PostAsync(string.Format("Days until your birthday: {0}", GetDaysToBirthday(birthday)));
                    context.UserData.SetValue("Birthday", birthday);
                    context.UserData.SetValue("expectingBirthDayMessage", false);
                }
            }
            else
            {
                await context.PostAsync("When is your birth date: DD-MM-YYYY ?");
                context.UserData.SetValue("expectingBirthDayMessage", true);
            }
            context.Wait(MessageReceivedAsync);
        }
    }
}