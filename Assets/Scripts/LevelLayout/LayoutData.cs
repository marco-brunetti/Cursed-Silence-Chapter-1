using UnityEngine;

[CreateAssetMenu(fileName = "LayoutData", menuName = "ScriptableObjects/LayoutData", order = 1)]
public class LayoutData : ScriptableObject
{
    [Header("Style0: Original blue cursed silence layout")]
    public Material WallMat1;
    public Material LowerWallMat1;
    public Material FloorMat1;
    public Material CeilingMat1;
    public Material WindowDecorMat1;
    [Header("Style1: Green and wood walls")]
    public Material WallMat2;
    public Material LowerWallMat2;
    public Material FloorMat2;
    public Material CeilingMat2;
    public Material WindowDecorMat2;
    [Header("Style2: Old wooden house level")]
    public Material WallMat3;
    public Material LowerWallMat3;
    public Material FloorMat3;
    public Material CeilingMat3;
    public Material WindowDecorMat3;
    [Header("Style3: Old concrete wall level")]
    public Material WallMat4;
    public Material LowerWallMat4;
    public Material FloorMat4;
    public Material CeilingMat4;
    public Material WindowDecorMat4;
    [Header("Style4: Fabric white asylum walls")]
    public Material WallMat5;
    public Material LowerWallMat5;
    public Material FloorMat5;
    public Material CeilingMat5;
    public Material WindowDecorMat5;
}

public enum LayoutStyle
{
    Style0,
    Style1,
    Style2,
    Style3,
    Style4
}

public enum LayoutShape
{
    None,
    HallStraight,
    HallShortT,
    HallLongT,
    HallLeftL,
    HallRightL,
    HallThinStraight,
    RoomSmall
}