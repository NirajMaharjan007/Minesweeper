using System.Linq;
using Godot;

namespace Minesweeper.Scripts;

public partial class Main : Control
{
    /*START DEBUUGERR */
    RichTextLabel label;

    /*--------------*/
    Activity activity;
    VBoxContainer mainBox;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        /*----------------*/
        label = GetNode<RichTextLabel>("Debugger");
        /*----------------*/

        activity = GetNode<Activity>("Activity");

        mainBox = GetNode<VBoxContainer>("MainBox");

        Init();
    }

    private void Init()
    {
        label.Text = activity.Options.Length + " Lenght";

        HBoxContainer hBox = new();
        var copy = activity.GetButton("Button").Duplicate() as TextureButton;
        copy.Pressed += () => copy.Disabled = true;

        hBox.AddChild(copy);

        mainBox.AddChild(hBox);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }
}
