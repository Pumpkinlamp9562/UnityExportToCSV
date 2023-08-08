using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

public class T_ObjectListTopicWindow : EditorWindow
{
	private void CreateGUI()
	{
		//Input Field
		TextField input = new TextField("Topic Name");

		//Button to confirm
		Button appendBtn = new Button();
		appendBtn.text = "Append";

		appendBtn.clickable.clicked += () => {
			T_ObjectListCSVManager.AppendToReport(
				new string[1]
			{ input.text });
		};

		rootVisualElement.Add(input);
		rootVisualElement.Add(appendBtn);
	}
}
public class T_ObjectsListTool : MonoBehaviour
{
	[MenuItem("SCS/ObjectsList/Add topic to CSV")]
	static void AddTopicWindow()
	{
		EditorWindow window = EditorWindow.GetWindow<T_ObjectListTopicWindow>();
		window.titleContent = new GUIContent("Add Topic To CSV");
		window.ShowPopup();
	}

	[MenuItem("SCS/ObjectsList/Add selected object to CSV")]
	static void SelectObjectToExcel()
	{
		//List<GameObject> selectedObjects = new List<GameObject>();
		Object[] selectedObjects = Selection.objects;

		Debug.Log(Selection.objects.Length);

		T_ObjectListCSVManager.MeshRendererData.Clear();
		for (int i = 0; i< selectedObjects.Length; i++)
		{
			GameObject o = (GameObject)selectedObjects[i];
			MeshRenderer[] meshObjects = o.GetComponentsInChildren<MeshRenderer>(true);
			SkinnedMeshRenderer[] skinMeshObjects = o.GetComponentsInChildren<SkinnedMeshRenderer>(true);
			SpriteRenderer[] spriteObjects = o.GetComponentsInChildren<SpriteRenderer>(true);
			UnityEngine.UI.Image[] imageObjects = o.GetComponentsInChildren<UnityEngine.UI.Image>(true);
			ParticleSystemRenderer[] particleObjects = o.GetComponentsInChildren<ParticleSystemRenderer>(true);

			for (int f = 0; f < meshObjects.Length; f++)
			{
				T_ObjectListCSVManager.AddMeshRendererData(meshObjects[f]);
			}
			for (int f = 0; f < skinMeshObjects.Length; f++)
			{
				T_ObjectListCSVManager.AddSkinMeshRendererData(skinMeshObjects[f]);
			}
			for (int f = 0; f < spriteObjects.Length; f++)
			{
				T_ObjectListCSVManager.AddSpriteRendererData(spriteObjects[f]);
			}
			for (int f = 0; f < imageObjects.Length; f++)
			{
				T_ObjectListCSVManager.AddImageRendererData(imageObjects[f]);
			}
			for (int f = 0; f < particleObjects.Length; f++)
			{
				T_ObjectListCSVManager.AddParticleRendererData(particleObjects[f]);
			}
		}
		T_ObjectListCSVManager.UpdateAllRendererData();
	}

	[MenuItem("SCS/ObjectsList/Reset CSV")]
	static void ResetExcel()
	{
		T_ObjectListCSVManager.CreateReport();
		EditorApplication.Beep();
		Debug.Log("<color=red>This report has been reset</color>");
	}
}
