using Godot;

namespace Minesweeper.Scripts;

public partial class Main : Control
{
    /*START DEBUUGERR */
    RichTextLabel label;
    string text;

    /*--------------*/
    Activity activity;
    GridContainer mainBox;

    private int stateIndex = 0;
    private readonly Activity.ButtonType[] states =
    [
        Activity.ButtonType.BUTTON,
        Activity.ButtonType.REDFLAG,
        Activity.ButtonType.QUESTIONMARK,
    ];

    private readonly System.Collections.Generic.List<TextureButton> copies = [];

    // private float Height => GetWindow().Size.Y;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        /*----------------*/
        label = GetNode<RichTextLabel>("Debugger");
        /*----------------*/

        activity = GetNode<Activity>("Activity");

        mainBox = GetNode<GridContainer>("MainBox");
        mainBox.Columns = 30;

        Init();
    }

    private void Init()
    {
        int count = GetWindow().Size.Y * mainBox.Columns / 16;

        var rng = new RandomNumberGenerator();
        var buttonTypes = System.Enum.GetValues<Activity.ButtonType>();

        for (int i = 0; i < count; i++)
        {
            var randomType = buttonTypes[rng.RandiRange(0, buttonTypes.Length - 1)];
            var btn = activity.GetButton(randomType).Duplicate() as TextureButton;
            HandleButton(btn, randomType);

            copies.Add(btn);
            mainBox.AddChild(copies[i]);
        }
    }

    private void HandleButton(TextureButton btn, Activity.ButtonType type)
    {
        if (type is Activity.ButtonType.BUTTON or Activity.ButtonType.EXPLODE)
        {
            btn.GuiInput += @event =>
            {
                if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
                {
                    if (mouseEvent.ButtonIndex == MouseButton.Right)
                    {
                        stateIndex = (stateIndex + 1) % 3;
                        var type = states[stateIndex];

                        btn.TextureNormal = activity.GetTexture(type);
                        return;
                    }

                    if (mouseEvent.ButtonIndex == MouseButton.Left)
                    {
                        if (stateIndex > 0)
                            return;

                        btn.Disabled = true;
                        GD.PrintRich(
                            $"[color=#eb7821]LEFT CLICKED {btn.Disabled} Button Type {type} [/color]"
                        );
                    }
                }
            };
        }
    }
}
