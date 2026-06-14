using Godot;
using Minesweeper.Misc;

namespace Minesweeper.Scripts;

public partial class Option : Control
{
    private VBoxContainer mainContainer;
    private OptionButton optionButton;
    private RichTextLabel bodyLabel;

    private TextureButton doneBtn,
        cancelBtn;

    private readonly Definition definition = Definition.Instance;

    private Definition.GridSize gridSize;

    public override void _Ready()
    {
        base._Ready();

        mainContainer = GetNode<PanelContainer>("PanelContainer")
            .GetNode<VBoxContainer>("MainContainer");

        optionButton = mainContainer
            .GetNode<PanelContainer>("MainPanel")
            .GetNode<HBoxContainer>("HBoxContainer")
            .GetNode<OptionButton>("OptionButton");

        var items = definition.Items;
        for (int i = 0; i < items.Count; i++)
            optionButton.AddItem(items[i].ToString(), i);

        bodyLabel = mainContainer
            .GetNode<PanelContainer>("PanelContainer")
            .GetNode<VBoxContainer>("VBoxContainer")
            .GetNode<RichTextLabel>("Body");

        doneBtn = mainContainer
            .GetNode<PanelContainer>("ButtonsPanel")
            .GetNode<HBoxContainer>("HBoxContainer")
            .GetNode<TextureButton>("Done");

        cancelBtn = mainContainer
            .GetNode<PanelContainer>("ButtonsPanel")
            .GetNode<HBoxContainer>("HBoxContainer")
            .GetNode<TextureButton>("Cancel");

        CallDeferred(MethodName.Init);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        string body = string.Empty;
        body +=
            $"Difficulty: [b]{Definition.GetDifficultStatus(gridSize)}[/b]\n"
            + $"Grid Size: [b]{gridSize.ToString().Replace("_", "")}[/b]\n"
            + $"Acutal Size: [b]{Definition.GetCalculateSize(gridSize)}[/b]\n"
            + $"Total Mines [b]{Definition.GetCalculatedBomb(gridSize)}[/b]";

        bodyLabel.Text = body;
    }

    private void Init()
    {
        var panel = GetNode<PanelContainer>("PanelContainer");
        Vector2I size = new((int)panel.Size.X, (int)panel.Size.Y);

        //Vector2I size = new(GetWindow().Size.X, GetWindow().Size.Y);

        var window = GetWindow();
        window.ContentScaleSize = size;
        window.ContentScaleMode = Window.ContentScaleModeEnum.CanvasItems;
        window.ContentScaleAspect = Window.ContentScaleAspectEnum.Expand;

        optionButton.ItemSelected += index =>
        {
            var text = optionButton.GetItemText((int)index);
            if (System.Enum.TryParse(text, out Definition.GridSize parsed))
                gridSize = parsed;

            GD.Print($"Definition {Definition.GetCalculateColumn(gridSize)}");
            GD.Print($"Getter {text}");
        };

        doneBtn.Pressed += HandleChangeScene;
        cancelBtn.Pressed += OnExitButtonPressed;
    }

    private async void OnExitButtonPressed()
    {
        var panel = GetNode<PanelContainer>("PanelContainer");

        await SceneManager.FadeAndExit(panel);
    }

    private async void HandleChangeScene()
    {
        await ChangeState();
    }

    private async System.Threading.Tasks.Task ChangeState()
    {
        try
        {
            var panel = GetNode<PanelContainer>("PanelContainer");

            int selectedId = optionButton.GetSelectedId();
            var gridSize = (Definition.GridSize)selectedId;

            definition.GridProperty = gridSize;

            await SceneManager.LoadScene("res://Scenes/Main.tscn", "OutterPanel", panel);
        }
        catch (System.Exception e)
        {
            GD.PrintErr("ERROR: " + e.Message);
            //System.Environment.Exit(1);
        }
    }

    public override void _ExitTree()
    {
        try
        {
            doneBtn.Pressed -= HandleChangeScene;
            cancelBtn.Pressed -= OnExitButtonPressed;
        }
        catch (System.Exception e)
        {
            GD.PrintErr("FAIL: " + e.Message);
            System.Environment.Exit(1);
        }
    }
}
