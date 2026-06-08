using Godot;

namespace Minesweeper.Popup;

public partial class Popup : Window
{
    private VBoxContainer mainContainer;
    private PanelContainer container;

    private RichTextLabel head,
        body;

    private Button retry,
        exit;

    public override void _Ready()
    {
        base._Ready();

        CloseRequested += HandleExit;

        container = GetNode<PanelContainer>("PanelContainer");
        mainContainer = container
            .GetNode<PanelContainer>("InnerContainer")
            .GetNode<VBoxContainer>("MainContainer");

        head = mainContainer.GetNode<RichTextLabel>("Head");
        body = mainContainer.GetNode<RichTextLabel>("Body");

        retry = mainContainer.GetNode<HBoxContainer>("HBoxContainer").GetNode<Button>("Retry");
        exit = mainContainer.GetNode<HBoxContainer>("HBoxContainer").GetNode<Button>("Exit");
        exit.Pressed += HandleExit;
    }

    private async void HandleExit()
    {
        await ExitTask();
    }

    private async System.Threading.Tasks.Task ExitTask()
    {
        var fadeRect = new ColorRect
        {
            Color = Colors.Black,
            Size = GetViewport().GetVisibleRect().Size,
            Modulate = Colors.Transparent,
            ZIndex = 100,
        };
        AddChild(fadeRect);

        var tween = CreateTween();
        tween.TweenProperty(fadeRect, "modulate", Colors.Black, 0.5f);
        await ToSignal(tween, "finished");

        GetTree().Quit();
    }

    public string HeadText
    {
        set => head.Text = value;
        get => head.Text;
    }

    public string BodyText
    {
        set => body.Text = value;
        get => body.Text;
    }
}
