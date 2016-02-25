using UnityEngine;
using System;
using System.Linq;

namespace SceneVR
{
	public class Connector
	{
		public Uri uri;

		public Connector ()
		{
			string[] arguments = Environment.GetCommandLineArgs();

			// uri = new Uri("ws://grid.scenevr.com:8080/scenes/120");
			uri = new Uri("wss://grid.scenevr.com/scenes/120");
            // uri = new Uri("ws://samples.scenevr.hosting:8080/color-change.xml");
            // uri = new Uri("ws://192.168.1.4:8090/scenes/11");

            string uriArgument = arguments.Last();

			if (uriArgument.StartsWith("scenevr://")){
				string uriString = uriArgument.Substring(10);
				Debug.Log("Parsing: ");
				Debug.Log(uriString);
				uri = new Uri(uriString);
			}
		}
	}
}

