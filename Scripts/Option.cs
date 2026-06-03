using Godot;

namespace Minesweeper.Scripts;

public partial class Option : Control
{
    private VBoxContainer mainContainer;
    private OptionButton optionButton;
    private RichTextLabel bodyLabel;

    private Misc.Definition definition = Misc.Definition.Instance;

    public override void _Ready()
    {
        base._Ready();

        mainContainer = GetNode<VBoxContainer>("MainContainer");

        optionButton = mainContainer
            .GetNode<PanelContainer>("MainPanel")
            .GetNode<HBoxContainer>("HBoxContainer")
            .GetNode<OptionButton>("OptionButton");

        bodyLabel = mainContainer
            .GetNode<PanelContainer>("PanelContainer")
            .GetNode<VBoxContainer>("VBoxContainer")
            .GetNode<RichTextLabel>("Body");

        Init();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    private void Init()
    {
        Vector2I size = new((int)mainContainer.Size.X, (int)mainContainer.Size.Y);

        var window = GetWindow();
        window.ContentScaleSize = size;
        window.ContentScaleMode = Window.ContentScaleModeEnum.CanvasItems;
        window.ContentScaleAspect = Window.ContentScaleAspectEnum.Keep;

        string body = "THIS IS THE BODY";
        bodyLabel.Text = body;
    }
}
