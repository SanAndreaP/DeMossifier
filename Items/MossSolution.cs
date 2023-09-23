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
    private readonly int? _recpItem;
    private readonly bool _grow;
    private readonly bool _special;

    public MossSolution(bool grow, bool special, int? recipeItem = null) {
        this._recpItem = recipeItem;
        this._grow = grow;
        this._special = special;
    }

    public override void SetStaticDefaults() {
        global::DeMossifier.DeMossifier.RegisterSolution(this.Type, this._grow, this._special);
    }

    public override void SetDefaults() {
        this.Item.width = 12;
        this.Item.height = 18;
        this.Item.value = Item.buyPrice(copper: this._special ? 80 : 40);
        this.Item.rare = ItemRarityID.Orange;
        this.Item.maxStack = 9999;
        this.Item.consumable = true;
        this.Item.ResearchUnlockCount = 100;

        this.Item.shoot = ModContent.ProjectileType<T>();
        this.Item.ammo = ModContent.ItemType<GeneralWiltSolution>();
    }

    public override void AddRecipes() {
        if( this._recpItem == null ) {
            return;
        }

        var recipe = this.CreateRecipe(this._grow ? 1 : 10);
        recipe.AddCondition(new Condition(
            $"Mods.{(global::DeMossifier.DeMossifier.MODNAME)}.Conditions.DryadAndPainter", () => {
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
    public GeneralWiltSolution() : base(false, true) { }

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
    public GreenWiltSolution() : base(false, false, ItemID.GreenMoss) { }
}

public class BrownWiltSolution : MossSolution<Projectiles.BrownWiltSolution>
{
    public BrownWiltSolution() : base(false, false, ItemID.BrownMoss) { }
}

public class RedWiltSolution : MossSolution<Projectiles.RedWiltSolution>
{
    public RedWiltSolution() : base(false, false, ItemID.RedMoss) { }
}

public class BlueWiltSolution : MossSolution<Projectiles.BlueWiltSolution>
{
    public BlueWiltSolution() : base(false, false, ItemID.BlueMoss) { }
}

public class PurpleWiltSolution : MossSolution<Projectiles.PurpleWiltSolution>
{
    public PurpleWiltSolution() : base(false, false, ItemID.PurpleMoss) { }
}

public class LavaWiltSolution : MossSolution<Projectiles.LavaWiltSolution>
{
    public LavaWiltSolution() : base(false, true, ItemID.LavaMoss) { }
}

public class KryptonWiltSolution : MossSolution<Projectiles.KryptonWiltSolution>
{
    public KryptonWiltSolution() : base(false, true, ItemID.KryptonMoss) { }
}

public class XenonWiltSolution : MossSolution<Projectiles.XenonWiltSolution>
{
    public XenonWiltSolution() : base(false, true, ItemID.XenonMoss) { }
}

public class ArgonWiltSolution : MossSolution<Projectiles.ArgonWiltSolution>
{
    public ArgonWiltSolution() : base(false, true, ItemID.ArgonMoss) { }
}

[Autoload(false)]
public class SacchariteWiltSolution : MossSolution<Projectiles.SacchariteWiltSolution>
{
    public static int recipeItemOverride;
    public SacchariteWiltSolution() : base(false, true, recipeItemOverride) { }
}
#endregion

#region Grows
public class GreenGrowSolution : MossSolution<Projectiles.GreenGrowSolution>
{
    public GreenGrowSolution() : base(true, false, ItemID.GreenMoss) { }
}

public class BrownGrowSolution : MossSolution<Projectiles.BrownGrowSolution>
{
    public BrownGrowSolution() : base(true, false, ItemID.BrownMoss) { }
}

public class RedGrowSolution : MossSolution<Projectiles.RedGrowSolution>
{
    public RedGrowSolution() : base(true, false, ItemID.RedMoss) { }
}

public class BlueGrowSolution : MossSolution<Projectiles.BlueGrowSolution>
{
    public BlueGrowSolution() : base(true, false, ItemID.BlueMoss) { }
}

public class PurpleGrowSolution : MossSolution<Projectiles.PurpleGrowSolution>
{
    public PurpleGrowSolution() : base(true, false, ItemID.PurpleMoss) { }
}

public class LavaGrowSolution : MossSolution<Projectiles.LavaGrowSolution>
{
    public LavaGrowSolution() : base(true, true, ItemID.LavaMoss) { }
}

public class KryptonGrowSolution : MossSolution<Projectiles.KryptonGrowSolution>
{
    public KryptonGrowSolution() : base(true, true, ItemID.KryptonMoss) { }
}

public class XenonGrowSolution : MossSolution<Projectiles.XenonGrowSolution>
{
    public XenonGrowSolution() : base(true, true, ItemID.XenonMoss) { }
}

public class ArgonGrowSolution : MossSolution<Projectiles.ArgonGrowSolution>
{
    public ArgonGrowSolution() : base(true, true, ItemID.ArgonMoss) { }
}
#endregion