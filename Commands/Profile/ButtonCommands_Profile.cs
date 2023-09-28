using DSharpPlus;
using DSharpPlus.ButtonCommands;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.ModalCommands;
using Leaf_Village_Bot.DBUtil.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leaf_Village_Bot.Commands.Profile
{
    public class ButtonCommands_Profile : ButtonCommandModule
    {
        private string LeafSymbolURL = "https://i.imgur.com/spjhOGb.png";

        [ButtonCommand("btnCreateVillagerApplication")]
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

        [ButtonCommand("btnAcceptApplicant")]
        [RequireRoles(RoleCheckMode.Any, "Hokage")]
        public async Task AcceptVillager(ButtonContext ctx)
        {
            await ctx.Interaction.DeferAsync(true);

            var DBUtil_Profile = new DBUtil_Profile();

            var hasLMPFRole = ctx.Member.Roles.Any(x => x.Name == "Hokage");

            if (hasLMPFRole)
            {
                var embedMessage = ctx.Message.Embeds.First();

                var geninRole = ctx.Guild.Roles.FirstOrDefault(x => x.Value.Name == "Genin");

                var ingameNameField = ctx.Message.Embeds.Select(x => x.Fields.First().Value);
                var ingameName = String.Join("", ingameNameField);

                var username = await DBUtil_Profile.GetUserNameFromIngameNameAsync(ingameName);
                var discordMemberID = ctx.Guild.Members.Where(x => x.Value.Username == username.Item2).First().Key;
                
                DiscordMember member = await ctx.Guild.GetMemberAsync(discordMemberID);

                await member.GrantRoleAsync(geninRole.Value);

                var guildChannels = await ctx.Interaction.Guild.GetChannelsAsync();
                var recordsChannel = guildChannels.FirstOrDefault(x => x.Name == "application-records");

                await ctx.Client.SendMessageAsync(await ctx.Client.GetChannelAsync(recordsChannel.Id), embedMessage);

                await ctx.Message.DeleteAsync();

                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                                    .WithContent($"Successfully granted Genin role to {username.Item2}!"));

            } else
            {
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Unable to accept villager for {ctx.Interaction.User.Username}, please check required roles."));
            }            
        }

        [ButtonCommand("btnDeclineApplicant")]
        public async Task DeclineVillager(ButtonContext ctx)
        {
            await ctx.Interaction.DeferAsync(true);

            var DBUtil_Profile = new DBUtil_Profile();

            var hasLMPFRole = ctx.Member.Roles.Any(x => x.Name == "Hokage");

            if (hasLMPFRole)
            {
                var embedMessage = ctx.Message.Embeds.First();

                var ingameNameField = ctx.Message.Embeds.Select(x => x.Fields.First().Value);
                var ingameName = String.Join("", ingameNameField);

                var username = await DBUtil_Profile.GetUserNameFromIngameNameAsync(ingameName);
                var discordMemberID = ctx.Guild.Members.Where(x => x.Value.Username == username.Item2).First().Key;

                DiscordMember member = await ctx.Guild.GetMemberAsync(discordMemberID);

                var isDeleted = await DBUtil_Profile.DeleteVillagerApplication(username.Item2);

                if (isDeleted)
                {
                    var embedReason = new DiscordEmbedBuilder()
                    {
                        Color = DiscordColor.SpringGreen,
                        Title = "Please enter the reason for denying application as the next message."
                    };

                    var followupMessage = await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedReason));
                    var reason = await ctx.Channel.GetNextMessageAsync();

                    await ctx.Interaction.EditFollowupMessageAsync(followupMessage.Id, new DiscordWebhookBuilder().WithContent($"Your response was sent to {username.Item2}."));

                    await ctx.Message.DeleteAsync();

                    var embedDenied = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithColor(DiscordColor.SpringGreen)
                        .WithTitle($"Application Denied")
                        .WithImageUrl(member.AvatarUrl)
                        .WithThumbnail(LeafSymbolURL)
                        .WithDescription($"{reason.Result.Content}")
                    );

                    await member.SendMessageAsync(embedDenied);

                    await ctx.Channel.DeleteMessageAsync(reason.Result);

                    var guildChannels = await ctx.Interaction.Guild.GetChannelsAsync();
                    var recordsChannel = guildChannels.FirstOrDefault(x => x.Name == "application-records");


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
                        .WithThumbnail(LeafSymbolURL)
                        .AddField("IGN:", embedFieldLists[0], true)
                        .AddField("Level:", embedFieldLists[1], true)
                        .AddField("Masteries:", embedFieldLists[2], true)
                        .AddField("Clan:", embedFieldLists[3])
                        .AddField("Alt(s):", embedFieldLists[4])
                        .WithFooter($"Application Denied: \n{reason.Result.Content}\n")
                        );

                    await ctx.Client.SendMessageAsync(await ctx.Client.GetChannelAsync(recordsChannel.Id), embedApplicationDenied);

                }
                else
                {
                    await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Unable to deny applicant for {ctx.Interaction.User.Username}, isDeleted  was false!"));
                }

            } else
            {
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Unable to deny applicant for {ctx.Interaction.User.Username}, please check required roles."));
            }
        }



        


    }
}
