using Godot;
using System;
using System.IO;
using FileAccess = Godot.FileAccess;

public partial class LetterBox : Control
{
    [Export(PropertyHint.File, "*.txt")]  public string CustomText { get; set; } = "";

    public RichTextLabel _richText;

    public override void _Ready()
    {
        _richText = GetNode<RichTextLabel>("ColorRect/MarginContainer/RichTextLabel");

        _richText.Text = LoadText();
    }
    
    public String LoadText()
    {
        using var file = FileAccess.Open(CustomText, FileAccess.ModeFlags.Read);
        return file.GetAsText();
    }
}
