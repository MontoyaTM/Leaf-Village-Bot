using DSharpPlus.ButtonCommands;
using DSharpPlus.ButtonCommands.Extensions;
using DSharpPlus.ModalCommands;
using DSharpPlus.ModalCommands.Attributes;
using Leaf_Village_Bot.DBUtil.Profile;
using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leaf_Village_Bot.Commands.Profile
{
    public class ModalCommands_Profile : ModalCommandModule
    {
        private string LeafSymbolURL = "https://i.imgur.com/spjhOGb.png";
        private string LMPFSymbolURL = "https://i.imgur.com/QbBzNiR.png";

        [ModalCommand("ProfileVillagerApplication")]
        public async Task SubmitVillagerApplication(ModalContext ctx)
        {
            var modalValues = ctx.Values;
            var DBUtil_Profile = new DBUtil_Profile();

            ButtonCommandsExtension buttonCommand = ctx.Client.GetButtonCommands();

            var userInfo = new DBProfile
            {
                UserName =  ctx.Interaction.User.Username,
                InGameName = modalValues.ElementAt(0),
                Level = int.Parse(modalValues.ElementAt(1)),
                Masteries = modalValues.ElementAt(2),
                Clan = modalValues.ElementAt(3),
                Alts = modalValues.ElementAt(4),
            };

            var isApplicationStored = await DBUtil_Profile.StoreVillagerApplicationAsync(userInfo);

            if(isApplicationStored == true)
            {
                var guildChannels = await ctx.Interaction.Guild.GetChannelsAsync();
                var villagerApplicationsChannel = guildChannels.FirstOrDefault(x => x.Name == "villager-applications");

                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                                                                        .WithContent($"Succesfully created ticket for {ctx.Interaction.User.Username}"));

                var embedApplication = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithColor(DiscordColor.SpringGreen)
                        .WithTitle($"Leaf Village Application")
                        .WithImageUrl(ctx.Interaction.User.AvatarUrl)
                        .WithThumbnail(LeafSymbolURL)
                        .AddField("IGN:", userInfo.InGameName, true)
                        .AddField("Level:", userInfo.Level.ToString(), true)
                        .AddField("Masteries:", userInfo.Masteries)
                        .AddField("Clan:", userInfo.Clan)
                        .AddField("Alt(s):", userInfo.Alts))
                    .AddComponents(
                    new DiscordButtonComponent(ButtonStyle.Primary, buttonCommand.BuildButtonId("btnAcceptApplicant"), "Accept Applicant"),
                    new DiscordButtonComponent(ButtonStyle.Secondary, buttonCommand.BuildButtonId("btnDeclineApplicant"), "Deny Applicant")
                    );

                await ctx.Client.SendMessageAsync(await ctx.Client.GetChannelAsync(villagerApplicationsChannel.Id), embedApplication);
            } else
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                                                           .WithContent($"Failed to created application for {ctx.Interaction.User.Username}. Please be aware of one application per user!"));
            }
        }
    }
}
