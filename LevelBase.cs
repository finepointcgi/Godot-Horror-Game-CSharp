using Godot;
using System;

public partial class LevelBase : Node
{
    public override void _Ready()
    {
        base._Ready();
        GameManager.Instance.LevelBase = this;

    }
}
