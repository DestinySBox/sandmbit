using Sandbox;

namespace Sandmbit.Weapons;

public partial class Weapon
{
	public T GetComponent<T>() where T : WeaponComponent
	{
		return Components.Get<T>( false );
	}

	protected void SimulateComponents( IClient cl )
	{
		var player = Owner as Pawn;
		foreach ( var component in Components.GetAll<WeaponComponent>() )
		{
			component.Simulate( cl, player );
		}
	}

	public void RunGameEvent( string eventName )
	{
		Player?.RunGameEvent( eventName );
	}

	public override void BuildInput()
	{
		foreach( var component in Components.GetAll<WeaponComponent>() )
		{
			component.BuildInput();
		}
	}
}
