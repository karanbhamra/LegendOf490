using UnityEngine;
using UnityEngine.UI;

namespace VikingCrewTools {
	public class SpeechbubbleBehaviour : MonoBehaviour {
        public float timeToLive = 1f;

        private Transform objectToFollow;
        private Vector3 offset;

        public Image image;
        public Text text;
        
        // Update is called once per frame
        void Update() {
            timeToLive -= Time.deltaTime;
            
            // When text is about to die start fadin out
            if (0 < timeToLive && timeToLive < 1) {
                this.image.color = new Color(this.image.color.r, this.image.color.g, this.image.color.b, timeToLive);
                this.text.color = new Color(this.text.color.r, this.text.color.g, this.text.color.b, timeToLive);
            }
            if (timeToLive <= 0)
                gameObject.SetActive(false);
        }

        void LateUpdate() {
            if (objectToFollow != null)
                transform.position = objectToFollow.position + offset;
            
            transform.rotation = Camera.main.transform.rotation;
        }

        public void Setup(Vector3 position, string text, float timeToLive, Color color) {
            this.timeToLive = timeToLive;
            this.text.text = text;
            transform.position = position;
            this.objectToFollow = null;
            this.offset = Vector3.zero;
            this.image.color = color;
            this.text.color = new Color(this.text.color.r, this.text.color.g, this.text.color.b, 1);
            transform.rotation = Camera.main.transform.rotation;
            
            if (timeToLive > 0)
                gameObject.SetActive(true);
        }

        public void Setup(Transform objectToFollow, Vector3 offset, string text, float timeToLive, Color color) {
            this.timeToLive = timeToLive;
            this.text.text = text;
            this.objectToFollow = objectToFollow;
            transform.position = objectToFollow.position + offset;
            this.offset = offset;
            this.image.color = color;
            this.text.color = new Color(this.text.color.r, this.text.color.g, this.text.color.b, 1);
            transform.rotation = Camera.main.transform.rotation;

            if (timeToLive > 0)
                gameObject.SetActive(true);
        }
    }
}