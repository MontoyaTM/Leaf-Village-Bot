using DSharpPlus.ButtonCommands;
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

namespace Leaf_Village_Bot.Commands.Profile
{
    internal class PrefixCommands_Profile : BaseCommandModule
    {
        [Command("villager_form")]
        [RequireRoles(RoleCheckMode.Any, "Administrator")]
        [Description("Displays an Embed for a Villager Application.")]
        public async Task VillagerForm(CommandContext ctx)
        {
            ButtonCommandsExtension buttonCommand = ctx.Client.GetButtonCommands();

            var embedVillagerApplication = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.SpringGreen)
                    .WithTitle("Leaf Village — Villager Application")
                    .WithDescription(" ")
                    .WithImageUrl(Global.Villager_URL)
                    .WithThumbnail(Global.LeafSymbol_URL)
                    .AddField("IGN:", "The in game name of the character you are currently maining.")
                    .AddField("Level (1-60):", "The level of your character.")
                    .AddField("Masteries:", "The masteries of your character. Make sure to separate your masteries with a comma! " +
                                            "\n\nEx: Mastery1, Mastery2" +
                                            "\n\nFire, Earth, Wind, Water, Light, Medical, Weapon Master, Taijutsu, Gentle Fist")
                    .AddField("Clan:", "The clan your character has chosen. If you did not choose a clan, type Clanless." +
                                       "\n\nSasayaki, Muteki, Suwa, Ukiyo, Hayashi")
                    .AddField("Alt(s):", "An entire list of characters that you play on or have access to. Please include the IGN of the character for this application. " +
                                         "\n\nIf you are caught lying, an immediate punishment will follow which may include exile. Make sure to separate your alt(s) with a comma!" +
                                         "\n\n Ex: IGN, Alt1, Alt2, ...")
                    .WithFooter("Please be aware of one application per user, you will not be able to edit your application once submitted!")

                ).AddComponents(new DiscordComponent[]
                {
                    new DiscordButtonComponent(ButtonStyle.Success, buttonCommand.BuildButtonId("btn_CreateApplication"), "Create Application")
                });
            await ctx.Message.DeleteAsync();
            await ctx.Channel.SendMessageAsync(embedVillagerApplication);
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
