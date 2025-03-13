using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkSceneManager : NetworkSceneManagerDefault
{
	private Dictionary<NetworkObjectTypeId, NetworkObject> _sceneObjects;

	public bool TryResolveSceneObject(NetworkRunner runner,
		NetworkObjectTypeId sceneObjectGuid, out NetworkObject instance)
	{
		Debug.Log($"씬 오브젝트 복원 시도: {sceneObjectGuid}");

		if (_sceneObjects == null)
		{
			_sceneObjects = new Dictionary<NetworkObjectTypeId, NetworkObject>();
			var scene = SceneManager.GetActiveScene();
			var sceneNOs = scene.FindObjectsOfTypeInOrder<NetworkObject>();

			foreach (var sceneNO in sceneNOs)
			{
				Debug.Log($"씬 오브젝트 등록: {sceneNO.NetworkTypeId}");
				_sceneObjects.Add(sceneNO.NetworkTypeId, sceneNO);
			}
		}

		return _sceneObjects.TryGetValue(sceneObjectGuid, out instance);
	}
}