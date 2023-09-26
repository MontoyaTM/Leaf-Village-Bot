using DSharpPlus;
using DSharpPlus.ButtonCommands;
using DSharpPlus.Entities;
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
                .AddComponents(new TextInputComponent("Level:", "levelTextBox", "Level", null, true, TextInputStyle.Short))
                .AddComponents(new TextInputComponent("Masteries:", "masteriesTextBox", "Mastery1, Mastery2", null, true, TextInputStyle.Short))
                .AddComponents(new TextInputComponent("Clan:", "clanTextBox", "Clan", null, true, TextInputStyle.Short))
                .AddComponents(new TextInputComponent("Alt(s):", "altsTextBox", "Character1, Character2, ... \nPlease be sure to separate your alt(s) with a comma!", null, true, TextInputStyle.Paragraph));

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modalVillagerApplication);
        }

        [ButtonCommand("btnAcceptApplicant")]
        public async Task AcceptVillager(ButtonContext ctx)
        {
            await ctx.Interaction.DeferAsync(true);

            var user = (DiscordMember)ctx.Interaction.User;
            var userRoles = user.Roles.ToArray();

            foreach(var role in userRoles )
            {
                if(role.Name == "Hokage")
                {
                    var DBUtil_Profile = new DBUtil_Profile();

                    var geninRole = ctx.Guild.Roles.FirstOrDefault(x => x.Value.Name == "Genin");
                    var embedFields = ctx.Message.Embeds.Select(x => x.Fields);
                    var ingameNameField = embedFields.Select(x => x.First());

                    string ingameName = "";
                    foreach(var name in ingameNameField)
                    {
                        ingameName = name.Value;
                    }

                    var username = await DBUtil_Profile.GetUserNameFromIngameNameAsync(ingameName);
                    var discordMember = ctx.Guild.Members.Where(x => x.Value.Username == username.Item2);

                    ulong memberID = 0;

                    foreach(var item in discordMember)
                    {
                        memberID = item.Value.Id;
                    }

                    DiscordMember member = await ctx.Guild.GetMemberAsync(memberID);
                    await member.GrantRoleAsync(geninRole.Value);

                    var guildChannels = await ctx.Interaction.Guild.GetChannelsAsync();
                    var recordsChannel = guildChannels.FirstOrDefault(x => x.Name == "application-records");

                    var applicant = await DBUtil_Profile.GetApplicationFromIngameNameAsync(ingameName);
                    DBProfile profile = applicant.Item2;

                    await ctx.Message.DeleteAsync();

                    var embedApplication = new DiscordMessageBuilder()
                            .AddEmbed(new DiscordEmbedBuilder()
                                .WithColor(DiscordColor.SpringGreen)
                                .WithTitle($"Leaf Village Application for {username.Item2}:")
                                .WithImageUrl(member.AvatarUrl)
                                .WithThumbnail(LeafSymbolURL)
                                .AddField("IGN:", profile.InGameName, true)
                                .AddField("Level:", profile.Level.ToString(), true)
                                .AddField("Masteries:", profile.Masteries)
                                .AddField("Clan:", profile.Clan)
                                .AddField("Alt(s):", profile.Alts));

                    await ctx.Client.SendMessageAsync(await ctx.Client.GetChannelAsync(recordsChannel.Id), embedApplication);

                    await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                                        .WithContent($"Successfully granted Genin role to {username.Item2}!"));
                    return;
                } 
            }
            await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Unable to accept villager for {ctx.Interaction.User.Username}, please check required roles."));
        }

        [ButtonCommand("btnDeclineApplicant")]
        public async Task DeclineVillager(ButtonContext ctx)
        {

        }
    }
}
