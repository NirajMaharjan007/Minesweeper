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

    private static readonly Texture2D _redFlagTexture = Activity.GetTexture(
            Activity.ButtonType.REDFLAG
        ),
        _questionTexture = Activity.GetTexture(Activity.ButtonType.QUESTIONMARK);

    private int bombCount = 10;

    private readonly System.Collections.Generic.Dictionary<TextureButton, int> buttonStates = [];

    private readonly Activity.ButtonType[] states =
    [
        Activity.ButtonType.BUTTON,
        Activity.ButtonType.REDFLAG,
        Activity.ButtonType.QUESTIONMARK,
    ];

    private readonly System.Collections.Generic.List<TextureButton> copies = [];

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

        for (int i = 0; i < count; i++)
        {
            var type = Activity.ButtonType.BUTTON;
            var btn = activity.GetButton(Activity.ButtonType.BUTTON).Duplicate() as TextureButton;
            HandleButton(btn, type);

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
                        if (!buttonStates.ContainsKey(btn))
                            buttonStates[btn] = 0;

                        buttonStates[btn] = (buttonStates[btn] + 1) % states.Length;

                        var type = states[buttonStates[btn]];
                        btn.TextureNormal = Activity.GetTexture(type);
                    }

                    if (mouseEvent.ButtonIndex == MouseButton.Left)
                    {
                        var flag =
                            Activity.CompareTextures(btn.TextureNormal, _redFlagTexture)
                            || Activity.CompareTextures(btn.TextureNormal, _questionTexture);

                        if (flag)
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
