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

    System.Collections.Generic.List<TextureButton> copies = [];

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

    private static void HandleButton(TextureButton btn, Activity.ButtonType type)
    {
        if (type.Equals(Activity.ButtonType.BUTTON) || type.Equals(Activity.ButtonType.EXPLODE))
            btn.Pressed += () => btn.Disabled = true;
    }
}
