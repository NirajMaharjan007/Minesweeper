using Godot;

namespace Minesweeper.Popup;

public partial class Popup : Window
{
    [Export]
    internal Scripts.Main main;

    private VBoxContainer mainContainer;
    private PanelContainer container;

    public override void _Ready()
    {
        base._Ready();
        CloseRequested += Hide;

        Hide();

        container = GetNode<PanelContainer>("PanelContainer");
        mainContainer = container
            .GetNode<PanelContainer>("InnerContainer")
            .GetNode<VBoxContainer>("MainContainer");

        Init();
    }

    private void Init()
    {
        try
        {
            if (main is not null) { }
            else
            {
                _ = new System.Exception("Error, We should EXIT With ONE");
            }
        }
        catch (System.Exception e)
        {
            GD.PrintErr("ERROR...!!!! " + e.Message);
        }
    }
}
