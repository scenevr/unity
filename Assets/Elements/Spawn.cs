using UnityEngine;
using System.Xml;

using System;
namespace SceneVR
{
	public class Spawn : SceneVR.Element
	{
		public Spawn ()
		{
		}

		public static Spawn Create(XmlNode el){
			Spawn result = new Spawn();

			result.gameObject = new GameObject();
			result.gameObject.name = "<spawn />";
			result.node = el;

			result.SetPosition();
			result.SetRotation();

			return result;
		}
	}
}

