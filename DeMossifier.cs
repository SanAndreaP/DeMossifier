using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

// ReSharper disable ArrangeRedundantParentheses
// ReSharper disable ClassNeverInstantiated.Global

namespace DeMossifier;

public class DeMossifier : Mod
{
    public static readonly string MODNAME = "DeMossifier";
    
    private static readonly List<ushort> REGISTERED_MOSSES = new();
    private static readonly List<ushort> REGISTERED_MOSS_BRICKS = new();
    private static readonly List<SolutionDef> REGISTERED_SOLUTIONS = new();

    public static void RegisterSolution(int id, bool doesGrow, bool isSpecial, bool isDryadSellable) {
        REGISTERED_SOLUTIONS.Add(new SolutionDef(id, doesGrow, isSpecial, isDryadSellable));
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

    public static List<int> GetSolutions(bool? growing = null, bool? special = null, bool withGeneral = true, bool dryadSellableOnly = false) {
        var generalWilt = !withGeneral && !(growing ?? false) ? ModContent.GetInstance<Items.GeneralWiltSolution>().Type : -1;

        return (from s in REGISTERED_SOLUTIONS
                    where (growing == null || (growing.Value == s.doesGrow))
                          && (special == null || (special.Value == s.isSpecial))
                          && (withGeneral || (s.id != generalWilt))
                          && (!dryadSellableOnly || (s.isDryadSellable))
                    select s.id).ToList();
    }

    public override void Load() {
        if( ModLoader.TryGetMod("TheConfectionRebirth", out var confection) ) {
            var sacchariteId = confection.Find<ModTile>("SacchariteBlock").Type;
            Items.SacchariteWiltSolution.recipeItemOverride = sacchariteId;
            Projectiles.SacchariteWiltSolution.replaceTileOverride = sacchariteId;
            
            this.AddContent<Projectiles.SacchariteWiltSolution>();
            this.AddContent<Items.SacchariteWiltSolution>();
        }
    }

    public override void Unload() {
        REGISTERED_SOLUTIONS.Clear();
        REGISTERED_MOSSES.Clear();
        REGISTERED_MOSS_BRICKS.Clear();
    }

    private sealed class SolutionDef
    {
        internal readonly int id;
        internal readonly bool doesGrow;
        internal readonly bool isSpecial;
        internal readonly bool isDryadSellable;
        
        internal SolutionDef(int id, bool doesGrow, bool isSpecial, bool isDryadSellable) {
            this.id = id;
            this.doesGrow = doesGrow;
            this.isSpecial = isSpecial;
            this.isDryadSellable = isDryadSellable;
        }
    }
    
    // ReSharper disable once UnusedType.Global
    public sealed class UpdateTimeSystem : ModSystem
    {
        public override void PostUpdateWorld() {
            if( Main.time < Main.dayRate && Main.dayTime ) {
                NPCs.NpcTrades.Reset();
            }
        }
    }
}