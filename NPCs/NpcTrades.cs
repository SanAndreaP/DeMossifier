using System.Linq;
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

    private static Item GetEmptyItem(Item[] items) {
        foreach( var item in items ) {
            if( item.IsAir ) {
                return item;
            }
        }

        return null;
    }
    
    public override void ModifyActiveShop(NPC npc, string shopName, Item[] items) {
        switch( npc.type ) {
            case NPCID.Dryad: {
                var painter = Main.npc.FirstOrDefault(n => n.type == NPCID.Painter);

                if( painter is not { active: true, homeless: false } ) {
                    return;
                }

                GetEmptyItem(items)?.SetDefaults(ModContent.ItemType<Items.DeMossifier>());

                var special = ((_currWiltSolution ?? 0) != 0 || (_currGrowSolution ?? 0) != 0) && Main.rand.NextBool(100);
                if( _currWiltSolution == null ) {
                    var wilts = DeMossifier.GetSolutions(false, special, false);
                    _currWiltSolution = wilts.Count > 0 ? wilts[Main.rand.Next(wilts.Count)] : 0;
                }
                if( _currGrowSolution == null ) {
                    var grows = DeMossifier.GetSolutions(true, special, false);
                    _currGrowSolution = grows.Count > 0 ? grows[Main.rand.Next(grows.Count)] : 0;
                }

                if( _currWiltSolution != 0 ) {
                    GetEmptyItem(items)?.SetDefaults(_currWiltSolution.Value);
                }
                if( _currGrowSolution != 0 ) {
                    GetEmptyItem(items)?.SetDefaults(_currGrowSolution.Value);
                }
            } break;
            case NPCID.Wizard: {
                if( ModLoader.HasMod("TheConfectionRebirth") ) {
                    GetEmptyItem(items)?.SetDefaults(ModContent.ItemType<Items.SacchariteWiltSolution>());
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

    internal static void Reset() {
        _currWiltSolution = null;
        _currGrowSolution = null;
    }
}