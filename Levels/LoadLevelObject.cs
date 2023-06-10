using Godot;
using GodotHorrorGameCSharp.Scripts;
using System;
using System.Security.Cryptography.X509Certificates;

public partial class LoadLevelObject : StaticBody3D, Interactable
{
    [Export]
    public string HoverOverText;
    [Export]
    public string LevelToLoad;
    [Export]
    public int Index = 0;
    [Export]
    public int TargetIndex = 0;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        GetNode<SpawnIndex>("SpawnIndex").Index = Index;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public string GetInterfaceText()
    {
        return HoverOverText;
    }

    public void Interact()
    {
        GameManager.Instance.LoadLevel(LevelToLoad, TargetIndex);
    }

    public double GetSound()
    {
        throw new NotImplementedException();
    }
}
