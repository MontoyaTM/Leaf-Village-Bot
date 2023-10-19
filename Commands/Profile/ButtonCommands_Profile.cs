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
                .AddComponents(new TextInputComponent("Level:", "levelTextBox", "Level (1-60)", null, true, TextInputStyle.Short))
                .AddComponents(new TextInputComponent("Masteries:", "masteriesTextBox", "Mastery1, Mastery2", null, true, TextInputStyle.Short))
                .AddComponents(new TextInputComponent("Clan:", "clanTextBox", "Clan Name or Clanless", null, true, TextInputStyle.Short))
                .AddComponents(new TextInputComponent("Alt(s):", "altsTextBox", "IGN, Alt1, Alt2, ... \nPlease be sure to separate your alt(s) with a comma!", null, true, TextInputStyle.Paragraph));

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modalVillagerApplication);
        }

        [ButtonCommand("btn_AcceptApplicant")]
        public async Task AcceptVillager(ButtonContext ctx)
        {
            await ctx.Interaction.DeferAsync(true);

            var DBUtil_Profile = new DBUtil_Profile();

            var hasLMPFRole = ctx.Member.Roles.Any(x => x.Name == "Hokage");

            if (hasLMPFRole)
            {
                var embedMessage = ctx.Message.Embeds.First();

                var geninRole = ctx.Guild.Roles.FirstOrDefault(x => x.Value.Name == "Genin");

                var memberID = ulong.Parse(embedMessage.Footer.Text);

                DiscordMember member = await ctx.Guild.GetMemberAsync(memberID);

                await member.GrantRoleAsync(geninRole.Value);

                var guildChannels = await ctx.Interaction.Guild.GetChannelsAsync();
                var acceptedChannel = guildChannels.FirstOrDefault(x => x.Name == "application-accepted");

                if (acceptedChannel == null)
                {
                    await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                        .WithContent($"Channel: application-accepted does not exist, please create the channel to store records."));
                    return;
                }

                await ctx.Client.SendMessageAsync(await ctx.Client.GetChannelAsync(acceptedChannel.Id), embedMessage);

                await ctx.Message.DeleteAsync();

                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                                    .WithContent($"Successfully granted Genin role to {member.Username}!"));

            } else
            {
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                    .WithContent($"Unable to accept villager for {ctx.Interaction.User.Username}, please check required roles."));
            }            
        }

        [ButtonCommand("btn_DeclineApplicant")]
        public async Task DeclineVillager(ButtonContext ctx)
        {
            await ctx.Interaction.DeferAsync(true);

            var DBUtil_Profile = new DBUtil_Profile();

            var hasLMPFRole = ctx.Member.Roles.Any(x => x.Name == "Hokage");

            if (hasLMPFRole)
            {
                var embedMessage = ctx.Message.Embeds.First();

                var memberID = ulong.Parse(embedMessage.Footer.Text);

                DiscordMember member = await ctx.Guild.GetMemberAsync(memberID);

                var embedReason = new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.SpringGreen,
                    Title = "Please enter the reason for denying application as the next message."
                };

                var followupMessage = await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedReason));
                var reason = await ctx.Channel.GetNextMessageAsync();

                var embedDenied = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Red)
                    .WithTitle($"Application Denied")
                    .WithImageUrl(member.AvatarUrl)
                    .WithThumbnail(Global.LeafSymbol_URL)
                    .WithDescription($"{reason.Result.Content}")
                );

                await member.SendMessageAsync(embedDenied);

                await ctx.Message.DeleteAsync();
                await ctx.Interaction.EditFollowupMessageAsync(followupMessage.Id, new DiscordWebhookBuilder().WithContent($"Your response was sent to {member.Username}."));

                await ctx.Channel.DeleteMessageAsync(reason.Result);

                var guildChannels = await ctx.Interaction.Guild.GetChannelsAsync();
                var deniedChannel = guildChannels.FirstOrDefault(x => x.Name == "application-denied");

                if (deniedChannel == null)
                {
                    await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                        .WithContent($"Channel: application-denied does not exist, please create the channel to store records."));
                    return;
                }

                var embedFields = embedMessage.Fields;
                var embedFieldLists = new List<string>();

                foreach (var field in embedFields)
                {
                    embedFieldLists.Add(field.Value);
                }

                var embedApplicationDenied = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Red)
                    .WithTitle($"Leaf Village Application")
                    .WithImageUrl(member.AvatarUrl)
                    .WithThumbnail(Global.LeafSymbol_URL)
                    .AddField("IGN:", embedFieldLists[0], true)
                    .AddField("Level:", embedFieldLists[1], true)
                    .AddField("Masteries:", embedFieldLists[2], true)
                    .AddField("Clan:", embedFieldLists[3])
                    .AddField("Alt(s):", embedFieldLists[4])
                    .WithFooter($"{memberID}\nApplication Denied: \n{reason.Result.Content}\n")
                    );

                await ctx.Client.SendMessageAsync(await ctx.Client.GetChannelAsync(deniedChannel.Id), embedApplicationDenied);
            }
            else
            {
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Unable to deny applicant for {ctx.Interaction.User.Username}, please check required roles."));
            }
        }
    }
}
