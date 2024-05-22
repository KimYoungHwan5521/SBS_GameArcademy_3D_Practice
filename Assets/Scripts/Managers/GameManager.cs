using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 공통적으로 사용하는 함수들의 모양을 미리 정의해봅시다
// 델리게이트 (대리자) : Action Func를 만든 장본인
// 여러개의 함수를 한 번에 넣고 돌릴 수 있는 것
// delegate 함수모양
public delegate void UpdateFunction(float deltaTime);
public delegate void StartFunction();
public delegate void DestroyFunction();


// 게임 매니저를 무조건 "하나만" 있도록
// 싱글턴 패턴
public class GameManager : MonoBehaviour
{
    // static : 정적변수 : 프로그램내에서 유일
    static GameManager instance;
    public static GameManager Instance => instance;
    
    // 초기화 해야하는 순서대로
    ResourceManager resourceManager;
    public ResourceManager ResourceManager => resourceManager;
    SoundManager soundManager;
    public SoundManager SoundManager => soundManager;
    SaveManager saveManager;
    public SaveManager SaveManager => saveManager;
    OptionManager optionManager;
    public OptionManager OptionManager => optionManager;
    ControllerManager controllerManager;
    public ControllerManager ControllerManager => controllerManager;
    NetworkManager networkManager;
    public NetworkManager NetworkManager => networkManager;
    WorldManager worldManager;
    public WorldManager WorldManager => worldManager;
    UIManager uiManager;
    public UIManager UiManager => uiManager;

    public static UpdateFunction ManagerUpdates;
    public static UpdateFunction CharacterUpdates;
    public static UpdateFunction ObjectUpdates;
    public static UpdateFunction ControllerUpdates;

    public static StartFunction ManagerStarts;
    public static StartFunction CharacterStarts;
    public static StartFunction ObjectStarts;
    public static StartFunction ControllerStarts;

    public static DestroyFunction ManagerDestroies;
    public static DestroyFunction CharacterDestroies;
    public static DestroyFunction ObjectDestroies;
    public static DestroyFunction ControllerDestroies;

    LoadingCanvas loadingCanvas;
    public LogInCanvas loginCanvas;

    bool isGameStart = false;
    // 모두가 볼 수 있게
    public static bool IsGameStart => instance && instance.isGameStart;

    // Awake -> Start -> Update(loop)
    // OnEnabled : Awake와 Start 사이에 "활성화"되면 실행
    // Awake는 IEnumerator로 쓰지 않는다.
    // Start를 IEnumerator로 쓰면 StartCoroutine으로 돌려줌
    private IEnumerator Start()
    {
        this.MakeSingleton(ref instance);
        if (instance != this) yield break;
        DontDestroyOnLoad(gameObject);


        loadingCanvas = GetComponentInChildren<LoadingCanvas>();
        loginCanvas = GetComponentInChildren<LogInCanvas>();

        resourceManager = new ResourceManager();
        yield return resourceManager.Initiate();
        soundManager = new SoundManager();
        yield return soundManager.Initiate();
        saveManager= new SaveManager();
        yield return saveManager.Initiate();
        optionManager = new OptionManager();
        yield return optionManager.Initiate();
        controllerManager = new ControllerManager();
        yield return controllerManager.Initiate();
        networkManager = new NetworkManager();
        yield return networkManager.Initiate();
        uiManager = new UIManager();
        yield return uiManager.Initiate();

        ManagerUpdates += NetworkManager.ManagerUpdate;
        ManagerUpdates += SoundManager.ManagerUpdate;
        ManagerUpdates += ControllerManager.ManagerUpdate;
        ManagerUpdates += UiManager.ManagerUpdate;
        
        // 씬이 로드되었을 때 할 일을 등록
        SceneManager.sceneLoaded += OnSceneLoad;

        CloseLoadInfo();

        isGameStart= true;
        
    }

    // 이 게임의 Update는 귀찮지 않는이상 얘만
    private void Update()
    {
        if (!isGameStart) return;
        // start를 먼저해야함
        // 매니저는 제일 먼저
        if (ManagerStarts != null)
        { 
            ManagerStarts.Invoke();    
            // Start를 실행한 애들은 전부 빼준다.
            ManagerStarts = null; 
        }
        else
        {
            ObjectStarts?.Invoke();
            ObjectStarts = null;
            // 캐릭터는 매니저가 있어야 됨
            CharacterStarts?.Invoke();
            CharacterStarts= null;
            // 컨트롤러는 캐릭터가 있어야 됨
            ControllerStarts?.Invoke();
            ControllerStarts= null;

            ManagerUpdates?.Invoke(Time.deltaTime);
            // 컨트롤을 받아 캐릭터에 전달하기 위해 컨트롤러를 먼저
            ControllerUpdates?.Invoke(Time.deltaTime);
            ObjectUpdates?.Invoke(Time.deltaTime);
            CharacterUpdates?.Invoke(Time.deltaTime);
        }


        // 스타트 역순
        ControllerDestroies?.Invoke();
        ControllerDestroies= null;
        CharacterDestroies?.Invoke();
        CharacterDestroies= null;
        ObjectDestroies?.Invoke(); 
        ObjectDestroies= null;
        ManagerDestroies?.Invoke();
        ManagerDestroies= null;

    }
    
    public static void WorldChange(WorldManager newWorld)
    {
        if(!IsGameStart) 
        {
            Debug.LogWarning("Tried WorldChange before GameManager Loaded");
            return;
        }
        if (newWorld == null || newWorld == Instance.worldManager) return;
        // 기존 월드 제거
        if(Instance.worldManager != null) Instance.worldManager.Delete();

        // 새 월드 넣기
        Instance.worldManager = newWorld;

        // 새로운 월드가 하고싶었던 것
        newWorld.Create();
    }


    // 게임을 시작하자 => 업데이트 활성화
    // 업데이트를 어떻게하면 비활성화 할 수 있을까 (bool 없이)
    // 게임을 끝낸다 -> 업데이트 멈춤
    public void OnSceneLoad(Scene newScene, LoadSceneMode loadSceneMode)
    {
        SoundManager.StopAllSFX();
        SoundManager.StopBGM();
    }

    public static void ClaimLoadInfo(string info)
    {
        // static에서는 언제나 "대상"이 있는지 확인
        if (instance && instance.loadingCanvas)
        {
            instance.loadingCanvas.gameObject.SetActive(true);
            instance.loadingCanvas.SetLoadInfo(info);
            instance.isGameStart = false;
        }
        else
        {
            Debug.LogAssertion($"Loading Error : No GameManager instance or No loadingCanvas");
        }
    }

    public static void CloseLoadInfo()
    {
        if(instance && instance.loadingCanvas)
        {
            instance.loadingCanvas.gameObject.SetActive(false);
            instance.isGameStart = true;
        }
        else
        {
            Debug.LogAssertion($"Loading Error : No GameManager instance or No loadingCanvas");
        }
    }
}
