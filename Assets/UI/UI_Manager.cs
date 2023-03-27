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
    [SerializeField] private string placeholder = "Nouveau participant";
    private string placeholderClass;

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

        headerButton = uiDocument.rootVisualElement.Q("Header-Button") as Button;
        headerButton.RegisterCallback<ClickEvent>(Quit);

        addButton = uiDocument.rootVisualElement.Q("Add-Button") as Button;
        addButton.RegisterCallback<ClickEvent>(AddParticipant);

        deleteButton = uiDocument.rootVisualElement.Q("Delete-Button") as Button;
        deleteButton.RegisterCallback<ClickEvent>(RemoveParticipant);

        textField = uiDocument.rootVisualElement.Q("Add-TextField") as TextField;
        placeholderClass = TextField.ussClassName + "__placeholder";
        OnFocusOut();
        textField.RegisterCallback<FocusInEvent>(evt => OnFocusIn());
        textField.RegisterCallback<FocusOutEvent>(evt => OnFocusOut());

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
        if (!string.IsNullOrEmpty(textField.value) && textField.value != placeholder)
        {
            addButton.SetEnabled(true);
        }

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

    #region Handle TextField

    private void OnFocusIn()
    {
        if (textField.ClassListContains(placeholderClass))
        {
            textField.value = string.Empty;
            textField.RemoveFromClassList(placeholderClass);
        }
    }

    private void OnFocusOut()
    {
        if (string.IsNullOrEmpty(textField.value))
        {
            textField.SetValueWithoutNotify(placeholder);
            textField.AddToClassList(placeholderClass);

            addButton.SetEnabled(false);
        }
    }

    #endregion

    #region Handle List

    private void AddParticipant(ClickEvent evt)
    {
        if (textField == null || listView == null) return;

        characters.Add(textField.text);

        listView.RefreshItems();
    }

    private void RemoveParticipant(ClickEvent evt)
    {
        if (selectedElement == null || listView == null) return;

        Label label_Char = selectedElement.Q("Name") as Label;

        characters.Remove(label_Char.text);

        listView.RefreshItems();

        selectedElement = null;
    }

    private void SelectParticipant(ClickEvent evt)
    {
        selectedElement = (VisualElement)evt.currentTarget;

        Debug.Log($"Select '{selectedElement.name}'");
    }

    private void HandleList()
    {
        var customList = new List<string>(characters.Count);

        listView = uiDocument.rootVisualElement.Q<ListView>("Content");

        listView.makeItem = () =>
        {
            VisualElement template = elementTemplate.Instantiate();
            template.RegisterCallback<ClickEvent>(SelectParticipant);
            return template;
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
        //listView.onItemsChosen += OnItemsChosen;
    }

    private void OnItemsChosen(IEnumerable<object> objets)
    {
        Debug.Log($"{nameof(OnItemsChosen)}()");
    }

    #endregion
}