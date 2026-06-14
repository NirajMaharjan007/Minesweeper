using Godot;
using Minesweeper.Misc;

namespace Minesweeper.Scripts;

public partial class Main : Control
{
    Activity activity;
    GridContainer mainBox;
    VBoxContainer container;

    PanelContainer outerPanel;

    Button back,
        retry,
        exit;

    Label timer,
        status;

    private float elapsed = 0f;

    private int flagCount = 0;

    private readonly System.Collections.Generic.HashSet<int> bombIndices = [];

    private readonly Definition definition = Definition.Instance;

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
        definition.GameStateProperty = Definition.GameState.PLAYING;

        activity = GetNode<Activity>("Activity");

        outerPanel = GetNode<PanelContainer>("OutterPanel");

        container = outerPanel.GetNode<VBoxContainer>("VBoxContainer");

        timer = container
            .GetNode<PanelContainer>("BottomPanel")
            .GetNode<HBoxContainer>("TimerContainer")
            .GetNode<Label>("Counter");

        status = container
            .GetNode<PanelContainer>("BottomPanel")
            .GetNode<HBoxContainer>("HBoxContainer")
            .GetNode<Label>("Status");

        status.Text = string.Empty;

        back = container
            .GetNode<PanelContainer>("MainPanel")
            .GetNode<HBoxContainer>("HBoxContainer")
            .GetNode<Button>("Back");

        back.Pressed += HandleBack;

        retry = container
            .GetNode<PanelContainer>("MainPanel")
            .GetNode<HBoxContainer>("HBoxContainer")
            .GetNode<Button>("Retry");

        retry.Pressed += HandleRetry;

        exit = container
            .GetNode<PanelContainer>("MainPanel")
            .GetNode<HBoxContainer>("HBoxContainer")
            .GetNode<Button>("Exit");

        exit.Pressed += HandleExit;

        mainBox = container.GetNode<GridContainer>("MainBox");
        mainBox.Columns = Definition.GetCalculateColumn(definition.GridProperty);

        CallDeferred(MethodName.Init);
    }

    public override void _Process(double delta)
    {
        if (definition.GameStateProperty is not Definition.GameState.PLAYING)
        {
            status.Text = Definition.GetDescriptionState(definition.GameStateProperty);
            return;
        }
        elapsed += (float)delta;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        int minutes = (int)elapsed / 60;
        int seconds = (int)elapsed % 60;
        timer.Text = $"{minutes:00}:{seconds:00}";
    }

    private void Init()
    {
        var window = GetWindow();
        window.ContentScaleSize = Definition.GetCalculateSize(definition.GridProperty);
        window.ContentScaleMode = Window.ContentScaleModeEnum.CanvasItems;
        window.ContentScaleAspect = Window.ContentScaleAspectEnum.Expand;

        int count = mainBox.GetWindow().ContentScaleSize.Y * mainBox.Columns / 16;

        int bombCount = Definition.GetCalculatedBomb(definition.GridProperty);

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
        definition.GameStateProperty = Definition.GameState.LOST;

        GD.Print($"Reveal All Bombs {definition.GameStateProperty}");

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

    private void CheckWinCondition()
    {
        int revealedSafe = 0;
        int totalSafe = copies.Count - bombIndices.Count;

        foreach (var btn in copies)
        {
            int idx = copies.IndexOf(btn);
            bool isBomb = bombIndices.Contains(idx);

            if (btn.Disabled && !isBomb)
                revealedSafe++;
        }

        if (revealedSafe == totalSafe)
        {
            definition.GameStateProperty = Definition.GameState.WON;
            ChangeMines();
        }
    }

    private void ChangeMines()
    {
        foreach (var index in bombIndices)
        {
            var btn = copies[index];
            btn.TextureNormal = Activity.GetTexture(Activity.ButtonType.REDFLAG);
            btn.TextureDisabled = Activity.GetTexture(Activity.ButtonType.REDFLAG);
            btn.Disabled = true;
        }

        // Disable all non-bomb buttons
        for (int i = 0; i < copies.Count; i++)
        {
            if (!bombIndices.Contains(i))
            {
                copies[i].Disabled = true;
            }
        }
    }

    private async void HandleExit()
    {
        await SceneManager.FadeAndExit(outerPanel);
    }

    private async void HandleBack()
    {
        await SceneManager.LoadScene("res://Scenes/Option.tscn", "PanelContainer", outerPanel);
    }

    private async void HandleRetry()
    {
        definition.GameStateProperty = Definition.GameState.PLAYING;
        status.Text = string.Empty;
        await SceneManager.RestartScene(mainBox);
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
                            && flagCount >= Definition.GetCalculatedBomb(definition.GridProperty)
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
                        if (definition.GameStateProperty is not Definition.GameState.LOST)
                            CheckWinCondition();
                        GD.PrintRich(
                            $"[color=#eb7821]LEFT CLICKED {btn.Disabled} Button Type {type} [/color]"
                        );
                    }
                }
            };
        }
    }

    public override void _ExitTree()
    {
        try
        {
            back.Pressed -= HandleBack;
            retry.Pressed -= HandleRetry;
            exit.Pressed -= HandleExit;
        }
        catch (System.Exception e)
        {
            GD.PushWarning("Waring " + e.Message);
        }
        finally
        {
            QueueFree();
        }
    }
}
