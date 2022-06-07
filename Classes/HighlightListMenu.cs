using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

namespace Monomino.Classes;

public class HighlightListMenu
{
    protected MenuOption[] MenuOptions;
    protected int HighlightChangeDuration { get; set; } = 50;
    protected int TimeSinceLastChange { get; set; } = 0; // ms
    protected int ElapsedTime { get; set; } = 0;
    protected int HighlightIndex { get; set; } = 0;
    protected int PreviousHighlightIndex { get; set; } = 0;
    protected int TopOfMenu { get; set; } = 0;
    protected int LeftOfMenu { get; set; } = 0;
    protected Color HighlightColor
    {
        set
        {
            Highlight.SetData(new[] { value });
        }
    }
    protected enum TextAlignment
    {
        Left,
        Center,
        Right
    }
    private TextAlignment alignment;
    protected TextAlignment Alignment
    {
        get => alignment;
        set => SetTextAlignment(value);
    }

    protected SpriteFont Font;
    protected Texture2D Highlight;

    protected HighlightListMenu(GraphicsDevice graphicsDevice)
    {
        Font = LogManager.Load<SpriteFont>("Fonts/Roboto-Regular", App.contentManager);

        Highlight = new Texture2D(graphicsDevice, 1, 1);
        Highlight.SetData(new[] { Color.Black });
    }

    protected void SetTextAlignment(TextAlignment alignment)
    {
        this.alignment = alignment;
        switch (alignment)
        {
            case TextAlignment.Left:
                break;
            case TextAlignment.Right:
                break;
            case TextAlignment.Center:
                break;
        }
    }

    public float GetWidthOfLongestOption()
    {
        return MenuOptions
            .Select(x => Font.MeasureString(x.text).X / 2)
            .OrderByDescending(x => x)
            .FirstOrDefault();
    }

    public float GetHeightOfMenu()
    {
        float height = MenuOptions.Sum(opt => Font.MeasureString(opt.text).Y + 5);

        return height / 2.0F;
    }

    public Rectangle GetHighlightRectangle()
    {
        float percent = Math.Min((float)this.TimeSinceLastChange / (float)this.HighlightChangeDuration, 1.0F);
        int height = (int)this.Font.LineSpacing / 2;

        // Rectangle calculations
        int distance = (this.PreviousHighlightIndex - this.HighlightIndex) * height;
        int previousLength = (int)Font.MeasureString(this.MenuOptions[this.PreviousHighlightIndex].text).X / 2;
        int currentLength = (int)Font.MeasureString(this.MenuOptions[this.HighlightIndex].text).X / 2;
        int width = (int)(previousLength - (previousLength - currentLength) * percent) + 10;
        int x = LeftOfMenu;
        int y = TopOfMenu + (height * this.PreviousHighlightIndex) - (int)(distance * percent);

        return new Rectangle(x, y, width, height);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        int fontHeight = (int)this.Font.LineSpacing / 2;

        // Draw highlight
        spriteBatch.Draw(Highlight, GetHighlightRectangle(), Color.White);

        Func<int, int, Vector2> GetTextPosition = (longest, i) =>
        {
            var textWidth = Font.MeasureString(MenuOptions[i].text).X / 2;
            switch (alignment)
            {
                case TextAlignment.Left:
                    return new Vector2(LeftOfMenu + 5, TopOfMenu + (fontHeight * i));
                case TextAlignment.Center:
                    return new Vector2(LeftOfMenu + 5 + (longest / 2) - (textWidth / 2), TopOfMenu + (fontHeight * i));
                case TextAlignment.Right:
                    return new Vector2(LeftOfMenu + 5 + longest - textWidth, TopOfMenu + (fontHeight * i));
            }
            return Vector2.Zero;
        };

        // Draw menu options:
        var longest = (int)this.GetWidthOfLongestOption();
        for (int i = 0; i < this.MenuOptions.Length; i++)
        {
            var position = GetTextPosition(longest, i);
            int colorValue = (int)Math.Min(255.0, (255.0 * (0.5 * Math.Cos((double)ElapsedTime / 500.0) + 0.5)));
            var textColor = MenuOptions[i].enabled ?
                (i == HighlightIndex ? new Color(colorValue, colorValue, colorValue, 255) : Color.White)
                : Color.Gray;

            spriteBatch.DrawString(Font, MenuOptions[i].text, position, textColor, 0.0F, Vector2.Zero, 0.5F, SpriteEffects.None, 0);
        }
    }

    public void Update(GameTime gameTime)
    {
        TimeSinceLastChange += gameTime.ElapsedGameTime.Milliseconds;
        ElapsedTime += gameTime.ElapsedGameTime.Milliseconds;

        if (InputManager.GetKeyState("Down") == 1)
        {
            PreviousHighlightIndex = HighlightIndex;
            HighlightIndex = ++HighlightIndex % MenuOptions.Length;
            TimeSinceLastChange = 0;
        }
        else if (InputManager.GetKeyState("Up") == 1)
        {
            PreviousHighlightIndex = HighlightIndex;
            HighlightIndex = (HighlightIndex + MenuOptions.Length - 1) % MenuOptions.Length;
            TimeSinceLastChange = 0;
        }

    }
}
