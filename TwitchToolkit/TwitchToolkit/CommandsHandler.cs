using System;
using System.Linq;
using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace TwitchToolkit;

public static class CommandsHandler
{
	private static DateTime modsCommandCooldown = DateTime.MinValue;

	private static DateTime aliveCommandCooldown = DateTime.MinValue;

	public static void CheckCommand(ITwitchMessage twitchMessage)
	{
		Log.Message("Checking command - " + twitchMessage.Message);
		if (twitchMessage == null || twitchMessage.Message == null)
		{
			return;
		}
		string message = twitchMessage.Message;
		string user = twitchMessage.Username;
		Viewer viewer = Viewers.GetViewer(user);
		viewer.last_seen = DateTime.Now;
		if (viewer.IsBanned)
		{
            Log.Message("viewer is banned.");
            return;
		}
		Command commandDef = DefDatabase<Command>.AllDefs.ToList().Find((Command s) => twitchMessage.Message.StartsWith("!" + s.command));
		if (commandDef != null)
		{
            bool runCommand = true;

            if (commandDef.requiresMod && !viewer.mod && viewer.username.ToLower() != ToolkitSettings.Channel.ToLower())
            {
                Log.Message($"Command '{commandDef.command}' blocked: requiresMod=true, but viewer '{viewer.username}' is not a mod and not the channel owner.");
                runCommand = false;
            }

            if (commandDef.requiresAdmin && twitchMessage.Username.ToLower() != ToolkitSettings.Channel.ToLower())
            {
                Log.Message($"Command '{commandDef.command}' blocked: requiresAdmin=true, but '{twitchMessage.Username}' is not the channel owner.");
                runCommand = false;
            }

            if (!commandDef.enabled)
            {
                Log.Message($"Command '{commandDef.command}' blocked: command is disabled.");
                runCommand = false;
            }

            if (runCommand)
            {
                Log.Message($"Running command '{commandDef.command}' for user '{twitchMessage.Username}'.");
                commandDef.RunCommand(twitchMessage);
            }
            else
            {
                Log.Message($"Command '{commandDef.command}' was not run due to failed permission or disabled flag.");
            }

        }
    }

	public static bool SendToChatroom(ChatMessage msg)
	{
		return true;
	}
}
