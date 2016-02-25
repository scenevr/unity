using UnityEngine;
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SceneVR
{
	public class Skybox : SceneVR.Element
	{
		public Skybox()
		{
		}

		protected List<string> ParseLinearGradient(string value){
			Regex r = new Regex(@"\((.+)\)");
			Match m = r.Match(value);
			List<string> result = new List<string>();

			foreach (string color in m.Groups[1].Value.Split(',')){
				result.Add(color.Trim());
			}

			return result;
		}

		public void Initialise(){
			Material m = RenderSettings.skybox;

			if (HasStyle("color")){
				string style = GetStyle("color");

				if (style.StartsWith("linear-gradient")){
					List<string> colors = ParseLinearGradient(style);

					m.SetColor("_Color1", ParseColor(colors[0]));
					m.SetColor("_Color2", ParseColor(colors[1]));
				}
			}

			// todo add textured skybox support
		}

		public static Skybox Create(XmlNode el){
			Skybox result = new Skybox();

			result.node = el;
			result.Initialise();
			return result;
		}
	}
}

