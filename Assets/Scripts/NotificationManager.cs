using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class NotificationManager : MonoBehaviour
{
	List<Notification> queuedNotifications = new List<Notification>();
	
	[SerializeField] TextWriter notificationDisplay;
	[SerializeField] CanvasGroup canvasGroup;
	[SerializeField] float fadeSpeed;
	public static NotificationManager instance;
	void Awake()
	{
		if(instance != null){
			Debug.LogError("Another instance of NotificationManager already exists!");
			return;
		}
		instance = this;
	}


	void Update()
	{
		if(queuedNotifications.Count == 0){return;}
		if(!notificationActive){
			StartCoroutine(ProcessRecentNotification());
		}
	}
	bool notificationActive = false;
	IEnumerator ProcessRecentNotification(){
		notificationActive = true;
		//change notification text
		Notification notification = queuedNotifications[0];
		notificationDisplay.Set(notification.content);

		//if condition initially met
		if(!notification.condition()){
			//fade in notification
			while(canvasGroup.alpha < 1){
				canvasGroup.alpha += fadeSpeed*Time.deltaTime;
				canvasGroup.alpha = Mathf.Clamp01(canvasGroup.alpha);
				yield return null;
			}

			//wait for condition true
			while(!notification.condition()){
				yield return null;
			}

		}

		//callback
		if(notification.callback != null){
			notification.callback();
		}

		//fade out notification
		while(canvasGroup.alpha > 0){
			canvasGroup.alpha -= fadeSpeed*Time.deltaTime;
			canvasGroup.alpha = Mathf.Clamp01(canvasGroup.alpha);
			yield return null;
		}
		queuedNotifications.RemoveAt(0);
		notificationActive = false;
	}
	public void AddNotification(string content, Func<bool> condition, NotificationCallback callback = null){
		queuedNotifications.Add(new Notification(content, condition, callback));
	}

	public delegate void NotificationCallback();
	public class Notification
	{
		public Notification(string _content, Func<bool> _condition, NotificationCallback _callback){
			condition = _condition;
			content = _content;
			callback = _callback;
		}
		public Func<bool> condition;
		public string content;
		public NotificationCallback callback;
	}
}
