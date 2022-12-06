using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotHorrorGameCSharp.Scripts
{
    public partial class InterfaceManager : Control
    {
        public static InterfaceManager Manager;

        public override void _Ready()
        {
            base._Ready();
            Manager = this;
        }
        public void ShowInteractionInterface(string words)
        {
            GetNode<Label>("%InteractInterfaceLabel").Text = words;
        }
    }
}
