using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Destiny
{
    /// <summary>
    /// Command screen class.
    /// 
    /// The command screen enables settings to be configured and commands to be issued.
    /// </summary>
    public class CommandScreen : Screen
    {
        /// <summary>
        /// Content paths.
        /// </summary>
        private string m_BackgroundTexturePath;
        private string m_FontPath;

        /// <summary>
        /// Textures.
        /// </summary>
        private Texture2D m_BackgroundTexture;

        /// <summary>
        /// Fonts.
        /// </summary>
        private SpriteFont m_ConsoleFont;

        /// <summary>
        /// Prompt.
        /// </summary>
        private const string c_Prompt = "> ";

        /// <summary>
        /// Constructor.
        /// <param name="p_TexturePath">Path to the background texture.</param>
        /// <param name="p_FontPath">Path to the font.</param>
        public CommandScreen(string p_BackgroundTexturePath, string p_FontPath)
        {
            m_BackgroundTexturePath = p_BackgroundTexturePath;
            m_FontPath = p_FontPath;
            this.IsPopup = true;
        }

        /// <summary>
        /// Load content.
        /// </summary>
        public override void LoadContent()
        {
            // Load content manager, etc.
            base.LoadContent();

            // Load textures
            m_BackgroundTexture = this.ContentManager.Load<Texture2D>(m_BackgroundTexturePath);
            m_ConsoleFont = this.ContentManager.Load<SpriteFont>(m_FontPath);

            // Once the load has finished, we need to tell the game that we have just
            // finished a very long frame, and that it should not try to catch up
            this.ScreenManager.Game.ResetElapsedTime();
        }

        /// <summary>
        /// Unload content.
        /// </summary>
        public override void UnloadContent()
        {
            // Unload textures
            if (m_BackgroundTexture != null)
            {
                m_BackgroundTexture.Dispose();
                m_BackgroundTexture = null;
            }
            m_ConsoleFont = null;

            // Unload content manager, etc.
            base.UnloadContent();
        }

        /// <summary>
        /// Handle input.
        /// Only called when screen has focus.
        /// </summary>
        public override void HandleInput(InputState p_InputState)
        {
            if (IsHideConsole(p_InputState))
            {
                this.ExitScreen();
            }
            CommandManager.Instance.HandleInput(p_InputState);
        }

        private static bool IsHideConsole(InputState p_InputState)
        {
            return p_InputState.IsOpenCommandConsole();
        }

        /// <summary>
        /// Update screen.
        /// Called regardless of screen state.
        /// </summary>
        /// <param name="p_GameTime">Game time.</param>
        /// <param name="p_HasFocus">Whether screen has focus.</param>
        /// <param name="p_IsCoveredByOtherScreen">Whether screen is covered by another screen.</param>
        public override void Update(GameTime p_GameTime, bool p_HasFocus, bool p_IsCoveredByOtherScreen)
        {
            // Call base class to handle transitions
            base.Update(p_GameTime, p_HasFocus, p_IsCoveredByOtherScreen);
        }

        /// <summary>
        /// Draw screen.
        /// </summary>
        /// <param name="p_GameTime">Game time.</param>
        public override void Draw(GameTime p_GameTime)
        {
            // Start drawing sprites
            SpriteBatch spriteBatch = this.SpriteBatch;
            spriteBatch.Begin();

            //
            // Draw background
            //

            // Move console vertically into position
            int drawAreaY = (int)(this.TransitionPosition * this.Viewport.Height) - this.Viewport.Height;
            Rectangle area = new Rectangle(0, drawAreaY, this.Viewport.Width, this.Viewport.Height);

            byte finalAlpha = (byte)((int)this.TransitionAlpha * 0.5);
            spriteBatch.Draw(m_BackgroundTexture, area, ColorHelper.AddAlpha(Color.White, finalAlpha));

            //
            // Draw the console output
            //

            CommandManager commandManager = CommandManager.Instance;

            // Determine the number of output lines
            // Count the current command
            int totalLines = 1;
            foreach (CommandResult commandResult in commandManager.History)
            {
                // Command plus output
                totalLines += (1 + commandResult.OutputLines.Count);
            }

            // Determine at which line of console history to start writing
            int textSpacing = -2;
            int textHeight = (int)(m_ConsoleFont.MeasureString("W").Y) + textSpacing;
            int maxDisplayLines = area.Height / textHeight;
            int startLine = Math.Max(0, totalLines - maxDisplayLines);

            // Output command history
            int currentLine = 0;
            int textPositionY = 0;
            Color commandColor = new Color((byte)255, (byte)255, (byte)255, this.TransitionAlpha);
            Color outputColor = new Color((byte)0, (byte)255, (byte)0, this.TransitionAlpha);
            CommandResultList history = commandManager.History;
            for (int historyIndex = 0; historyIndex < history.Count; historyIndex++)
            {
                CommandResult commandResult = history[historyIndex];
                if (currentLine >= startLine)
                {
                    // Output command
                    spriteBatch.DrawString(
                        m_ConsoleFont,
                        "> " + commandResult.Command,
                        new Vector2(4, textPositionY),
                        commandColor
                        );
                    textPositionY += textHeight;
                }
                currentLine++;

                // Output command output
                StringList outputLines = commandResult.OutputLines;
                for (int outputIndex = 0; outputIndex < outputLines.Count; outputIndex++)
                {
                    if (currentLine >= startLine)
                    {
                        spriteBatch.DrawString(
                            m_ConsoleFont,
                            outputLines[outputIndex],
                            new Vector2(4, textPositionY),
                            outputColor
                            );
                        textPositionY += textHeight;
                    }
                    currentLine++;
                }
            }

            // Output current command
            Color currentCommandColor = new Color((byte)255, (byte)255, (byte)255, this.TransitionAlpha);
            spriteBatch.DrawString(
                m_ConsoleFont,
                "> " + commandManager.CurrentCommand,
                new Vector2(4, textPositionY),
                currentCommandColor
                );

            // Finish drawing sprites
            spriteBatch.End();
        }
    }
}
