using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionManager : Manager
{

    public struct OptionData
    {
        public GeneralEnum.ResolutionEnum resolution;
        public float volumeMaster;
        public float volumeBGM;
        public float volumeSFX;
        public float mouseSensitivity;
        public bool isFullScreen;
        public bool mouseVerticalFlip;
        public bool muteMaster;
        public bool muteBGM;
        public bool muteSFX;

    }

    public static OptionData defaultOptionData = new OptionData
    {
        resolution = GeneralEnum.ResolutionEnum._1920x1080,
        volumeMaster = 0.5f,
        volumeBGM = 0.5f,
        volumeSFX = 0.5f,
        mouseSensitivity = 1,
        isFullScreen = true,
        muteMaster = false,
        muteBGM = false,
        muteSFX = false,
        mouseVerticalFlip= false,
    };

    public OptionData currentOption;

    public override IEnumerator Initiate()
    {
        currentOption = GameManager.Instance.SaveManager.LoadedOptionData;
        ApplyOption();
        yield return null;
    }

    // 들어온 옵션을 토대로 게임에 적용해보기
    public void ApplyOption()
    {
        // 해상도와 풀스크린
        Screen.SetResolution(GeneralEnum.ToVector(currentOption.resolution).x, GeneralEnum.ToVector(currentOption.resolution).y, currentOption.isFullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);

        // 볼륨
        GameManager.Instance.SoundManager.SetSoundInfo(ref currentOption);

    }
}
