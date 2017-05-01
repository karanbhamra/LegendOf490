using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using VikingCrewTools;
using System.Linq;

namespace VikingCrewDevelopment {
    public class UISpeechController : MonoBehaviour {

        public InputField txtMessage;
        public SayRandomThingsBehaviour talkBehaviour;
        public ToggleGroup toggles;


        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }

        public void OnTalk() {

            talkBehaviour.SaySomething(txtMessage.text, (SpeechbubbleManager.SpeechbubbleType)toggles.ActiveToggles().First<Toggle>().transform.GetSiblingIndex());
        }
    }
}