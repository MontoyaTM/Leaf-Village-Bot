using DSharpPlus.ButtonCommands;
using DSharpPlus;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Leaf_Village_Bot.DBUtil.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using DSharpPlus.SlashCommands.Attributes;
using DSharpPlus.CommandsNext;
using DSharpPlus.Net.Models;

namespace Leaf_Village_Bot.Commands.Profile
{
    public class SlashCommands_Profile : ApplicationCommandModule
    {
        [SlashCommand("profile", "Displays the user's profile as an embed")]
        [SlashCooldown(2, 30, SlashCooldownBucketType.User)]
        public async Task DisplayProfile(InteractionContext ctx)
        {
            await ctx.DeferAsync();

            DBUtil_Profile DBUtil_Profile = new DBUtil_Profile();

            var result = await DBUtil_Profile.GetProfile(ctx.User.Id);
            var profile = result.Item2;

            var embedApplication = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithColor(DiscordColor.SpringGreen)
                        .WithTitle($"Leaf Village Application")
                        .WithImageUrl(profile.ProfileImage)
                        .WithThumbnail(Global.LeafSymbol_URL)
                        .AddField("IGN:", profile.InGameName, true)
                        .AddField("Level:", profile.Level.ToString(), true)
                        .AddField("Masteries:", profile.Masteries, true)
                        .AddField("Clan:", profile.Clan)
                        .AddField("Organization:", profile.Organization)
                        .AddField("Organization Rank:", profile.OrgRank)
                        .AddField("Proctored Missions:", profile.ProctoredMissions.ToString())
                        .AddField("Raids:", profile.Raids.ToString(), true)
                        .AddField("Fame:", profile.Fame.ToString(), true)
                        );

            await ctx.EditResponseAsync(new DiscordWebhookBuilder(embedApplication));

            //var hasRankedRole = ctx.Member.Roles.Any(x => x.Name == "Genin");

            //if(hasRankedRole)
            //{
            //    var result = await DBUtil_Profile.GetProfile(ctx.User.Id);
            //    var profile = result.Item2;

            //    var embedApplication = new DiscordMessageBuilder()
            //            .AddEmbed(new DiscordEmbedBuilder()
            //                .WithColor(DiscordColor.SpringGreen)
            //                .WithTitle($"Leaf Village Application")
            //                .WithImageUrl(profile.ProfileImage)
            //                .WithThumbnail(Global.LeafSymbol_URL)
            //                .AddField("IGN:", profile.InGameName, true)
            //                .AddField("Level:", profile.Level.ToString(), true)
            //                .AddField("Masteries:", profile.Masteries, true)
            //                .AddField("Clan:", profile.Clan)
            //                .AddField("Organization:", profile.Organization)
            //                .AddField("Organization Rank:", profile.OrgRank)
            //                .AddField("Proctored Missions:", profile.ProctoredMissions.ToString())
            //                .AddField("Raids:", profile.Raids.ToString(), true)
            //                .AddField("Fame:", profile.Fame.ToString(), true)
            //                );

            //    await ctx.EditResponseAsync(new DiscordWebhookBuilder(embedApplication));
            //} else
            //{
            //    await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("You do not have the Genin role to access this command!"));
            //}
            
        }

        [SlashCommand("update_profileimage", "Allows the user to change their profile image")]
        [SlashCooldown(2, 30, SlashCooldownBucketType.User)]
        public async Task UpdateProfileImage(InteractionContext ctx, [Option("Image", "The image you want to upload.")] DiscordAttachment image)
        {
            await ctx.DeferAsync();

            var hasRankedRole = ctx.Member.Roles.Any(x => x.Name == "Genin");

            if(hasRankedRole)
            {
                DBUtil_Profile DBUtil_Profile = new DBUtil_Profile();
                var memberid = ctx.Interaction.User.Id;

                var isUpdated = await DBUtil_Profile.UpdateProfileImage(memberid, image.Url);

                if (isUpdated)
                {
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Successfully updated profile image!"));
                }
                else
                {
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Failed to updated profile image!"));
                }
            } else
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("You do not have the Genin role to access this command!"));
            }
        }

        [SlashCommand("update_level", "Allows the user to change their level.")]
        [SlashCooldown(2, 30, SlashCooldownBucketType.User)]
        public async Task UpdateProfileLevel(InteractionContext ctx, [Option("Level", "Level(1-60)")] long Level)
        {
            await ctx.DeferAsync();

            var hasRankedRole = ctx.Member.Roles.Any(x => x.Name == "Genin");

            if(hasRankedRole)
            {
                DBUtil_Profile DBUtil_Profile = new DBUtil_Profile();
                var memberid = ctx.Interaction.User.Id;

                try
                {
                    if (Level <= 0 || Level > 60)
                    {
                        await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Failed to created application for {ctx.Interaction.User.Username}. Level field must within 1-60!"));
                        return;
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }

                var isUpdated = await DBUtil_Profile.UpdateLevel(memberid, (int)Level);

                if (isUpdated)
                {
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Successfully updated profile level!"));
                } else
                {
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Failed to updated profile level!"));
                }
            } else
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("You do not have the Genin role to access this command!"));
            }
        }



        [SlashCommand("update_profile", "Allows the user to update their profile information")]
        [SlashCooldown(2, 30, SlashCooldownBucketType.User)]
        public async Task UpdateProfile(InteractionContext ctx, [Option("IGN", "Character's in game name.")]string IGN,
                                                                [Option("Level", "Character's current level.")]long Level,
                                                                [Option("Masteries", "Character's masteries.")] string Masteries,
                                                                [Option("Clan", "Character's clan.")] string Clan)
        {
            await ctx.DeferAsync();

            var hasRankedRole = ctx.Member.Roles.Any(x => x.Name == "Genin");

            if (hasRankedRole)
            {
                DBUtil_Profile dBUtil_Profile = new DBUtil_Profile();

                var MemberID = ctx.Interaction.User.Id;
                var profile = new DBProfile()
                {
                    InGameName = IGN,
                    Level = (int)Level,
                    Masteries = Masteries,
                    Clan = Clan
                };

                var isUpdated = await dBUtil_Profile.UpdateProfile(MemberID, profile);

                if(isUpdated)
                {
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Successfully updated profile information!"));
                } else
                {
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Failed to updated profile information!"));
                }
            } else
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("You do not have the Genin role to access this command!"));
            }
        }

        [SlashCommand("give_fame", "Gives fame to a Discord User; 3 day cooldown.")]
        [SlashCooldown(1, 86400, SlashCooldownBucketType.User)]
        public async Task GiveFame(InteractionContext ctx, [Option("User", "Discord User to recieve your fame.")] DiscordUser User)
        {
            await ctx.DeferAsync();

            var hasRole = ctx.Member.Roles.Any(x => x.Name == "Genin");

            if (hasRole)
            {
                DBUtil_Profile dBUtil_Profile = new DBUtil_Profile();
                
                var MemberID = ctx.Interaction.User.Id;
                var profile = await dBUtil_Profile.GetProfileImageAsync(MemberID);
                

                var UserID = User.Id;

                var isUpdated = await dBUtil_Profile.UpdateFame(UserID);

                if (isUpdated)
                {
                    var embed = new DiscordMessageBuilder()
                        .AddEmbed(new DiscordEmbedBuilder()
                            .WithColor(DiscordColor.SpringGreen)
                            .WithTitle($"{ctx.Interaction.User.Username} gave fame to {User.Username}!")
                            .WithImageUrl(profile.Item2)
                        );

                    var message = new DiscordEmbedBuilder()
                    {
                        Color = DiscordColor.SpringGreen,
                        Title = $"{ctx.Interaction.User.Username} gave fame to {User.Username}!"
                    };

                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(message));
                }
                else
                {
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Unable to give {User.Username} fame!"));
                }

            }
            else
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("You do not have the Genin role to access this command!"));
            }

            
        }
    }
}
