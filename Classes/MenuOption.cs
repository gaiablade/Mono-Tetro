namespace BinksFarm.Classes;

public class MenuOption
{
    public MenuOption(string text, bool enabled = true)
    {
        this.text = text;
        this.enabled = enabled;
    }

    public string text;
    public bool enabled;
}
