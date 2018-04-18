// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// Ritchie Lozada (rlozada@microsoft.com)


//#define VP8_ENCODING

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

#if !UNITY_EDITOR
using Org.WebRtc;
using WebRtcWrapper;
using PeerConnectionClient.Model;
using PeerConnectionClient.Signalling;
using PeerConnectionClient.Utilities;
#endif

public class ControlScript : MonoBehaviour
{
    private const int textureWidth = 640;
    private const int textureHeight = 480;
    private string _serverAddress;
    private string _peerName;
    public string MessageToSend;
    public Renderer RenderTexture;
    public Transform VirtualCamera;
    public float TextureScale = 1f;
    public int PluginMode = 0;

    private Transform camTransform;
    private Vector3 prevPos;
    private Quaternion prevRot;

    private int frameCounter = 0;
    private int fpsCounter = 0;
    private float fpsCount = 0f;
    private float startTime = 0;
    private float endTime = 0;

#if !UNITY_EDITOR
    private WebRtcControl _webRtcControl;
    private static readonly ConcurrentQueue<Action> _executionQueue = new ConcurrentQueue<Action>();
#else
    private static readonly Queue<Action> _executionQueue = new Queue<Action>();
#endif
    private bool frame_ready_receive = true;
    private string messageText;
    public InputField serverInput;

    #region Graphics Low-Level Plugin DLL Setup
#if !UNITY_EDITOR
    public RawVideoSource rawVideo;
    public EncodedVideoSource encodedVideo;
    private MediaVideoTrack _peerVideoTrack;

    [DllImport("TexturesUWP")]
    private static extern void SetTextureFromUnity(System.IntPtr texture, int w, int h);

    [DllImport("TexturesUWP")]
    private static extern void ProcessRawFrame(uint w, uint h, IntPtr yPlane, uint yStride, IntPtr uPlane, uint uStride,
        IntPtr vPlane, uint vStride);

    [DllImport("TexturesUWP")]
    private static extern void ProcessH264Frame(uint w, uint h, IntPtr data, uint dataSize);

    [DllImport("TexturesUWP")]
    private static extern IntPtr GetRenderEventFunc();

    [DllImport("TexturesUWP")]
    private static extern void SetPluginMode(int mode);
#endif
    #endregion

    void Awake()
    {
        // Local Dev Setup - Define Host Workstation IP Address
        //Apprentice 54.236.254.140
        _serverAddress = "192.168.1.209:8888";
        _peerName = "HoloLens";
    }

    void Start()
    {
        VirtualCamera = Camera.main.transform;
        camTransform = Camera.main.transform;
        prevPos = camTransform.position;
        prevRot = camTransform.rotation;

#if !UNITY_EDITOR        
        _webRtcControl = new WebRtcControl();
        _webRtcControl.OnInitialized += WebRtcControlOnInitialized;
        _webRtcControl.OnPeerMessageDataReceived += WebRtcControlOnPeerMessageDataReceived;
        _webRtcControl.OnStatusMessageUpdate += WebRtcControlOnStatusMessageUpdate;

        Conductor.Instance.OnAddRemoteStream += Conductor_OnAddRemoteStream;
        _webRtcControl.Initialize();


        // Setup Low-Level Graphics Plugin        
        CreateTextureAndPassToPlugin();
        SetPluginMode(PluginMode);
        StartCoroutine(CallPluginAtEndOfFrames());
#endif
    }

#if !UNITY_EDITOR
    private void Conductor_OnAddRemoteStream(MediaStreamEvent evt)
    {
        //try 
        //{
            System.Diagnostics.Debug.WriteLine("Conductor_OnAddRemoteStream()");
            _peerVideoTrack = evt.Stream.GetVideoTracks().FirstOrDefault();
            if (_peerVideoTrack != null)
            {
            System.Diagnostics.Debug.WriteLine("\r\n We found video tracks: " + evt.Stream.GetVideoTracks().Count);
                System.Diagnostics.Debug.WriteLine(
                    "Conductor_OnAddRemoteStream() - GetVideoTracks: {0}",
                    evt.Stream.GetVideoTracks().Count);
            // Raw Video from VP8 Encoded Sender
            // H264 Encoded Stream does not trigger this event

            // TODO:  Switch between VP8 Decoded RAW or H264 ENCODED Frame
#if VP8_ENCODING
                System.Diagnostics.Debug.WriteLine("Subscribing to EncodedVideo_OnRawVideoFrame - VP8");
                rawVideo = Media.CreateMedia().CreateRawVideoSource(_peerVideoTrack);
                rawVideo.OnRawVideoFrame += Source_OnRawVideoFrame;
#else
            //Here we take media with stream
            encodedVideo = Media.CreateMedia().CreateEncodedVideoSource(_peerVideoTrack);
            if (encodedVideo != null)
            System.Diagnostics.Debug.WriteLine("Subscribing to EncodedVideo_OnEncodedVideoFrame - H.264");
            encodedVideo.OnEncodedVideoFrame += EncodedVideo_OnEncodedVideoFrame;
#endif
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Conductor_OnAddRemoteStream() - peerVideoTrack NULL");
            }
            _webRtcControl.IsReadyToDisconnect = true;
        //}
        //catch (Exception ex)
        //{
        //    Debug.Log(Environment.NewLine + ex.ToString());
        //
        //}

    }   

    private void EncodedVideo_OnEncodedVideoFrame(uint w, uint h, byte[] data)
    {
        System.Diagnostics.Debug.WriteLine(w + "x" + h + " data length: " + data.Length);

        frameCounter++;
        fpsCounter++;

        messageText = data.Length.ToString();

        if (data.Length == 0)
            return;

        if (frame_ready_receive)
            frame_ready_receive = false;
        else
            return;

        GCHandle buf = GCHandle.Alloc(data, GCHandleType.Pinned);
        ProcessH264Frame(w, h, buf.AddrOfPinnedObject(), (uint)data.Length);
        buf.Free();
    }

    private void Source_OnRawVideoFrame(uint w, uint h, byte[] yPlane, uint yStride, byte[] vPlane, uint vStride, byte[] uPlane, uint uStride)
    {
        frameCounter++;
        fpsCounter++;

        messageText = string.Format("{0}-{1}\n{2}-{3}\n{4}-{5}", 
            yPlane != null ? yPlane.Length.ToString() : "X", yStride,
            vPlane != null ? vPlane.Length.ToString() : "X", vStride,
            uPlane != null ? uPlane.Length.ToString() : "X", uStride);

        System.Diagnostics.Debug.WriteLine(messageText);


        if ((yPlane == null) || (uPlane == null) || (vPlane == null))
            return;

        if (frame_ready_receive)
            frame_ready_receive = false;
        else
            return;

        GCHandle yP = GCHandle.Alloc(yPlane, GCHandleType.Pinned);
        GCHandle uP = GCHandle.Alloc(uPlane, GCHandleType.Pinned);
        GCHandle vP = GCHandle.Alloc(vPlane, GCHandleType.Pinned);
        ProcessRawFrame(w, h, yP.AddrOfPinnedObject(), yStride, uP.AddrOfPinnedObject(), uStride,
            vP.AddrOfPinnedObject(), vStride);
        yP.Free();
        uP.Free();
        vP.Free();        
    }
#endif

    private void WebRtcControlOnInitialized()
    {
        EnqueueAction(OnInitialized);
    }


    private void OnInitialized()
    {
#if !UNITY_EDITOR
        // _webRtcUtils.SelectedVideoCodec = _webRtcUtils.VideoCodecs.FirstOrDefault(x => x.Name.Contains("H264"));
        // _webRtcUtils.IsMicrophoneEnabled = false;
//      //  PeerConnectionClient.Signalling.Conductor.Instance.MuteMicrophone();
#if VP8_ENCODING
        _webRtcUtils.SelectedVideoCodec = _webRtcUtils.VideoCodecs.FirstOrDefault(x => x.Name.Contains("VP8"));
#else
        _webRtcControl.SelectedVideoCodec = _webRtcControl.VideoCodecs.FirstOrDefault(x => x.Name.Contains("H264"));
        System.Diagnostics.Debug.WriteLine("H264 Encoding");
        //This is the one that is being used in HoloLens
#endif
        _webRtcControl.Peers.CollectionChanged += Peers_CollectionChanged;
#endif
        System.Diagnostics.Debug.WriteLine(" WebRTC Initialized\n");


    }

#if !UNITY_EDITOR
    private void Peers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("New Peer Connected");
    }
#endif

    private void WebRtcControlOnPeerMessageDataReceived(int peerId, string message)
    {
        System.Diagnostics.Debug.WriteLine("PeerMessageDataReceived: " + message);       
    }

    private void WebRtcControlOnStatusMessageUpdate(string msg)
    {
        System.Diagnostics.Debug.WriteLine(msg);
    }

    public void ConnectToServer()
    {
        if (serverInput.text.Contains("."))
        {
            System.Diagnostics.Debug.WriteLine("Input field value: " + serverInput.text);
            _serverAddress = serverInput.text;
            System.Diagnostics.Debug.WriteLine("Server Address input field is not empty, assigning value");
        }
        System.Diagnostics.Debug.WriteLine("Selected address is " + _serverAddress);
        var signalhost = _serverAddress.Split(':');
        System.Diagnostics.Debug.WriteLine("After splitting address is " + signalhost[0]);
        var host = string.Empty;
        var port = string.Empty;
        if (signalhost.Length > 1)
        {
            host = signalhost[0];
            port = signalhost[1];
        }
        else
        {
            host = signalhost[0];
            port = "8888";
        }
        System.Diagnostics.Debug.WriteLine("Connecting To Server " + host + ":" + port);
        MessagePanel.Instance.ShowMessage("Connecting To Server " + host + ":" + port);
#if !UNITY_EDITOR
        _webRtcControl.ConnectToServer(host, port, _peerName);
#endif
    }

    public void DisconnectFromServer()
    {
#if !UNITY_EDITOR
        _webRtcControl.DisconnectFromServer();
#endif
    }

    public void ConnectToPeer()
    {
        MessagePanel.Instance.ShowMessage("Connecting To Peer");
        // TODO: Support Peer Selection        
#if !UNITY_EDITOR
        if(_webRtcControl.Peers.Count > 0)
        {
            _webRtcControl.SelectedPeer = _webRtcControl.Peers[0];
            _webRtcControl.ConnectToPeer();
            endTime = (startTime = Time.time) + 10f;
        }
#endif
    }

    public void DisconnectFromPeer()
    {
#if !UNITY_EDITOR
        if(encodedVideo != null)
        {
            encodedVideo.OnEncodedVideoFrame -= EncodedVideo_OnEncodedVideoFrame;            
        }

        _webRtcControl.DisconnectFromPeer();
#endif
    }

    public void SendMessageToPeer()
    {

#if !UNITY_EDITOR
        _webRtcControl.SendPeerMessageData(MessageToSend);
        System.Diagnostics.Debug.WriteLine("MessageToPeer: " + MessageToSend);
#endif
    }


    public void EnqueueAction(Action action)
    {
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(action);
        }
    }

    void Update()
    { //TODO: Check method SendPeerMessageData
#region Virtual Camera Control
        if (Vector3.Distance(prevPos, VirtualCamera.position) > 0.05f ||
            Quaternion.Angle(prevRot, VirtualCamera.rotation) > 2f)
        {
            prevPos = VirtualCamera.position;
            prevRot = VirtualCamera.rotation;
            var eulerRot = prevRot.eulerAngles;
            var camMsg = string.Format(
                @"{{""camera-transform"":""{0},{1},{2},{3},{4},{5}""}}",
                prevPos.x,
                prevPos.y,
                prevPos.z,
                eulerRot.x,
                eulerRot.y,
                eulerRot.z);

#if !UNITY_EDITOR
            _webRtcControl.SendPeerMessageData(camMsg);
#endif
        }
#endregion


        if (Time.time > endTime)
        {
            fpsCount = (float)fpsCounter / (Time.time - startTime);
            fpsCounter = 0;
            endTime = (startTime = Time.time) + 3;
        }

#if !UNITY_EDITOR
        lock (_executionQueue)            
        {
            while (!_executionQueue.IsEmpty)
            {
                Action qa;
                if (_executionQueue.TryDequeue(out qa))
                {
                    if(qa != null)
                        qa.Invoke();
                }
            }
        }
#endif
    }

    private void CreateTextureAndPassToPlugin()
    {
        //TODO: Change scale correctly
#if !UNITY_EDITOR

        RenderTexture.transform.localScale = new Vector3(-TextureScale, (float) textureHeight / textureWidth * TextureScale, 1f);

        Texture2D tex = new Texture2D(textureWidth, textureHeight, TextureFormat.ARGB32, false);        
        tex.filterMode = FilterMode.Point;       
        tex.Apply();
        RenderTexture.material.mainTexture = tex;

        SetTextureFromUnity(tex.GetNativeTexturePtr(), tex.width, tex.height);
#endif
        System.Diagnostics.Debug.WriteLine("CreateTextureAndPassToPlugin");
    }

    private IEnumerator CallPluginAtEndOfFrames()
    {
        while (true)
        {

            // Wait until all frame rendering is done
            yield return new WaitForEndOfFrame();

            // Issue a plugin event with arbitrary integer identifier.
            // The plugin can distinguish between different
            // things it needs to do based on this ID.
            // For our simple plugin, it does not matter which ID we pass here.

#if !UNITY_EDITOR

            switch (PluginMode)
            {
                case 0:
                    if (!frame_ready_receive)
                    {
                        GL.IssuePluginEvent(GetRenderEventFunc(), 1);
                        frame_ready_receive = true;
                    }
                    break;
                default:
                    GL.IssuePluginEvent(GetRenderEventFunc(), 1);
                    break;                
            }          
#endif
        }
    }
}
