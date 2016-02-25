using UnityEngine;
using System.Xml;

using System;
namespace SceneVR
{
	public class Box : SceneVR.Element
	{
		public Box ()
		{
		}

		public static Box Create(XmlNode el){
			Box result = new Box();
				
			GameObject cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
			result.gameObject = cube;
			result.gameObject.name = "<box />";
			result.node = el;
			result.SetCommonAttributes();
			result.SetMaterial();

			return result;
		}
	}
}

