using UnityEngine;
using System.Collections;

public class CameraTool : MonoBehaviour {

	private static  CameraTool    _instance;
	public  static  CameraTool    Instance
	{
		get{return _instance;}
	}
	void Awake()
	{
		_instance = this;
	}


	/// <summary>
	/// 获取摄像机的大小
	/// </summary>
	/// <returns>The camera size in unit.</returns>
	public Vector2 GetCameraSizeInUnit()
	{
		var camera = this.GetComponent<Camera> ();
		float orthographicSize = camera.orthographicSize;
		//计算宽高比
		float aspectRatio = Screen.width * 1.0f / Screen.height;
		//固定高度，所以高度可以直接计算出来
		float cameraHeight = orthographicSize * 2;
		//宽度根据宽高比计算一下
		float cameraWidth = cameraHeight * aspectRatio;
		return new Vector2( cameraWidth , cameraHeight);
	}

}
