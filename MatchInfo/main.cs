﻿using MelonLoader;
using RUMBLE.Managers;
using RUMBLE.Networking.MatchFlow;
using RumbleModdingAPI;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

namespace MatchInfo
{
    public class main : MelonMod
    {
        private string FILEPATH = @"UserData\MatchInfo\MatchInfo.txt";
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

        public override void OnEarlyInitializeMelon()
        {
            string[] tempStringList = ReadFileText(FILEPATH);
            foreach (string tempString in tempStringList) { fileText.Add(tempString); }
        }

        public override void OnApplicationQuit() { File.WriteAllLines(FILEPATH, fileText); }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
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
                        if (waitedTicks == 120)
                        {
                            //Initialization
                            playerManager = Calls.Managers.GetPlayerManager();
                            matchInfoGameObject = new GameObject();
                            matchInfoGameObject.name = "MatchInfoMod";
                            matchInfoGameObject.SetActive(false);

                            player1GameObject = GameObject.Instantiate(Calls.GameObjects.Gym.Logic.HeinhouserProducts.Leaderboard.PlayerTags.HighscoreTag0.Nr.GetGameObject());
                            player1GameObject.transform.parent = matchInfoGameObject.transform;
                            player1GameObject.name = "Player1Name";

                            player1Component = player1GameObject.GetComponent<TextMeshPro>();
                            player1Component.text = "Name";
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

                            GameObject.DontDestroyOnLoad(matchInfoGameObject);
                            GameObject.DontDestroyOnLoad(gymMatchInfoGameObject);
                            init = true;
                            MelonLogger.Msg("Initialized");
                        }
                        else { waitedTicks++; }
                    }
                    //Stop if not Initialized
                    if (!init) { return; }
                    //Gym Scene Change
                    if (currentScene == "Gym")
                    {
                        matchInfoGameObject.SetActive(false);
                        CountWinLoss();
                        gymMatchInfoGameObject.SetActive(true);
                    }
                    //not gym scene change
                    else
                    {
                        gymMatchInfoGameObject.SetActive(false);
                    }
                    //Matchmaking Scene Change
                    if ((currentScene == "Map0") || (currentScene == "Map1"))
                    {
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
                        player2BPComponent.text = playerManager.AllPlayers[0].Data.GeneralData.BattlePoints.ToString() + " BP";
                        if (playerManager.AllPlayers.Count > 1)
                        {
                            player1Component.text = playerManager.AllPlayers[1].Data.GeneralData.PublicUsername;
                            player1BPComponent.text = playerManager.AllPlayers[1].Data.GeneralData.BattlePoints.ToString() + " BP";
                            bool playerFound = false;
                            for (int spot = 0; spot < fileText.Count; spot += 3)
                            {
                                if (fileText[spot] == playerManager.AllPlayers[1].Data.GeneralData.InternalUsername)
                                {
                                    playerFileSpot = spot;
                                    playerFound = true;
                                    break;
                                }
                            }
                            if (!playerFound)
                            {
                                fileText.Add(playerManager.AllPlayers[1].Data.GeneralData.InternalUsername);
                                fileText.Add("0");
                                fileText.Add("0");
                                playerFileSpot = fileText.Count - 3;
                            }
                            player1MatchWinsComponent.text = fileText[playerFileSpot + 2] + " Wins";
                            player2MatchWinsComponent.text = fileText[playerFileSpot + 1] + " Wins";
                            matchActive = true;
                            ranWinLoss = false;
                        }
                        matchInfoGameObject.SetActive(true);
                        hideTime = DateTime.Now.AddSeconds(7);
                        textActive = true;
                    }
                }
                catch (Exception e) { MelonLogger.Msg(e); return; }
                sceneChanged = false;
            }
            if ((textActive) && (hideTime <= DateTime.Now))
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
                    }
                } catch { return; }
            }
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
        
        //Reads the File and Returns the Lines
        public static string[] ReadFileText(string inputFile)
        {
            if (File.Exists(inputFile))
            {
                try
                {
                    List<string> output = new List<string>();
                    return File.ReadAllLines(inputFile);
                }
                catch (Exception e) { MelonLogger.Error(e); }
            }
            else { MelonLogger.Error($"File not Found: Check Game Folder for {inputFile}"); }
            return null;
        }
    }
}