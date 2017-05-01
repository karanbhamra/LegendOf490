using UnityEngine;
using System.Collections;
using SpeechbubbleManager = VikingCrewTools.SpeechbubbleManager;

namespace VikingCrewDevelopment {
    public class SayRandomThingsBehaviour : MonoBehaviour {
        [Multiline]
        public string[] thingsToSay = new string[] { "Hello world" };
        [Header("Leave as null if you just want center of character to emit speechbubbles")]
        public Transform mouth;
        public float timeBetweenSpeak = 5f;
        public bool doTalkOnYourOwn = true;
        private float timeToNextSpeak;
        // Use this for initialization
        void Start() {
            timeToNextSpeak = timeBetweenSpeak;
        }

        // Update is called once per frame
        void Update() {
            timeToNextSpeak -= Time.deltaTime;

            if (doTalkOnYourOwn && timeToNextSpeak  <= 0 && thingsToSay.Length > 0)
                SaySomething();
        }

        public void SaySomething() {
            string message = thingsToSay[Random.Range(0, thingsToSay.Length)];
            SaySomething(message);
        }

        public void SaySomething(string message) {
            SaySomething(message, GetRandomSpeechbubbleType());
        }

        public void SaySomething(string message, SpeechbubbleManager.SpeechbubbleType speechbubbleType) {
            if (mouth == null)
                VikingCrewTools.SpeechbubbleManager.Instance.AddSpeechbubble(transform, message, speechbubbleType);
            else
                VikingCrewTools.SpeechbubbleManager.Instance.AddSpeechbubble(mouth, message, speechbubbleType);

            timeToNextSpeak = timeBetweenSpeak;
        }

        SpeechbubbleManager.SpeechbubbleType GetRandomSpeechbubbleType() {
            return SpeechbubbleManager.Instance.prefabs[Random.Range(0, SpeechbubbleManager.Instance.prefabs.Count)].type;
        }
    }
}