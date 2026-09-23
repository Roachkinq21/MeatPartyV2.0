using Godot;
using System;

public partial class NoteInteractable : Interactable
{

    private Control _letterbox;
    private RichTextLabel _richtext;
    [Export(PropertyHint.File, "*.txt")]  public string CustomText { get; set; } = "";

    public override void _Ready()
    {
        base._Ready();
        _letterbox = GetNode<Control>("LetterBox");
        _richtext = _letterbox.GetNode<RichTextLabel>("ColorRect/MarginContainer/RichTextLabel");
        _letterbox.Hide();
        _richtext.Text = LoadText();
        
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        base._UnhandledKeyInput(@event);
        if (!Input.IsActionPressed("tab")) return;
        _letterbox.Hide();
        var global = GetNode<Global>("/root/Global");
        global._inMenu = false;
    }

    public String LoadText()
    {
        using var file = FileAccess.Open(CustomText, FileAccess.ModeFlags.Read);
        return file.GetAsText();
    }


    public void _on_interacted(Node body)
    {
        _letterbox.Show();
        var global = GetNode<Global>("/root/Global");
        global._inMenu = true;
    }
}
