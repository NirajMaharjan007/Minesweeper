using Godot;

namespace Minesweeper.Scripts;

public partial class Option : Control
{
    private VBoxContainer mainContainer;
    private OptionButton optionButton;
    private RichTextLabel bodyLabel;

    private TextureButton doneBtn,
        cancelBtn;

    private readonly Misc.Definition definition = Misc.Definition.Instance;

    private Misc.Definition.GridSize gridSize;

    public override void _Ready()
    {
        base._Ready();

        mainContainer = GetNode<VBoxContainer>("MainContainer");

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

        Init();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        string body = string.Empty;
        body +=
            $"Difficulty: [b]{definition.GetDifficultStatus(gridSize)}[/b]\n"
            + $"Grid Size: [b]{gridSize.ToString().Replace("_", "")}[/b]\n"
            + $"Acutal Size: [b]{definition.GetCalculateSize(gridSize)}[/b]\n"
            + $"Total Mines [b]{definition.GetCalculatedBomb(gridSize)}[/b]";

        bodyLabel.Text = body;
    }

    private void Init()
    {
        Vector2I size = new((int)mainContainer.Size.X, (int)mainContainer.Size.Y);

        var window = GetWindow();
        window.ContentScaleSize = size;
        window.ContentScaleMode = Window.ContentScaleModeEnum.CanvasItems;
        window.ContentScaleAspect = Window.ContentScaleAspectEnum.Expand;

        optionButton.ItemSelected += index =>
        {
            var text = optionButton.GetItemText((int)index);
            if (System.Enum.TryParse(text, out Misc.Definition.GridSize parsed))
                gridSize = parsed;

            GD.Print($"Definition {definition.GetCalculateColumn(gridSize)}");
            GD.Print($"Getter {text}");
        };

        doneBtn.Pressed += HandleChangeScene;
        cancelBtn.Pressed += OnExitButtonPressed;
    }

    private async void OnExitButtonPressed()
    {
        await FadeAndExit();
    }

    private async void HandleChangeScene()
    {
        await ChangeState();
    }

    private async System.Threading.Tasks.Task ChangeState()
    {
        try { }
        catch (System.Exception e)
        {
            GD.PrintErr("ERROR: " + e.Message);
            System.Environment.Exit(1);
        }
    }

    private async System.Threading.Tasks.Task FadeAndExit()
    {
        var fadeRect = new ColorRect();
        fadeRect.Color = Colors.Black;
        fadeRect.Size = GetViewport().GetVisibleRect().Size;
        fadeRect.Modulate = Colors.Transparent;
        fadeRect.ZIndex = 100;
        AddChild(fadeRect);

        var tween = CreateTween();
        tween.TweenProperty(fadeRect, "modulate", Colors.Black, 0.5f);
        await ToSignal(tween, "finished");

        GetTree().Quit();
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
