using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private StatesBlackboard blackboard;
    [SerializeField] private bool locked = false;
    [SerializeField] private float openAngle = -100f;
    [SerializeField] private float closedAngle = 0f;
    [SerializeField] private float smoothSpeed = 5f;

    private bool doorState = false;
    private bool debounce = false;
    private Quaternion targetRotation;

    private void Start()
    {
        targetRotation = Quaternion.Euler(0f, closedAngle, 0f);
    }

    private void Update()
    {
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * smoothSpeed);
    }

    public void Interact()
    {
        if (debounce)
            return;

        debounce = true;

        if (locked)
        {
            if (blackboard.Get<bool>("has_master_key"))
                locked = false;
            else
                return;
        }

        doorState = !doorState;
        if (doorState)
            targetRotation = Quaternion.Euler(0f, openAngle, 0f);
        else
            targetRotation = Quaternion.Euler(0f, closedAngle, 0f);

        StartCoroutine(InteractionDebounceRoutine());
    }

    public string GetInteractPrompt()
    {
        if (debounce)
            return string.Empty;

        if (locked)
        {
            if (blackboard.Get<bool>("has_master_key"))
                return "Unlock";
            else
                return "It's Locked";
        }

        if (doorState)
            return "Close";
        else
            return "Open";
    }

    private IEnumerator InteractionDebounceRoutine()
    {
        debounce = true;

        yield return new WaitForSeconds(1.0f);

        debounce = false;
    }
}