using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UI_Manager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VisualTreeAsset elementTemplate;
    [SerializeField] private List<string> characters;
    UIDocument uiDocument;

    [SerializeField] private string placeholder = "Nouveau participant";
    private TextField textField;
    private string placeholderClass;

    private Button headerButton;
    private Button addButton;
    private Button deleteButton;

    [SerializeField] private int maxParticipants = 20;
    private int nbParticipants;
    private Label limitParticipantsLabel;

    private ProgressBar progressBar;

    private ListView listView;
    private VisualElement selectedElement;

    private Toggle switchToggle;
    private Label offLabel;
    private Label onLabel;

    private VisualElement knob;

    private void Start()
    {
        uiDocument = GetComponent<UIDocument>();

        headerButton = uiDocument.rootVisualElement.Q("Header-Button") as Button;
        headerButton.RegisterCallback<ClickEvent>(Quit);

        addButton = uiDocument.rootVisualElement.Q("Add-Button") as Button;
        addButton.RegisterCallback<ClickEvent>(AddParticipant);

        deleteButton = uiDocument.rootVisualElement.Q("Delete-Button") as Button;
        deleteButton.RegisterCallback<ClickEvent>(RemoveParticipant);

        InitializeParticipantTextField();

        switchToggle = uiDocument.rootVisualElement.Q("Switch") as Toggle;
        offLabel = uiDocument.rootVisualElement.Q("Off-Label") as Label;
        onLabel = uiDocument.rootVisualElement.Q("On-Label") as Label;

        HandleSwitchOption();

        nbParticipants = characters.Count;

        limitParticipantsLabel = uiDocument.rootVisualElement.Q("Limit-Texts-Numbers") as Label;
        limitParticipantsLabel.text = $"{nbParticipants} / {maxParticipants}";

        progressBar = uiDocument.rootVisualElement.Q("CustomProgressBar") as ProgressBar;
        progressBar.highValue = maxParticipants;
        progressBar.value = nbParticipants;

        HandleList();
    }

    private void Update()
    {
        // Handle Add button state
        if (nbParticipants < maxParticipants)
        {
            if (!string.IsNullOrEmpty(textField.value) && textField.value != placeholder)
            {
                addButton.SetEnabled(true);
            }
        }
        else
        {
            addButton.SetEnabled(false);
        }

        // Handle Delete button state
        if (selectedElement == null)
        {
            deleteButton.SetEnabled(false);
        }
        else
        {
            deleteButton.SetEnabled(true);
        }

        HandleSwitchOption();
    }

    private void Quit(ClickEvent evt)
    {
        Debug.Log($"Quit Application");
    }

    private void UpdateParticipants()
    {
        limitParticipantsLabel.text = $"{nbParticipants} / {maxParticipants}";
        progressBar.value = nbParticipants;
    }

    private void HandleSwitchOption()
    {
        if (!switchToggle.value)
        {
            offLabel.style.opacity = 1f;
            onLabel.style.opacity = 0.5f;
        }
        else
        {
            offLabel.style.opacity = 0.5f;
            onLabel.style.opacity = 1f;
        }
    }

    #region Handle New Participant TextField

    private void InitializeParticipantTextField()
    {
        textField = uiDocument.rootVisualElement.Q("Add-TextField") as TextField;

        placeholderClass = TextField.ussClassName + "__placeholder";

        OnFocusOut();

        textField.RegisterCallback<FocusInEvent>(evt => OnFocusIn());
        textField.RegisterCallback<FocusOutEvent>(evt => OnFocusOut());
    }

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

    #region Handle Participants List

    private void HandleList()
    {
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
        listView.reorderable = true;
        listView.reorderMode = ListViewReorderMode.Animated;

        listView.Q("unity-low-button").visible = false;
        listView.Q("unity-high-button").visible = false;
    }

    private void SelectParticipant(ClickEvent evt)
    {
        selectedElement = (VisualElement)evt.currentTarget;
    }

    private void AddParticipant(ClickEvent evt)
    {
        if (textField == null || listView == null) return;

        if (nbParticipants == maxParticipants) return;

        if (!switchToggle.value)
        {
            characters.Add(textField.text);
        }
        else
        {
            characters.Insert(0, textField.text);
        }

        nbParticipants++;
        UpdateParticipants();

        textField.value = string.Empty;
        OnFocusOut();

        listView.RefreshItems();
    }

    private void RemoveParticipant(ClickEvent evt)
    {
        if (selectedElement == null || listView == null) return;

        Label label_Char = selectedElement.Q("Name") as Label;

        characters.Remove(label_Char.text);
        nbParticipants--;

        UpdateParticipants();

        listView.RefreshItems();

        selectedElement = null;
    }

    #endregion
}