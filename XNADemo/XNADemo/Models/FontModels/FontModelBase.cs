using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.IO;
using Microsoft.Xna.Framework;

namespace XNADemo.Models.FontModels
{
    internal class FontModelBase
    {

        private ContentManager ContentManager { get; set; }

        public SpriteFont SpriteFont { get; set; }

        public FontModelBase(ContentManager contentManager, string fontsFolderName, string fontName)
        {
            ContentManager = contentManager;
            FontsFolderName = fontsFolderName;
            FontName = fontName;
        }

        public string FontsFolderName { get; set; }
        public string FontName { get; set; }
        public string FontFileName
        {
            get
            {
                return Path.Combine(FontsFolderName, FontName);
            }
        }

        public virtual void Load()
        {
            SpriteFont = ContentManager.Load<SpriteFont>(FontFileName);
        }

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 position, int rotation, string text, Color color)
        {
            const float defaultLayerDepth = 0.5f;
            const float defaultScale = 1.0f;

            Vector2 fontOrigin = SpriteFont.MeasureString(text) / 2;

            spriteBatch.DrawString(SpriteFont, text, position, color, 
                rotation, fontOrigin, defaultScale, SpriteEffects.None, defaultLayerDepth);
        }

    }
}
