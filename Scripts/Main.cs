using Godot;

namespace Minesweeper.Scripts;

public partial class Main : Control
{
    [Export]
    public Misc.Definition.GridSize size = Misc.Definition.GridSize._9X9;

    /*START DEBUUGERR */
    RichTextLabel label;
    string text;

    /*--------------*/
    Activity activity;
    GridContainer mainBox;

    private int flagCount = 0;

    private readonly System.Collections.Generic.HashSet<int> bombIndices = [];

    private readonly Misc.Definition definition = Misc.Definition.Instance;

    private static readonly Texture2D _redFlagTexture = Activity.GetTexture(
            Activity.ButtonType.REDFLAG
        ),
        _questionTexture = Activity.GetTexture(Activity.ButtonType.QUESTIONMARK);

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

        VBoxContainer container = GetNode<VBoxContainer>("VBoxContainer");

        mainBox = container.GetNode<GridContainer>("MainBox");
        mainBox.Columns = definition.GetCalculateColumn(size);

        Init();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    private void Init()
    {
        var window = GetWindow();
        window.ContentScaleSize = definition.GetCalculateSize(size);
        window.ContentScaleMode = Window.ContentScaleModeEnum.Viewport;
        window.ContentScaleAspect = Window.ContentScaleAspectEnum.Keep;

        int count = window.ContentScaleSize.Y * mainBox.Columns / 16;

        int bombCount = definition.GetCalculatedBomb(size);

        var rng = new RandomNumberGenerator();

        while (bombIndices.Count < bombCount)
            bombIndices.Add(rng.RandiRange(0, count - 1));

        for (int i = 0; i < count; i++)
        {
            var type = bombIndices.Contains(i)
                ? Activity.ButtonType.EXPLODE
                : Activity.ButtonType.BUTTON;

            var btn = activity.GetButton(type).Duplicate() as TextureButton;

            HandleButton(btn, type);

            copies.Add(btn);
            mainBox.AddChild(copies[i]);
        }

        GD.Print(
            $"Total copies {copies.Count} Grid Columns {mainBox.Columns} Bomb-count {bombCount}"
        );
    }

    private void RevealAllBombs(TextureButton clicked)
    {
        GD.Print("Reveal All Bombs");

        // Handle bomb buttons
        foreach (var index in bombIndices)
        {
            var btn = copies[index];

            if (btn == clicked)
            {
                // Clicked bomb shows EXPLODE
                btn.TextureNormal = Activity.GetTexture(Activity.ButtonType.EXPLODE);
                btn.TextureDisabled = Activity.GetTexture(Activity.ButtonType.EXPLODE);
            }
            else
            {
                // Other bombs show REVEALEDBOMB
                btn.TextureNormal = Activity.GetTexture(Activity.ButtonType.REVEALEDBOMB);
                btn.TextureDisabled = Activity.GetTexture(Activity.ButtonType.REVEALEDBOMB);
            }

            btn.Disabled = true;
        }

        // Disable all non-bomb buttons
        for (int i = 0; i < copies.Count; i++)
        {
            if (!bombIndices.Contains(i))
            {
                copies[i].TextureDisabled = Activity.GetTexture(Activity.ButtonType.DISABLED);
                copies[i].Disabled = true;
            }
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
                        //if (!buttonStates.ContainsKey(btn))
                        //buttonStates[btn] = 0;

                        //buttonStates[btn] = (buttonStates[btn] + 1) % states.Length;

                        //var type = states[buttonStates[btn]];
                        //btn.TextureNormal = Activity.GetTexture(type);

                        if (!buttonStates.ContainsKey(btn))
                            buttonStates[btn] = 0;

                        var currentState = states[buttonStates[btn]];

                        // Block adding flag if at max
                        if (
                            currentState == Activity.ButtonType.BUTTON
                            && flagCount >= definition.GetCalculatedBomb(size)
                        )
                            return;

                        // Track flag count
                        if (currentState == Activity.ButtonType.BUTTON)
                            flagCount++;
                        else if (currentState == Activity.ButtonType.REDFLAG)
                            flagCount--;

                        buttonStates[btn] = (buttonStates[btn] + 1) % states.Length;
                        var newType = states[buttonStates[btn]];
                        btn.TextureNormal = Activity.GetTexture(newType);
                    }

                    if (mouseEvent.ButtonIndex == MouseButton.Left)
                    {
                        var flag =
                            Activity.CompareTextures(btn.TextureNormal, _redFlagTexture)
                            || Activity.CompareTextures(btn.TextureNormal, _questionTexture);

                        if (flag || btn.Disabled)
                            return;

                        btn.Disabled = true;

                        if (type == Activity.ButtonType.EXPLODE)
                            RevealAllBombs(btn);

                        GD.PrintRich(
                            $"[color=#eb7821]LEFT CLICKED {btn.Disabled} Button Type {type} [/color]"
                        );
                    }
                }
            };
        }
    }
}
