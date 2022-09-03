using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;

// ReSharper disable ArrangeRedundantParentheses
// ReSharper disable ClassNeverInstantiated.Global

namespace DeMossifier;

public class DeMossifier : Mod
{
    private static readonly List<ushort> REGISTERED_MOSSES = new();
    private static readonly List<ushort> REGISTERED_MOSS_BRICKS = new();
    private static readonly List<SolutionDef> REGISTERED_SOLUTIONS = new();

    public static void RegisterSolution(int id, bool doesGrow, bool isSpecial) {
        REGISTERED_SOLUTIONS.Add(new SolutionDef(id, doesGrow, isSpecial));
    }

    public static void RegisterMoss(ushort mossId, ushort mossBrickId) {
        REGISTERED_MOSSES.Add(mossId);
        REGISTERED_MOSS_BRICKS.Add(mossBrickId);
    }

    public static bool CanDeMossStone(ushort? currMossId, ushort tileId) {
        return (currMossId == null && REGISTERED_MOSSES.Contains(tileId)) || currMossId == tileId;
    }

    public static bool CanDeMossBrick(ushort? currMossId, ushort tileId) {
        return (currMossId == null && REGISTERED_MOSS_BRICKS.Contains(tileId)) || currMossId == tileId;
    }

    public static List<int> GetSolutions(bool? growingOnly = null, bool? specialOnly = null, bool withGeneral = true) {
        var generalWilt = !withGeneral && !(growingOnly ?? false) ? ModContent.GetInstance<Items.GeneralWiltSolution>().Type : -1;

        return (from s in REGISTERED_SOLUTIONS
                    where (growingOnly == null || (growingOnly.Value == s.doesGrow))
                          && (specialOnly == null || (specialOnly.Value == s.isSpecial))
                          && (withGeneral || (s.id != generalWilt))
                    select s.id).ToList();
    }

    private static void OnStartDay(On.Terraria.Main.orig_UpdateTime_StartDay orig, ref bool stopEvents) {
        NPCs.NpcTrades.Reset();
        
        orig(ref stopEvents);
    }

    public override void Load() {
        On.Terraria.Main.UpdateTime_StartDay += OnStartDay;
    }

    public override void Unload() {
        REGISTERED_SOLUTIONS.Clear();
        REGISTERED_MOSSES.Clear();
        REGISTERED_MOSS_BRICKS.Clear();
        
        On.Terraria.Main.UpdateTime_StartDay -= OnStartDay;
    }

    private sealed class SolutionDef
    {
        internal readonly int id;
        internal readonly bool doesGrow;
        internal readonly bool isSpecial;
        
        internal SolutionDef(int id, bool doesGrow, bool isSpecial) {
            this.id = id;
            this.doesGrow = doesGrow;
            this.isSpecial = isSpecial;
        }
    }
}