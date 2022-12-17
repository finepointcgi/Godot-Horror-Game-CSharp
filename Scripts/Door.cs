using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotHorrorGameCSharp.Scripts
{
    public partial class Door : Node3D, Interactable
    {
        [Export]
        public string HoverOverText;
        private bool DoorOpen;
        private AnimationPlayer player;
        public override void _Ready()
        {
            base._Ready();
            player = GetNode<AnimationPlayer>("AnimationPlayer");
        }
        public string GetInterfaceText()
        {
            return HoverOverText;
        }

        public double GetSound()
        {
            throw new NotImplementedException();
        }

        public void Interact()
        {
            if (DoorOpen)
                player.Play("Close");
            else
                player.Play("Open");
            DoorOpen = !DoorOpen;
        }
    }
}
