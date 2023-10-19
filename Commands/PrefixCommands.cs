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

            var embedRules = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.SpringGreen)
                    .WithTitle("Leaf Village Rulebook")
                    .WithThumbnail(Global.LeafSymbol_URL)
                    .WithImageUrl(Global.LeafGate_URL)
                    .AddField("Rules:", "1: Orders from the Hokage are absolute. You must follow and respect them.")
                    .AddField("2:", "You must aid a fellow Leaf Ninja if they or the village is being attacked no matter who it is. (If it's not clear that a Leaf ninja needs help then the person that requires the aid has to make it clear that he needs assistance).")
                    .AddField("3:", "It is strictly forbidden to attack or kill any Leaf Ninja. (This rule does not apply to spars, tournaments, village free for alls, etc. If all parties do not consent to a fight, do not attack them).")
                    .AddField("4:", "Respect and follow orders from the chain of command: " +
                                    "\nKage > Council > LMPF > Org Leaders > Former Hokage > Jonin > Chunin > Specialized Jonin> Genin")
                    .AddField("5:", "Verbal abuse and hate speech are not tolerated.")
                    .AddField("6:", "Respect each other boss farming (first come first served) any issues will be handled with LMPF")
                    .AddField("7:", "Do not talk about or expose the identity of any ANBU members.")
                    .AddField("8:", "Giving the enemy information regarding the Leaf is prohibited.")
                    .AddField("9:", "It is forbidden to aid the enemy in any form of way that would endanger or give them an advantage against the Leaf.")
                    .AddField("10:", "A friendly outsider is only allowed in Leaf under the condition that they are in the process of being pardoned, or escorted by a ranked ninja/org member.")
                    .AddField("11:", "Do not place a bounty on a comrade.")
                    .AddField("12:", "Do not intentionally steal from another Leaf Ninja.")
                    .AddField("13:", "If you argue with a villager, try to solve it respectfully, and if unable to come to an agreement contact your superiors.")
                    .AddField("14:", "Do not wear a mask inside the Leaf without permission. The only exception is the ANBU within the Leaf.")
                );

            await ctx.Channel.SendMessageAsync(embedRules);
            await ctx.Message.DeleteAsync();

        }
    }
}
