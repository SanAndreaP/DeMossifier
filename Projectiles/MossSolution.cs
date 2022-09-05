using DeMossifier.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable PublicConstructorInAbstractClass
// ReSharper disable ClassNeverInstantiated.Global

namespace DeMossifier.Projectiles;

public abstract class MossSolution : ModProjectile
{
    public override string Texture => "DeMossifier:Projectiles/EmptyProjectile";

    private readonly ushort? _mossTypeId;
    private readonly ushort? _stoneTypeId;
    private readonly ushort? _mossBrickTypeId;
    private readonly ushort? _brickTypeId;
    private readonly bool _grow;
    private readonly int _dustTypeId;

    public MossSolution(bool grow, int dustTypeId,
                        ushort? stoneTypeId = TileID.Stone, ushort? brickTypeId = TileID.GrayBrick)
    {
        this._grow = grow;
        this._dustTypeId = dustTypeId;
        
        this._stoneTypeId = stoneTypeId;
        this._brickTypeId = brickTypeId;
    }

    public MossSolution(ushort mossTypeId, ushort mossBrickTypeId, bool grow, int dustTypeId,
                        ushort? stoneTypeId = TileID.Stone, ushort? brickTypeId = TileID.GrayBrick)
            : this(grow, dustTypeId, stoneTypeId, brickTypeId)
    {
        this._mossTypeId = mossTypeId;
        this._mossBrickTypeId = mossBrickTypeId;
        
        DeMossifier.RegisterMoss(mossTypeId, mossBrickTypeId);
    }
    
    public override void SetStaticDefaults() {
        this.DisplayName.SetDefault("DeMossifier Projectile");
    }

    public override void SetDefaults() {
        this.Projectile.aiStyle = 0;
        this.Projectile.friendly = true;
        this.Projectile.alpha = byte.MaxValue;
        this.Projectile.penetrate = -1;
        this.Projectile.extraUpdates = 2;
        this.Projectile.tileCollide = false;
        this.Projectile.ignoreWater = true;

        this.Projectile.width = 6;
        this.Projectile.height = 6;
    }

    public override bool? CanCutTiles() {
        return false;
    }

    public override void AI() {
        if( this.Projectile.owner == Main.myPlayer ) {
            if( this._grow ) {
                this.GrowMoss((int)(this.Projectile.position.X + this.Projectile.width / 2.0F) / 16,
                              (int)(this.Projectile.position.Y + this.Projectile.height / 2.0F) / 16);
            } else {
                this.WiltMoss((int)(this.Projectile.position.X + this.Projectile.width / 2.0F) / 16,
                              (int)(this.Projectile.position.Y + this.Projectile.height / 2.0F) / 16);
            }
        }

        if( this.Projectile.timeLeft > 66 ) {
            this.Projectile.timeLeft = 66;
        }

        if( this.Projectile.ai[0] > 7.0F ) {
            var dustId = Dust.NewDust(new Vector2(this.Projectile.position.X, this.Projectile.position.Y), this.Projectile.width, this.Projectile.height, this._dustTypeId, this.Projectile.velocity.X * 0.2f, this.Projectile.velocity.Y * 0.2f, 100);

            var dust = Main.dust[dustId];
            dust.noGravity = true;
            dust.scale *= 1.75f;
            dust.velocity.X *= 2.0F;
            dust.velocity.Y *= 2.0F;
            dust.scale *= (int)this.Projectile.ai[0] switch {
                8 => 0.2F,
                9 => 0.4F,
                10 => 0.6F,
                11 => 0.8F,
                _ => 1.0F
            };
        }

        this.Projectile.ai[0] += 1f;
        this.Projectile.rotation += 0.3f * this.Projectile.direction;
    }
    
    protected void WiltMoss(int x, int y, int size = 2) {
        for( int currX = x - size, maxX = x + size; currX <= maxX; currX++ ) {
            for( int currY = y - size, maxY = y + size; currY <= maxY; currY++ ) {
                var currTile = Main.tile[currX, currY];

                if( DeMossifier.CanDeMossStone(this._mossTypeId, currTile.TileType) ) {
                    if( this._stoneTypeId != null ) {
                        currTile.TileType = this._stoneTypeId.Value;
                    } else {
                        currTile.ClearTile();
                    }
                } else if( DeMossifier.CanDeMossBrick(this._mossBrickTypeId, currTile.TileType) ) {
                    if( this._brickTypeId != null ) {
                        currTile.TileType = this._brickTypeId.Value;
                    } else {
                        currTile.ClearTile();
                    }
                } else {
                    continue;
                }
                
                WorldGen.SquareTileFrame(currX, currY);
                NetMessage.SendTileSquare(-1, currX, currY);
            }
        }
    }
    
    protected void GrowMoss(int x, int y, int size = 2) {
        for( int currX = x - size, maxX = x + size; currX <= maxX; currX++ ) {
            for( int currY = y - size, maxY = y + size; currY <= maxY; currY++ ) {
                var currTile = Main.tile[currX, currY];

                if( this._mossTypeId != null && currTile.TileType == this._stoneTypeId ) {
                    CheckAndGrowMossTile(currX, currY, this._mossTypeId.Value, currTile);
                } else if( this._mossBrickTypeId != null && currTile.TileType == this._brickTypeId ) {
                    CheckAndGrowMossTile(currX, currY, this._mossBrickTypeId.Value, currTile);
                }
            }
        }
    }

    protected static void CheckAndGrowMossTile(int x, int y, ushort mossTileId, Tile currTile) {
        for( var stoneX = -1; stoneX <= 1; stoneX++ ) {
            for( var stoneY = -1; stoneY <= 1; stoneY++ ) {
                var tileX = x + stoneX;
                var tileY = y + stoneY;
                if( Main.tile[tileX, tileY].HasTile && !Main.tileLighted[currTile.TileType] ) {
                    continue;
                }

                currTile.TileType = mossTileId;
                    
                WorldGen.SquareTileFrame(x, y);
                NetMessage.SendTileSquare(-1, x, y);

                return;
            }
        }
    }
}

#region Wilts
public class GeneralWiltSolution : MossSolution
{
    public GeneralWiltSolution() : base(false, ModContent.DustType<GeneralMossDust>()) { }
}

public class GreenWiltSolution : MossSolution
{
    public GreenWiltSolution() : base(TileID.GreenMoss, TileID.GreenMossBrick, false, DustID.Clentaminator_Green) { }
}

public class BrownWiltSolution : MossSolution
{
    public BrownWiltSolution() : base(TileID.BrownMoss, TileID.BrownMossBrick, false, ModContent.DustType<BrownMossDust>()) { }
}

public class RedWiltSolution : MossSolution
{
    public RedWiltSolution() : base(TileID.RedMoss, TileID.RedMossBrick, false, DustID.Clentaminator_Red) { }
}

public class BlueWiltSolution : MossSolution
{
    public BlueWiltSolution() : base(TileID.BlueMoss, TileID.BlueMossBrick, false, DustID.Clentaminator_Blue) { }
}

public class PurpleWiltSolution : MossSolution
{
    public PurpleWiltSolution() : base(TileID.PurpleMoss, TileID.PurpleMossBrick, false, DustID.Clentaminator_Purple) { }
}

public class LavaWiltSolution : MossSolution
{
    public LavaWiltSolution() : base(TileID.LavaMoss, TileID.LavaMossBrick, false, DustID.Clentaminator_Red) { }
}

public class KryptonWiltSolution : MossSolution
{
    public KryptonWiltSolution() : base(TileID.KryptonMoss, TileID.KryptonMossBrick, false, DustID.Clentaminator_Green) { }
}

public class XenonWiltSolution : MossSolution
{
    public XenonWiltSolution() : base(TileID.XenonMoss, TileID.XenonMossBrick, false, DustID.Clentaminator_Cyan) { }
}

public class ArgonWiltSolution : MossSolution
{
    public ArgonWiltSolution() : base(TileID.ArgonMoss, TileID.ArgonMossBrick, false, DustID.Clentaminator_Purple) { }
}

//TheConfection
[Autoload(false)]
public class SacchariteWiltSolution : MossSolution
{
    public static ushort replaceTileOverride;
    
    public SacchariteWiltSolution() : base(replaceTileOverride, replaceTileOverride, false, ModContent.DustType<BrownMossDust>(), stoneTypeId: null, brickTypeId: null) { }
}
#endregion

#region Grows
public class GreenGrowSolution : MossSolution
{
    public GreenGrowSolution() : base(TileID.GreenMoss, TileID.GreenMossBrick, true, DustID.Clentaminator_Green) { }
}

public class BrownGrowSolution : MossSolution
{
    public BrownGrowSolution() : base(TileID.BrownMoss, TileID.BrownMossBrick, true, ModContent.DustType<BrownMossDust>()) { }
}

public class RedGrowSolution : MossSolution
{
    public RedGrowSolution() : base(TileID.RedMoss, TileID.RedMossBrick, true, DustID.Clentaminator_Red) { }
}

public class BlueGrowSolution : MossSolution
{
    public BlueGrowSolution() : base(TileID.BlueMoss, TileID.BlueMossBrick, true, DustID.Clentaminator_Blue) { }
}

public class PurpleGrowSolution : MossSolution
{
    public PurpleGrowSolution() : base(TileID.PurpleMoss, TileID.PurpleMossBrick, true, DustID.Clentaminator_Purple) { }
}

public class LavaGrowSolution : MossSolution
{
    public LavaGrowSolution() : base(TileID.LavaMoss, TileID.LavaMossBrick, true, DustID.Clentaminator_Red) { }
}

public class KryptonGrowSolution : MossSolution
{
    public KryptonGrowSolution() : base(TileID.KryptonMoss, TileID.KryptonMossBrick, true, DustID.Clentaminator_Green) { }
}

public class XenonGrowSolution : MossSolution
{
    public XenonGrowSolution() : base(TileID.XenonMoss, TileID.XenonMossBrick, true, DustID.Clentaminator_Cyan) { }
}

public class ArgonGrowSolution : MossSolution
{
    public ArgonGrowSolution() : base(TileID.ArgonMoss, TileID.ArgonMossBrick, true, DustID.Clentaminator_Purple) { }
}
#endregion