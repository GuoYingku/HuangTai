using UnityEngine;

public class ToggleGameObject : MonoBehaviour
{
    public GameObject objectToShow; // Ҫ��ʾ�� GameObject
    public GameObject objectToHide; // Ҫ�رյ� GameObject

    public void ToggleObjects()
    {
        if (objectToShow != null)
        {
            objectToShow.SetActive(true); // ��ʾ objectToShow
            Debug.Log("Showing objectToShow");
        }

        if (objectToHide != null)
        {
            objectToHide.SetActive(false); // �ر� objectToHide
            Debug.Log("Showing objectTohide");
        }
    }
}
