﻿using DSharpPlus.ButtonCommands;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.ButtonCommands.Extensions;
using DSharpPlus.Net.Models;

namespace Leaf_Village_Bot.Commands.Hokage
{
    public class PrefixCommands_Hokage : BaseCommandModule
    {
        [Command("hokage_dashboard")]
        [RequireRoles(RoleCheckMode.Any, "Administrator")]
        [Description("Leaf Hokage Dashboard used to manage village affairs.")]
        public async Task HokageDashboard(CommandContext ctx)
        {
            ButtonCommandsExtension buttonCommand = ctx.Client.GetButtonCommands();

            var embedDashboard = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.SpringGreen)
                    .WithTitle("Leaf Village Hokage Dashboard")
                    .WithThumbnail(Global.LeafSymbol_URL)
                    .WithImageUrl(Global.HokageNPC_URL)
                    .AddField("Delete Application", "Enter the Member ID of the applicant you wish to delete. This will remove the data stored in the database of the user, allowing the individual to create a new villager application.")
                    .AddField("Retrieve Alt(s)", "Enter the Member ID of the user you want to retrieve a list of alt(s). An embeded list of alt(s) will be sent in this channel.")
                )
                .AddComponents(new DiscordComponent[]
                {
                    new DiscordButtonComponent(ButtonStyle.Danger, buttonCommand.BuildButtonId("btn_DeleteApplication"), "Delete Application"),
                    new DiscordButtonComponent(ButtonStyle.Primary, buttonCommand.BuildButtonId("btn_GetAltList"), "Retrieve Alt(s)")
                });

            await ctx.Message.DeleteAsync();
            await ctx.Channel.SendMessageAsync(embedDashboard);
        }

        [Command("application_information")]
        [RequireRoles(RoleCheckMode.Any, "Administrator")]
        [Description("Displays an Embed used as a guide to accepting and denying applications.")]
        public async Task ApplicationInfo2(CommandContext ctx)
        {
            ButtonCommandsExtension buttonCommand = ctx.Client.GetButtonCommands();

            var embedApplication = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithColor(DiscordColor.SpringGreen)
                        .WithTitle($"Button Information")
                        .WithThumbnail(Global.LeafSymbol_URL)
                        .AddField("Accept Applicant:", "Pressing this button will grant the user Genin role and their application will be sent to the application-accepted channel for storage. ")
                        .AddField("Deny Applicant:", "Pressing this button will display an Embed telling you to type the reason for denying the applicant. Your response will then be sent to the user and their application will be sent to the application-denied channel.")
                        .WithFooter("The buttons are for display purposes, they do not have functionality.")
                    ).AddComponents(new DiscordComponent[]
                    {
                        new DiscordButtonComponent(ButtonStyle.Primary, buttonCommand.BuildButtonId("displayOnly1"), "Accept Applicant"),
                        new DiscordButtonComponent(ButtonStyle.Secondary, buttonCommand.BuildButtonId("displayOnly2"), "Deny Applicant")
                    });
            await ctx.Message.DeleteAsync();
            await ctx.Channel.SendMessageAsync(embedApplication);
        }
    }
}
