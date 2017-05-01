using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VikingCrewTools {
    public class SpeechbubbleManager : MonoBehaviour {
        public enum SpeechbubbleType {
            NORMAL,
            SERIOUS,
            ANGRY,
            THINKING,
        }
        [System.Serializable]
        public class SpeechbubblePrefab {
            public SpeechbubbleType type;
            public GameObject prefab;
        }
        [Header("Default settings:")]
        public Color defaultColor = Color.white;
        public float defaultTimeToLive = 1;
        public bool is2D = true;
        [Tooltip("If you want to change the size of your speechbubbles in a scene without having to change the prefabs then change this value")]
        public float sizeMultiplier = 1f;
        [Header("Prefabs mapping to each type:")]
        public List<SpeechbubblePrefab> prefabs;

        private Dictionary<SpeechbubbleType, GameObject> prefabsDict = new Dictionary<SpeechbubbleType, GameObject>();
        
        private static SpeechbubbleManager instance;
        public static SpeechbubbleManager Instance { get { return instance; } }
        
        private Dictionary<SpeechbubbleType, Queue<SpeechbubbleBehaviour>> speechbubbleQueue = new Dictionary<SpeechbubbleType, Queue<SpeechbubbleBehaviour>>();

        private void Awake() {
            instance = this;
            prefabsDict.Clear();
            speechbubbleQueue.Clear();
            foreach (var prefab in prefabs) {
                prefabsDict.Add(prefab.type, prefab.prefab);
                speechbubbleQueue.Add(prefab.type, new Queue<SpeechbubbleBehaviour>());
            }
        }
        
        /// <summary>
        /// Adds a speechbubble to a certain position
        /// </summary>
        /// <param name="position"></param>
        /// <param name="text"></param>
        /// <param name="type"></param>
        /// <param name="timeToLive"></param>
        /// <param name="color"></param>
        public void AddSpeechbubble(Vector3 position, string text, SpeechbubbleType type, float timeToLive, Color color) {
            SpeechbubbleBehaviour bubbleBehaviour = GetBubble(type);
            bubbleBehaviour.Setup(position, text, timeToLive, color);
            speechbubbleQueue[type].Enqueue(bubbleBehaviour);
        }
        
        /// <summary>
        /// Adds a speechbubble to a certain position
        /// </summary>
        /// <param name="position"></param>
        /// <param name="text"></param>
        /// <param name="type"></param>
        public void AddSpeechbubble(Vector3 position, string text, SpeechbubbleType type = SpeechbubbleType.NORMAL) {
            AddSpeechbubble(position, text,type, defaultTimeToLive, Color.white);
        }
        
        /// <summary>
        /// Adds a speechbubble that will follow a certain transform.
        /// It is recommended you use a character's head or mouth transform.
        /// </summary>
        /// <param name="objectToFollow"></param>
        /// <param name="text"></param>
        /// <param name="type"></param>
        /// <param name="timeToLive"></param>
        /// <param name="color"></param>
        /// <param name="offset"></param>
        public void AddSpeechbubble(Transform objectToFollow, string text, SpeechbubbleType type, float timeToLive, Color color, Vector3 offset) {
            SpeechbubbleBehaviour bubbleBehaviour = GetBubble(type);
            bubbleBehaviour.Setup(objectToFollow, offset, text, timeToLive, color);
            speechbubbleQueue[type].Enqueue(bubbleBehaviour);
        }

        /// <summary>
        /// Adds a speechbubble that will follow a certain transform.
        /// It is recommended you use a character's head or mouth transform.
        /// </summary>
        /// <param name="objectToFollow"></param>
        /// <param name="text"></param>
        /// <param name="type"></param>
        public void AddSpeechbubble(Transform objectToFollow, string text, SpeechbubbleType type = SpeechbubbleType.NORMAL) {
            AddSpeechbubble(objectToFollow, text, type, defaultTimeToLive, Color.white, Vector3.zero);
        }
        
        /// <summary>
        /// Gets a reused speechbubble from the queue or, if no free ones exist already, creates
        /// a new one.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private SpeechbubbleBehaviour GetBubble(SpeechbubbleType type = SpeechbubbleType.NORMAL) {
            SpeechbubbleBehaviour bubbleBehaviour;
            //Check to see if there is a free speechbuble of the right kind to reuse
            if (speechbubbleQueue[type].Count == 0 || speechbubbleQueue[type].Peek().gameObject.activeInHierarchy) {
                GameObject newBubble = (GameObject)GameObject.Instantiate(GetPrefab(type));
                newBubble.transform.SetParent(transform);
                newBubble.transform.localScale = sizeMultiplier * GetPrefab(type).transform.localScale;
                bubbleBehaviour = newBubble.GetComponent<SpeechbubbleBehaviour>();
                //If this is not 2D then the speechbubble will need a world space canvas.
                if (!is2D) {
                    var canvas = newBubble.AddComponent<Canvas>();
                    canvas.renderMode = RenderMode.WorldSpace;
                    canvas.overrideSorting = true;
                }
            } else {
                bubbleBehaviour = speechbubbleQueue[type].Dequeue();
            }
            //Set as last to always place latest on top (in case of screenspace ui that is..)
            bubbleBehaviour.transform.SetAsLastSibling();
            return bubbleBehaviour;
        }

        private GameObject GetPrefab(SpeechbubbleType type) {
            return prefabsDict[type];
        }
        
    }
}