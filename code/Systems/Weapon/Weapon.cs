using Sandbox.UI;
using Sandbox;
using static Sandmbit.Weapons.PrimaryFire;

namespace Sandmbit.Weapons;

[Prefab, Title( "Weapon" ), Icon( "track_changes" )]
public partial class Weapon : AnimatedEntity
{
	protected PrimaryFire PrimaryFireData => GetComponent<PrimaryFire>();

	// Won't be Net eventually, when we serialize prefabs on client
	[Net, Prefab, Category( "Animation" )] public WeaponHoldType HoldType { get; set; } = WeaponHoldType.Pistol;
	[Net, Prefab, Category( "Animation" )] public WeaponHandedness Handedness { get; set; } = WeaponHandedness.Both;

	[Net, Prefab, Category( "General" )] public WeaponType WeaponType { get; set; }
	[Net, Prefab, Category( "General" )] public AmmoType AmmoType { get; set; }
	[Net, Prefab, Category( "General" )] public int Ammo { get; set; }
	[Net, Prefab, Category( "General" )] public int ReserveAmmo { get; set; }
	[Net, Prefab, Category( "General" ), ResourceType("png")] public string IconPath { get; set; }

	public AnimatedEntity EffectEntity => ViewModelEntity.IsValid() ? ViewModelEntity : this;
	public WeaponViewModel ViewModelEntity { get; protected set; }
	public Pawn Player => Owner as Pawn;

	[Net] public int MaxAmmo { get; set; }

	public override void Spawn()
	{
		MaxAmmo = Ammo;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;
		EnableDrawing = false;
	}

	/// <summary>
	/// Can we holster the weapon right now? Reasons to reject this could be that we're reloading the weapon..
	/// </summary>
	/// <returns></returns>
	public bool CanHolster( Pawn player )
	{
		return true;
	}

	/// <summary>
	/// Called when the weapon gets holstered.
	/// </summary>
	public void OnHolster( Pawn player )
	{
		EnableDrawing = false;
		
		if ( Game.IsServer )
			DestroyViewModel( To.Single( player ) );
	}

	/// <summary>
	/// Can we deploy this weapon? Reasons to reject this could be that we're performing an action.
	/// </summary>
	/// <returns></returns>
	public bool CanDeploy( Pawn player )
	{
		return true;
	}

	/// <summary>
	/// Called when the weapon gets deployed.
	/// </summary>
	public void OnDeploy( Pawn player )
	{
		SetParent( player, true );
		Owner = player;

		EnableDrawing = true;

		if ( Game.IsServer )
			CreateViewModel( To.Single( player ) );
	}

	[ClientRpc]
	public void CreateViewModel()
	{
		if ( GetComponent<ViewModelComponent>() is not ViewModelComponent comp ) return;

		var vm = new WeaponViewModel( this );
		vm.Model = Model.Load( comp.ViewModelPath );

		if( comp.ViewModelHandsPath != null)
		{
			var arms = new WeaponViewModel( this, true );
			arms.Model = Model.Load( comp.ViewModelHandsPath );
			arms.SetParent( vm, true );
		}
		
		ViewModelEntity = vm;
		ViewModelEntity.SetAnimParameter( "deploy", true );
	}

	[ClientRpc]
	public void DestroyViewModel()
	{
		if ( ViewModelEntity.IsValid() )
		{
			ViewModelEntity.Delete();
		}
	}

	public override void Simulate( IClient cl )
	{
		SimulateComponents( cl );
	}

	protected override void OnDestroy()
	{
		ViewModelEntity?.Delete();
	}

	public override string ToString()
	{
		return $"Weapon ({Name})";
	}

	public string Type()
	{
		return WeaponType.GetAttributeOfType<ClassNameAttribute>().Value;
	}
}

/// <summary>
/// Describes the holdtype of a weapon, which tells our animgraph which animations to use.
/// </summary>
public enum WeaponHoldType
{
	None,
	Pistol,
	Rifle,
	Shotgun,
	Item,
	Fists,
	Swing
}

/// <summary>
/// Describes the handedness of a weapon, which hand (or both) we hold the weapon in.
/// </summary>
public enum WeaponHandedness
{
	Both,
	Right,
	Left
}

/// <summary>
/// Describes what type of ammo this weapon uses.
/// </summary>
public enum AmmoType
{
	Primary,
	Special,
	Heavy
}

/// <summary>
/// Describes what type of weapon this is.
/// </summary>
public enum WeaponType
{
	[ClassName("Auto Rifle")]
	AutoRifle,
	[ClassName( "Combat Bow" )]
	CombatBow,
	[ClassName( "Fusion Rifle" )]
	FusionRifle,
	[ClassName( "Glaive" )]
	Glaive,
	[ClassName( "Grenade Launcher" )]
	GrenadeLauncher,
	[ClassName( "Hand Cannon" )]
	HandCannon,
	[ClassName( "Linear Fusion Rifle" )]
	LinearFusionRifle,
	[ClassName( "Machine Gun" )]
	MachineGun,
	[ClassName( "Pulse Rifle" )]
	PulseRifle,
	[ClassName( "Rocket Launcher" )]
	RocketLauncher,
	[ClassName( "Scout Rifle" )]
	ScoutRifle,
	[ClassName( "Shotgun" )]
	Shotgun,
	[ClassName( "Sidearm" )]
	Sidearm,
	[ClassName( "Sniper Rifle" )]
	SniperRifle,
	[ClassName( "Submachine Gun" )]
	SubmachineGun,
	[ClassName( "Sword" )]
	Sword,
	[ClassName( "Trace Rifle" )]
	TraceRifle
}

