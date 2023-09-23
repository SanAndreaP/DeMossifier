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
        var clr = this.GetLightColor(Math.Min(dust.scale * 0.1F, 1.0F)).ToVector3();
        
        Lighting.AddLight((int) (dust.position.X / 16.0), (int) (dust.position.Y / 16.0), clr.X, clr.Y, clr.Z);

        return true;
    }

    internal virtual Color GetLightColor(float brightness) {
        return new Color(brightness * this._color[0], brightness * this._color[1], brightness * this._color[2]);
    }
}

public class BrownMossDust : MossDust { public BrownMossDust() : base(Color.Brown) { } }

public class GeneralMossDust : MossDust { public GeneralMossDust() : base(Color.LightGray) { } }

public class RainbowMossDust : MossDust
{
    public RainbowMossDust() : base(Color.LightGray) { }

    public override Color? GetAlpha(Dust dust, Color lightColor) {
        return Main.DiscoColor;
    }

    internal override Color GetLightColor(float brightness) {
        return Main.DiscoColor;
    }
}