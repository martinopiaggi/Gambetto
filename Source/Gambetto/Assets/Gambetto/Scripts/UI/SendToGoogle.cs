using System.Collections;
using Gambetto.Scripts;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SendToGoogle : MonoBehaviour
{
    private string[] _videogames_names = new string[30]
    {
        "ACat_sVacation",
        "BeNeutral",
        "BlankForger",
        "BotanicalAlchemy",
        "BubbleQuest",
        "ChronoGear",
        "ColorShift",
        "CubysDream",
        "Deeper",
        "Diagonaliz",
        "DjVsZombies",
        "Don_tDrop",
        "DontStopMeNow!",
        "FlamebornOdyssey",
        "FromTheDead",
        "Gambetto",
        "Henosis",
        "Insectathlon",
        "MamaMia",
        "NecroDicer",
        "NPC",
        "Onigiri",
        "PhantomBrews",
        "ReclaimHumanity",
        "RumNGun",
        "Shoto",
        "TheDoomedDog",
        "ThereIsNoMainCharacter",
        "WoodlandMystery",
        "TestGame"
    };

    enum VideoGamesName
    {
        ACat_sVacation,
        BeNeutral,
        BlankForger,
        BotanicalAlchemy,
        BubbleQuest,
        ChronoGear,
        ColorShift,
        CubysDream,
        Deeper,
        Diagonaliz,
        DjVsZombies,
        Don_tDrop,
        DontStopMeNow,
        FlamebornOdyssey,
        FromTheDead,
        Gambetto,
        Henosis,
        Insectathlon,
        MamaMia,
        NecroDicer,
        NPC,
        Onigiri,
        PhantomBrews,
        ReclaimHumanity,
        RumNGun,
        Shoto,
        TheDoomedDog,
        ThereIsNoMainCharacter,
        WoodlandMystery,
        TestGame
    };

    [SerializeField]
    private VideoGamesName Videogame;

    [SerializeField]
    private InputField Feedback;

    public void SendFeedback()
    {
        var feedback = Feedback.text;
        feedback += "\n" + "levels completed: " + GameManager.Instance.GetLevelCount(true);
        feedback += "\n" + "deaths: " + GameManager.Instance.DeathCount;
        StartCoroutine(PostFeedback(_videogames_names[(int)Videogame], feedback));
        // StartCoroutine(PostFeedback(Videogame.ToString(),feedback));
    }

    IEnumerator PostFeedback(string videogame_name, string feedback)
    {
        // https://docs.google.com/forms/d/e/1FAIpQLSdyQkpRLzqRzADYlLhlGJHwhbKZvKJILo6vGmMfSePJQqlZxA/viewform?usp=pp_url&entry.631493581=Simple+Game&entry.1313960569=Very%0AGood!

        string URL =
            "https://docs.google.com/forms/d/e/1FAIpQLSdyQkpRLzqRzADYlLhlGJHwhbKZvKJILo6vGmMfSePJQqlZxA/formResponse";

        WWWForm form = new WWWForm();

        form.AddField("entry.631493581", videogame_name);
        form.AddField("entry.1313960569", feedback);

        UnityWebRequest www = UnityWebRequest.Post(URL, form);

        yield return www.SendWebRequest();

        print(www.error);

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
        }
    }
}
