using ToolkitCore;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkitDev;

namespace TwitchToolkit.Commands.ModCommands;

public class RefreshViewers : CommandDriver
{
	public override void RunCommand(ITwitchMessage twitchMessage)
	{
               TwitchWrapper.SendChatMessage("This command is deprecated.");
       }
}
