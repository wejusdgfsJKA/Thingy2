using System;
using System.IO;
using UnityEngine;
namespace Global
{
    [Serializable]
    public struct GameSave
    {
        public float PlayerPower, EnemyPower;
        public int PlayerKills;
        public static GameSave? Load(string saveFilePath)
        {
            if (!File.Exists(saveFilePath))
            {
                Debug.LogError("No save file found!");
                return null;
            }
            string json = File.ReadAllText(saveFilePath);
            return JsonUtility.FromJson<GameSave>(json);
        }
        public static void Save(string saveFilePath)
        {
            string json = JsonUtility.ToJson(new GameSave(GameManager.PlayerPower, GameManager.EnemyPower, GameManager.PlayerKills));
            File.WriteAllText(saveFilePath, json);
        }
        public GameSave(float currentPlayerPower = 0, float currentEnemyPower = 0, int playerKills = 0)
        {
            PlayerPower = currentPlayerPower;
            EnemyPower = currentEnemyPower;
            PlayerKills = playerKills;
        }
    }
}