using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
public class TutorialManager : MonoBehaviour
{
	[SerializeField] PlantCreator plantCreator;
	void OnTutorialComplete(){
		SceneManager.LoadScene("MainMenu");
	}
	Coroutine TutorialProcess;
	void Start()
	{
		// if(PlayerPrefs.HasKey("tutorialComplete")){
		// 	if(PlayerPrefs.GetInt("tutorialComplete") == 1 && !forceTutorial){
		// 		return;
		// 	}
		// }
		TutorialProcess = StartCoroutine(Tutorial());

	}
	void OnDestroy(){
		if(TutorialProcess != null){
			StopCoroutine(TutorialProcess);
		}
	}
	IEnumerator Tutorial(){
		NotificationManager notifs = NotificationManager.instance;
		bool movedCamera = false;
		bool zoomedCamera = false;
		
		Func<bool> CameraCondition = () => movedCamera && zoomedCamera;
		notifs.AddNotification("Move your viewport by dragging LMB and zoom in with the scroll wheel", CameraCondition);
		while(!CameraCondition()){
			if (Input.GetMouseButtonDown(1)){
				movedCamera = true;
			}

			if (Input.mouseScrollDelta.y != 0){
				zoomedCamera = true;
			}
			yield return null;
		}
		bool plantSpawned = false;
		bool plantCreated = false;
		plantCreator.OnPlantSpawned += () => plantSpawned = true;
		plantCreator.OnPlantCreated += () => plantCreated = true;
		notifs.AddNotification("Alright! Time to plant your first plant! You can only place it next to water or other plants. You can rotate the plant with the scroll wheel if you press down shift.",  () => plantSpawned);
		yield return new WaitUntil(() => plantSpawned);
		
		notifs.AddNotification("Very nice! Now pick the plant's starting cell. The plant will start growing from there", () => plantCreated);
		yield return new WaitUntil(() => plantCreated);
		
		bool plantGrown = false;
		GameManager.instance.OnGrowEnd += () => plantGrown = true;
		notifs.AddNotification("Time to watch the plant grow!", () => plantGrown);
		yield return new WaitUntil(() => plantGrown);
		
		
		bool gridExpanded = false;
		GameManager.instance.OnGridExpanded += () => gridExpanded = true;
		notifs.AddNotification("Looks like you've planted enough plants to earn an expansion!", () => gridExpanded);
		yield return new WaitUntil(() => gridExpanded);
		
		
		bool plantCollision = false;
		PlantManager.instance.OnPlantDestroyed += () => plantCollision = true;
		notifs.AddNotification("Alright one last thing. Be careful where you grow you plants! If they intersect both plants will die. If a plant loses access to water it will also die. Give it a shot!", () => plantCollision);
		yield return new WaitUntil(() => plantCollision);

		notifs.AddNotification("Now expand your garden as much as possible!", () => true);

		yield return new WaitForSeconds(3);
		OnTutorialComplete();
	}
}
