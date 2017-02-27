using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Invector;

public class vHeadTrack : MonoBehaviour
{    
    [HideInInspector]
    public float minAngleX = -90f, maxAngleX = 90, minAngleY = -90f, maxAngleY = 90f;

    public Transform head;
    public float headWeight = 1f;
    public float bodyWeight = 0.25f;
    public float distanceToDetect = 10f;    
    public float smooth = 1f;
    public float updateTargetInteration = 1;
    public LayerMask obstacleLayer = 1 << 0;
    [Header("--- Gameobjects Tags to detect ---")]
    public List<string> tagsToDetect = new List<string>() { "LookAt" };
    [Header("--- Animator State Tag to ignore the HeadTrack ---")]
    public List<string> animatorTags = new List<string>() { "Attack", "LockMovement", "CustomAction" };
    public bool followCamera;
    public bool useLimitAngle = true;

    protected List<vLookTarget> targetsInArea = new List<vLookTarget>();

    private float yRotation, xRotation;
    private float _currentHeadWeight, _currentbodyWeight;
    private Animator animator;
    private float headHeight;
    private vLookTarget lookTarget;
    private Transform simpleTarget;
    private List<int> tagsHash;
    private vHeadTrackSensor sensor;
    private float interation;

    vCharacter vchar;
    Vector2 cameraAngle, targetAngle;

    void Start()
    {
        if (!sensor)
        {
            var sensorObj = new GameObject("HeadTrackSensor");
            sensor = sensorObj.AddComponent<vHeadTrackSensor>();
        }

        vchar = GetComponent<vCharacter>();

        sensor.headTrack = this;
        animator = GetComponentInParent<Animator>();
        if (animator.isHuman) head = animator.GetBoneTransform(HumanBodyBones.Head);
        if (head)
        {
            headHeight = Vector3.Distance(transform.position, head.position);
            sensor.transform.position = head.transform.position;
        }
        else
        {
            sensor.transform.position = transform.position;
        }
        var layer = LayerMask.NameToLayer("HeadTrack");
        sensor.transform.parent = transform;
        sensor.gameObject.layer = layer;
        sensor.gameObject.tag = transform.tag;
        tagsHash = new List<int>();
        for (int i = 0; i < animatorTags.Count; i++)
        {
            tagsHash.Add(Animator.StringToHash(animatorTags[i]));
        }
        GetLookPoint();
    }

    Vector3 headPoint { get { return transform.position + (transform.up * headHeight); } }

    void OnAnimatorIK()
    {
        if (vchar != null && vchar.currentHealth > 0f)
        {
            animator.SetLookAtWeight(_currentHeadWeight, _currentbodyWeight);
            animator.SetLookAtPosition(GetLookPoint());
        }
    }

    bool lookConditions { get { return head != null && (followCamera && Camera.main != null) || (!followCamera && (lookTarget || simpleTarget)); } }

    Vector3 GetLookPoint()
    {
        var distanceToLoock = 100;
        if (lookConditions && !IgnoreHeadTrack())
        {
            var lookPosition = headPoint + (transform.forward * distanceToLoock);
            if (followCamera)
                lookPosition = (Camera.main.transform.position + (Camera.main.transform.forward * distanceToLoock));

            var dir = lookPosition - headPoint;
            if (lookTarget != null && TargetIsOnRange(lookTarget.lookPoint - headPoint) && lookTarget.IsVisible(headPoint, obstacleLayer))            
                dir = lookTarget.lookPoint - headPoint;            
            else if (simpleTarget != null)
                dir = simpleTarget.position - headPoint;

            var angle = GetTargetAngle(dir);
            if (useLimitAngle)
            {
                if (TargetIsOnRange(dir))
                    SmoothValues(headWeight, bodyWeight, angle.x, angle.y);
                else
                    SmoothValues();
            }
            else
                SmoothValues(headWeight, bodyWeight, angle.x, angle.y);

            if (targetsInArea.Count > 1)
                SortTargets();
        }
        else
        {
            SmoothValues();
            if (targetsInArea.Count > 1)
                SortTargets();
        }

        var rotA = Quaternion.AngleAxis(yRotation, transform.up);
        var rotB = Quaternion.AngleAxis(xRotation, transform.right);
        var finalRotation = (rotA * rotB);
        var lookDirection = finalRotation * transform.forward;
        return headPoint + (lookDirection * distanceToLoock);
    }

    Vector2 GetTargetAngle(Vector3 direction)
    {
        var lookRotation = Quaternion.LookRotation(direction, transform.up);        //rotation from head to camera point
        var angleResult = lookRotation.eulerAngles - transform.eulerAngles;         // diference between transform rotation and desiredRotation
        Quaternion desiredRotation = Quaternion.Euler(angleResult);                 // convert angleResult to Rotation
        var x = (float)System.Math.Round(NormalizeAngle(desiredRotation.eulerAngles.x), 2);
        var y = (float)System.Math.Round(NormalizeAngle(desiredRotation.eulerAngles.y), 2);
        return new Vector2(x, y);
    }

    bool TargetIsOnRange(Vector3 direction)
    {
        var angle = GetTargetAngle(direction);
        return (angle.x >= minAngleX && angle.x <= maxAngleX && angle.y >= minAngleY && angle.y <= maxAngleY);
    }

    /// <summary>
    /// Set vLookTarget
    /// </summary>
    /// <param name="target"></param>
    public void SetLookTarget(vLookTarget target, bool priority = false)
    {
        if (!targetsInArea.Contains(target)) targetsInArea.Add(target);
        if (priority)
            lookTarget = target;
    }

    /// <summary>
    /// Set Simple target
    /// </summary>
    /// <param name="target"></param>
    public void SetLookTarget(Transform target)
    {
        simpleTarget = target;
    }

    public void RemoveLookTarget(vLookTarget target)
    {
        if (targetsInArea.Contains(target)) targetsInArea.Remove(target);
        if (lookTarget == target) lookTarget = null;
    }

    public void RemoveLookTarget(Transform target)
    {
        if (simpleTarget == target) simpleTarget = null;
    }

    /// <summary>
    /// Make angle to work with -180 and 180 
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    float NormalizeAngle(float angle)
    {
        if (angle < -180)
            return angle + 360;
        else if (angle > 180)
            return angle - 360;
        else
            return angle;
    }

    void ResetValues()
    {
        _currentHeadWeight = 0;
        _currentbodyWeight = 0;
        yRotation = 0;
        xRotation = 0;
    }

    void SmoothValues(float _headWeight = 0, float _bodyWeight = 0, float _x = 0, float _y = 0)
    {
        _currentHeadWeight = Mathf.Lerp(_currentHeadWeight, _headWeight, smooth * Time.deltaTime);
        _currentbodyWeight = Mathf.Lerp(_currentbodyWeight, _bodyWeight, smooth * Time.deltaTime);
        yRotation = Mathf.Lerp(yRotation, _y, smooth * Time.deltaTime);
        xRotation = Mathf.Lerp(xRotation, _x, smooth * Time.deltaTime);
        yRotation = Mathf.Clamp(yRotation, minAngleY, maxAngleY);
        xRotation = Mathf.Clamp(xRotation, minAngleX, maxAngleX);
    }

    void SortTargets()
    {
        interation += Time.deltaTime;
        if (interation > updateTargetInteration)
        {
            interation -= updateTargetInteration;
            if (targetsInArea == null || targetsInArea.Count < 2)
            {
                if (targetsInArea != null && targetsInArea.Count > 0)
                    lookTarget = targetsInArea[0];
                return;
            }

            for (int i = targetsInArea.Count - 1; i >= 0; i--)
            {
                if (targetsInArea[i] == null)
                {
                    targetsInArea.RemoveAt(i);
                }
            }
            targetsInArea.Sort(delegate (vLookTarget c1, vLookTarget c2)
            {
                return Vector3.Distance(this.transform.position, c1 != null ? c1.transform.position : Vector3.one * Mathf.Infinity).CompareTo
                    ((Vector3.Distance(this.transform.position, c2 != null ? c2.transform.position : Vector3.one * Mathf.Infinity)));
            });
            if (targetsInArea.Count > 0)
            {
                lookTarget = targetsInArea[0];
            }
        }
    }

    public void OnDetect(Collider other)
    {
        if (tagsToDetect.Contains(other.gameObject.tag) && other.GetComponent<vLookTarget>() != null)
        {
            lookTarget = other.GetComponentInParent<vLookTarget>();
            var headTrack = other.GetComponentInParent<vHeadTrack>();
            if (!targetsInArea.Contains(lookTarget) && (headTrack == null || headTrack != this))
            {
                targetsInArea.Add(lookTarget);
                SortTargets();
                lookTarget = targetsInArea[0];
            }
        }
    }

    public void OnLost(Collider other)
    {
        if (tagsToDetect.Contains(other.gameObject.tag) && other.GetComponentInParent<vLookTarget>() != null)
        {
            lookTarget = other.GetComponentInParent<vLookTarget>();
            if (targetsInArea.Contains(lookTarget))
            {
                targetsInArea.Remove(lookTarget);
            }
            SortTargets();
            if (targetsInArea.Count > 0)
                lookTarget = targetsInArea[0];
            else
                lookTarget = null;
        }
    }

    public bool IgnoreHeadTrack()
    {
        for (int index = 0; index < animator.layerCount; index++)
        {
            var info = animator.GetCurrentAnimatorStateInfo(index);
            if (tagsHash.Contains(info.tagHash))
            {
                return true;
            }
        }
        return false;
    }
    
}
