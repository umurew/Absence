public interface IInteractable
{
    void Initialize(StatesBlackboard statesBlackboard);
    void Interact();
    string GetInteractPrompt();
}