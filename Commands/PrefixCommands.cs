using DSharpPlus;
using DSharpPlus.ButtonCommands;
using DSharpPlus.ButtonCommands.Extensions;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace Leaf_Village_Bot.Commands
{
    public class PrefixCommands : BaseCommandModule
    {
        [Command("rules_embed")]
        [RequireRoles(RoleCheckMode.Any, "Administrator")]
        public async Task DisplayRules(CommandContext ctx)
        {
            ButtonCommandsExtension buttonCommand = ctx.Client.GetButtonCommands();

            var embed0 = new DiscordEmbedBuilder()
            {
                Title = "Leaf Village Rulebook",
                Color = DiscordColor.SpringGreen,
                ImageUrl = Global.LeafGate_URL
            };

            List<DiscordEmbed> embeds1 = new List<DiscordEmbed>();
            List<DiscordEmbed> embeds2 = new List<DiscordEmbed>();

            string[] rules_1 = new string[10];
            rules_1[0] = "1: Orders from the Hokage are absolute. You must follow and respect them.";
            rules_1[1] = "2: You must aid a fellow Leaf Ninja if they or the village is being attacked no matter who it is. " +
                         "\n(If it's not clear that a Leaf ninja needs help then the person that requires the aid has to make it clear that he needs assistance).";
            rules_1[2] = "3: It is strictly forbidden to attack or kill any Leaf Ninja. " +
                         "\n(This rule does not apply to spars, tournaments, village free for alls, etc. If all parties do not consent to a fight, do not attack them).";
            rules_1[3] = "4: Respect and follow orders from the chain of command: " +
                                    "\nKage > Council > LMPF > Org Leaders > Former Hokage > Jonin > Chunin > Specialized Jonin> Genin";
            rules_1[4] = "5: Verbal abuse and hate speech are not tolerated.";
            rules_1[5] = "6: Respect each other boss farming (first come first served) any issues will be handled with LMPF";
            rules_1[6] = "7: Do not talk about or expose the identity of any ANBU members.";
            rules_1[7] = "8: Giving the enemy information regarding the Leaf is prohibited.";
            rules_1[8] = "9: It is forbidden to aid the enemy in any form of way that would endanger or give them an advantage against the Leaf.";
            rules_1[9] = "10: A friendly outsider is only allowed in Leaf under the condition that they are in the process of being pardoned, or escorted by a ranked ninja/org member.";
            
            
            string[] rules_2 = new string[4];
            rules_2[0] = "11: Do not place a bounty on a comrade.";
            rules_2[1] = "12: Do not intentionally steal from another Leaf Ninja.";
            rules_2[2] = "13: If you argue with a villager, try to solve it respectfully, and if unable to come to an agreement contact your superiors.";
            rules_2[3] = "14: Do not wear a mask inside the Leaf without permission. The only exception is the ANBU within the Leaf.";


            foreach(var rule in rules_1)
            {
                var embed = new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.SpringGreen,
                    Description = rule
                };
                embeds1.Add(embed);
            }

            foreach (var rule in rules_2)
            {
                var embed = new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.SpringGreen,
                    Description = rule
                };
                embeds2.Add(embed);
            }

            var embedRules = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.SpringGreen)
                    .WithTitle("Leaf Village Rulebook")
                    .WithImageUrl(Global.LeafGate_URL)
                   );

            var embedrules_1 = new DiscordMessageBuilder()
                .AddEmbeds(embeds1);
            var embedrules_2 = new DiscordMessageBuilder()
                .AddEmbeds(embeds2);

            await ctx.Channel.SendMessageAsync(embedRules);
            await ctx.Channel.SendMessageAsync(embedrules_1);
            await ctx.Channel.SendMessageAsync(embedrules_2);

            await ctx.Message.DeleteAsync();

        }

        [Command("embed_test")]
        [RequireRoles(RoleCheckMode.Any, "Administrator")]
        public async Task Test(CommandContext ctx)
        {
            var embed1 = new DiscordEmbedBuilder()
                .WithUrl("https://www.ninonline.com/forum/")
                .WithImageUrl(Global.Villager_URL);

            var embed2 = new DiscordEmbedBuilder()
                .WithUrl("https://www.ninonline.com/forum/")
                .WithImageUrl(Global.LMPFNPC_URL);

            var embed3 = new DiscordEmbedBuilder()
                .WithUrl("https://www.ninonline.com/forum/")
                .WithImageUrl(Global.RankedNPC_URL);

            var embed4 = new DiscordEmbedBuilder()
                .WithUrl("https://www.ninonline.com/forum/")
                .WithImageUrl(Global.HokageNPC_URL);

            List<DiscordEmbed> embeds = new List<DiscordEmbed>();
            embeds.Add(embed1);
            embeds.Add(embed2);
            embeds.Add(embed3);
            

            var embedrules = new DiscordMessageBuilder()
                .AddEmbeds(embeds);


            await ctx.Channel.SendMessageAsync(embedrules);
        }

        [Command("welcome_msg")]
        [RequireRoles(RoleCheckMode.Any, "Administrator")]
        public async Task Welcome(CommandContext ctx)
        {
            var message = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithTitle("# :leaves: Welcome to the Leaf Village!")
                    .WithDescription("A peaceful village hidden within a vast forest, residing in the Fire Country. We are a community committed to bringing together Leaf Ninjas in the world of NinOnline! " +
                    "Our passion is to promote activity within the village and promote the Will of Fire! \n\nOur interactive buttons below this message all you to view our rules")

                );
        }
    }
}
