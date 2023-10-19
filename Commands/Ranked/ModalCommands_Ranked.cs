using DSharpPlus.ButtonCommands.Extensions;
using DSharpPlus.ButtonCommands;
using DSharpPlus.ModalCommands;
using DSharpPlus.ModalCommands.Attributes;
using Leaf_Village_Bot.DBUtil.RPRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;

namespace Leaf_Village_Bot.Commands.Ranked
{
    public class ModalCommands_Ranked : ModalCommandModule
    {
        [ModalCommand("RPRequestModal")]
        public async Task SubmitRPRequest(ModalContext ctx)
        {
            await ctx.Interaction.DeferAsync(true);

            var modalValues = ctx.Values;
            var DBUtil_RPRequest = new DBUtil_RPRequest();

            ButtonCommandsExtension buttonCommand = ctx.Client.GetButtonCommands();

            var requestInfo = new DBRPRequest
            {
                RequestID = Global.CreateID(),
                UserName = ctx.Interaction.User.Username,
                MemberID = ctx.Interaction.User.Id,
                ServerID = ctx.Interaction.Guild.Id,
                IngameName = modalValues.ElementAt(0),
                RPMission = modalValues.ElementAt(1),
                Timezone = modalValues.ElementAt(2),
                Attendees = modalValues.ElementAt(3)
            };

            var isStored = await DBUtil_RPRequest.StoreRequestAsync(requestInfo);

            if(isStored)
            {
                var createDiscussionChannel = await ctx.Interaction.Guild.CreateChannelAsync($"{requestInfo.IngameName}: {requestInfo.RPMission}", ChannelType.Text, ctx.Interaction.Channel.Parent);
                
                var embedRequest = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithColor(DiscordColor.SpringGreen)
                                    .WithTitle($"RP Mission Request for {requestInfo.IngameName}")
                                    .WithImageUrl(ctx.Interaction.User.AvatarUrl)
                                    .WithThumbnail(Global.LeafSymbol_URL)
                                    .AddField("IGN:", requestInfo.IngameName, true)
                                    .AddField("RP Mission:", requestInfo.RPMission, true)
                                    .AddField("Timezone:", requestInfo.Timezone, true)
                                    .AddField("Attendees:", requestInfo.Attendees)
                                    .WithFooter($"Request ID: {requestInfo.RequestID}"))
                        .AddComponents(
                            new DiscordButtonComponent(ButtonStyle.Primary, buttonCommand.BuildButtonId("btn_CloseRequest"), "Close Request")
                        );
                await createDiscussionChannel.SendMessageAsync(embedRequest);
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Succesfully created request for {ctx.Interaction.User.Username}"));
            } else
            {
                await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Failed to created request for {ctx.Interaction.User.Username}, try again."));
            }
        }

    }
}
