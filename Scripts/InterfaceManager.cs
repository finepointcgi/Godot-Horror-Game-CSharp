using Godot;

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
