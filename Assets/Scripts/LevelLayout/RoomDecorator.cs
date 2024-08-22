using UnityEngine;

[System.Serializable]
public class RoomDecorator : MonoBehaviour
{
    public GameObject decoratorObject;
    public Vector3 positionOffset;
    public Quaternion rotationOffset;

    public void ApplyDecorator(LevelLayout room)
    {
        var decoratorInstance = GameObject.Instantiate(decoratorObject);
        decoratorInstance.transform.SetParent(room.transform);
        decoratorInstance.transform.localPosition = positionOffset;
        decoratorInstance.transform.localRotation = rotationOffset;
    }
}