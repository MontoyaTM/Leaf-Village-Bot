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
using System.Net.Sockets;
using Leaf_Village_Bot.DBUtil.Profile;

namespace Leaf_Village_Bot.Commands.Profile
{
    public class PrefixCommands_Profile : BaseCommandModule
    {

        [Command("villager_application")]
        [RequireRoles(RoleCheckMode.Any, "Administrator")]
        public async Task VillagerApplication(CommandContext ctx)
        {
            ButtonCommandsExtension buttonCommand = ctx.Client.GetButtonCommands();
          
            var embedApplication = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.SpringGreen)
                    .WithTitle("Leaf Village — Villager Application")
                    .WithImageUrl(Global.VillagerApplication_URL)
                    .WithThumbnail(Global.LeafSymbol_URL)
                    .AddField("IGN:", "The in game name for the character you are currently maining.")
                    .AddField("Introduction:", "A short introduction about yourself and why you want to be apart of the Leaf community.")
                    .AddField("Alt(s):", "An entire list of characters that you play on or have access to, including the IGN of the character for this application. Make sure to separate your alt(s) with a comma!" +
                                         "\n\n Ex: IGN, Alt1, Alt2, ...")
                    .WithFooter("Please be aware of one application per user, you will not be able to edit your application once submitted!")
                ).AddComponents
                (
                    new DiscordButtonComponent(ButtonStyle.Success, buttonCommand.BuildButtonId("btn_CreateApplication"), "Create Application")
                );

            await ctx.Message.DeleteAsync();
            await ctx.Channel.SendMessageAsync(embedApplication);
        }

        [Command("character_profile")]
        [RequireRoles(RoleCheckMode.Any, "Administrator")]
        public async Task UpdateProfile(CommandContext ctx)
        {
            ButtonCommandsExtension buttonCommand = ctx.Client.GetButtonCommands();

            var dropdownClanOptions = new List<DiscordSelectComponentOption>()
            {
                new DiscordSelectComponentOption("Clanless", "Clanless", null, false, new DiscordComponentEmoji(1175069988982362246)),
                new DiscordSelectComponentOption("Sasayaki", "Sasyaki", null, false, new DiscordComponentEmoji(1174898737642995743)),
                new DiscordSelectComponentOption("Muteki", "Muteki", null, false, new DiscordComponentEmoji(1174898803069964368)),
                new DiscordSelectComponentOption("Suwa", "Suwa", null, false, new DiscordComponentEmoji(1174898822246322246)),
                new DiscordSelectComponentOption("Ukiyo", "Ukiyo", null, false, new DiscordComponentEmoji(1174898833554149397)),
                new DiscordSelectComponentOption("Hayashi", "Hayashi", null, false, new DiscordComponentEmoji(1174898789702717562)),

            };
            var createClanDropdown = new DiscordSelectComponent("dpdwn_ClanEmoji", "Update Clan", dropdownClanOptions, false, 1, 1);

            var dropdownMasteryOptions = new List<DiscordSelectComponentOption>()
            {
                new DiscordSelectComponentOption("Fire", "Fire", null, false, new DiscordComponentEmoji(1175079183957889054)),
                new DiscordSelectComponentOption("Wind", "Wind", null, false, new DiscordComponentEmoji(1175079245295407204)),
                new DiscordSelectComponentOption("Lightning", "Lightning", null, false, new DiscordComponentEmoji(1175079192791109714)),
                new DiscordSelectComponentOption("Earth", "Earth", null, false, new DiscordComponentEmoji(1175079173006565386)),
                new DiscordSelectComponentOption("Water", "Water", null, false, new DiscordComponentEmoji(1175079227394113536)),
                new DiscordSelectComponentOption("Medical", "Medical", null, false, new DiscordComponentEmoji(1175079203163611156)),
                new DiscordSelectComponentOption("Weapon Master", "Weapon Master", null, false, new DiscordComponentEmoji(1175079236416061462)),
                new DiscordSelectComponentOption("Taijutsu", "Taijutsu", null, false, new DiscordComponentEmoji(1175079215016726538)),
                new DiscordSelectComponentOption("Gentle Fist", "Gentle Fist", null, false, new DiscordComponentEmoji(1180996595018387466)),
                new DiscordSelectComponentOption("Fan", "Fan", null, false, new DiscordComponentEmoji(1180996613246812220)),
                new DiscordSelectComponentOption("Bubble", "Bubble", null, false, new DiscordComponentEmoji(1180996625456447598))
            };
            var createMasteryDropdown = new DiscordSelectComponent("dpdwn_MasteryEmoji", "Update Mastery(s)", dropdownMasteryOptions, false, 1, 2);

            var embedUpdate = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.SpringGreen)
                    .WithTitle("Character Profile")
                    .AddField("Description:", "Please use the following options below to update your character profile information.")
                    .WithImageUrl(Global.CharacterProfile_URL)
                    .WithThumbnail(Global.LeafSymbol_URL)
                )
                .AddComponents(createClanDropdown)
                .AddComponents(createMasteryDropdown)
                .AddComponents(
                new DiscordButtonComponent(ButtonStyle.Success, buttonCommand.BuildButtonId("btn_UpdateIGN"), "Update IGN"),
                new DiscordButtonComponent(ButtonStyle.Success, buttonCommand.BuildButtonId("btn_UpdateLevel"), "Update Level"));

            await ctx.Message.DeleteAsync();
            await ctx.Channel.SendMessageAsync(embedUpdate);
        }
    }
}
