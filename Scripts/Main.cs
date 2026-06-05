using Godot;

namespace Minesweeper.Scripts;

public partial class Main : Control
{
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

    private readonly System.Collections.Generic.HashSet<int> clickedIndices = [];

    private readonly Activity.ButtonType[] states =
    [
        Activity.ButtonType.BUTTON,
        Activity.ButtonType.REDFLAG,
        Activity.ButtonType.QUESTIONMARK,
    ];

    private readonly System.Collections.Generic.List<TextureButton> copies = [];

    private int[] adjacentBombs;

    public override void _Ready()
    {
        activity = GetNode<Activity>("Activity");

        VBoxContainer container = GetNode<VBoxContainer>("VBoxContainer");

        mainBox = container.GetNode<GridContainer>("MainBox");
        mainBox.Columns = definition.GetCalculateColumn(definition.GridProperty);

        Init();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    private void Init()
    {
        var window = GetWindow();
        window.ContentScaleSize = definition.GetCalculateSize(definition.GridProperty);
        window.ContentScaleMode = Window.ContentScaleModeEnum.Viewport;
        window.ContentScaleAspect = Window.ContentScaleAspectEnum.Keep;

        int count = window.ContentScaleSize.Y * mainBox.Columns / 16;

        int bombCount = definition.GetCalculatedBomb(definition.GridProperty);

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

        CalculateAdjacentBombs();

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
                if (clickedIndices.Contains(i))
                    // already revealed, leave as is
                    continue;
                else
                    copies[i].TextureDisabled = Activity.GetTexture(Activity.ButtonType.DISABLED);
                copies[i].Disabled = true;
            }
        }
    }

    private void CalculateAdjacentBombs()
    {
        adjacentBombs = new int[copies.Count];
        int cols = mainBox.Columns;
        int rows = copies.Count / cols;

        for (int i = 0; i < copies.Count; i++)
        {
            if (bombIndices.Contains(i))
                continue;

            int x = i % cols;
            int y = i / cols;
            int count = 0;

            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    if (dx == 0 && dy == 0)
                        continue;
                    int nx = x + dx;
                    int ny = y + dy;
                    if (nx < 0 || nx >= cols || ny < 0 || ny >= rows)
                        continue;
                    int neighbor = ny * cols + nx;
                    if (bombIndices.Contains(neighbor))
                        count++;
                }
            }

            adjacentBombs[i] = count;
        }
    }

    private void RevealCell(int startIndex)
    {
        if (startIndex < 0 || startIndex >= copies.Count)
            return;
        if (bombIndices.Contains(startIndex))
            return;

        System.Collections.Generic.Queue<int> toReveal = new();
        toReveal.Enqueue(startIndex);

        int cols = mainBox.Columns;
        int rows = copies.Count / cols;

        while (toReveal.Count > 0)
        {
            int index = toReveal.Dequeue();

            if (clickedIndices.Contains(index))
                continue;
            if (bombIndices.Contains(index))
                continue;

            clickedIndices.Add(index);
            var btn = copies[index];
            btn.Disabled = true;

            int bombs = adjacentBombs[index];

            if (bombs > 0)
            {
                var numberType = (Activity.ButtonType)(
                    (int)Activity.ButtonType.NUMBER_ONE + bombs - 1
                );
                btn.TextureNormal = Activity.GetTexture(numberType);
                btn.TextureDisabled = Activity.GetTexture(numberType);
                continue;
            }

            // Empty cell
            btn.TextureNormal = Activity.GetTexture(Activity.ButtonType.REVEALED);
            btn.TextureDisabled = Activity.GetTexture(Activity.ButtonType.REVEALED);

            int x = index % cols;
            int y = index / cols;

            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    if (dx == 0 && dy == 0)
                        continue;
                    int nx = x + dx;
                    int ny = y + dy;
                    if (nx < 0 || nx >= cols || ny < 0 || ny >= rows)
                        continue;

                    int neighborIndex = ny * cols + nx;
                    if (
                        !clickedIndices.Contains(neighborIndex)
                        && !bombIndices.Contains(neighborIndex)
                    )
                    {
                        toReveal.Enqueue(neighborIndex);
                    }
                }
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
                            && flagCount >= definition.GetCalculatedBomb(definition.GridProperty)
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

                        var index = copies.IndexOf(btn);
                        //clickedIndices.Add(index);

                        if (type == Activity.ButtonType.EXPLODE)
                            RevealAllBombs(btn);
                        else
                            RevealCell(index);

                        GD.PrintRich(
                            $"[color=#eb7821]LEFT CLICKED {btn.Disabled} Button Type {type} [/color]"
                        );
                    }
                }
            };
        }
    }
}
