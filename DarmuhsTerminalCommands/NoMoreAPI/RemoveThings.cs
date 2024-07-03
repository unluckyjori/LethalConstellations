﻿using System;
using System.Collections.Generic;
using static TerminalStuff.NoMoreAPI.TerminalHook;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalStuff.NoMoreAPI
{
    internal class RemoveThings
    {
        internal static void DeleteNounWord(ref TerminalKeyword keyWord, string terminalKeyword)
        {

            List<CompatibleNoun> keywordList = [.. keyWord.compatibleNouns];
            List<CompatibleNoun> nounsToRemove = [];
            foreach (CompatibleNoun compatibleNoun in keywordList)
            {
                if(compatibleNoun.noun.word.ToLower() == terminalKeyword.ToLower())
                {
                    nounsToRemove.Add(compatibleNoun);
                }
            }

            for (int i = nounsToRemove.Count - 1; i >= 0; i--)
            {
                Plugin.Spam($"Deleting noun: {nounsToRemove[i].noun.word} from word: {keyWord.word}");
                UnityEngine.Object.Destroy(nounsToRemove[i].noun);
            }

            keyWord.compatibleNouns = [.. keywordList];
        }


        internal static void DeleteAllKeywords(ref List<TerminalKeyword> keywordList)
        {
            if (keywordList.Count == 0)
                return;

            List<TerminalKeyword> wordsToDelete = keywordList;
            List<TerminalKeyword> allKeywords = [.. Plugin.instance.Terminal.terminalNodes.allKeywords];

            foreach (TerminalKeyword keyword in keywordList)
            {
                if (TryGetKeyword("buy", out TerminalKeyword buyKeyword))
                {
                    DeleteNounWord(ref buyKeyword, keyword.word);
                }

                for (int i = allKeywords.Count - 1; i >= 0; i--)
                {
                    if (allKeywords[i] == keyword)
                    {
                        Plugin.Spam($"Removing {keyword.word} from all keywords list");
                        allKeywords.RemoveAt(i);
                        break;
                    }     
                }
            }

            for (int i = wordsToDelete.Count - 1; i >= 0; i--)
            {
                Plugin.Spam($"Deleting keyword Object: {wordsToDelete[i].word}");
                UnityEngine.Object.Destroy(wordsToDelete[i]);
            }

            keywordList.Clear();
            Plugin.instance.Terminal.terminalNodes.allKeywords = [.. allKeywords];
        }

        internal static void DeleteAllNodes(ref Dictionary<TerminalNode, int> nodeDictionary) //viewtermnodes
        {
            List<TerminalNode> nodesToDelete = [];

            foreach (KeyValuePair<TerminalNode, int> item in nodeDictionary)
            {
                nodesToDelete.Add(item.Key);
            }

            nodeDictionary.Clear();

            for (int i = nodesToDelete.Count - 1; i >= 0; i--)
            {
                Plugin.Spam($"Deleting node: {nodesToDelete[i].name}");
                UnityEngine.Object.Destroy(nodesToDelete[i]);
            }
        }

        internal static void DeleteAllNodes(ref Dictionary<TerminalNode, Func<string>> nodeDictionary) //all nodes
        {
            if (nodeDictionary.Count == 0)
                return;

            List<TerminalNode> nodesToDelete = [];

            foreach (KeyValuePair<TerminalNode, Func<string>> item in nodeDictionary)
            {
                nodesToDelete.Add(item.Key);
            }

            nodeDictionary.Clear();

            for (int i = nodesToDelete.Count - 1; i >= 0; i--)
            {
                Plugin.Spam($"Deleting node: {nodesToDelete[i].name}");
                UnityEngine.Object.Destroy(nodesToDelete[i]);
            }
        }

        internal static void DeleteMatchingNode(string nodeName)
        {
            TerminalNode[] allTerminalNodes = UnityEngine.Object.FindObjectsOfType<TerminalNode>();

            List<TerminalNode> allNodesList = [.. allTerminalNodes];

            for (int i = allNodesList.Count - 1; i >= 0; i--)
            {
                if (allNodesList[i].name.Equals(nodeName))
                {
                    UnityEngine.Object.Destroy(allNodesList[i]);
                    //Plugin.MoreLogs($"Keyword: [{keyWord}] removed");
                    break;
                }
            }
        }

        internal static bool TryGetAndDeleteUnlockableName(string unlockableName, out int indexPos) //unused
        {
            List<UnlockableItem> unlockableList = [.. StartOfRound.Instance.unlockablesList.unlockables];

            for (int i = unlockableList.Count - 1; i >= 0; i--)
            {
                if (unlockableList[i].unlockableName.Equals(unlockableName))
                {
                    Plugin.Spam($"Unlockable: [{unlockableName}] found! Removing unlockable and noting index position");
                    StartOfRound.Instance.unlockablesList.unlockables.Remove(unlockableList[i]);
                    indexPos = i;
                    return true;
                }
            }
            indexPos = -1;
            return false;
        }
    }
}
