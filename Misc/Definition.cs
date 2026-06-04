namespace Minesweeper.Misc;

public sealed class Definition
{
    public enum GridSize
    {
        _9X9,
        _16X16,
        _16X30,
    }

    private static readonly System.Lazy<Definition> instance = new(() => new());

    private readonly System.Collections.Generic.List<GridSize> _items = new()
    {
        GridSize._9X9,
        GridSize._16X16,
        GridSize._16X30,
    };

    private Definition() { }

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

    public int GetCalculatedBomb(GridSize size)
    {
        switch (size)
        {
            case GridSize._9X9:
                return 10;

            case GridSize._16X16:
                return 40;

            case GridSize._16X30:
                return 99;

            default:
                return 0;
        }
    }

    public string GetDifficultStatus(GridSize size)
    {
        switch (size)
        {
            case GridSize._9X9:
                return "Eazy";

            case GridSize._16X16:
                return "Medium";

            case GridSize._16X30:
                return "Hard";

            default:
                return "N/A";
        }
    }

    public static Definition Instance => instance.Value;

    public System.Collections.Generic.List<GridSize> Items => _items;
}
