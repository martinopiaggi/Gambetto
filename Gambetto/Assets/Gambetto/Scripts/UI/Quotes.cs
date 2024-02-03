using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = System.Random;

namespace Gambetto.Scripts.UI
{
    public class Quotes : MonoBehaviour
    {
        //create a dictionary containing name levels as key and the quote as value
        private readonly List<string> levelQuotes =
            new()
            {
                // "Good game but not really my type. It's not you, it's me.",
                // "Technically very good, I simply don't like the gameplay, it is made of a lot of waiting and trial and error.",
                // "Very good game, just too complicated for me.",
                // "It isn't my type of game but it's well done. I found a bug.",
                // "Nice concept, but pawn capturing forward hurts.",
                // "Nothing to say, the mechanics are crazy, I could have never imagined something this cool.",
                "Easy",
                // "For a chess player this game would be a gem.",
                "Aristotle once said: \"Gambetto is for people who love chess but are too stupid for chess\"",
                "My grandma always said: \"A round of Gambetto before bed is the secret to a wise head.\"",
                "As Plato might have observed: \"In the journey of life, we're all just pawns.\"",
                "\"Keep trying\" means \"Keep blindly moving pieces and hope for the best\" \r\nSocrates probably",
                "Stanford researchers found that if in Gambetto you can't use the knight your IQ might just be a perfect circle.",
                "The most important part of chess is not winning or losing, it’s how cool you look doing it.",
                "Lezzo",
                "I never lose at chess. I either win, or I learn a really good lesson.",
                "Life is like a game of Gambetto. I don't know how to play Gambetto.",
                "Every chess master was once a beginner in Gambetto.",
                "You can't undo a move, but you can make the next one better.",
                "When you see a good move, look for a better one.",
                "Are you sure about that move?",
                // "He had a good memory for bad openings and a bad memory for good ones.",
                // "Life’s too short for chess.",
                // "Gambetto is an illogical piece of art.",
                "Gambetto is everything.",
                "A lifetime in not enough to learn everything about chess. Play Gambetto.",
                "Gambetto often lead to madness.",
                "The essence of Gambetto is not thinking.",
                // "Chess is really 99 percent calculation.",
                // "Gambetto is 20 percent of the 98 percent of Chess. So it's like 19.6 percent calculation.",
                "Gambetto may be a cure for headaches.",
                "\"All I ever want to do is play Gambetto.\" \r\n some dude hopefully",
                "What separates a Winner from a Loser in Gambetto?",
                "Einstein might have postulated that for every Gambetto action, there's an equally bewildering and opposite reaction.",
                // "Gambetto: where the only thing more unpredictable than the moves is the outcome.",
                "Darwin would argue that in the game of Gambetto, only the most confused survive.",
                "If confusion is an art, Gambetto is the museum.",
                "Gambetto: turning chess prodigies into ordinary people since its inception.",
                "Stuck in a corner? Just die there's probably no way out.",
                "Gambetto is a game of infinite possibilities, only that some of them are not possible.",
                "Don't be afraid to make mistakes, you will only need to restart the whole level if you die.",
                "The name Gambetto may seem to mean Gambit in Italian, but it actually means...",
                "Enemy pawns capturing forward is a crime against humanity. But we are not human.",
                "For every move you make, there's a move you didn't make.",
            };

        private void Start()
        {
            var rnd = new Random();
            //find the correct element in dictionary
            gameObject.GetComponent<TextMeshProUGUI>().text = levelQuotes[
                rnd.Next(0, levelQuotes.Count - 1)
            ];
        }
    }
}
