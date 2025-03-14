using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class JSYVoteSystem : SingletonManager<JSYVoteSystem>
{
    private Dictionary<string, int> voteResults = new Dictionary<string, int>();
    private Dictionary<string, bool> isAnswerChoice = new Dictionary<string, bool>();

    public GameObject testPanel;
    public TextMeshProUGUI voteStatusText;
    public List<DialogData.DialogChoice> currentChoices = new List<DialogData.DialogChoice>();
    
    public void SetCurrentChoices(DialogData.DialogChoice[] choices)
    {
        currentChoices.Clear();
        currentChoices.AddRange(choices);
        
        voteResults.Clear();
        isAnswerChoice.Clear();
        foreach (var choice in choices)
        {
            voteResults[choice.optionText] = 0;
            isAnswerChoice[choice.optionText] = choice.isAnswer;
        }

        if (JSYModeManager.Instance.currentGameMode == ModeType.multiMode)
        {
            ShowTestUI();
        }
        
        UpdateVoteStatus();
    }

    private void ShowTestUI()
    {
        testPanel.SetActive(true);
    }

    public void AddVote(DialogData.DialogChoice choice)
    {
        voteResults[choice.optionText]++;
        UpdateVoteStatus();
    }

    private void UpdateVoteStatus()
    {
        // 투표 현황 UI -> 사람 이모티콘으로
    }

    public void FinishVote()
    {
        VoteResult();
        testPanel.SetActive(false);
    }
    
    private void VoteResult()
    {
        var maxVotes = voteResults.Max(x=>x.Value);
        var topOptions = voteResults.Where(x => x.Value == maxVotes).ToList();

        string selectedOption;
        if (topOptions.Count > 1)
        {
            var answerOption = topOptions.FirstOrDefault(x => isAnswerChoice[x.Key]);
            if (answerOption.Key != null)
            {
                selectedOption = answerOption.Key;
            }
            else
            {
                selectedOption = topOptions[0].Key;
            }
        }

        else
        {
            selectedOption = topOptions[0].Key;
        }


    }
    
    
}
