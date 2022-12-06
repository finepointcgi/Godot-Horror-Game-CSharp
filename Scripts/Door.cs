using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotHorrorGameCSharp.Scripts
{
    public partial class Door : Interactable
    {
        public override void _Ready()
        {
            base._Ready();
            player = GetNode<AnimationPlayer>("AnimationPlayer");
        }
    }
}
