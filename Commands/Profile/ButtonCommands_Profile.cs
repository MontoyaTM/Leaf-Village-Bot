using DSharpPlus;
using DSharpPlus.ButtonCommands;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.ModalCommands;
using Leaf_Village_Bot.DBUtil.Profile;

namespace Leaf_Village_Bot.Commands.Profile
{
    public class ButtonCommands_Profile : ButtonCommandModule
    {

        [ButtonCommand("btn_CreateApplication")]
        public async Task CreateVillagerApplication(ButtonContext ctx)
        {
            var modalVillagerApplication = ModalBuilder.Create("ProfileVillagerApplication")
                .WithTitle("Leaf Village Villager Application")
                .AddComponents(new TextInputComponent("IGN:", "ingamenameTextBox", "Name of Character", null, true, TextInputStyle.Short))
                .AddComponents(new TextInputComponent("Introduction:", "introductionTextBox", "Introduce youself & reason for joining", null, true, TextInputStyle.Paragraph))
                .AddComponents(new TextInputComponent("Alt(s):", "altsTextBox", "IGN, Alt1, Alt2, ... \nPlease be sure to separate your alt(s) with a comma!", null, true, TextInputStyle.Paragraph));

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modalVillagerApplication);
        }

        [ButtonCommand("btn_UpdateLevel")]
        public async Task UpdateLevel(ButtonContext ctx)
        {
            await ctx.Interaction.DeferAsync(true);

            var DBUtil_Profile = new DBUtil_Profile();

            var userExists = await DBUtil_Profile.UserExistsAsync(ctx.Interaction.User.Id);

            if(userExists)
            {
                var modalUpdateLevel = ModalBuilder.Create("ProfileLevel")
                .WithTitle("Update Profile — Character Level")
                .AddComponents(new TextInputComponent("Level:", "ProfileLevel", "Level (1-60)", null, true, TextInputStyle.Short));

                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modalUpdateLevel);
            } else
            {
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent("You do not have a profile in the database!"));
            }
        }

        [ButtonCommand("btn_UpdateIGN")]
        public async Task UpdateIGN(ButtonContext ctx)
        {
            await ctx.Interaction.DeferAsync(true);

            var DBUtil_Profile = new DBUtil_Profile();

            var userExists = await DBUtil_Profile.UserExistsAsync(ctx.Interaction.User.Id);

            if(userExists)
            {
                var modalUpdateLevel = ModalBuilder.Create("ProfileIGN")
                .WithTitle("Update Profile — Character IGN")
                .AddComponents(new TextInputComponent("IGN:", "ProfileIGN", "Ingame Name", null, true, TextInputStyle.Short));

                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modalUpdateLevel);
            } else
            {
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent("You do not have a profile in the database!"));
            }
        }

    }
}
