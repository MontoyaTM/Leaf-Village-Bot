using DSharpPlus.ButtonCommands;
using DSharpPlus.ButtonCommands.Extensions;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace Leaf_Village_Bot.Commands
{
    public class PrefixCommands : BaseCommandModule
    {
        private string LeafSymbolURL = "https://i.imgur.com/spjhOGb.png";
        private string LMPFSymbolURL = "https://i.imgur.com/QbBzNiR.png";
        private string VillagerNPCURL = "https://i.imgur.com/LZPDi1r.png";
        private string LMPFNPCURL = "https://i.imgur.com/CDsvN88.png";

        [Command("lmpf_ticketreport")]
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
                    .WithImageUrl(LMPFNPCURL)
                    .WithThumbnail(LeafSymbolURL)
                    .AddField("Plantiff:", "The person who issued the report.")
                    .AddField("Defendant:", "The person(s) who are being accused.")
                    .AddField("Date:", "The date the incident took place.")
                    .AddField("Details:", "A detailed explainination of the situation. (Who, What, When, Where, Why)")
                    .AddField("Screenshot URL:", "Link(s) to any screenshots to back up your claim. (i.e Imgur)  If you have issues uploading to third-party websites, share your screen shots in " +
                              "the report discussion. Make sure to mention that you are doing so in the initial report.")
                    .WithFooter("The Leaf Military Police Force will investigate your report and will be reaching out to you shortly. Please be aware that false reports are a crime and can be punishable.")
                    )
                .AddComponents(new DiscordComponent[]
                {
                    new DiscordButtonComponent(DSharpPlus.ButtonStyle.Primary, buttonCommand.BuildButtonId("btnLMPFCreateReport"), "Create Report")
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
                    .WithTitle("Leaf Military Police Report Dashboard")
                    .WithThumbnail(LeafSymbolURL)
                    .WithImageUrl(LMPFNPCURL)
                    .AddField("View Tickets:", "Displays a list of active tickets that needs to be investigated by LMPF.")
                    .AddField("Get Ticket:", "Displays a dropdown menu of active tickets and returns the selected ticket to be displayed")
                    .AddField("Delete Ticket:", "Displays a dropdown menu of active tickets and deletes the selected ticket from active reports.")

                )
                .AddComponents(new DiscordComponent[]
                {
                    new DiscordButtonComponent(DSharpPlus.ButtonStyle.Primary, buttonCommand.BuildButtonId("btnLMPFViewTicket"), "View Tickets"),
                    new DiscordButtonComponent(DSharpPlus.ButtonStyle.Primary, buttonCommand.BuildButtonId("btnLMPFGetTicket"), "Get Ticket"),
                    new DiscordButtonComponent(DSharpPlus.ButtonStyle.Primary, buttonCommand.BuildButtonId("btnLMPFDeleteTicket"), "Delete Ticket"),
                });

            await ctx.Message.DeleteAsync();
            await ctx.Channel.SendMessageAsync(embedDashboard);
        }

        [Command("villager_form")]
        [RequireRoles(RoleCheckMode.Any, "Administrator")]
        [Description("Displays and Embed for a Villager Application.")]
        public async Task VillagerForm(CommandContext ctx)
        {
            ButtonCommandsExtension buttonCommand = ctx.Client.GetButtonCommands();

            var embedVillagerApplication = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.SpringGreen)
                    .WithTitle("Leaf Village — Villager Application")
                    .WithDescription(" ")
                    .WithImageUrl(VillagerNPCURL)
                    .WithThumbnail(LeafSymbolURL)
                    .AddField("IGN:", "The in game name of the character you are currently maining.")
                    .AddField("Level (1-60):", "The level of your character.")
                    .AddField("Masteries:", "The masteries of your character. Make sure to separate your masteries with a comma!" +
                                            "\n\n Ex: Fire, Earth \n")
                    .AddField("Clan:", "The clan your character has chosen. If you did not choose a clan, type \"Clanless\".")
                    .AddField("Alt(s):", "An entire list of characters that you play on or have access to. Please include the IGN of the character for this application. " +
                                         "\n\nIf you are caught lying, an immediate punishment will follow which may include exile. Make sure to separate your alt(s) with a comma!" +
                                         "\n\n Ex: IGN, Alt1, Alt2, ...")
                    .WithFooter("Please be aware of one application per user, you will not be able to edit your application once submitted!")

                ).AddComponents(new DiscordComponent[]
                {
                    new DiscordButtonComponent(DSharpPlus.ButtonStyle.Primary, buttonCommand.BuildButtonId("btnCreateVillagerApplication"), "Create Application")
                });
            await ctx.Message.DeleteAsync();
            await ctx.Channel.SendMessageAsync(embedVillagerApplication);
        }
    }
}
