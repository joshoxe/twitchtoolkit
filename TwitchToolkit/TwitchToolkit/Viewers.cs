using System;
using System.Collections.Generic;
using System.Linq;
using ToolkitCore;
using TwitchToolkit.Store;
using TwitchToolkit.Utilities;
using Verse;

namespace TwitchToolkit;

public static class Viewers {
    public static List<Viewer> All = new List<Viewer>();

    public static void AwardViewersCoins(int setamount = 0)
    {
        List<string> usernames = GetActiveViewerUsernames();
        if (usernames == null)
        {
            return;
        }

        foreach (string username in usernames)
        {
            Viewer viewer = GetViewer(username);
            if (viewer.IsBanned)
                continue;

            if (setamount > 0)
            {
                viewer.GiveViewerCoins(setamount);
                continue;
            }

            int baseCoins = ToolkitSettings.CoinAmount;
            float baseMultiplier = (float)viewer.GetViewerKarma() / 100f;

            if (viewer.IsSub)
            {
                baseCoins += ToolkitSettings.SubscriberExtraCoins;
                baseMultiplier *= ToolkitSettings.SubscriberCoinMultiplier;
            }
            else if (viewer.IsVIP)
            {
                baseCoins += ToolkitSettings.VIPExtraCoins;
                baseMultiplier *= ToolkitSettings.VIPCoinMultiplier;
            }
            else if (viewer.mod)
            {
                baseCoins += ToolkitSettings.ModExtraCoins;
                baseMultiplier *= ToolkitSettings.ModCoinMultiplier;
            }

            int minutesSinceActive = TimeHelper.MinutesElapsed(viewer.last_seen);
            if (ToolkitSettings.ChatReqsForCoins)
            {
                if (minutesSinceActive > ToolkitSettings.TimeBeforeHalfCoins)
                    baseMultiplier *= 0.5f;
                if (minutesSinceActive > ToolkitSettings.TimeBeforeNoCoins)
                    baseMultiplier *= 0f;
            }

            double coinsToReward = baseCoins * baseMultiplier;
            Store_Logger.LogString($"{viewer.username} gets {baseCoins} * {baseMultiplier} coins, total {(int)Math.Ceiling(coinsToReward)}");
            viewer.GiveViewerCoins((int)Math.Ceiling(coinsToReward));
        }
    }

    public static void GiveAllViewersCoins(int amount, List<Viewer> viewers = null)
    {
        var targetList = viewers ?? All;
        foreach (Viewer viewer in targetList)
        {
            if (viewer.GetViewerKarma() > 1)
                viewer.GiveViewerCoins(amount);
        }
    }

    public static void SetAllViewersCoins(int amount, List<Viewer> viewers = null)
    {
        var targetList = viewers ?? All;
        foreach (Viewer viewer in targetList)
        {
            viewer?.SetViewerCoins(amount);
        }
    }

    public static void GiveAllViewersKarma(int amount, List<Viewer> viewers = null)
    {
        var targetList = viewers ?? All;
        foreach (Viewer viewer in targetList)
        {
            if (viewer.GetViewerKarma() > 1)
                viewer.SetViewerKarma(Math.Min(ToolkitSettings.KarmaCap, viewer.GetViewerKarma() + amount));
        }
    }

    public static void TakeAllViewersKarma(int amount, List<Viewer> viewers = null)
    {
        var targetList = viewers ?? All;
        foreach (Viewer viewer in targetList)
        {
            viewer?.SetViewerKarma(Math.Max(0, viewer.GetViewerKarma() - amount));
        }
    }

    public static void SetAllViewersKarma(int amount, List<Viewer> viewers = null)
    {
        var targetList = viewers ?? All;
        foreach (Viewer viewer in targetList)
        {
            viewer?.SetViewerKarma(amount);
        }
    }

    public static List<string> GetActiveViewerUsernames()
    {
        return All
            .Where(v => TimeHelper.MinutesElapsed(v.last_seen) <= ToolkitSettings.TimeBeforeHalfCoins)
            .Select(v => v.username)
            .ToList();
    }

    public static void ResetViewers()
    {
        All = new List<Viewer>();
    }

    public static Viewer GetViewer(string user)
    {
        var lowerUser = user.ToLower();
        Viewer viewer = All.Find(x => x.username == lowerUser);
        if (viewer == null)
        {
            viewer = new Viewer(lowerUser)
            {
                karma = ToolkitSettings.StartingKarma
            };
            viewer.SetViewerCoins(ToolkitSettings.StartingBalance);
            All.Add(viewer);
        }
        return viewer;
    }

    public static Viewer GetViewerById(int id)
    {
        return All.Find(s => s.id == id);
    }

    public static void ResetViewersCoins()
    {
        foreach (Viewer viewer in All)
        {
            viewer.coins = ToolkitSettings.StartingBalance;
        }
    }

    public static void ResetViewersKarma()
    {
        foreach (Viewer viewer in All)
        {
            viewer.karma = ToolkitSettings.StartingKarma;
        }
    }
}
