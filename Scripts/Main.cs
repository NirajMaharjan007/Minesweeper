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

    // private float Height => GetWindow().Size.Y;

    // Called when the node enters the scene tree for the first time.
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

        GD.Print($"Count {count}");

        System.Collections.Generic.List<TextureButton> copies = [];

        for (int i = 0; i < count; i++)
        {
            var btn = activity.GetButton("Button").Duplicate() as TextureButton;
            btn.Pressed += () => btn.Disabled = true;

            copies.Add(btn);

            mainBox.AddChild(copies[i]);
        }

        // hBox.AddChild(copy);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }
}
