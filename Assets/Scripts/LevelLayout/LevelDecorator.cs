using UnityEngine;

[System.Serializable]
public class LevelDecorator : MonoBehaviour
{
    public GameObject decoratorObject;
    public Vector3 positionOffset;
    public Vector3 rotationOffset;

    public void ApplyDecorator(LevelLayout layout)
    {
        var decoratorInstance = Instantiate(decoratorObject);
        decoratorInstance.transform.SetParent(layout.transform);
        decoratorInstance.transform.localPosition = positionOffset;
        decoratorInstance.transform.localRotation = Quaternion.Euler(rotationOffset);
    }
}