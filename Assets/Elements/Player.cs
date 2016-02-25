using UnityEngine;
using System.Xml;
using System;

namespace SceneVR
{
	public class Player : SceneVR.Element
	{
		public GameObject Head;
		public GameObject Body;
		
		public Player ()
		{
		}

		public override void SetRotation(){
			Head.transform.rotation = ParseQuaternion("rotation");
		}

		public void CreateGeometry(){
			Head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			Head.name = "head";
			Head.transform.SetParent(gameObject.transform);
            Head.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
			Head.transform.localPosition = new Vector3(0f, 0.6f, 0f);

			Renderer rend = Head.GetComponent<Renderer>();
			rend.material = new Material(Shader.Find("Specular"));
			rend.material.color = new Color(1f, 1f, 0.66f);
            
			Body = (GameObject) GameObject.Instantiate (GameObject.Find ("PlayerConeMesh"));
			Body.name = "body";
			Body.transform.SetParent(gameObject.transform);

			rend = Body.GetComponent<Renderer>();
			rend.material = new Material(Shader.Find("Specular"));
			rend.material.color = Color.grey;
		}
		
		public static Player Create(XmlNode el){
			Player result = new Player();

			result.gameObject = new GameObject();
			result.gameObject.name = "<player />";

			result.node = el;
			
			result.CreateGeometry();
			result.SetPosition();
			result.SetRotation();
			result.SetScale();

			return result;
		}
	}
}

