using Godot;

namespace Minesweeper.Scripts;

public partial class MenuController : Control
{
    MenuButton menuButton;

    public override void _Ready()
    {
        base._Ready();

        var menuBar = GetNode<MenuBar>("MenuBar");
        var container = menuBar.GetNode<HBoxContainer>("HBoxContainer");

        menuButton = container.GetNode<MenuButton>("MenuButton");
        Init();
    }

    private void Init()
    {
        if (menuButton is null)
            return;

        PopupMenu popup = menuButton.GetPopup();
        popup.AddItem("Game Option", 0);
        popup.AddItem("TODO", 1);
        popup.AddSeparator();
        popup.AddItem("Exit", 2);

        popup.IdPressed += HandleItems;
    }

    private void HandleItems(long id)
    {
        switch (id)
        {
            case 0:
                GD.Print("Game Option");
                break;
            case 1:
                GD.Print("TODO");
                break;
            case 2:
                System.Environment.Exit(0);
                break;
            default:
                GD.Print($"Item {id} pressed");
                break;
        }
    }
}
