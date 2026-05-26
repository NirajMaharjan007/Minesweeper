namespace Minesweeper.Misc;

public sealed class Definition
{
    private static readonly System.Lazy<Definition> instance = new(() => new());

    private Definition() { }

    public enum GridSize
    {
        _9X9,
        _16X16,
        _16X30,
    }

    private readonly System.Collections.Generic.Dictionary<GridSize, int> settings = new()
    {
        { GridSize._9X9, 10 },
        { GridSize._16X16, 40 },
        { GridSize._16X30, 99 },
    };

    public int GetCalculateColumn(GridSize size)
    {
        switch (size)
        {
            case GridSize._9X9:
                return 9;

            case GridSize._16X16:
            case GridSize._16X30:
                return 16;

            default:
                return 0;
        }
    }

    public Godot.Vector2I GetCalculateSize(GridSize size)
    {
        switch (size)
        {
            case GridSize._9X9:
                return new(144, 144);

            case GridSize._16X16:
                return new(256, 256);

            case GridSize._16X30:
                return new(256, 480);

            default:
                return new(0, 0);
        }
    }

    public System.Collections.Generic.Dictionary<GridSize, int> Setting => settings;
    public static Definition Instance => instance.Value;
}
