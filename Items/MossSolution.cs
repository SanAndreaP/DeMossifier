using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

// ReSharper disable PublicConstructorInAbstractClass
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedType.Global

namespace DeMossifier.Items;

public abstract class MossSolution<T> : ModItem where T : ModProjectile
{
    private readonly string _dispName;
    private readonly string _tooltip;
    private readonly short? _recpItem;
    private readonly bool _grow;
    private readonly bool _special;

    public MossSolution(string displayName, string tooltip, bool grow, bool special, short? recipeItem = null) {
        this._dispName = displayName;
        this._tooltip = tooltip;
        this._recpItem = recipeItem;
        this._grow = grow;
        this._special = special;
    }

    public override void SetStaticDefaults() {
        this.DisplayName.SetDefault(this._dispName);
        this.Tooltip.SetDefault("Used by the Demossifier\n" + this._tooltip);
        
        global::DeMossifier.DeMossifier.RegisterSolution(this.Type, this._grow, this._special);
    }

    public override void SetDefaults() {
        this.Item.width = 12;
        this.Item.height = 18;
        this.Item.value = Item.buyPrice(copper: this._special ? 80 : 40);
        this.Item.rare = ItemRarityID.Orange;
        this.Item.maxStack = 999;
        this.Item.consumable = true;

        this.Item.shoot = ModContent.ProjectileType<T>();
        this.Item.ammo = ModContent.ItemType<GeneralWiltSolution>();
    }

    public override void AddRecipes() {
        if( this._recpItem == null ) {
            return;
        }

        var recipe = this.CreateRecipe(this._grow ? 1 : 10);
        recipe.AddCondition(new Recipe.Condition(
            NetworkText.FromLiteral("Only available if you have Dryad and Painter"), _ => {
                var dryad = Main.npc.FirstOrDefault(n => n.type == NPCID.Dryad);
                var painter = Main.npc.FirstOrDefault(n => n.type == NPCID.Painter);

                return dryad is { active: true } && painter is { active: true };
            }));
        recipe.AddIngredient(ItemID.BottledWater);
        recipe.AddIngredient(this._grow ? ItemID.Blinkroot : ItemID.Deathweed);
        recipe.AddIngredient(this._recpItem.Value);
        recipe.AddTile(TileID.AlchemyTable);
        recipe.AddConsumeItemCallback(Recipe.ConsumptionRules.Alchemy);
        recipe.Register();
    }
}

#region Wilts
public class GeneralWiltSolution : MossSolution<Projectiles.GeneralWiltSolution>
{
    public GeneralWiltSolution() : base("Moss Wilt Solution", "Wilts all mosses", false, true) { }

    public override void SetDefaults() {
        base.SetDefaults();

        this.Item.value = 0;
    }

    public override void AddRecipes() {
        var solutions = global::DeMossifier.DeMossifier.GetSolutions(false, withGeneral: false);
        var recipe = this.CreateRecipe(solutions.Count);
        foreach( var id in solutions ) {
            recipe.AddIngredient(id);
        }
        recipe.AddTile(TileID.AlchemyTable);
        recipe.Register();
    }
}

public class GreenWiltSolution : MossSolution<Projectiles.GreenWiltSolution>
{
    public GreenWiltSolution() : base("Green Moss Wilt Solution", "Wilts green moss", false, false, ItemID.GreenMoss) { }
}

public class BrownWiltSolution : MossSolution<Projectiles.BrownWiltSolution>
{
    public BrownWiltSolution() : base("Brown Moss Wilt Solution", "Wilts brown moss", false, false, ItemID.BrownMoss) { }
}

public class RedWiltSolution : MossSolution<Projectiles.RedWiltSolution>
{
    public RedWiltSolution() : base("Red Moss Wilt Solution", "Wilts red moss", false, false, ItemID.RedMoss) { }
}

public class BlueWiltSolution : MossSolution<Projectiles.BlueWiltSolution>
{
    public BlueWiltSolution() : base("Blue Moss Wilt Solution", "Wilts blue moss", false, false, ItemID.BlueMoss) { }
}

public class PurpleWiltSolution : MossSolution<Projectiles.PurpleWiltSolution>
{
    public PurpleWiltSolution() : base("Purple Moss Wilt Solution", "Wilts purple moss", false, false, ItemID.PurpleMoss) { }
}

public class LavaWiltSolution : MossSolution<Projectiles.LavaWiltSolution>
{
    public LavaWiltSolution() : base("Lava Moss Wilt Solution", "Wilts lava moss", false, true, ItemID.LavaMoss) { }
}

public class KryptonWiltSolution : MossSolution<Projectiles.KryptonWiltSolution>
{
    public KryptonWiltSolution() : base("Krypton Moss Wilt Solution", "Wilts krypton moss", false, true, ItemID.KryptonMoss) { }
}

public class XenonWiltSolution : MossSolution<Projectiles.XenonWiltSolution>
{
    public XenonWiltSolution() : base("Xenon Moss Wilt Solution", "Wilts xenon moss", false, true, ItemID.XenonMoss) { }
}

public class ArgonWiltSolution : MossSolution<Projectiles.ArgonWiltSolution>
{
    public ArgonWiltSolution() : base("Argon Moss Wilt Solution", "Wilts argon moss", false, true, ItemID.ArgonMoss) { }
}
#endregion

#region Grows
public class GreenGrowSolution : MossSolution<Projectiles.GreenGrowSolution>
{
    public GreenGrowSolution() : base("Green Moss Grow Solution", "Grows green moss", true, false, ItemID.GreenMoss) { }
}

public class BrownGrowSolution : MossSolution<Projectiles.BrownGrowSolution>
{
    public BrownGrowSolution() : base("Brown Moss Grow Solution", "Grows brown moss", true, false, ItemID.BrownMoss) { }
}

public class RedGrowSolution : MossSolution<Projectiles.RedGrowSolution>
{
    public RedGrowSolution() : base("Red Moss Grow Solution", "Grows red moss", true, false, ItemID.RedMoss) { }
}

public class BlueGrowSolution : MossSolution<Projectiles.BlueGrowSolution>
{
    public BlueGrowSolution() : base("Blue Moss Grow Solution", "Grows blue moss", true, false, ItemID.BlueMoss) { }
}

public class PurpleGrowSolution : MossSolution<Projectiles.PurpleGrowSolution>
{
    public PurpleGrowSolution() : base("Purple Moss Grow Solution", "Grows purple moss", true, false, ItemID.PurpleMoss) { }
}

public class LavaGrowSolution : MossSolution<Projectiles.LavaGrowSolution>
{
    public LavaGrowSolution() : base("Lava Moss Grow Solution", "Grows lava moss", true, true, ItemID.LavaMoss) { }
}

public class KryptonGrowSolution : MossSolution<Projectiles.KryptonGrowSolution>
{
    public KryptonGrowSolution() : base("Krypton Moss Grow Solution", "Grows krypton moss", true, true, ItemID.KryptonMoss) { }
}

public class XenonGrowSolution : MossSolution<Projectiles.XenonGrowSolution>
{
    public XenonGrowSolution() : base("Xenon Moss Grow Solution", "Grows xenon moss", true, true, ItemID.XenonMoss) { }
}

public class ArgonGrowSolution : MossSolution<Projectiles.ArgonGrowSolution>
{
    public ArgonGrowSolution() : base("Argon Moss Grow Solution", "Grows argon moss", true, true, ItemID.ArgonMoss) { }
}
#endregion