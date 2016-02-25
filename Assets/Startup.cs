using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using WebSocketSharp;
using System.Xml;
using SceneVR;
using System.Linq;

public class Startup : MonoBehaviour {

    Dictionary<string, Element> ElementMap = new Dictionary<string, Element>();
    WebSocket ws;
    Queue messages = new Queue();
    Queue messagesSync;
    String MyUuid;
    Connector connector;
    Spawn SpawnElement;

    // Use this for initialization
    void Start() {
    }

    void Awake() {
        ElementMap.Clear();

        messages.Clear();
        messagesSync = Queue.Synchronized(messages);

        ConnectToServer();
    }

    void OnDestroy() {
        Debug.Log("Destroyed");
        ws.Close();
    }

    Vector3 GetSpawnPosition() {
        if (SpawnElement != null) {
            return SpawnElement.gameObject.transform.position;
        }

        return new Vector3(0, 0, 0);
    }

    void Respawn() {
        // GameObject.Find("FPSController").transform.position = GetSpawnPosition();
    }

    void ConnectToServer() {
        connector = new Connector();

		Uri uri = connector.uri;
			
		// some weird bug with grid.scenevr.com TLS certificate and mono, so connect over 434
		if (uri.Scheme == "wss" && uri.Host == "grid.scenevr.com") {
			var builder = new UriBuilder(uri) {
				Scheme = "ws",
				Port = 444
			};

			uri = builder.Uri;
		}

		Debug.Log("Loading: " + uri.ToString());

        ws = new WebSocket(uri.ToString(), "scenevr");

//		ws.Log.Level = LogLevel.Trace;
//		ws.Log.File = "websocket.log";

        ws.OnOpen += (sender, e) => {
            Debug.Log("Connected!");
        };

//		ws.SslConfiguration.ServerCertificateValidationCallback = null;
			
        ws.OnClose += (object sender, CloseEventArgs e) => {
            Debug.Log("Disconnected!");
            Debug.Log(e.Reason);
        };

        ws.OnMessage += (sender, e) => {
            //Debug.Log ("Server says: " + e.Data);
            messagesSync.Enqueue(e.Data);
        };

        // ws.Origin = "http://unity.scenevr.com";

        ws.Connect();

        InvokeRepeating("SendPosition", 0f, 0.2f);
    }

    // Update is called once per frame
    void Update() {
        while (messagesSync.Count > 0) {
            OnMessage((string)messagesSync.Dequeue());
        }
    }

    void SendPosition() {
		/*
        Vector3 v = GameObject.Find("FPSController").transform.position;

        XmlDocument doc = new XmlDocument();

        XmlElement p = doc.CreateElement("packet");

        XmlElement n = doc.CreateElement("player");
        n.SetAttribute("position", v.x + " " + (0.75 + v.y) + " " + -v.z);
        n.SetAttribute("rotation", "0 0 0");
        p.AppendChild(n);

        ws.Send(p.OuterXml);
        */
    }

    Element AddElement(XmlNode node, Element parentElement = null)
    {
        Element el = new Element();

        if (node.Name == "box")
        {
            el = SceneVR.Box.Create(node);
        }
        else if (node.Name == "billboard")
        {
            el = SceneVR.Billboard.Create(node);
        }
        else if (node.Name == "group")
        {
            el = SceneVR.Group.Create(node);

            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                XmlNode childNode = node.ChildNodes[i];
                AddElement(childNode, el);
            }
        }
        else if (node.Name == "model")
        {
            el = SceneVR.Model.Create(node);
            el.connector = connector;
            StartCoroutine(((Model)el).LoadModel());
        }
        else if (node.Name == "player")
        {
            if (node.Attributes["uuid"].Value != MyUuid)
            {
                el = SceneVR.Player.Create(node);
            }
        }
        else if (node.Name == "link")
        {
            el = SceneVR.Link.Create(node);
        }
        else if (node.Name == "spawn")
        {
            el = SceneVR.Spawn.Create(node);
            SpawnElement = (Spawn)el;
            Respawn();
        }
        else if (node.Name == "skybox")
        {
            el = SceneVR.Skybox.Create(node);
        }
        else {
            Debug.Log("Unknown object " + node.Name);
            return null;
        }

        if (parentElement != null)
        {
            el.gameObject.transform.SetParent(parentElement.gameObject.transform);
        }

        return el;
    }

    void OnMessage(string data)
    {
        XmlDocument xml = new XmlDocument();
        xml.LoadXml(data);

        XmlNode packetNode = xml.FirstChild;

        if (packetNode.Name != "packet")
        {
            Debug.Log("Invalid packet from server");
            return;
        }

        for (int i = 0; i < packetNode.ChildNodes.Count; i++)
        {
            XmlNode node = packetNode.ChildNodes[i];
            ProcessMessage(node);
        }
    }

    void ProcessMessage(XmlNode node) {
        if (node.Name == "version") {
            // We can ignore these
            return;
        }

        if (node.Name == "event") {
            string name = node.Attributes["name"].Value;

            if (name == "ready") {
                MyUuid = node.Attributes["uuid"].Value;
                return;
            }

            Debug.Log("Unhandled <event name='" + name + "' />");
            return;
        }

        String uuid = node.Attributes["uuid"].Value;

        if (uuid == null)
        {
            Debug.Log("No uuid found in element <" + node.Name + "/>");
            return;
        }

        Element el = null;

        if (node.Name == "dead") {
            if (ElementMap.ContainsKey(uuid)) {
                ElementMap[uuid].Remove();
                ElementMap.Remove(uuid);
            }

            return;
        }

        if (ElementMap.ContainsKey(uuid)) {
            el = ElementMap[uuid];
        } else {
            el = AddElement(node);

            if (el != null) {
                ElementMap[uuid] = el;
            }

            return;
        }

        if (el == null) {
            // shouldnt happen
            return;
        } else if (!el.HasGameObject()) {
            // might happen (skybox / fog)
            return;
        } else if (el.SubstantialDifference(node)) {
            el.RegenerateFrom(node);
        } else {
            el.node = node;

            LeanTween.move(el.gameObject, el.ParsePosition(), 0.2f);
            // el.SetPosition();

            //LeanTween.scale (el.gameObject, el.ParseScale(), 0.2f);
            el.SetScale();
            el.SetRotation();
        }
    }
}