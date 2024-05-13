using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���������� ����ϴ� �Լ����� ����� �̸� �����غ��ô�
// ��������Ʈ (�븮��) : Action Func�� ���� �庻��
// �������� �Լ��� �� ���� �ְ� ���� �� �ִ� ��
// delegate �Լ����
public delegate void UpdateFunction(float deltaTime);
public delegate void StartFunction();
public delegate void DestroyFunction();



// ���� �Ŵ����� ������ "�ϳ���" �ֵ���
// �̱��� ����
public class GameManager : MonoBehaviour
{
    // static : �������� : ���α׷������� ����
    static GameManager instance;
    public static GameManager Instance => instance;
    
    // �ʱ�ȭ �ؾ��ϴ� �������
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
    UIManager uiManager;
    public UIManager UiManager => uiManager;

    public static UpdateFunction ManagerUpdates;
    public static UpdateFunction CharacterUpdates;
    public static UpdateFunction ControllerUpdates;
    public static StartFunction ManagerStarts;
    public static StartFunction CharacterStarts;
    public static StartFunction ControllerStarts;
    public static DestroyFunction ManagerDestroies;
    public static DestroyFunction CharacterDestroies;
    public static DestroyFunction ControllerDestroies;

    LoadingCanvas loadingCanvas;

    bool isGameStart = false;
    // ��ΰ� �� �� �ְ�
    public static bool IsGameStart => instance && instance.isGameStart;

    // Awake -> Start -> Update(loop)
    // OnEnabled : Awake�� Start ���̿� "Ȱ��ȭ"�Ǹ� ����
    // Awake�� IEnumerator�� ���� �ʴ´�.
    // Start�� IEnumerator�� ���� StartCoroutine���� ������
    private IEnumerator Start()
    {
        this.MakeSingleton(ref instance);

        loadingCanvas = GetComponentInChildren<LoadingCanvas>();

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
        
        CloseLoadInfo();

        isGameStart= true;
        
    }

    // �� ������ Update�� ������ �ʴ��̻� �길
    private void Update()
    {
        if (!isGameStart) return;
        Debug.Log("gd");
        // start�� �����ؾ���
        // �Ŵ����� ���� ����
        ManagerStarts?.Invoke();    
        // Start�� ������ �ֵ��� ���� ���ش�.
        ManagerStarts = null; 
        // ĳ���ʹ� �Ŵ����� �־�� ��
        CharacterStarts?.Invoke();
        CharacterStarts= null;
        // ��Ʈ�ѷ��� ĳ���Ͱ� �־�� ��
        ControllerStarts?.Invoke();
        ControllerStarts= null;

        ManagerUpdates?.Invoke(Time.deltaTime);
        // ��Ʈ���� �޾� ĳ���Ϳ� �����ϱ� ���� ��Ʈ�ѷ��� ����
        ControllerUpdates?.Invoke(Time.deltaTime);
        CharacterUpdates?.Invoke(Time.deltaTime);

        // ��ŸƮ ����
        ControllerDestroies?.Invoke();
        ControllerDestroies= null;
        CharacterDestroies?.Invoke();
        CharacterDestroies= null;
        ManagerDestroies?.Invoke();
        ManagerDestroies= null;

    }
    
    // ������ �������� => ������Ʈ Ȱ��ȭ
    // ������Ʈ�� ����ϸ� ��Ȱ��ȭ �� �� ������ (bool ����)
    // ������ ������ -> ������Ʈ ����


    public static void ClaimLoadInfo(string info)
    {
        // static������ ������ "���"�� �ִ��� Ȯ��
        if (instance && instance.loadingCanvas)
        {
            instance.loadingCanvas.gameObject.SetActive(true);
            instance.loadingCanvas.SetLoadInfo(info);
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
        }
        else
        {
            Debug.LogAssertion($"Loading Error : No GameManager instance or No loadingCanvas");
        }
    }
}
