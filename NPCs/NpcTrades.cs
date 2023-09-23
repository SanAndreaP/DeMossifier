using System.Linq;
using DeMossifier.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

// ReSharper disable ClassNeverInstantiated.Global

namespace DeMossifier.NPCs;

public class NpcTrades : GlobalNPC
{
    private static int? _currWiltSolution;
    private static int? _currGrowSolution;

    public override void ModifyShop(NPCShop shop) {
        if( _currGrowSolution == null || _currWiltSolution == null ) {
            Reset(false);
        }
        
        switch( shop.NpcType ) {
            case NPCID.Dryad: {
                Condition painterAvailable = new($"Mods.{DeMossifier.MODNAME}.Conditions.PainterAvailable",
                        () => Main.npc.FirstOrDefault(n => n.type == NPCID.Painter) is { active: true, homeless: false });

                shop.Add(ModContent.ItemType<Items.DeMossifier>(), painterAvailable);

                Condition CondWilt(int itemId) => new($"Mods.{DeMossifier.MODNAME}.Conditions.WiltSolutionDryad", () => itemId == _currWiltSolution);
                foreach( var solution in DeMossifier.GetSolutions(growing:false, withGeneral:false, dryadSellableOnly:true) ) {
                    shop.Add(solution, painterAvailable, CondWilt(solution));
                }

                Condition CondGrow(int itemId) => new($"Mods.{DeMossifier.MODNAME}.Conditions.GrowSolutionDryad", () => itemId == _currGrowSolution);
                foreach( var solution in DeMossifier.GetSolutions(growing:true, withGeneral:false, dryadSellableOnly:true) ) {
                    shop.Add(solution, painterAvailable, CondGrow(solution));
                }
            } break;
            case NPCID.Wizard: {
                if( ModLoader.HasMod("TheConfectionRebirth") ) {
                    shop.Add(ModContent.ItemType<SacchariteWiltSolution>());
                }
            } break;
        }
    }

    public override void LoadData(NPC npc, TagCompound tag) {
        if( npc.type != NPCID.Dryad ) {
            return;
        }
        
        if( tag.ContainsKey("CurrWiltSolution2") ) {
            _currWiltSolution = tag.GetInt("CurrWiltSolution2");
        } else {
            _currWiltSolution = null;
        }
        if( tag.ContainsKey("CurrGrowSolution2") ) {
            _currGrowSolution = tag.GetInt("CurrGrowSolution2");
        } else {
            _currGrowSolution = null;
        }
    }

    public override void SaveData(NPC npc, TagCompound tag) {
        if( npc.type != NPCID.Dryad ) {
            return;
        }
        
        if( _currWiltSolution != null ) {
            tag.Set("CurrWiltSolution2", _currWiltSolution.Value);
        }
        if( _currGrowSolution != null ) {
            tag.Set("CurrGrowSolution2", _currGrowSolution.Value);
        }
    }

    internal static void Reset(bool overwrite = true) {
        var special = Main.rand.NextBool(100);
        if( overwrite || _currWiltSolution == null ) {
            var wilts = DeMossifier.GetSolutions(false, special, false, true);
            _currWiltSolution = wilts.Count > 0 ? wilts[Main.rand.Next(wilts.Count)] : 0;
        }
        if( overwrite || _currGrowSolution == null ) {
            var grows = DeMossifier.GetSolutions(true, special, false, true);
            _currGrowSolution = grows.Count > 0 ? grows[Main.rand.Next(grows.Count)] : 0;
        }
    }
}