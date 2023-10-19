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

namespace Leaf_Village_Bot.Commands.Profile
{
    internal class SlashCommands_Profile : ApplicationCommandModule
    {

        [SlashCommand("profile", "Displays the user's profile as an embed")]
        [RequireRoles(RoleCheckMode.Any, "Genin")]
        [Cooldown(2, 30, CooldownBucketType.User)]
        public async Task DisplayProfile(InteractionContext ctx)
        {
            await ctx.DeferAsync();

            DBUtil_Profile DBUtil_Profile = new DBUtil_Profile();

            var hasRankedRole = ctx.Member.Roles.Any(x => x.Name == "Genin");

            if(hasRankedRole)
            {
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
            } else
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("You do not have the Genin role to access this command!"));
            }
            
        }

        [SlashCommand("update_profileimage", "Allows the user to change their profile image")]
        [RequireRoles(RoleCheckMode.Any, "Genin")]
        [Cooldown(2, 30, CooldownBucketType.User)]
        public async Task SetProfileImage(InteractionContext ctx, [Option("Image", "The image you want to upload.")] DiscordAttachment image)
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

        [SlashCommand("update_profile", "d")]
        [RequireRoles(RoleCheckMode.Any, "Genin")]
        [Cooldown(2, 30, CooldownBucketType.User)]
        public async Task UpdateProfile(InteractionContext ctx, [Option("IGN", "Character's in game name.")]string IGN,
                                                                [Option("Level", "Character's current level.")]long Level,
                                                                [Option("Masteries", "Character's masteries.")] string Masteries,
                                                                [Option("Clan", "Character's clan.")] string Clan)
        {




        }

        [SlashCommand("Caption", "Give any image a caption")]
        public async Task CaptionCommand(InteractionContext context, [Option("Caption", "The caption you want the image to have.")] string caption,
                                                                     [Option("Image", "The image you want to upload.")] DiscordAttachment image)
        {
            await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                                                                                .WithContent("Starting Slash Command..."));

            try
            {
                var captionMessage = new DiscordEmbedBuilder()
                {
                    Title = caption,
                    ImageUrl = image.Url
                };

                await context.Channel.SendMessageAsync(embed: captionMessage);
            }
            catch (Exception ex)
            {
                var errorMessage = new DiscordEmbedBuilder()
                {
                    Title = "Something went wrong!",
                    Description = ex.Message,
                    Color = DiscordColor.Red
                };

                await context.Channel.SendMessageAsync(embed: errorMessage);
            }
        }



    }
}
