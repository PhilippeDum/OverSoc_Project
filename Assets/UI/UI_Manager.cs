using UnityEngine;
using UnityEngine.UIElements;

public class UI_Manager : MonoBehaviour
{
    UIDocument uiDocument;

    private Button headerButton;

    private void Start()
    {
        uiDocument = GetComponent<UIDocument>();

        headerButton = uiDocument.rootVisualElement.Q("Header-Button") as Button;

        headerButton.RegisterCallback<ClickEvent>(Quit);
        //SetupButton(headerButton);
    }

    /*private void SetupButton(VisualElement button)
    {
        var buttonIcon = button.Q(name: "Header-Button-Icon");
        var iconPath = "Icons/Vector";
        var iconAsset = Resources.Load<Texture2D>(iconPath);

        buttonIcon.style.backgroundImage = iconAsset;

        button.RegisterCallback<ClickEvent>(Quit);
    }*/

    private void Quit(ClickEvent evt)
    {
        Debug.Log($"Call event : {evt}");
    }
}