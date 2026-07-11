using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

public class Bootstrapper : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private ObjectRegistry objectRegistry;
    [SerializeField] private StatesBlackboard statesBlackboard;
    [SerializeField] private UIDocument hudDocument;
    [SerializeField] private UIDocument consoleDocument;

    [Space(10)]
    [SerializeField] private InputService inputServicePrefab;
    [SerializeField] private LightingService lightingServicePrefab;
    [SerializeField] private SpawnerService spawnerServicePrefab;
    [SerializeField] private ConsoleService consoleServicePrefab;

    private InputService _inputService;
    private LightingService _lightingService;
    private SpawnerService _spawnerService;
    private ConsoleService _consoleService;

    private PlayerController playerController;
    private ICommand[] commands;

    private void Awake()
    {
        // Bind essential references

        /// Bind player controller to initialize PlayerBehaviour
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        /// Initialize Object Registry for SpawnerService
        objectRegistry.Initialize();

        /// Initialize IInteractable's on scene
        foreach (IInteractable interactable in UnityEngine.Object.FindObjectsByType<MonoBehaviour>().OfType<IInteractable>().ToList())
            interactable.Initialize(statesBlackboard);

        /// Get commands
        commands = Assembly.GetExecutingAssembly().GetTypes()
            .Where(type => typeof(ICommand).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
            .Select(type => (ICommand)Activator.CreateInstance(type))
            .ToArray();

        // Initialize core services
        _inputService = Instantiate(inputServicePrefab, transform);
        _inputService.Initialize();

        _lightingService = Instantiate(lightingServicePrefab, transform);
        _lightingService.Initialize(mainCamera);

        _spawnerService = Instantiate(spawnerServicePrefab, transform);
        _spawnerService.Initialize(objectRegistry, statesBlackboard);

        _consoleService = Instantiate(consoleServicePrefab, transform);
        _consoleService.Initialize(_inputService, consoleDocument, commands);

        // Inject services into dependent classes

        /// Initialize PlayerController (which initializes PlayerInteraction and PlayerMovement)
        playerController.Initialize(_inputService, mainCamera.transform, hudDocument);

        // Start the game logic
    }
}