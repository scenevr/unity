using UnityEngine;
using System.Xml;
using System.Collections;
using System;
using System.Linq;

namespace SceneVR
{
	public class Billboard : SceneVR.Element
	{
		public Billboard ()
		{
		}

		public override Vector3 ParseScale(){
			return ParseVector ("scale", 2f, 2f, 0.2f);
		}

        // Todo - rotate texture 180'

        public void Render(){
            Billboard.RenderToTexture(this);
		}

        // Get the html and rewrite all the image urls
        public string GetHtml()
        {
            string html = node.FirstChild.InnerText;
            return "<style>body,html{overflow: hidden;background:white;}body{-webkit-transform: scaleX(-1);}</style><body><div style='font-size: 22px; font-family: sans-serif; background: white; width: 512px; height: 512px; border: 1px solid #ccc'>" + html + "</div></body>";
        }

		public static Billboard Create(XmlNode el){
			Billboard result = new Billboard();

			result.gameObject = GameObject.CreatePrimitive (PrimitiveType.Cube);
			result.gameObject.name = "<billboard />";
			result.node = el;
			result.SetCommonAttributes();
			result.Render();

			return result;
		}

        public static Queue RenderJobs;

		/*
        public static void loadFinished(UWKWebView view)
        {
            Debug.Log("Finished rendering html!");

            if (RenderJobs.Count == 0)
            {
                Debug.Log("Render queue is empty, but rendering just finished, bug.");
                return;
            }

            Billboard b = (Billboard) RenderJobs.Dequeue();
            GameObject go = GameObject.Find("WebTexture");

            b.gameObject.GetComponent<Renderer>().material = go.GetComponent<Renderer>().material;

            // Texture t = GameObject.Instantiate(view.WebTexture);
            // b.gameObject.GetComponent<Renderer>().material.mainTexture = t;
            // .mainTexture = t;
        }
		*/

        public static void RenderToTexture(Billboard b)
        {
			/*
            GameObject go = GameObject.Find("WebTexture");
            UWKWebView view = go.GetComponent<UWKWebView>();

            if (RenderJobs == null)
            {
                view.LoadFinished += loadFinished;
                RenderJobs = new Queue();
            }

            RenderJobs.Enqueue(b);
            view.LoadHTML(b.GetHtml());
            */
        }
    }
}

