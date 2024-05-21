using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Manager
{
    public enum UIType { LogIn, Chat, ErrorWindow }
    Dictionary<UIType, GameObject> prefabDictionary;
    Dictionary<UIType, GameObject> instanceDictionary;

    [SerializeField] Canvas errorCanvas;

    public override IEnumerator Initiate()
    {
        GameManager.ClaimLoadInfo("UI");
        if(prefabDictionary!= null && instanceDictionary != null) 
        {
            yield break;
        }
        prefabDictionary = new();
        instanceDictionary = new();
        prefabDictionary.Add(UIType.LogIn, ResourceManager.GetPrefab(ResourceEnum.Prefab.LogInCanvas));
        prefabDictionary.Add(UIType.ErrorWindow, ResourceManager.GetPrefab(ResourceEnum.Prefab.ErrorWindow));

        GameObject canvasObject = new("ErrorCanvas", typeof(Canvas), typeof(CanvasRenderer), typeof(CanvasScaler), typeof(GraphicRaycaster));
        errorCanvas = canvasObject.GetComponent<Canvas>();
        errorCanvas.sortingOrder = short.MaxValue;
        errorCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        errorCanvas.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        errorCanvas.transform.SetParent(GameManager.Instance.transform);
        
        yield return null;
    }

    public static void Toggle(UIType wantUI)
    {
        GameObject target = GetUI(wantUI);
        target.SetActive(!target.activeInHierarchy);
    }

    public static void Open(UIType wantUI)
    {
        GetUI(wantUI).SetActive(true);
    }

    public static void Close(UIType wantUI)
    {
        GetUI(wantUI).SetActive(false);
    }

    static GameObject GetUI(UIType wantUI)
    {
        UIManager uiManager = GameManager.Instance.UiManager;

        if(uiManager.prefabDictionary == null && uiManager.instanceDictionary == null) 
        { 
            Debug.LogWarning("UIManager.Open() has requested before game start!");
            return null;
        }
        else
        {
            // 인스턴스가 있으면 켜놓기
            if(uiManager.instanceDictionary.TryGetValue(wantUI, out GameObject result) && result != null)
            {
                return result;
            }
            // 없으면 프리팹이 있는지, 있으면 인스턴스 만들기
            else if(uiManager.prefabDictionary.TryGetValue(wantUI, out GameObject prefab))
            {
                GameObject inst = GameObject.Instantiate(prefab);
                if(!uiManager.instanceDictionary.TryAdd(wantUI, inst))
                {
                    uiManager.instanceDictionary[wantUI] = inst;
                }
                return inst;
            }
            // 프리팹도 없으면 에러
            else
            {
                Debug.LogWarning($"There is no prefab : {wantUI}");
            }

        }
        return null;
    }

    public static void ClaimError(string bar, string context, string confirm, System.Action confirmAction)
    {
        UIManager uiManager = GameManager.Instance.UiManager;

        // 에러를 어느 캔버스에 올릴까요?
        // uiManager가 에러를 올릴 캔버스를 가지고 있어야함.
        // 에러창을 생성하고 캔버스의 자식으로 넣기
        // 에러윈도우 정보를 전달.
        if(uiManager != null && uiManager.errorCanvas != null)
        {
            if(uiManager.prefabDictionary.TryGetValue(UIType.ErrorWindow, out GameObject prefab))
            {
                GameObject inst = GameObject.Instantiate(prefab, uiManager.errorCanvas.transform);
                inst.GetComponent<ErrorWindow>().SetText(bar, context, confirm, confirmAction + (() => { GameObject.Destroy(inst); }));
            }
        }
        
    }

}
