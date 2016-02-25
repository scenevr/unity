using UnityEngine;
using System.Xml;
using System;

namespace SceneVR
{
	public class Link : SceneVR.Element
	{
		public GameObject InnerSphere;
		public GameObject OuterSphere;

        public Link ()
		{
		}

		public void CreateGeometry(){
			InnerSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			InnerSphere.transform.localScale = new Vector3(0.25f,0.25f,0.25f);
			InnerSphere.transform.SetParent(gameObject.transform);

			Renderer rend = InnerSphere.GetComponent<Renderer>();
			rend.material = new Material(Shader.Find("Specular"));
			rend.material.color = new Color(1, 0.4f, 0);
            
			OuterSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			OuterSphere.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
			OuterSphere.transform.SetParent(gameObject.transform);

			rend = OuterSphere.GetComponent<Renderer>();
			rend.material = new Material(Shader.Find("Specular"));
			rend.material.color = new Color(1, 0.4f, 0, 0.5f);
           
        }
        
        public static Link Create(XmlNode el){
			Link result = new Link();
			result.gameObject = new GameObject();
			result.gameObject.name = "<link />";
			result.node = el;

			result.CreateGeometry();

			result.SetPosition();
			result.SetRotation();
			result.SetScale();
			
			return result;
		}
	}
}

