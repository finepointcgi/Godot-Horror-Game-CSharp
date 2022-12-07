using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotHorrorGameCSharp.Scripts
{
    public interface Interactable 
    {
        string GetInterfaceText();
        void PlayAnimation(string animation);
        void Interact();
    }
}
