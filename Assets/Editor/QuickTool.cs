using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System;


public class QuickTool : EditorWindow
{
    // "_%#T" ==> shortcut to open window = CTRL + SHIFT + T
    [MenuItem("QuickTool/Open _%#T")]
    public static void ShowWindow()
    {
        // Opens the window, otherwise focuses it if it's already open
        QuickTool window = GetWindow<QuickTool>();

        // Adds a title to the window
        window.titleContent = new GUIContent("QuickTool");

        // Sets a minimum size to the window
        window.minSize = new Vector2(280, 50);
    }

    public void CreateGUI()
    {
        // Reference to the root of the window
        VisualElement root = rootVisualElement;

        // Associates a stylesheet to the root - Thanks to inheritance, all root's children will have access to it
        root.styleSheets.Add(Resources.Load<StyleSheet>("QuickTool_Style"));

        // Loads and clones the VisualTree (eg. UXML structure) inside the root
        VisualTreeAsset quickToolVisualTree = Resources.Load<VisualTreeAsset>("QuickTool_Main");
        quickToolVisualTree.CloneTree(root);

        // Queries all the buttons (via class name) in the root and passes them in the SetupButton method
        var toolButtons = root.Query(className: "quicktool-button");
        toolButtons.ForEach(SetupButton);
    }

    private void SetupButton(VisualElement button)
    {
        // Reference to the VisualElement inside the button that serves as the button's icon
        var buttonIcon = button.Q(className: "quicktool-button-icon");

        // Icon's path in the project
        var iconPath = "Icons/" + button.parent.name + "_icon";

        // Loads the actual asset from the above path
        var iconAsset = Resources.Load<Texture2D>(iconPath);

        // Applies the above asset as a background image for the icon
        buttonIcon.style.backgroundImage = iconAsset;

        // Instantiates the primitive object on a left click
        button.RegisterCallback<PointerUpEvent, string>(CreateObject, button.parent.name);

        // Sets a basic tooltip to the button itself
        button.tooltip = button.parent.name;
    }

    private void CreateObject(PointerUpEvent _, string primitiveTypeName)
    {
        var primitiveType = (PrimitiveType) Enum.Parse(typeof(PrimitiveType), primitiveTypeName, true);

        var gameObject = ObjectFactory.CreatePrimitive(primitiveType);
        gameObject.transform.position = Vector3.zero;
    }
}