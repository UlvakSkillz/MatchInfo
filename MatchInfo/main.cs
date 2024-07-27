using MelonLoader;
using Il2CppRUMBLE.Managers;
using Il2CppRUMBLE.Networking.MatchFlow;
using RumbleModdingAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Il2CppTMPro;
using UnityEngine;

namespace MatchInfo
{
    public class main : MelonMod
    {
        private string FILEPATH = @"UserData\MatchInfo";
        private string FILENAME = @"MatchInfo.txt";
        private string BACKUPFILENAME = @"MatchInfo_Backup";
        private string SETTINGFILENAME = @"Settings.txt";
        private List<string> fileText = new List<string>();
        private bool sceneChanged = false;
        private string currentScene = "";
        private bool init = false;
        private GameObject matchInfoGameObject, player1GameObject, player2GameObject, vsGameObject, player1BPGameObject, player2BPGameObject, player1MatchWinsGameObject, player2MatchWinsGameObject;
        private TextMeshPro player1Component, player2Component, vsComponent, player1BPComponent, player2BPComponent, player1MatchWinsComponent, player2MatchWinsComponent;
        private GameObject gymMatchInfoGameObject, gymMatchInfoTotalGameObject, gymMatchInfoWinsGameObject, gymMatchInfoLossesGameObject, gymMatchInfoWinPercentGameObject;
        private TextMeshPro gymMatchInfoTitleComponent, gymMatchInfoTotalComponent, gymMatchInfoWinsComponent, gymMatchInfoLossesComponent, gymMatchInfoWinPercentComponent;
        private DateTime hideTime = DateTime.Now;
        private int waitedTicks = 0;
        private bool textActive = false;
        private bool matchActive = false;
        private bool ranWinLoss = true;
        private PlayerManager playerManager;
        private MatchHandler matchHandler;
        private int playerFileSpot = 0;
        private bool showWinRate = true;
        private bool showWinLoss = true;
        private bool backedUp = false;
        bool newSettingsFile = false;

        public override void OnLateInitializeMelon()
        {
            MelonCoroutines.Start(CheckIfFileExists(FILEPATH, FILENAME));
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (init)
            {
                ReadSettingsFile();
            }
            currentScene = sceneName;
            sceneChanged = true;
        }

        public override void OnFixedUpdate()
        {
            if (sceneChanged)
            {
                try
                {
                    if ((currentScene == "Gym") && (!init))
                    {
                        if (!backedUp)
                        {
                            Log("Backing up File 2 -> 3");
                            File.Copy($"{FILEPATH}\\Backups\\{BACKUPFILENAME}2.txt", $"{FILEPATH}\\Backups\\{BACKUPFILENAME}3.txt", true);
                            Log("Backing up File 1 -> 2");
                            File.Copy($"{FILEPATH}\\Backups\\{BACKUPFILENAME}1.txt", $"{FILEPATH}\\Backups\\{BACKUPFILENAME}2.txt", true);
                            Log("Backing up File MatchInfo.txt -> 1");
                            File.Copy($"{FILEPATH}\\{FILENAME}", $"{FILEPATH}\\Backups\\{BACKUPFILENAME}1.txt", true);
                            backedUp = true;
                        }
                        if (newSettingsFile)
                        {
                            string[] settingsFileText = new string[2];
                            settingsFileText[0] = "Show Gym Info: True";
                            settingsFileText[1] = "Show Win/Loss in Match: True";
                            File.WriteAllLines($"{FILEPATH}\\{SETTINGFILENAME}", settingsFileText);
                            newSettingsFile = false;
                        }
                        if (waitedTicks == 120)
                        {
                            //Initialization
                            playerManager = PlayerManager.instance;
                            matchInfoGameObject = new GameObject();
                            matchInfoGameObject.name = "MatchInfoMod";
                            matchInfoGameObject.SetActive(false);

                            player1GameObject = GameObject.Instantiate(Calls.GameObjects.Gym.Logic.HeinhouserProducts.Leaderboard.PlayerTags.HighscoreTag0.Nr.GetGameObject());
                            player1GameObject.transform.parent = matchInfoGameObject.transform;
                            player1GameObject.name = "Player1Name";

                            player1Component = player1GameObject.GetComponent<TextMeshPro>();
                            player1Component.text = "name";
                            player1Component.fontSize = 5f;
                            player1Component.color = new Color(1, 1, 1, 1);
                            player1Component.outlineColor = new Color(0, 0, 0, 0.5f);
                            player1Component.alignment = TextAlignmentOptions.Right;
                            player1Component.enableWordWrapping = false;
                            player1Component.outlineWidth = 0.1f;

                            player2GameObject = GameObject.Instantiate(player1GameObject);
                            player2GameObject.transform.parent = matchInfoGameObject.transform;
                            player2GameObject.name = "Player2Name";

                            player2Component = player2GameObject.GetComponent<TextMeshPro>();
                            player2Component.alignment = TextAlignmentOptions.Left;

                            vsGameObject = GameObject.Instantiate(player2GameObject);
                            vsGameObject.transform.parent = matchInfoGameObject.transform;
                            vsGameObject.name = "VSText";

                            vsComponent = vsGameObject.GetComponent<TextMeshPro>();
                            vsComponent.alignment = TextAlignmentOptions.Center;
                            vsComponent.color = new Color(0, 0, 0, 1);
                            vsComponent.text = "vs";
                            vsComponent.fontSize = 3.5f;

                            player1BPGameObject = GameObject.Instantiate(player2GameObject);
                            player1BPGameObject.transform.parent = matchInfoGameObject.transform;
                            player1BPGameObject.name = "Player1BP";

                            player1BPComponent = player1BPGameObject.GetComponent<TextMeshPro>();
                            player1BPComponent.alignment = TextAlignmentOptions.Center;
                            player1BPComponent.text = "P1BP";
                            player1BPComponent.fontSize = 4f;

                            player2BPGameObject = GameObject.Instantiate(player2GameObject);
                            player2BPGameObject.transform.parent = matchInfoGameObject.transform;
                            player2BPGameObject.name = "Player2BP";

                            player2BPComponent = player2BPGameObject.GetComponent<TextMeshPro>();
                            player2BPComponent.alignment = TextAlignmentOptions.Center;
                            player2BPComponent.text = "P2BP";
                            player2BPComponent.fontSize = 4f;

                            player1MatchWinsGameObject = GameObject.Instantiate(player1BPGameObject);
                            player1MatchWinsGameObject.transform.parent = matchInfoGameObject.transform;
                            player1MatchWinsGameObject.name = "P1Wins";

                            player1MatchWinsComponent = player1MatchWinsGameObject.GetComponent<TextMeshPro>();
                            player1MatchWinsComponent.alignment = TextAlignmentOptions.Center;
                            player1MatchWinsComponent.text = "0 Wins";
                            player1MatchWinsComponent.color = new Color(1, 1, 1, 1);

                            player2MatchWinsGameObject = GameObject.Instantiate(player1MatchWinsGameObject);
                            player2MatchWinsGameObject.transform.parent = matchInfoGameObject.transform;
                            player2MatchWinsGameObject.name = "P2Wins";

                            player2MatchWinsComponent = player2MatchWinsGameObject.GetComponent<TextMeshPro>();
                            player2MatchWinsComponent.alignment = TextAlignmentOptions.Center;
                            player2MatchWinsComponent.text = "0 Wins";

                            gymMatchInfoGameObject = GameObject.Instantiate(player1GameObject);
                            gymMatchInfoGameObject.name = "GymMatchInfo";

                            gymMatchInfoTitleComponent = gymMatchInfoGameObject.GetComponent<TextMeshPro>();
                            gymMatchInfoTitleComponent.alignment = TextAlignmentOptions.Left;
                            gymMatchInfoTitleComponent.text = "Match History";
                            gymMatchInfoTitleComponent.fontSize = 3.5f;

                            gymMatchInfoTotalGameObject = GameObject.Instantiate(gymMatchInfoGameObject);
                            gymMatchInfoTotalGameObject.transform.parent = gymMatchInfoGameObject.transform;
                            gymMatchInfoTotalGameObject.name = "Total";

                            gymMatchInfoTotalComponent = gymMatchInfoTotalGameObject.GetComponent<TextMeshPro>();
                            gymMatchInfoTotalComponent.text = "Total: ";

                            gymMatchInfoWinsGameObject = GameObject.Instantiate(gymMatchInfoTotalGameObject);
                            gymMatchInfoWinsGameObject.transform.parent = gymMatchInfoGameObject.transform;
                            gymMatchInfoWinsGameObject.name = "Wins";

                            gymMatchInfoWinsComponent = gymMatchInfoWinsGameObject.GetComponent<TextMeshPro>();
                            gymMatchInfoWinsComponent.text = "Wins: ";

                            gymMatchInfoLossesGameObject = GameObject.Instantiate(gymMatchInfoTotalGameObject);
                            gymMatchInfoLossesGameObject.transform.parent = gymMatchInfoGameObject.transform;
                            gymMatchInfoLossesGameObject.name = "Losses";

                            gymMatchInfoLossesComponent = gymMatchInfoLossesGameObject.GetComponent<TextMeshPro>();
                            gymMatchInfoLossesComponent.text = "Losses: ";

                            gymMatchInfoWinPercentGameObject = GameObject.Instantiate(gymMatchInfoTotalGameObject);
                            gymMatchInfoWinPercentGameObject.transform.parent = gymMatchInfoGameObject.transform;
                            gymMatchInfoWinPercentGameObject.name = "WinPercent";

                            gymMatchInfoWinPercentComponent = gymMatchInfoWinPercentGameObject.GetComponent<TextMeshPro>();
                            gymMatchInfoWinPercentComponent.text = "Win Rate: ";

                            player1GameObject.transform.position = new Vector3(-0.75f, 1f, 0);
                            player1BPGameObject.transform.position = new Vector3(-1.25f, 0.5f, 0);
                            vsGameObject.transform.position = new Vector3(0, 0.75f, 0);
                            player2GameObject.transform.position = new Vector3(0.75f, 1f, 0);
                            player2BPGameObject.transform.position = new Vector3(1.25f, 0.5f, 0);
                            player1MatchWinsGameObject.transform.position = new Vector3(-1.25f, 0, 0);
                            player2MatchWinsGameObject.transform.position = new Vector3(1.25f, 0, 0);

                            gymMatchInfoGameObject.transform.position = new Vector3(5.5236f, 2.75f, 11.3364f);
                            gymMatchInfoGameObject.transform.rotation = Quaternion.Euler(0, 30.5f, 0);
                            gymMatchInfoTotalGameObject.transform.localPosition = new Vector3(0.0334f, -0.3f, -0.0012f);
                            gymMatchInfoWinsGameObject.transform.localPosition = new Vector3(0.0334f, -0.6f, -0.0012f);
                            gymMatchInfoLossesGameObject.transform.localPosition = new Vector3(0.0334f, -0.9f, -0.0012f);
                            gymMatchInfoWinPercentGameObject.transform.localPosition = new Vector3(0.0334f, -1.2f, -0.0012f);
                            gymMatchInfoGameObject.SetActive(false);
                            
                            GameObject.DontDestroyOnLoad(matchInfoGameObject);
                            GameObject.DontDestroyOnLoad(gymMatchInfoGameObject);
                            init = true;
                            Log("Initialized");
                        }
                        else { waitedTicks++; }
                    }
                    //Stop if not Initialized
                    if (!init) { return; }
                    //Gym Scene Change
                    if (currentScene == "Gym")
                    {
                        matchInfoGameObject.SetActive(false);
                        gymMatchInfoGameObject.SetActive(true);
                        CountWinLoss();
                        if (!showWinRate)
                        {
                            gymMatchInfoGameObject.SetActive(false);
                        }
                    }
                    //not gym scene change
                    else
                    {
                        gymMatchInfoGameObject.SetActive(false);
                    }
                    //Matchmaking Scene Change
                    if ((currentScene == "Map0") || (currentScene == "Map1"))
                    {
                        matchInfoGameObject.SetActive(true);
                        try
                        {
                            if (currentScene == "Map0") { matchHandler = Calls.GameObjects.Map0.Logic.MatchHandler.GetGameObject().GetComponent<MatchHandler>(); }
                            else if (currentScene == "Map1") { matchHandler = Calls.GameObjects.Map1.Logic.MatchHandler.GetGameObject().GetComponent<MatchHandler>(); }
                        } catch { return; }
                        if (Calls.Players.IsHost())
                        {
                            matchInfoGameObject.transform.position = new Vector3(-3, 3f, 0);
                            matchInfoGameObject.transform.rotation = Quaternion.Euler(0, -90, 0);
                        }
                        else
                        {
                            matchInfoGameObject.transform.position = new Vector3(3, 3f, 0);
                            matchInfoGameObject.transform.rotation = Quaternion.Euler(0, 90, 0);
                        }
                        player2Component.text = playerManager.AllPlayers[0].Data.GeneralData.PublicUsername;
                        if (playerManager.AllPlayers[0].Data.GeneralData.InternalUsername == "5832566FD2375E31")
                        {
                            player2Component.text = "<#990099>U<#ff00ff>l<#ff66ff>v<#cc66ff>a<#9966ff>k<#6666ff>S<#3366ff>k<#6699ff>i<#809fff>l<#b3c6ff>l<#e6ecff>z";
                        }
                        player2BPComponent.text = playerManager.AllPlayers[0].Data.GeneralData.BattlePoints.ToString() + " BP";
                        if (playerManager.AllPlayers.Count > 1)
                        {
                            player1Component.text = playerManager.AllPlayers[1].Data.GeneralData.PublicUsername;
                            if (playerManager.AllPlayers[1].Data.GeneralData.InternalUsername == "5832566FD2375E31")
                            {
                                player1Component.text = "<#990099>U<#ff00ff>l<#ff66ff>v<#cc66ff>a<#9966ff>k<#6666ff>S<#3366ff>k<#6699ff>i<#809fff>l<#b3c6ff>l<#e6ecff>z";
                            }
                            player1BPComponent.text = playerManager.AllPlayers[1].Data.GeneralData.BattlePoints.ToString() + " BP";
                            bool playerFound = false;
                            for (int spot = 0; spot < fileText.Count; spot += 3)
                            {
                                if (fileText[spot].Contains(playerManager.AllPlayers[1].Data.GeneralData.InternalUsername))
                                {
                                    if (fileText[spot] == playerManager.AllPlayers[1].Data.GeneralData.InternalUsername)
                                    {
                                        fileText[spot] += " - " + playerManager.AllPlayers[1].Data.GeneralData.PublicUsername;
                                    }
                                    else if (!fileText[spot].Contains(playerManager.AllPlayers[1].Data.GeneralData.PublicUsername))
                                    {
                                        fileText[spot] = playerManager.AllPlayers[1].Data.GeneralData.InternalUsername + " - " + playerManager.AllPlayers[1].Data.GeneralData.PublicUsername;
                                    }
                                    playerFileSpot = spot;
                                    playerFound = true;
                                    break;
                                }
                            }
                            if (!playerFound)
                            {
                                fileText.Add(playerManager.AllPlayers[1].Data.GeneralData.InternalUsername + " - " + playerManager.AllPlayers[1].Data.GeneralData.PublicUsername);
                                fileText.Add("0");
                                fileText.Add("0");
                                playerFileSpot = fileText.Count - 3;
                            }
                            player1MatchWinsGameObject.SetActive(true);
                            player2MatchWinsGameObject.SetActive(true);
                            player1MatchWinsComponent.text = fileText[playerFileSpot + 2] + " Wins";
                            player2MatchWinsComponent.text = fileText[playerFileSpot + 1] + " Wins";
                            if (!showWinLoss)
                            {
                                player1MatchWinsGameObject.SetActive(false);
                                player2MatchWinsGameObject.SetActive(false);
                            }
                            matchActive = true;
                            ranWinLoss = false;
                            hideTime = DateTime.Now.AddSeconds(7);
                            textActive = true;
                        }
                        else
                        {
                            MelonCoroutines.Start(OnePlayerFound());
                        }
                    }
                }
                catch { return; }
                sceneChanged = false;
            }
            if ((textActive) && (hideTime <= DateTime.Now))
            {
                MoveSign();
                textActive = false;
            }
            if (matchActive)
            {
                try
                {
                    if ((!ranWinLoss) && (playerManager.AllPlayers.Count == 2) && (matchHandler.CurrentMatchPhase == MatchHandler.MatchPhase.WinnerDecided))
                    {
                        bool won = false;
                        string winOrLose = "";
                        if (currentScene == "Map0")
                        {
                            if (Calls.Players.IsHost())
                            {
                                winOrLose = Calls.GameObjects.Map0.Logic.MatchSlabOne.MatchSlab.SlabBuddyMatchVariant.MatchForm.MatchFormCanvas.MatchText.GetGameObject().GetComponent<TextMeshProUGUI>().text;
                            }
                            else
                            {
                                winOrLose = Calls.GameObjects.Map0.Logic.MatchSlabTwo.MatchSlab.SlabBuddyMatchVariant.MatchForm.MatchFormCanvas.MatchText.GetGameObject().GetComponent<TextMeshProUGUI>().text;
                            }
                        }
                        else if (currentScene == "Map1")
                        {
                            if (Calls.Players.IsHost())
                            {
                                winOrLose = Calls.GameObjects.Map1.Logic.MatchSlabOne.MatchSlab.SlabBuddyMatchVariant.MatchForm.MatchFormCanvas.MatchText.GetGameObject().GetComponent<TextMeshProUGUI>().text;
                            }
                            else
                            {
                                winOrLose = Calls.GameObjects.Map1.Logic.MatchSlabTwo.MatchSlab.SlabBuddyMatchVariant.MatchForm.MatchFormCanvas.MatchText.GetGameObject().GetComponent<TextMeshProUGUI>().text;
                            }
                        }
                        if (winOrLose == "")
                        {
                            return;
                        }
                        if (winOrLose == "Victory!")
                        {
                            won = true;
                        }
                        else
                        {
                            won = false;
                        }
                        if (won)
                        {
                            fileText[playerFileSpot + 1] = (int.Parse(fileText[playerFileSpot + 1]) + 1).ToString();
                            player2MatchWinsComponent.text = fileText[playerFileSpot + 1] + " Wins";
                        }
                        else
                        {
                            fileText[playerFileSpot + 2] = (int.Parse(fileText[playerFileSpot + 2]) + 1).ToString();
                            player1MatchWinsComponent.text = fileText[playerFileSpot + 2] + " Wins";
                        }
                        player2BPComponent.text = playerManager.AllPlayers[0].Data.GeneralData.BattlePoints.ToString() + " BP";
                        player1BPComponent.text = playerManager.AllPlayers[1].Data.GeneralData.BattlePoints.ToString() + " BP";
                        ranWinLoss = true;
                        File.WriteAllLines($"{FILEPATH}\\{FILENAME}", fileText);
                    }
                } catch { return; }
            }
        }

        public void MoveSign()
        {
            if (currentScene == "Map0")
            {
                matchInfoGameObject.transform.position = new Vector3(-1, -0.5f, 20.5f);
                matchInfoGameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else if (currentScene == "Map1")
            {
                matchInfoGameObject.transform.position = new Vector3(0, 5.25f, 11);
                matchInfoGameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }

        public IEnumerator OnePlayerFound()
        {
            for (int i = 0; i < 200; i++)
            {
                yield return new WaitForFixedUpdate();
            }
            matchInfoGameObject.SetActive(false);
        }

        public void CountWinLoss()
        {
            int wins = 0;
            int losses = 0;
            for (int spot = 0; spot < fileText.Count; spot += 3)
            {
                wins += int.Parse(fileText[spot + 1]);
                losses += int.Parse(fileText[spot + 2]);
            }
            gymMatchInfoTotalComponent.text = "Total: " + (wins + losses);
            gymMatchInfoWinsComponent.text = "Wins: " + wins;
            gymMatchInfoLossesComponent.text = "Losses: " + losses;
            gymMatchInfoWinPercentComponent.text = "Win Rate: " + ((float)(int)(wins / (float)(wins + losses) * 10000) / 100).ToString() + "%";
            if (wins + losses == 0) { gymMatchInfoWinPercentComponent.text = "Win Rate: N/A"; }
        }

        public IEnumerator CheckIfFileExists(string filePath, string fileName)
        {
            bool newMatchInfoFile = false;
            if ((!File.Exists($"{filePath}\\{fileName}")) || (!File.Exists($"{filePath}\\{SETTINGFILENAME}")) || (!File.Exists($"{filePath}\\Backups\\{BACKUPFILENAME}1.txt")) || (!File.Exists($"{filePath}\\Backups\\{BACKUPFILENAME}2.txt")) || (!File.Exists($"{filePath}\\Backups\\{BACKUPFILENAME}3.txt")))
            {
                if (!Directory.Exists(filePath))
                {
                    Log($"Folder Not Found, Creating Folder: {filePath}");
                    Directory.CreateDirectory(filePath);
                }
                if (!File.Exists($"{filePath}\\{fileName}"))
                {
                    Log($"Creating File {fileName}");
                    File.Create($"{filePath}\\{fileName}");
                    newMatchInfoFile = true;
                }
                if (!File.Exists($"{filePath}\\{SETTINGFILENAME}"))
                {
                    newSettingsFile = true;
                    Log($"Creating File {SETTINGFILENAME}");
                    File.Create($"{filePath}\\{SETTINGFILENAME}");
                }
                if (!Directory.Exists(filePath))
                {
                    Log($"Folder Not Found, Creating Folder: {filePath}");
                    Directory.CreateDirectory(filePath);
                }
                if (!Directory.Exists($"{filePath}\\Backups"))
                {
                    Log($"Folder Not Found, Creating Folder: {filePath}\\Backups");
                    Directory.CreateDirectory($"{filePath}\\Backups");
                }
                if (!File.Exists($"{filePath}\\Backups\\{BACKUPFILENAME}1.txt"))
                {
                    Log($"Creating File {BACKUPFILENAME}1.txt");
                    File.Create($"{filePath}\\Backups\\{BACKUPFILENAME}1.txt");
                }
                if (!File.Exists($"{filePath}\\Backups\\{BACKUPFILENAME}2.txt"))
                {
                    Log($"Creating File {BACKUPFILENAME}2.txt");
                    File.Create($"{filePath}\\Backups\\{BACKUPFILENAME}2.txt");
                }
                if (!File.Exists($"{filePath}\\Backups\\{BACKUPFILENAME}3.txt"))
                {
                    Log($"Creating File {BACKUPFILENAME}3.txt");
                    File.Create($"{filePath}\\Backups\\{BACKUPFILENAME}3.txt");
                }
            }
            for (int i = 0; i < 60; i++) { yield return new WaitForFixedUpdate(); }
            if (!newSettingsFile)
            {
                ReadSettingsFile();
            }
            if (!newMatchInfoFile)
            {
                string[] tempStringList = ReadFileText(FILEPATH, FILENAME);
                foreach (string tempString in tempStringList) { fileText.Add(tempString); }
            }
            yield return null;
        }

        public void ReadSettingsFile()
        {
            try
            {
                string[] tempSettingsFileText = ReadFileText(FILEPATH, SETTINGFILENAME);
                showWinRate = !tempSettingsFileText[0].ToLower().Contains("false");
                showWinLoss = !tempSettingsFileText[1].ToLower().Contains("false");
            }
            catch (Exception e)
            {
                MelonLogger.Error("Error Reading Settings File: " + e);
            }
        }
        
        //Reads the File and Returns the Lines
        public static string[] ReadFileText(string filePath, string fileName)
        {
            try { return File.ReadAllLines($"{filePath}\\{fileName}"); }
            catch (Exception e) { MelonLogger.Error(e); }
            return null;
        }

        public static void Log(string msg)
        {
            MelonLogger.Msg(msg);
        }
    }
}
