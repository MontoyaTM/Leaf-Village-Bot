using DSharpPlus.ButtonCommands.Extensions;
using DSharpPlus.ButtonCommands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leaf_Village_Bot.Commands.LMPF
{
    public class PrefixCommands_LMPF : BaseCommandModule
    {
        [Command("lmpf_reportticket")]
        [RequireRoles(RoleCheckMode.Any, "Administrator")]
        [Description("Displays an Embed for the Leaf Military Police Force Ticket Report.")]
        public async Task LMPFReportForm(CommandContext ctx)
        {
            ButtonCommandsExtension buttonCommand = ctx.Client.GetButtonCommands();

            var embedSupportForm = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.SpringGreen)
                    .WithTitle("Leaf Military Police Force Ticket Report")
                    .WithDescription(" ")
                    .WithImageUrl(Global.LMPFNPC_URL)
                    .WithThumbnail(Global.LeafSymbol_URL)
                    .AddField("Plantiff:", "The person who issued the report.")
                    .AddField("Defendant:", "The person(s) who are being accused.")
                    .AddField("Date:", "The date the incident took place.")
                    .AddField("Details:", "A detailed explainination of the situation. (Who, What, When, Where, Why)")
                    .AddField("Screenshot(s):", "Any links to images or screenshots to support your ticket report should be posted in the discussion channel created after submission.")
                    .WithFooter("Leaf Military Police Force personnel will investigate your claim and will reach out to you in the discussion channel. Please be aware that false reports are a crime and will be punished.")
                    )
                .AddComponents(new DiscordComponent[]
                {
                    new DiscordButtonComponent(ButtonStyle.Primary, buttonCommand.BuildButtonId("btn_CreateTicket"), "Create Report")
                });

            await ctx.Channel.SendMessageAsync(embedSupportForm);
            await ctx.Message.DeleteAsync();
        }

        [Command("lmpf_dashboard")]
        [RequireRoles(RoleCheckMode.Any, "Administrator")]
        [Description("Leaf Military Police Force Dashboard used to manage report tickets.")]
        public async Task LMPFDashboard(CommandContext ctx)
        {
            ButtonCommandsExtension buttonCommand = ctx.Client.GetButtonCommands();

            var embedDashboard = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.SpringGreen)
                    .WithTitle("Leaf Military Police Force Dashboard")
                    .WithThumbnail(Global.LeafSymbol_URL)
                    .WithImageUrl(Global.LMPFNPC_URL)
                    .AddField("View Tickets:", "Displays a list of active tickets that needs to be investigated by the Leaf Military Police Force.")
                    .AddField("Get Ticket:", "Displays a dropdown menu of active tickets and returns the selected ticket to be displayed")
                    .AddField("Delete Ticket:", "Displays a dropdown menu of active tickets and deletes the selected ticket from active reports.")

                )
                .AddComponents(new DiscordComponent[]
                {
                    new DiscordButtonComponent(ButtonStyle.Primary, buttonCommand.BuildButtonId("btn_ViewTicket"), "View Tickets"),
                    new DiscordButtonComponent(ButtonStyle.Primary, buttonCommand.BuildButtonId("btn_GetTicket"), "Get Ticket"),
                    new DiscordButtonComponent(ButtonStyle.Primary, buttonCommand.BuildButtonId("btn_DeleteTicket"), "Delete Ticket")
                });

            await ctx.Message.DeleteAsync();
            await ctx.Channel.SendMessageAsync(embedDashboard);
        }
    }
}
