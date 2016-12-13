using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Arrival : MonoBehaviour {


	public Transform target;
	public float max_velocity;
	public Vector3 velocity;


	//arrival
	public float slowDownRadius;

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

		Vector3 toTarget = target.position - transform.position;
		float distance   = toTarget.magnitude;

		Vector3 steeringForce = new Vector3(0,0,0);

		if(distance>slowDownRadius)
		{
			//plan a
			//Vector3 desiredVelocity = (target.position - transform.position).normalized*max_velocity;

			//plan b
			Vector3 desiredVelocity = toTarget.normalized*max_velocity*(distance/slowDownRadius);

			steeringForce = desiredVelocity - velocity;
		}
		else
		{
			Vector3 desiredVelocity = toTarget - velocity;
			steeringForce = desiredVelocity - velocity;
		}
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
		UpdateForward();
	}


	private void UpdateForward()
	{
		//计算出夹角(如果利用Atan只能处理1、4象限,而且不用处理除0错误)
		float radians = Mathf.Atan2( this.velocity.y , this.velocity.x );
		//转化为角度
		float degrees = radians * Mathf.Rad2Deg;

		//旋转
		this.transform.rotation=Quaternion.Euler(0,0,degrees);
	}
}
