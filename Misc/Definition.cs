namespace Minesweeper.Misc;

public sealed class Definition
{
    public enum GridSize
    {
        _9X9,
        _16X16,
        _16X30,
    }

    public enum GameState
    {
        WON,
        LOST,
        PAUSED,
    }

    private static readonly System.Lazy<Definition> instance = new(() => new());
    private GridSize _size;

    private Definition() { }

    public int GetCalculateColumn(GridSize size)
    {
        return size switch
        {
            GridSize._9X9 => 9,
            GridSize._16X16 or GridSize._16X30 => 16,
            _ => 0,
        };
    }

    public Godot.Vector2I GetCalculateSize(GridSize size)
    {
        return size switch
        {
            GridSize._9X9 => new(144, 144),
            GridSize._16X16 => new(256, 256),
            GridSize._16X30 => new(256, 480),
            _ => new(0, 0),
        };
    }

    public int GetCalculatedBomb(GridSize size)
    {
        return size switch
        {
            GridSize._9X9 => 10,
            GridSize._16X16 => 40,
            GridSize._16X30 => 99,
            _ => 0,
        };
    }

    public string GetDifficultStatus(GridSize size)
    {
        return size switch
        {
            GridSize._9X9 => "Eazy",
            GridSize._16X16 => "Medium",
            GridSize._16X30 => "Hard",
            _ => "N/A",
        };
    }

    public System.Collections.Generic.Dictionary<string, string> GetDescriptionState(
        GameState state
    )
    {
        return state switch
        {
            GameState.WON => new()
            {
                { "title", "You Won!" },
                { "body", "Congratulations! All mines cleared." },
                // { "color", "#22eb21" },
            },
            GameState.LOST => new()
            {
                { "title", "Game Over" },
                { "body", "You hit a mine!" },
                // { "color", "#eb2121" },
            },
            GameState.PAUSED => new()
            {
                { "title", "Game Pause!!!!" },
                { "body", "Game Paused!!!!!" },
            },
            _ => new()
            {
                { "title", "" },
                { "body", "" },
                // { "color", "#ffffff" },
            },
        };
    }

    public static Definition Instance => instance.Value;

    public System.Collections.Generic.List<GridSize> Items { get; } =
    [GridSize._9X9, GridSize._16X16, GridSize._16X30];

    public GridSize GridProperty
    {
        set { _size = value; }
        get { return _size; }
    }

    public GameState GameStateProperty { set; get; }
}
