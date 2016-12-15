using UnityEngine;
using System.Collections;
using System.Collections.Generic;

struct WanderParameter
{
	//wander圈的大小
	public float Radius;
	//wander突出在智能体前面的距离
	public float Distance;
	//每秒加到目标随机位置的最大值
	public float WanderJitter;

	public float AngleChange;
}


public class Wander : MonoBehaviour {

	//
	public Vector3 velocity;
	private const float velocityMax=6f;
	private const float forceMax =50f;

	//wander参数
	//private Vector2 wanderTarget;


	WanderParameter wanderParameter;
	float wanderTheta;
	float timeSinceLastGernateWander;
	Vector2 lastWanderForce;




	//绘制轨迹
	private List<Vector3> pathList;
	private float timer=0;

	// Use this for initialization
	void Start () {

		//初始化wander的参数
		wanderParameter.Distance = 2f;
		wanderParameter.Radius = 2f;
		wanderParameter.WanderJitter = 100f;
		wanderParameter.AngleChange = Mathf.PI;

		//stuff for the wander behavior
		wanderTheta = UnityEngine.Random.Range(0f,1f) * 2 * Mathf.PI;

		//初始化绘制路径的参数
		pathList = new List<Vector3>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		//更新位置
		Vector3 steeringForce = CalWanderForce();
		steeringForce=TruncateForce(steeringForce);

		Vector3 acc = steeringForce/1;
		velocity += acc*Time.deltaTime;
		velocity=TruncateSpeed();
		transform.position+=velocity*Time.deltaTime;

		//如果超出边界，改变位置
		WrapAround();


		//更新绘制路径
		//DrawPath();

		//更新朝向
		UpdateForward();
	}

	/// <summary>
	/// Wander 游荡
	/// </summary>
	Vector2 CalWanderForce()
	{ 
		//计算时间
		timeSinceLastGernateWander += Time.deltaTime;

		//达到定时器时间
		if(timeSinceLastGernateWander > 2f)
		{
			Vector2 wanderForce        = GernateWanderForce();
			timeSinceLastGernateWander = 0;
			lastWanderForce            = wanderForce;
			return wanderForce;
		}
		//返回上一次力
		else
		{
			return lastWanderForce;
		}
	}

	/// <summary>
	/// 产生一个随机的力
	/// </summary>
	/// <returns>The wander force.</returns>
	Vector2 GernateWanderForce()
	{
		// Calculate the circle center
		Vector2 circleCenter = new Vector2(this.velocity.x,this.velocity.y);
		circleCenter.Normalize();
		circleCenter *= wanderParameter.Distance;

		// Calculate the displacement force
		Vector2 displacement = new Vector2(0, -1);
		displacement *= wanderParameter.Radius;

		//
		// Randomly change the vector direction
		// by making it change its current angle
		displacement = SetAngle(displacement, wanderTheta);


		//
		// Change wanderAngle just a bit, so it
		// won't have the same value in the
		// next game frame.
		wanderTheta += UnityEngine.Random.Range(-1f,1f) * wanderParameter.AngleChange;


		// Finally calculate and return the wander force
		Vector2 wanderForce = circleCenter+displacement;

		return  wanderForce;
	}


	Vector2 SetAngle( Vector2 vector,float angle) {
		var length= vector.magnitude;
		vector.x = Mathf.Cos(angle) * length;
		vector.y = Mathf.Sin(angle) * length;
		return vector;
	}


	/// <summary>
	/// 截断最大力
	/// </summary>
	/// <returns>The force.</returns>
	/// <param name="force">Force.</param>
	private Vector2 TruncateForce(Vector2 force)
	{
		//利用速度的大小的平方来比较，省去了开平方根的消耗时间
		if(force.sqrMagnitude > 2 * forceMax*forceMax)
		{
			return force.normalized * forceMax;
		}
		else
		{
			return force;
		}
	}


	/// <summary>
	/// 如果速度大于最大速度，那么截断
	/// </summary>
	private Vector2 TruncateSpeed()
	{
		//利用速度的大小的平方来比较，省去了开平方根的消耗时间
		if(velocity.sqrMagnitude > 2 * velocityMax*velocityMax)
		{
			return velocity.normalized * velocityMax;
		}
		else
		{
			return velocity;
		}
	}


	/// <summary>
	/// 把屏幕做成一个圆环
	/// </summary>
	void WrapAround( )
	{
		Vector2 cameraSize = CameraTool.Instance.GetCameraSizeInUnit();
		//Rect screen=new Rect(Screen.width);
		Vector2 nowPos = new Vector2(transform.position.x,transform.position.y);
		if (nowPos.y > cameraSize.y / 2)
		{
			transform.position = new Vector3(nowPos.x, nowPos.y - cameraSize.y, 0);
		}
		else if(nowPos.y < - cameraSize.y / 2)
		{
			transform.position = new Vector3(nowPos.x, nowPos.y + cameraSize.y, 0);
		}

		if (nowPos.x > cameraSize.x / 2)
		{
			transform.position = new Vector3(nowPos.x-cameraSize.x, nowPos.y, 0);

		}
		else if(nowPos.x < -cameraSize.x / 2)
		{
			transform.position = new Vector3(nowPos.x+cameraSize.x, nowPos.y, 0);

		};

	}

	/// <summary>
	/// 绘制路径
	/// </summary>
	void DrawPath()
	{
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
	}


	/// <summary>
	/// 更新朝向
	/// </summary>
	private void UpdateForward()
	{
		//计算出夹角(如果利用Atan只能处理1、4象限,而且不用处理除0错误)
		float radians = Mathf.Atan2( this.velocity.y , this.velocity.x );
		//转化为角度
		float degrees = radians * Mathf.Rad2Deg;

		//旋转
		this.transform.rotation=Quaternion.Euler(0,0,degrees);
	}




	/// <summary>
	/// 测试诞生的随机力
	/// </summary>
	void OnDrawGizmos()
	{

		// 设置颜色
		Gizmos.color = Color.green;

		//计算绘制圆圈的中心偏移量
		//移动距离
		Vector2 forward=getForward();
		Vector3 offset = new Vector3(wanderParameter.Distance * forward.y,
			wanderParameter.Distance * forward.x,0);

		// 绘制圆环
		Vector3 beginPoint =   transform.position;
		Vector3 firstPoint =   transform.position;

		//转一圈
		float m_Theta = 0.1f;
		for (float theta = 0; theta < 2 * Mathf.PI; theta += m_Theta)
		{
			//计算
			float x = wanderParameter.Radius * Mathf.Cos(theta);
			float y = wanderParameter.Radius * Mathf.Sin(theta);

			Vector3 endPoint = transform.position +offset + new Vector3(x , y, 0);
			if (theta == 0)
			{
				firstPoint = endPoint;
			}
			else
			{
				Gizmos.DrawLine(beginPoint, endPoint);
			}
			beginPoint = endPoint;
		}

		// 绘制最后一条线段
		Gizmos.DrawLine(firstPoint, beginPoint);

		//再话一条直线
		Vector2 wanderForce = lastWanderForce;
		Gizmos.DrawLine( transform.position, transform.position + new Vector3(wanderForce.x,wanderForce.y,0));

	}


	/// <summary>
	/// 获取朝向
	/// </summary>
	/// <returns>朝向.</returns>
	public Vector2  getForward()
	{
		float radians = Mathf.Atan2( velocity.y , velocity.x );
		return new Vector2(Mathf.Sin(radians), Mathf.Cos(radians));
	}
}
