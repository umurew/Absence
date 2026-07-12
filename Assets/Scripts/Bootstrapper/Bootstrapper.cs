using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Bootstrapper : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private UIDocument hudDocument;
    [SerializeField] private UIDocument consoleDocument;

    [Header("Registries & Data")]
    [SerializeField] private ObjectRegistry objectRegistry;
    [SerializeField] private StatesBlackboard statesBlackboard;

    [Header("Service Prefabs")]
    [SerializeField] private InputService inputServicePrefab;
    [SerializeField] private LightingService lightingServicePrefab;
    [SerializeField] private SpawnerService spawnerServicePrefab;
    [SerializeField] private ConsoleService consoleServicePrefab;

    private InputService _inputService;
    private LightingService _lightingService;
    private SpawnerService _spawnerService;
    private ConsoleService _consoleService;

    private ICommand[] commands;

    private void Awake()
    {
        #region Initialize Input Service
        _inputService = Instantiate(inputServicePrefab, transform);
        _inputService.Initialize();
        #endregion

        #region Initialize Lighting Service
        _lightingService = Instantiate(lightingServicePrefab, transform);
        _lightingService.Initialize(mainCamera);
        #endregion

        #region Initialize Spawner Service
        objectRegistry.Initialize();
        statesBlackboard.Initialize();

        _spawnerService = Instantiate(spawnerServicePrefab, transform);
        _spawnerService.Initialize(objectRegistry, statesBlackboard);
        #endregion

        #region Initialize Console Service
        commands = new ICommand[] { new ClearCommand() };

        _consoleService = Instantiate(consoleServicePrefab, transform);
        _consoleService.Initialize(_inputService, consoleDocument, commands);
        #endregion

        #region Inject dependencies to dependants
        playerController.Initialize(_inputService, mainCamera.transform, hudDocument);
        #endregion

        #region Initialize scene
        foreach (var interactable in Object.FindObjectsByType<MonoBehaviour>().OfType<IInteractable>())
            interactable.Initialize(statesBlackboard);
        #endregion
    }
}