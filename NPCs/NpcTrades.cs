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
    
    public override void SetupShop(int type, Chest shop, ref int nextSlot) {
        var painter = Main.npc.FirstOrDefault(npc => npc.type == NPCID.Painter);

        if( type != NPCID.Dryad || painter is not { active: true, homeless: false } ) {
            return;
        }

        shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Items.DeMossifier>());

        var special = ((_currWiltSolution ?? -1) < 0 || (_currGrowSolution ?? -1) < 0) && Main.rand.NextBool(100);
        if( _currWiltSolution == null ) {
            var wilts = DeMossifier.GetSolutions(false, special, false);
            _currWiltSolution = wilts.Count > 0 ? wilts[Main.rand.Next(wilts.Count)] : -1;
        }
        if( _currGrowSolution == null ) {
            var grows = DeMossifier.GetSolutions(true, special, false);
            _currGrowSolution = grows.Count > 0 ? grows[Main.rand.Next(grows.Count)] : -1;
        }

        if( _currWiltSolution > 0 ) {
            shop.item[nextSlot++].SetDefaults(_currWiltSolution.Value);
        }
        if( _currGrowSolution > 0 ) {
            shop.item[nextSlot++].SetDefaults(_currGrowSolution.Value);
        }
    }

    public override void LoadData(NPC npc, TagCompound tag) {
        if( npc.type != NPCID.Dryad ) {
            return;
        }
        
        if( tag.ContainsKey("CurrWiltSolution") ) {
            _currWiltSolution = tag.GetInt("CurrWiltSolution");
        } else {
            _currWiltSolution = null;
        }
        if( tag.ContainsKey("CurrGrowSolution") ) {
            _currGrowSolution = tag.GetInt("CurrGrowSolution");
        } else {
            _currGrowSolution = null;
        }
    }

    public override void SaveData(NPC npc, TagCompound tag) {
        if( npc.type != NPCID.Dryad ) {
            return;
        }
        
        if( _currWiltSolution != null ) {
            tag.Set("CurrWiltSolution", _currWiltSolution.Value);
        }
        if( _currGrowSolution != null ) {
            tag.Set("CurrGrowSolution", _currGrowSolution.Value);
        }
    }

    internal static void Reset() {
        _currWiltSolution = -1;
        _currGrowSolution = -1;
    }
}