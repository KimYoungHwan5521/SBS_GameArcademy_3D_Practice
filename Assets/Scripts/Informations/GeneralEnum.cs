using UnityEngine;

public static class GeneralEnum
{
    public enum ResolutionEnum
    {
        _800x600,
        _1024x768,
        _1280x720,
        _1366x768,
        _1440x900,
        _1920x1080,
    }

    // 이 Enum에 맞는 값을 꺼내올 수 있는 방법 -> 패턴 일치 식
    public static Vector2Int ToVector(ResolutionEnum resolution) => resolution switch
    {
        ResolutionEnum._800x600 => new Vector2Int(800, 600),
        ResolutionEnum._1024x768 => new Vector2Int(1024, 768),
        ResolutionEnum._1280x720 => new Vector2Int(1280, 720),
        ResolutionEnum._1366x768 => new Vector2Int(1366, 768),
        ResolutionEnum._1440x900 => new Vector2Int(1440, 900),
        ResolutionEnum._1920x1080 => new Vector2Int(1920, 1080),
        _ => new Vector2Int(1920, 1080),
    };
}
