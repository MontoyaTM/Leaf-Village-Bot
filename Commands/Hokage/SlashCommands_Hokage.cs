using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Net.Models;
using DSharpPlus.SlashCommands;
using Leaf_Village_Bot.DBUtil.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leaf_Village_Bot.Commands.Hokage
{
    public class SlashCommands_Hokage : ApplicationCommandModule
    {
        [SlashCommand("update_user_organization", "Updates a user's Organization & Rank title on their profile.")]
        public async Task UpdateOrg(InteractionContext ctx, [Option("User", "The user you wish to assign Organization and Rank.")] DiscordUser User,

                                                            [Choice("12 Guardians", "12 Guardians")]
                                                            [Choice("Leaf Military Police Force", "Leaf Military Police Force")]
                                                            [Choice("Lead Medical Corp", "Lead Medical Corp")]
                                                            [Choice("Leaf ANBU", "Leaf ANBU")]
                                                            [Option("Organization", "The organization to assign the user.")] string Organization, 
            
                                                            [Option("Rank", "The rank to assign the user.")] string Rank)
        {
            await ctx.DeferAsync(true);

            var hasRole = ctx.Member.Roles.Any(x => x.Name == "Hokage" || x.Name == "Org Leader");

            if (hasRole)
            {
                DBUtil_Profile dBUtil_Profile = new DBUtil_Profile();

                var Member = (DiscordMember)User;
                var isAssigned = await dBUtil_Profile.UpdateOrgAsync(Member.Id, Organization, Rank);

                if(isAssigned)
                {
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Successfully assigned {Organization} {Rank} to {User.Username}!"));
                } else
                {
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Failed to assign {Organization} {Rank} to {User.Username}!"));
                }

            } else
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("You do not have the role to access this command!"));
            }

        }   
    }
}
