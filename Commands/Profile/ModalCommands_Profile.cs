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

        [ModalCommand("ProfileVillagerApplication")]
        public async Task SubmitVillagerApplication(ModalContext ctx)
        {
            await ctx.Interaction.DeferAsync(true);

            var modalValues = ctx.Values;
            var DBUtil_Profile = new DBUtil_Profile();

            ButtonCommandsExtension buttonCommand = ctx.Client.GetButtonCommands();

            try
            {
                var level = int.Parse(modalValues.ElementAt(1));

                if(level <= 0 || level > 60)
                {
                    await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Failed to created application for {ctx.Interaction.User.Username}. Level field must within 1-60!"));
                    return;
                }

            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Failed to created application for {ctx.Interaction.User.Username}. Level field must within 1-60!"));
                return;

            }

            var userInfo = new DBProfile
            {
                UserName =  ctx.Interaction.User.Username,
                MemberID = ctx.Interaction.User.Id,
                ServerID = ctx.Interaction.Guild.Id,
                AvatarURL = ctx.Interaction.User.AvatarUrl,
                ProfileImage = ctx.Interaction.User.AvatarUrl,
                InGameName = modalValues.ElementAt(0),
                Level = int.Parse(modalValues.ElementAt(1)),
                Masteries = modalValues.ElementAt(2),
                Clan = modalValues.ElementAt(3),
                Alts = modalValues.ElementAt(4),
            };

            var isApplicationStored = await DBUtil_Profile.StoreVillagerApplicationAsync(userInfo);

            if (isApplicationStored == true)
            {
                var guildChannels = await ctx.Interaction.Guild.GetChannelsAsync();
                var villagerApplicationsChannel = guildChannels.FirstOrDefault(x => x.Name == "villager-applications");

                if (villagerApplicationsChannel == null)
                {
                    var embedFailed = new DiscordEmbedBuilder()
                    {
                        Title = "Villager Application Failed",
                        Color = DiscordColor.Red,
                        Description = "Channel: villager-applications does not exist!"
                    };

                    await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedFailed));
                    return;
                }

                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Succesfully created a villager application for {ctx.Interaction.User.Username}"));

                var embedApplication = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithColor(DiscordColor.SpringGreen)
                        .WithTitle($"Leaf Village Application")
                        .WithImageUrl(ctx.Interaction.User.AvatarUrl)
                        .WithThumbnail(Global.LeafSymbol_URL)
                        .WithFooter(ctx.Interaction.User.Id.ToString())
                        .AddField("IGN:", userInfo.InGameName, true)
                        .AddField("Level:", userInfo.Level.ToString(), true)
                        .AddField("Masteries:", userInfo.Masteries, true)
                        .AddField("Clan:", userInfo.Clan)
                        .AddField("Alt(s):", userInfo.Alts))
                    .AddComponents(
                    new DiscordButtonComponent(ButtonStyle.Primary, buttonCommand.BuildButtonId("btn_AcceptApplicant"), "Accept Applicant"),
                    new DiscordButtonComponent(ButtonStyle.Secondary, buttonCommand.BuildButtonId("btn_DeclineApplicant"), "Deny Applicant")
                    );

                await ctx.Client.SendMessageAsync(await ctx.Client.GetChannelAsync(villagerApplicationsChannel.Id), embedApplication);
            } else
            {
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Failed to created application for {ctx.Interaction.User.Username}. Please be aware of one application per user!"));
            }
        }
    }
}
