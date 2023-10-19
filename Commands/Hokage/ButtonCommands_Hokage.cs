using DSharpPlus.ButtonCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leaf_Village_Bot.DBUtil.Profile;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;

namespace Leaf_Village_Bot.Commands.Hokage
{
    public class ButtonCommands_Hokage : ButtonCommandModule
    {
        [ButtonCommand("btn_DeleteApplication")]
        public async Task DeleteApplication(ButtonContext ctx)
        {
            await ctx.Interaction.DeferAsync(true);

            var DBUtil_Profile = new DBUtil_Profile();

            var hasLMPFRole = ctx.Member.Roles.Any(x => x.Name == "Hokage");

            if(hasLMPFRole)
            {
                var embedMessage = new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.SpringGreen,
                    Title = "Enter the MemberID of the application you wish to delete as the next message in this channel."
                };
                var followupMessage = await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedMessage));

                var nextMessage = await ctx.Channel.GetNextMessageAsync();
                var memberID = ulong.Parse(nextMessage.Result.Content);
                DiscordMember member = null;

                try
                {
                     member = await ctx.Guild.GetMemberAsync(memberID);
                } catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                    var invalidUser = new DiscordEmbedBuilder()
                    {
                        Color = DiscordColor.Red,
                        Title = $"MemberID does not match any existing villager application, please double check the MemberID!"
                    };
                    await ctx.Channel.DeleteMessageAsync(nextMessage.Result);
                    await ctx.Interaction.EditFollowupMessageAsync(followupMessage.Id, new DiscordWebhookBuilder().AddEmbed(invalidUser));
                    return;
                }

                var isDeleted = await DBUtil_Profile.DeleteVillagerApplication(memberID);

                await ctx.Channel.DeleteMessageAsync(nextMessage.Result);

                if (isDeleted)
                {
                    var success = new DiscordEmbedBuilder()
                    {
                        Color = DiscordColor.SpringGreen,
                        Title = $"Successfully deleted {member.Username}'s villager application!"
                    };

                    await ctx.Interaction.EditFollowupMessageAsync(followupMessage.Id, new DiscordWebhookBuilder().AddEmbed(success));
                } else
                {
                    var failed = new DiscordEmbedBuilder()
                    {
                        Color = DiscordColor.Red,
                        Title = $"Failed to delete {member.Username}'s villager application!"
                    };

                    await ctx.Interaction.EditFollowupMessageAsync(followupMessage.Id, new DiscordWebhookBuilder().AddEmbed(failed));
                }
            }
        }







    }
}
