using Destiny;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace OSA
{
    /// <summary>
    /// Scene object content factory class.
    /// </summary>
    public static class SceneObjectContentFactory
    {
        public delegate Texture2D TextureGetter(string p_ContentPath);

        /// <summary>
        /// Load the content of a scene object.
        /// </summary>
        /// <param name="p_ContentPath">Path to content.</param>
        /// <param name="p_ContentManager">Content manager.</param>
        /// <param name="p_GetTextureDelegate">Method to get texture.</param>
        /// <returns>Content of a scene object.</returns>
        public static SceneObjectContent Load(
            string p_ContentPath,
            ContentManager p_ContentManager,
            TextureGetter p_GetTextureDelegate
            )
        {
            XmlSerializer serializer = new XmlSerializer(typeof(OSATypes.Visuals));
#if (MOBILE)
			Stream fs = TitleContainer.OpenStream(Path.Combine(p_ContentManager.RootDirectory, p_ContentPath + ".visuals"));
#else
            FileStream fs = new FileStream(Path.Combine(p_ContentManager.RootDirectory, p_ContentPath + ".visuals"), FileMode.Open);
#endif
            OSATypes.Visuals visualsLoad;
            visualsLoad = (OSATypes.Visuals)serializer.Deserialize(fs);

            // Check that file exists
            if (visualsLoad == null)
            {
                return null;
            }
            OSATypes.VisualsAnimations annimationLoad = new OSATypes.VisualsAnimations();
            OSATypes.VisualsSprites spritesLoad = new OSATypes.VisualsSprites();
            foreach (object item in visualsLoad.Items)
            {
                switch (item.GetType().ToString())
                {
                    case "OSATypes.VisualsAnimations":
                        annimationLoad = (OSATypes.VisualsAnimations)item;
                        break;
                    case "OSATypes.VisualsSprites":
                        spritesLoad = (OSATypes.VisualsSprites)item;
                        break;
                }
            }


            SceneObjectContent content = new SceneObjectContent();

            // Load sprites
            Texture2D texture = null;
            Dictionary<string, Rectangle> nameToTextureRegion = null;
            if (spritesLoad != null)
            {
                // Load texture
                string textureContentPath = spritesLoad.Texture != null ? spritesLoad.Texture : p_ContentPath;
                if (textureContentPath != p_ContentPath)
                {
                    textureContentPath = Path.Combine(Path.GetDirectoryName(p_ContentPath), textureContentPath);
                }
                texture = p_GetTextureDelegate(textureContentPath);

                // Load texture regions
                nameToTextureRegion = new Dictionary<string, Rectangle>();
                for (int i = 0; i < spritesLoad.Sprite.Length; i++)
                {
                    OSATypes.VisualsSpritesSprite xmlSpriteNode = spritesLoad.Sprite[i];
                    string name = xmlSpriteNode.Name != null ? xmlSpriteNode.Name : string.Empty;
                    Rectangle textureRegion = xmlSpriteNode.TextureRegion != null ? OSATypes.Functions.LoadRectangle(xmlSpriteNode.TextureRegion) : Rectangle.Empty;
                    nameToTextureRegion.Add(name, textureRegion);
                }
            }

            // Load animations
            for (int i = 0; i < annimationLoad.Animation.Length; i++)
            {
                SpriteAnimation spriteAnimation = new SpriteAnimation();

                OSATypes.VisualsAnimationsAnimation xmlAnimationNode = annimationLoad.Animation[i];
                string name = xmlAnimationNode.Name != null ? xmlAnimationNode.Name : null;
                string type = xmlAnimationNode.Type != null ? xmlAnimationNode.Type : "Frames";
                Size displayOffset = xmlAnimationNode.DisplayOffset != null ? new Size(int.Parse(xmlAnimationNode.DisplayOffset.Width), int.Parse(xmlAnimationNode.DisplayOffset.Height)) : new Size();

                string sprite = xmlAnimationNode.Sprite != null ? xmlAnimationNode.Sprite : null;
                if (!string.IsNullOrEmpty(sprite))
                {
                    // There is only one frame, add it with zero duration
                    Rectangle textureRegion = nameToTextureRegion[sprite];
                    Sprite animationFrame = new Sprite(texture, textureRegion, displayOffset);
                    spriteAnimation.AddFrame(animationFrame, 0);
                }
                else
                {
                    //  There are many frames, iterate through them
                    for (int j = 0; j < xmlAnimationNode.Frames.Frame.Length; j++)
                    {
                        OSATypes.VisualsAnimationsAnimationFramesFrame xmlFrameNode = xmlAnimationNode.Frames.Frame[j];
                        string frameSprite = xmlFrameNode.Sprite != null ? xmlFrameNode.Sprite : null;
                        int frameDuration = xmlFrameNode.Duration != null ? int.Parse(xmlFrameNode.Duration) : 0;

                        // Add frame
                        Rectangle textureRegion = nameToTextureRegion[frameSprite];
                        Sprite animationFrame = new Sprite(texture, textureRegion, displayOffset);
                        spriteAnimation.AddFrame(animationFrame, frameDuration);
                    }
                }

                // Register sprite animation by name
                content.RegisterSpriteAnimation(name, spriteAnimation);
            }

            return content;
        }
    }
}
