using UnityEngine;
using System.Xml;
using System;

namespace SceneVR
{
    public class Group : SceneVR.Element
    {
        public Group()
        {
        }

        public static Group Create(XmlNode el)
        {
            Group result = new Group();

            result.gameObject = new GameObject();
            result.gameObject.name = "<group />";
            result.node = el;
            result.SetCommonAttributes();
            Debug.Log(el.Attributes["rotation"].Value.ToString());
            Debug.Log(result.gameObject.transform.rotation.ToEulerAngles().ToString());

            return result;
        }
    }
}

