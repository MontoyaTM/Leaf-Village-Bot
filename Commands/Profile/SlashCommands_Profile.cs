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
using System.Reflection.Emit;
using System.Drawing;

namespace Leaf_Village_Bot.Commands.Profile
{
    public class SlashCommands_Profile : ApplicationCommandModule
    {
        [SlashCommand("profile", "Displays the user's profile as an embed.")]
        [SlashCooldown(2, 30, SlashCooldownBucketType.User)]
        public async Task DisplayProfile(InteractionContext ctx)
        {
            await ctx.DeferAsync();

            DBUtil_Profile DBUtil_Profile = new DBUtil_Profile();

            var isRetieved = await DBUtil_Profile.GetProfileAsync(ctx.User.Id);

            if(isRetieved.Item1)
            {
                var profile = isRetieved.Item2;

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
            } else
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Failed to retrieved profile!"));
            }
        }

        [SlashCommand("update_profile", "Updates all fields of the user's profile.")]
        [SlashCooldown(2, 30, SlashCooldownBucketType.User)]
        public async Task UpdateProfile(InteractionContext ctx, [Option("IGN", "Character's in game name.")] string IGN,
                                                                [Option("Level", "Character's current level.")] long Level,
                                                                [Option("Masteries", "Character's masteries.")] string Masteries,
                                                                [Option("Clan", "Character's clan.")] string Clan)
        {
            await ctx.DeferAsync(true);
            
            DBUtil_Profile dBUtil_Profile = new DBUtil_Profile();

            var MemberID = ctx.Interaction.User.Id;
            var Profile = new DBProfile()
            {
                InGameName = IGN,
                Level = (int)Level,
                Masteries = Masteries,
                Clan = Clan
            };

            var isUpdated = await dBUtil_Profile.UpdateProfileAsync(MemberID, Profile);

            if (isUpdated)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Successfully updated profile information!"));
            }
            else
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Failed to updated profile information!"));
            }
        }




        [SlashCommand("update_level", "Updates the user's profile level.")]
        [SlashCooldown(2, 30, SlashCooldownBucketType.User)]
        public async Task UpdateProfileLevel(InteractionContext ctx, [Option("Level", "Level(1-60)")] long Level)
        {
            await ctx.DeferAsync(true);

            DBUtil_Profile DBUtil_Profile = new DBUtil_Profile();

            var MemberID = ctx.Interaction.User.Id;

            try
            {
                if (Level <= 0 || Level > 60)
                {
                    await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Failed to update level for {ctx.Interaction.User.Username}. Level field must within 1-60!"));
                    return;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            var isUpdated = await DBUtil_Profile.UpdateLevelAsync(MemberID, (int)Level);

            if (isUpdated)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Successfully updated profile level!"));
            }
            else
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Failed to updated profile level!"));
            }
        }

        [SlashCommand("update_masteries", "Updates the user's profile masteries.")]
        [SlashCooldown(2, 30, SlashCooldownBucketType.User)]
        public async Task UpdateMasteries(InteractionContext ctx, [Option("Masteries", "The masteries you want to update.")] string Masteries)
        {
            await ctx.DeferAsync(true);

            DBUtil_Profile DBUtil_Profile = new DBUtil_Profile();

            var MemberID = ctx.Interaction.User.Id;

            var isUpdated = await DBUtil_Profile.UpdateMasteriesAsync(MemberID, Masteries);

            if (isUpdated)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Successfully updated profile masteries!"));
            }
            else
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Failed to updated profile masteries!"));
            }
        }

        [SlashCommand("update_clan", "Updates the user's profile clan.")]
        [SlashCooldown(2, 30, SlashCooldownBucketType.User)]
        public async Task UpdateClan(InteractionContext ctx, [Option("Clan", "The clan you want to update.")] string Clan)
        {
            await ctx.DeferAsync(true);

            DBUtil_Profile DBUtil_Profile = new DBUtil_Profile();

            var MemberID = ctx.Interaction.User.Id;

            var isUpdated = await DBUtil_Profile.UpdateClanAsync(MemberID, Clan);

            if (isUpdated)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Successfully updated profile masteries!"));
            }
            else
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Failed to updated profile masteries!"));
            }
        }



        [SlashCommand("update_profileimage", "Updates the user's profile image.")]
        [SlashCooldown(2, 30, SlashCooldownBucketType.User)]
        public async Task UpdateProfileImage(InteractionContext ctx, [Option("Image", "The image you want to upload.")] DiscordAttachment image)
        {
            await ctx.DeferAsync(true);

            DBUtil_Profile DBUtil_Profile = new DBUtil_Profile();

            var MemberID = ctx.Interaction.User.Id;
            var isUpdated = await DBUtil_Profile.UpdateProfileImageAsync(MemberID, image.Url);

            if (isUpdated)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Successfully updated profile image!"));
            }
            else
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Failed to updated profile image!"));
            }
        }

        [SlashCommand("add_alt", "Inserts a character into your alt(s) list. ")]
        [SlashCooldown(2, 30, SlashCooldownBucketType.User)]
        public async Task InsertAlt(InteractionContext ctx, [Option("IGN", "The ingame name of the character you want to add.")] string Alt)
        {
            await ctx.DeferAsync(true);

            DBUtil_Profile DBUtil_Profile = new DBUtil_Profile();

            var MemberID = ctx.Interaction.User.Id;
            var isUpdated = await DBUtil_Profile.AddAltAsync(MemberID, Alt);

            if (isUpdated)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Successfully added {Alt} to your alt(s) list!"));

                var isRetrieved = await DBUtil_Profile.GetAltsListAsync(MemberID);

                DiscordMember member = await ctx.Guild.GetMemberAsync(MemberID);

                var embedAlts = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Red)
                    .WithTitle($"{ctx.Interaction.User.Username} Alt(s) List:")
                    .WithDescription(isRetrieved.Item2)
                    .WithThumbnail(Global.LeafSymbol_URL)
                    .WithImageUrl(ctx.Interaction.User.AvatarUrl)
                );

                await member.SendMessageAsync(embedAlts);
            }
            else
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Failed to add {Alt} to your alt(s) list!"));
            }
        }

        [SlashCommand("give_fame", "Gives fame to a user: 1 day cooldown.")]
        [SlashCooldown(1, 86400, SlashCooldownBucketType.User)]
        public async Task GiveFame(InteractionContext ctx, [Option("User", "Discord User to recieve your fame.")] DiscordUser User)
        {
            await ctx.DeferAsync();

            DBUtil_Profile dBUtil_Profile = new DBUtil_Profile();

            var MemberID = ctx.Interaction.User.Id;
            var UserID = User.Id;

            var Profile = await dBUtil_Profile.GetProfileImageAsync(MemberID);
            var isUpdated = await dBUtil_Profile.UpdateFameAsync(UserID);

            if (isUpdated)
            {
                var embedFame = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithColor(DiscordColor.SpringGreen)
                        .WithDescription($"{ctx.Interaction.User.Username} gave fame to {User.Username}!")
                        .WithThumbnail(Profile.Item2)
                        
                    );

                await ctx.EditResponseAsync(new DiscordWebhookBuilder(embedFame));
            }
            else
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Unable to give {User.Username} fame!"));
            }
        }
    }
}
