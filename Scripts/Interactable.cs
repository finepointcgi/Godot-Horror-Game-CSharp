using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotHorrorGameCSharp.Scripts
{
    public partial class Interactable : Node3D
    {
        public AnimationPlayer player;
        [Export]
        public string HoverText;

        public void PlayAnimation(string animation)
        {
            player.Play(animation);
        }
    }
}
