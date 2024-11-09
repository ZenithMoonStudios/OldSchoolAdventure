using Destiny;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Text.Json;

namespace OSA
{
    /// <summary>
    /// User profile class.
    /// </summary>
    public class UserProfile
    {
        const string c_ContainerName = "Old School Adventure";
        const string c_FileName = "Save Game.xml";

        public PlayerIndex PlayerIndex { get; private set; }
        public string GamerTag { get; private set; }
        public GameState SavedGameState { get; set; }
        public bool IsLoaded { get; private set; }

        public UserProfile(PlayerIndex p_PlayerIndex, string p_GamerTag)
        {
            this.PlayerIndex = p_PlayerIndex;
            this.GamerTag = p_GamerTag;
            this.IsLoaded = false;
        }

        public void Initialize()
        {
			SavedGameState = LoadGameState();
        }

        public GameState LoadGameState()
        {
            return LoadGameState(c_FileName);
        }

        public static string GetPath(string name) => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, name);
        public static T LoadJson<T>(string name) where T : new()
        {
            T json;
            string jsonPath = GetPath(name);

            if (File.Exists(jsonPath))
            {
                json = JsonSerializer.Deserialize<T>(File.ReadAllText(jsonPath));
            }
            else
            {
                json = new T();
            }

            return json;
        }

        public static void SaveJson<T>(string name, T json)
        {
            string jsonPath = GetPath(name);
            Directory.CreateDirectory(Path.GetDirectoryName(jsonPath));
            string jsonString = JsonSerializer.Serialize(json);
            File.WriteAllText(jsonPath, jsonString);
        }

        public GameState LoadGameState(String filename)
        {
            try
            {
                this.IsLoaded = false;
                this.SavedGameState = LoadJson<GameState>(filename);
                this.IsLoaded = (this.SavedGameState.Level >= 0);
            }
            catch
            {
                MessageBoxScreen.Show(
                    PlatformerGame.Instance.ScreenManager,
                    "Storage Device Failure",
                    "The storage device could not be accessed. You will not be able to load or save.",
                    "OK", null,
                    (isAccept) => { }
                    );
            }
            return this.SavedGameState;
        }

        public bool SaveGameState(GameState p_NewGameState)
        {
            return SaveGameState(p_NewGameState, c_FileName);
        }

        public bool SaveGameState(GameState p_NewGameState, String filename)
        {
            bool result = false;
            filename = filename != null ? filename : c_FileName;
            try
            {
                this.SavedGameState = p_NewGameState;
                SaveJson(filename, this.SavedGameState);
                result = true;
            }
            catch
            {
                // Catch any exceptions, and return failure
            }
            return result;
        }
    }
}
