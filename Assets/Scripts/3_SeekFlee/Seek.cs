using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Seek : MonoBehaviour {

	public Transform target;
	public float max_velocity;
	public Vector3 velocity;

	//绘制轨迹
	private List<Vector3> pathList;
	private float timer=0;

	// Use this for initialization
	void Start () {
		pathList = new List<Vector3>();
	}
	
	// Update is called once per frame
	void Update () {


		//update位置关系
		Vector3 desiredVelocity = (target.position - transform.position).normalized*max_velocity;
		Vector3 steeringForce   = desiredVelocity - velocity;
		Vector3 acc = steeringForce/1;
		velocity += acc*Time.deltaTime;
		transform.position+=velocity*Time.deltaTime;



		//更新绘制路径
		timer+=Time.deltaTime;

		if(timer>0.1)
		{
			timer=0;
			pathList.Add(transform.position);
		}

		//绘制
		for(int i=0;i<pathList.Count-1;i++)
		{
			Debug.DrawLine(pathList[i],pathList[i+1],Color.red);
		}

		//更新朝向

	}
}
