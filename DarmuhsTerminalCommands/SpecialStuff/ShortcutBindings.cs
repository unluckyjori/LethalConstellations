﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using static TerminalStuff.BoolStuff;
using static TerminalStuff.DynamicCommands;
using static TerminalStuff.NoMoreAPI.TerminalHook;
using Key = UnityEngine.InputSystem.Key;

namespace TerminalStuff
{
    internal class ShortcutBindings
    {
        // Define a dictionary to map keys to actions
        internal static Dictionary<Key, string> keyActions = [];
        internal static List<Key> invalidKeys;
        internal static Key keyBeingPressed;
        public static bool stopForAnyReason;
        internal static bool shortcutListenEnum = false;

        internal static void InitSavedShortcuts()
        {
            Plugin.MoreLogs("Loading shortcuts from config");
            invalidKeys = [
            Key.A, Key.B, Key.C, Key.D, Key.E, Key.F, Key.G, Key.H, Key.I, Key.J,
            Key.K, Key.L, Key.M, Key.N, Key.O, Key.P, Key.Q, Key.R, Key.S, Key.T,
            Key.U, Key.V, Key.W, Key.X, Key.Y, Key.Z, Key.Space
            ];
            DeserializeKeyActions(ConfigSettings.keyActionsConfig.Value);
        }

        private static void DeserializeKeyActions(string serializedData)
        {
            // Clear existing keyActions dictionary
            keyActions.Clear();

            // Deserialize the serialized data into dictionary
            var pairs = serializedData.Split(';');
            foreach (var pair in pairs)
            {
                var keyValue = pair.Split('=');
                if (keyValue.Length == 2 && Enum.TryParse(keyValue[0], out Key key))
                {
                    keyActions[key] = keyValue[1];
                    Plugin.MoreLogs($"Adding shortcut: Key({keyValue[0]}) Command({keyValue[1]})");
                }
            }
        }

        private static string SerializeKeyActions()
        {
            // Serialize the dictionary into a delimited string
            return string.Join(";", keyActions.Select(kv => $"{kv.Key}={kv.Value}"));
        }

        private static void SaveShortcutsToConfig()
        {
            // Serialize the dictionary into a format that can be stored in the configuration
            ConfigSettings.keyActionsConfig.Value = SerializeKeyActions();

            Plugin.MoreLogs("Shortcuts saved to config");
        }

        internal static void UnbindKey(string[] words, int wordCount, out string displayText)
        {
            string invalidInput = "Unable to unbind key.\n\nUsage: unbind <key> \r\nexample: unbind f1 \r\n";

            if (wordCount < 2 || wordCount > 2)
            {
                Plugin.MoreLogs("Invalid amount of words!");
                displayText = invalidInput;
                return;
            }

            Plugin.MoreLogs("Unbind command detected!");
            string givenKey = words[1].ToLower();
            string bindNotFound = $"Unable to find keybinding for key <{givenKey}>";

            if (!IsValidKey(givenKey, invalidKeys))
            {
                Plugin.MoreLogs("Invalid key detected!");
                displayText = bindNotFound;
                return;
            }
            else
            {
                Enum.TryParse(givenKey, ignoreCase: true, out Key keyFromString);
                keyActions.Remove(keyFromString);
                SaveShortcutsToConfig();
                displayText = $"Keybind removed! Key: {givenKey} has been removed from any command mappings.\r\n";
                Plugin.MoreLogs($"Unbound shortcut tied to {givenKey}");
                return;
            }
        }

        internal static void BindToCommand(string[] words, int wordCount, out string displayText)
        {
            string invalidBind = "Unable to bind key to command.\n\nUsage: bind <key> <keyword>\nexample: bind f1 switch\r\n";
            if (wordCount < 3)
            {
                Plugin.MoreLogs("Not enough words detected!");
                displayText = invalidBind;
                return;
            }
            Plugin.MoreLogs("Bind command detected!");
            string givenKey = words[1].ToLower();

            if (wordCount > 3)
            {
                if (!IsValidKey(givenKey, invalidKeys))
                {
                    Plugin.MoreLogs("Invalid key detected!");
                    displayText = invalidBind;
                    return;
                }
                else
                {
                    StringBuilder command = new();
                    for (int i = 2; i < words.Length; i++)
                    {
                        command.Append($"{words[i]} ");
                    }

                    Plugin.MoreLogs($"Command: {command}");
                    Enum.TryParse(givenKey, ignoreCase: true, out Key keyFromString);
                    keyActions.Add(keyFromString, command.ToString());
                    SaveShortcutsToConfig();
                    displayText = $"Keybind created! Key: {givenKey} has been mapped to the following multi-word input: {command}\r\n";
                    Plugin.MoreLogs($"Keybind created mapping {givenKey} to [{command}]");
                    return;
                }
            }

            string givenWord = words[2].ToLower();

            if (!MatchToKeyword(givenWord))
            {
                Plugin.MoreLogs("Invalid word detected!");
                displayText = invalidBind;
                return;
            }
            else if (!IsValidKey(givenKey, invalidKeys))
            {
                Plugin.MoreLogs("Invalid key detected!");
                displayText = invalidBind;
                return;
            }
            else
            {
                Enum.TryParse(givenKey, ignoreCase: true, out Key keyFromString);
                keyActions.Add(keyFromString, givenWord);
                SaveShortcutsToConfig();
                displayText = $"Keybind created! Key: {givenKey} has been mapped to the command: {givenWord}\r\n";
                Plugin.MoreLogs($"Keybind created mapping {givenKey} to {givenWord}");
            }
        }

        private static bool IsValidKey(string key, List<Key> invalidKeys)
        {
            if (Enum.TryParse(key, ignoreCase: true, out Key keyFromString))
            {
                if (invalidKeys.Contains(keyFromString))
                {
                    Plugin.MoreLogs("Alphabetical Key detected, rejecting bind.");
                    return false;
                }
                else if (keyActions.ContainsKey(keyFromString))
                {
                    keyActions.Remove(keyFromString);
                    SaveShortcutsToConfig();
                    Plugin.MoreLogs("Key was already bound, removing bind and returning true");
                    return true;
                }
                else
                {
                    //keyBeingPressed = keyFromString;
                    Plugin.MoreLogs("Valid Key Detected and being assigned to bind");
                    return true;
                }
            }
            else
                return false;
        }

        internal static bool MatchToKeyword(string input)
        {
            TerminalKeyword[] allKeywords = Plugin.instance.Terminal.terminalNodes.allKeywords;

            foreach (TerminalKeyword keyword in allKeywords)
            {
                if (keyword.word.ToLower() == input)
                {
                    return true;
                }
            }

            Plugin.MoreLogs("Unable to find keyword");
            return false;
        }



        internal static void MatchToBind(string input)
        {
            List<string> skipAllKeywords = ["switch"];

            if (BannedWords(input))
            {
                Plugin.MoreLogs("Banned word detected.");
                return;
            }

            TerminalKeyword[] allKeywords = Plugin.instance.Terminal.terminalNodes.allKeywords;
            foreach (TerminalKeyword keyword in allKeywords)
            {
                if (keyword.word == input && !skipAllKeywords.Contains(input))
                {
                    Plugin.MoreLogs("Loading node from Terminal Keywords");
                    Func<string> displayTextSupplier = TerminalEvents.GetCommandDisplayTextSupplier(keyword.specialKeywordResult);

                    if (displayTextSupplier != null)
                    {
                        string displayText = displayTextSupplier();
                        Plugin.MoreLogs("running function related to displaytext supplier");
                        keyword.specialKeywordResult.displayText = displayText;
                    }

                    MoreCamStuff.CamPersistance(keyword.specialKeywordResult.name);
                    MoreCamStuff.VideoPersist(keyword.specialKeywordResult.name);
                    Plugin.instance.Terminal.LoadNewNode(keyword.specialKeywordResult);
                    return;
                }
            }

            string[] words = [input];
            StartofHandling.HandleParsed(Plugin.instance.Terminal, Plugin.instance.Terminal.currentNode, words, out TerminalNode resultNode);

            if (resultNode != null)
            {
                Plugin.MoreLogs($"handling parsed node for shortcut");
                MoreCamStuff.CamPersistance(resultNode.name);
                MoreCamStuff.VideoPersist(resultNode.name);
                Plugin.instance.Terminal.LoadNewNode(resultNode);
                return;
            }

            //"kick", fColor, "fov", Gamble, Lever, "vitalspatch", "bioscanpatch", sColor, Link, Link2, Restart }; // keyword catcher
            //banned words - (word == Gamble || word == "fov" || word == "kick" || word == sColor || word == fColor)
            //remaining words - Lever, "vitalspatch", "bioscanpatch", Link, Link2, Restart
        }

        private static bool BannedWords(string word)
        {
            List<string> bannedWords = [Gamble, "fov", "kick", sColor, fColor, "bind", "unbind"];
            if (bannedWords.Contains(word))
                return true;
            else
                return false;
        }

        // Method to check if any key in the dictionary is pressed
        internal static bool AnyKeyIsPressed()
        {
            foreach (var keyAction in keyActions)
            {
                if (Keyboard.current[keyAction.Key].isPressed)
                {
                    keyBeingPressed = keyAction.Key;
                    Plugin.MoreLogs($"Key detected in use: {keyAction.Key}");
                    return true;
                }
            }
            return false;
        }

        // Method to handle key presses
        private static void HandleKeyPress(Key key)
        {
            // Check if the key exists in the dictionary
            if (keyActions.ContainsKey(key))
            {
                // Get the keyword associated to the key
                keyActions.TryGetValue(key, out string value);

                if (value == string.Empty)
                    return;

                if (value.Contains(" "))
                {
                    SetTerminalInput(value);
                    Plugin.instance.Terminal.OnSubmit();
                    return;
                }
                // Execute the action corresponding to the key
                Plugin.MoreLogs($"Attempting to match given word to keyword: {value}");
                MatchToBind(value);
                return;
            }
            else
                Plugin.Log.LogError("Shortcut KeyActions list not updating properly");
        }


        internal static IEnumerator TerminalShortCuts()
        {
            if (shortcutListenEnum)
                yield break;

            shortcutListenEnum = true;

            //Plugin.MoreLogs("Listening for shortcuts");
            while (Plugin.instance.Terminal.terminalInUse && ConfigSettings.terminalShortcuts.Value)
            {
                if (AnyKeyIsPressed() && ListenForShortCuts())
                {
                    HandleKeyPress(keyBeingPressed);
                    yield return new WaitForSeconds(0.1f);
                }
                else
                    yield return new WaitForSeconds(0.1f);
            }

            if (!Plugin.instance.Terminal.terminalInUse)
                Plugin.MoreLogs("No longer monitoring for shortcuts");

            shortcutListenEnum = false;
            yield break;
        }
    }
}
