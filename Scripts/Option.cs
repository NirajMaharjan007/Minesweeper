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
        optionButton.AddItem(Misc.Definition.GridSize._9X9.ToString(), 0);
        optionButton.AddItem(Misc.Definition.GridSize._16X16.ToString(), 1);
        optionButton.AddItem(Misc.Definition.GridSize._16X30.ToString(), 2);

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
        window.ContentScaleMode = Window.ContentScaleModeEnum.Viewport;
        window.ContentScaleAspect = Window.ContentScaleAspectEnum.Keep;

        optionButton.ItemSelected += index =>
        {
            var text = optionButton.GetItemText((int)index);
            if (System.Enum.TryParse(text, out Misc.Definition.GridSize parsed))
                gridSize = parsed;

            GD.Print($"Definition {definition.GetCalculateColumn(gridSize)}");
            GD.Print($"Getter {text}");
        };

        cancelBtn.Pressed += () =>
        {
            GetTree().Quit();
            System.Environment.Exit(0);
        };
    }
}
