public interface IInteractable
{
    void Interact();
     string PromptText { get; } // e.g. "Pick Up", "Open", "Collect"
     
}
