using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

// ReSharper disable ClassNeverInstantiated.Global

namespace DeMossifier.Items;

public class DeMossifier : ModItem
{
	public override void SetStaticDefaults()
	{
		this.DisplayName.SetDefault("Demossifier");
		this.Tooltip.SetDefault("Grows and wilts moss on stone blocks when sprayed\nUses moss solutions");
	}

	public override void SetDefaults()
	{
		this.Item.width = 50;
		this.Item.height = 24;
		this.Item.consumeAmmoOnLastShotOnly = true;
		this.Item.useStyle = ItemUseStyleID.Shoot;
		this.Item.autoReuse = true;
		this.Item.useAnimation = 30;
		this.Item.useTime = 5;
		this.Item.UseSound = SoundID.Item34;
		this.Item.knockBack = 0.3F;
		this.Item.shootSpeed = 7.0F;
		this.Item.noMelee = true;
		this.Item.value = Item.buyPrice(gold: 50);
		this.Item.rare = ItemRarityID.Pink;
		this.Item.shoot = ModContent.ProjectileType<Projectiles.GeneralWiltSolution>();
		this.Item.useAmmo = ModContent.ItemType<GeneralWiltSolution>();
	}

	public override void UseStyle(Player player, Rectangle heldItemFrame) {
		var rotVec = player.itemRotation.ToRotationVector2();
		var offsetX = rotVec.X * 13.0F * -player.direction;
		var offsetY = rotVec.Y * 13.0F * -player.direction;
		
		player.itemLocation.X = player.position.X + player.width * 0.5F - heldItemFrame.Width * 0.5F - player.direction * 2 + offsetX;
		player.itemLocation.Y = player.MountedCenter.Y - heldItemFrame.Height * 0.5F + offsetY;
	}
}