using UnityEngine;
using System.Collections;

public class WallAvoid : MonoBehaviour {

	private Rigidbody2D rb;

	// Use this for initialization
	void Start () {
		rb=GetComponent<Rigidbody2D>();
		rb.velocity=new Vector3(1,0);
	}
	
	// Update is called once per frame
	void Update () {

		Debug.DrawLine(transform.position,transform.position+5*getForward(),Color.green);
		Debug.DrawLine(transform.position,transform.position+5*(getForward()+getRight()),Color.green);
		Debug.DrawLine(transform.position,transform.position+5*(getForward()+getLeft()),Color.green);
	}

	/// <summary>
	/// 获取朝向
	/// </summary>
	/// <returns>The forward.</returns>
	public Vector3  getForward()
	{
		Vector2 velocity = rb.velocity;
		float radians = Mathf.Atan2( velocity.y , velocity.x );
		return new Vector3(Mathf.Cos(radians), Mathf.Sin(radians),0);
	}

	public Vector3  getRight()
	{
		return -getLeft();
	}

	public Vector3  getLeft()
	{
		Vector2 velocity = rb.velocity;
		float radians = Mathf.Atan2( velocity.y , velocity.x );
		return new Vector3(-Mathf.Sin(radians), -Mathf.Cos(radians),0);
	}
}
