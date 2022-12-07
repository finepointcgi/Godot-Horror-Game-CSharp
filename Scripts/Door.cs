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
        private AnimationPlayer player;
        private bool DoorOpen;
        public string GetInterfaceText()
        {
            return HoverOverText;
        }

        public void Interact()
        {
            PlayAnimation("Open");
        }

        public void PlayAnimation(string animation)
        {
            if (DoorOpen) 
                player.Play("Close");
            else
                player.Play("Open");
            DoorOpen = !DoorOpen;
        }

        public override void _Ready()
        {
            base._Ready();
            player = GetNode<AnimationPlayer>("AnimationPlayer");
        }
    }
}
