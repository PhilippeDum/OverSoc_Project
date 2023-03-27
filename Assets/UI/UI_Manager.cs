using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class UI_Manager : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset elementTemplate;

    [SerializeField] private List<string> characters;

    UIDocument uiDocument;

    private TextField textField;

    private Button headerButton;
    private Button addButton;
    private Button deleteButton;

    private Label limitLabel;
    private int maxLimit = 20;
    private int currentLimit;

    private ProgressBar progressBar;

    private ListView listView;
    private VisualElement selectedElement;

    private void Start()
    {
        uiDocument = GetComponent<UIDocument>();

        textField = uiDocument.rootVisualElement.Q("Add-TextField") as TextField;

        headerButton = uiDocument.rootVisualElement.Q("Header-Button") as Button;
        headerButton.RegisterCallback<ClickEvent>(Quit);

        addButton = uiDocument.rootVisualElement.Q("Add-Button") as Button;
        addButton.RegisterCallback<ClickEvent>(AddElement);

        deleteButton = uiDocument.rootVisualElement.Q("Delete-Button") as Button;
        deleteButton.RegisterCallback<ClickEvent>(RemoveElement);

        currentLimit = characters.Count;

        limitLabel = uiDocument.rootVisualElement.Q("Limit-Texts-Numbers") as Label;
        limitLabel.text = $"{currentLimit} / {maxLimit}";

        progressBar = uiDocument.rootVisualElement.Q("CustomProgressBar") as ProgressBar;
        progressBar.highValue = maxLimit;
        progressBar.value = currentLimit;
        
        HandleList();
    }

    private void Update()
    {
        if (selectedElement == null)
        {
            deleteButton.SetEnabled(false);
        }
        else
        {
            deleteButton.SetEnabled(true);
        }
    }

    private void Quit(ClickEvent evt)
    {
        Debug.Log($"Quit Application");
    }

    private void AddElement(ClickEvent evt)
    {
        if (textField == null || listView == null) return;

        Debug.Log($"Add {textField.text}");

        var template = elementTemplate.Instantiate();
        var customElement = template.Q("CustomElement");
        Label label_Char = customElement.Q("Name") as Label;
        label_Char.text = textField.text;

        characters[characters.Count] = textField.text;
        uiDocument.rootVisualElement.Q<ListView>("Content").RefreshItems();
    }

    private void RemoveElement(ClickEvent evt)
    {
        if (selectedElement == null || listView == null) return;

        Debug.Log($"Remove {selectedElement} - {selectedElement.parent}");
        selectedElement.RemoveFromHierarchy();
        listView.Remove(selectedElement);
        listView.RefreshItems();
    }

    private VisualElement SetupCustomElement(VisualTreeAsset elementTemplate)
    {
        var template = elementTemplate.Instantiate();
        var customElement = template.Q("CustomElement");
        customElement.RegisterCallback<ClickEvent>(SelectElement);
        return template;
    }

    private void SelectElement(ClickEvent evt)
    {
        selectedElement = (VisualElement)evt.currentTarget;
    }

    private void HandleList()
    {
        var customList = new List<string>(characters.Count);

        listView = uiDocument.rootVisualElement.Q<ListView>("Content");

        listView.makeItem = () =>
        {
            return SetupCustomElement(elementTemplate);
        };

        listView.bindItem = (element, index) =>
        {
            element.name = $"Element_{index}";

            Label label_Pos = element.Q("ID") as Label;
            label_Pos.text = $"{index + 1}";

            Label label_Char = element.Q("Name") as Label;
            label_Char.text = characters[index];

            var elementIcon = element.Q("ElementIcon");

            var iconPath = "Icons/Vector_List";
            var iconAsset = Resources.Load<Texture2D>(iconPath);

            elementIcon.style.backgroundImage = iconAsset;
        };

        listView.itemsSource = characters;
        listView.selectionType = SelectionType.Multiple;
        listView.fixedItemHeight = 52;
        listView.onItemsChosen += OnItemsChosen;
    }

    private void OnItemsChosen(IEnumerable<object> objets)
    {
        Debug.Log($"{nameof(OnItemsChosen)}()");
    }
}