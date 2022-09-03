using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

// ReSharper disable ClassNeverInstantiated.Global

namespace DeMossifier.Dusts;

public abstract class MossDust : ModDust
{
    private readonly float[] _color;
    
    protected MossDust(Color color) {
        this._color = new [] {
            color.R / 255.0F, color.G / 255.0F, color.B / 255.0F
        };
    }

    public override void OnSpawn(Dust dust) {
        dust.noLight = true;
    }

    public override Color? GetAlpha(Dust dust, Color lightColor) {
        return Color.White;
    }

    public override bool Update(Dust dust) {
        var g = Math.Min(dust.scale * 0.1F, 1.0F);
        
        Lighting.AddLight((int) (dust.position.X / 16.0), (int) (dust.position.Y / 16.0),
                          g * this._color[0], g * this._color[1], g * this._color[2]);

        return true;
    }
}

public class BrownMossDust : MossDust { public BrownMossDust() : base(Color.Brown) { } }

public class GeneralMossDust : MossDust { public GeneralMossDust() : base(Color.LightGray) { } }