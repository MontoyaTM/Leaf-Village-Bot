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
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection.Emit;

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

            var isLevel = await LevelCheck(ctx, modalValues);
            
            if(!isLevel)
            {
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                    .WithContent($"Failed to created application for {ctx.Interaction.User.Username}. Level field must within 1-60!"));
                return;
            }

            var isMasteries = await MasteriesCheck(ctx, modalValues);
            if(!isMasteries)
            {
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                    .WithContent($"Failed to created application for {ctx.Interaction.User.Username}. Masteries do not match the ones listed below!"));
                return;
            }

            var isClan = await ClanCheck(ctx, modalValues);
            if(!isClan)
            {
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                    .WithContent($"Failed to created application for {ctx.Interaction.User.Username}. Clan does not match the ones listed below!"));
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

            var applicationExists = await DBUtil_Profile.UserExistsAsync(userInfo.MemberID);

            if(applicationExists)
            {
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Unable to submit application, please be aware of one application per user."));
                return;
            }

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
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Failed to created application for {ctx.Interaction.User.Username}."));
            }
        }

        public async Task<bool> MasteriesCheck(ModalContext ctx, string[] modalValues)
        {
            var masteries = modalValues.ElementAt(2).Split(",").Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

            var masteriesCount = masteries.Count();
            if(masteriesCount <= 0 || masteriesCount > 2) 
            {
                return false;

            }

            string[] masteryArray = new string[]
            {
                "Fire",
                "Earth",
                "Water",
                "Wind",
                "Light",
                "Weapon Master",
                "Taijutsu",
                "Medical",
                "Gentle Fist"
            };


            foreach( string mastery in masteries )
            {
                if (!masteryArray.Contains(mastery))
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> ClanCheck(ModalContext ctx, string[] modalValues)
        {
            var clan = modalValues.ElementAt(3).Trim();

            string[] clanArray = new string[]
            {
                "Sasayaki",
                "Muteki",
                "Suwa",
                "Ukiyo",
                "Hayashi",
                "Clanless"
            };

            if(!clanArray.Contains(clan))
            {
                return false;
            }

            return true;
        }

        public async Task<bool> LevelCheck(ModalContext ctx, string[] modalValues)
        {
            int level;
            try
            {
                level = int.Parse(modalValues.ElementAt(1));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }


            if (level <= 0 || level > 60)
            {
                return false;
            }

            return true;
        }

    }
}
