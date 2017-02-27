﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Invector.CharacterController;
using System;
using System.Collections.Generic;

public class vHUDController : MonoBehaviour 
{
    #region General Variables

    #region Health/Stamina Variables
    [Header("Health/Stamina")]
	public Slider healthSlider;
	public Slider staminaSlider;
	[Header("DamageHUD")]
	public Image damageImage;
	public float flashSpeed = 5f;
	public Color flashColour = new Color(1f, 0f, 0f, 0.1f);   
	[HideInInspector] public bool damaged;
    #endregion

    #region Action Text Variables
    [HideInInspector] public bool showInteractiveText = true;
    [Header("Interactable Text-Icon")]
    public Text interactableText;
    public Image interactableIcon;
    public Sprite joystickButton;
    public Sprite keyboardButton;
    public float actionTextDelay = 1f;
    float currentActionTextTime;
    vTriggerAction currentTriggerAction;
    vTriggerLadderAction currentTriggerLadderAction;
    #endregion

    #region Display Controls Variables
    [Header("Controls Display")]
    [HideInInspector] public bool controllerInput;
    public Image displayControls;
    public Sprite joystickControls;
    public Sprite keyboardControls;
    #endregion

    #region Debug Info Variables
    [Header("Debug Window")]
    public GameObject debugPanel;
    [HideInInspector]
    public Text debugText;
    #endregion

    #region Change Input Text Variables
    [Header("Text with FadeIn/Out")]
    public Text fadeText;
    private float textDuration, fadeDuration, durationTimer, timer;
    private Color startColor, endColor;
    private bool fade;
    #endregion

    #endregion

    private static vHUDController _instance;
    public static vHUDController instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<vHUDController>();
                //Tell unity not to destroy this object when loading a new scene
                //DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }

    void Start()
    {        
        HideActionText();
        InitFadeText();
        if (debugPanel != null)
            debugText = debugPanel.GetComponentInChildren<Text>();
    }

    public void Init(vThirdPersonController cc)
    {
        cc.onDead.AddListener(OnDead);
        cc.onReceiveDamage.AddListener(EnableDamageSprite);       
        damageImage.color = new Color(0f, 0f, 0f, 0f);
    }

    private void OnDead(GameObject arg0)
    {
        FadeText("You are Dead!", 2f, 0.5f);
    }

    public virtual void UpdateHUD(vThirdPersonController cc)
    {
        UpdateDebugWindow(cc);
        UpdateSliders(cc);
        ChangeInputDisplay();
        ShowDamageSprite();
        FadeEffect();
        CheckActionTrigger(cc);
        CheckLadderActionTrigger(cc);     
    }

    public void ShowText(string message)
    {
        FadeText(message, 2f, 0.5f);
    }

    private void CheckActionTrigger(vThirdPersonController cc)
    {
        if (cc.ladderAction != null) return;
        if ((cc.triggerAction != null && currentActionTextTime <= 0 && !showInteractiveText) || (cc.triggerAction != null && cc.triggerAction != currentTriggerAction))
        {
            if (!cc.actions)
            {
                currentTriggerAction = cc.triggerAction;
                // check if you can use this action - display hud info
                if (cc.triggerAction.CanUse())
                {
                    currentActionTextTime = actionTextDelay;
                    if (!cc.triggerAction.autoAction) ShowActionText(cc.triggerAction.message, true);
                }
                else
                {
                    if (!cc.triggerAction.autoAction) ShowActionText("Can't " + cc.triggerAction.message, false);
                }
            }
            else
            {
                HideActionText();
            }                
        }
        else if(cc.triggerAction == null)
        {
            currentTriggerAction = null;
            currentActionTextTime -= Time.deltaTime;
            HideActionText();   
        }
    }

    private void CheckLadderActionTrigger(vThirdPersonController cc)
    {
        if (cc.triggerAction != null) return;
        if ((cc.ladderAction != null && currentActionTextTime <= 0 && !showInteractiveText) || (cc.ladderAction != null && cc.ladderAction != currentTriggerLadderAction))
        {
            if (!cc.isUsingLadder)
            {
                currentTriggerLadderAction = cc.ladderAction;                
                currentActionTextTime = actionTextDelay;                
                if (!cc.ladderAction.autoAction)
                    ShowActionText(cc.ladderAction.enterMessage, true);                
            }
            else
            {                
                HideActionText();
            }
        }
        else if (cc.ladderAction == null || cc.isUsingLadder)
        {           
            currentTriggerLadderAction = null;
            currentActionTextTime -= Time.deltaTime;
            HideActionText();
        }
    }

    void UpdateSliders(vThirdPersonController cc)
    {
        if (cc.maxHealth!= healthSlider.maxValue)
        {
            healthSlider.maxValue = Mathf.Lerp(healthSlider.maxValue, cc.maxHealth, 2f * Time.fixedDeltaTime);
            healthSlider.onValueChanged.Invoke(healthSlider.value);
        }
        healthSlider.value = Mathf.Lerp(healthSlider.value, cc.currentHealth, 2f * Time.fixedDeltaTime);
        if (cc.maxStamina != staminaSlider.maxValue)
        {
            staminaSlider.maxValue = Mathf.Lerp(staminaSlider.maxValue, cc.maxStamina, 2f * Time.fixedDeltaTime);
            staminaSlider.onValueChanged.Invoke(staminaSlider.value);
        }
        staminaSlider.value = cc.currentStamina;
    }

    public void ShowDamageSprite()
    {

        if (damaged)
        {
            damaged = false;
            if (damageImage != null)
                damageImage.color = flashColour;
        }
        else if (damageImage != null)
            damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
    }

    public void EnableDamageSprite(Damage damage)
    {       
        if (damageImage != null)       
            damageImage.enabled = true;
        damaged = true;
    }

    void UpdateDebugWindow(vThirdPersonController cc)
    {
        if (cc.debugWindow)
        {
            if (debugPanel != null && !debugPanel.activeSelf)
                debugPanel.SetActive(true);
            if (debugText)
                debugText.text = cc.DebugInfo();
        }
        else
        {
            if (debugPanel != null && debugPanel.activeSelf)
                debugPanel.SetActive(false);
        }
    }

    public void ShowActionText(string name, bool showIcon)
	{
		showInteractiveText = true;
		if(controllerInput)
			interactableIcon.sprite = joystickButton;
		else
			interactableIcon.sprite = keyboardButton;
        if(showIcon)
		    interactableIcon.enabled = true;
        else
            interactableIcon.enabled = false;
        interactableText.enabled = true;
		interactableText.text = name;
	}
	
	public void HideActionText()
	{
		showInteractiveText = false;
		interactableIcon.enabled = false;
		interactableText.enabled = false;
		interactableText.text = "";
	}
    
    void ChangeInputDisplay()
	{
		#if MOBILE_INPUT
		displayControls.enabled = false;
		#else
		if(controllerInput)		
			displayControls.sprite = joystickControls;		
		else		
			displayControls.sprite = keyboardControls;
		#endif
	}
    
    void InitFadeText()
    {
        if (fadeText != null)
        {
            startColor = fadeText.color;
            endColor.a = 0f;
            fadeText.color = endColor;
        }
        else
            Debug.Log("Please assign a Text object on the field Fade Text");
    }
	
	void FadeEffect()
	{
		if(fadeText != null)
		{
			if(fade)
			{
				fadeText.color = Color.Lerp(endColor, startColor, timer);
				
				if(timer < 1)			
					timer += Time.deltaTime/fadeDuration;
				
				if(fadeText.color.a >= 1)
				{			
					fade = false;
					timer = 0f;
				}
			}
			else
			{
				if(fadeText.color.a >= 1)
					durationTimer += Time.deltaTime;
				
				if(durationTimer >= textDuration)
				{
					fadeText.color = Color.Lerp(startColor, endColor, timer);
					if(timer < 1)			
						timer += Time.deltaTime/fadeDuration;
				}
			}
		}
	}
	
	public void FadeText(string textToFade, float textTime, float fadeTime)
	{
		if(fadeText != null && !fade)
		{            
			fadeText.text = textToFade; 	
			textDuration = textTime;	
			fadeDuration = fadeTime;
			durationTimer = 0f;
			timer = 0f;	
			fade = true;
		}
		else
			Debug.Log("Please assign a Text object on the field Fade Text");
	}    

}
