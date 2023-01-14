namespace GodotHorrorGameCSharp.Scripts
{
    public interface Interactable
    {
        string GetInterfaceText();
        void PlayAnimation(string animation);
        void Interact();
    }
}
