using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace HuggingFace.API.Examples {
public class pulsarcubo : MonoBehaviour
{

    private AudioClip clip;
    private byte[] bytes;
    //private bool recording;


    void Start()
    {
        clip = Microphone.Start(null, false, 10, 44100);
        //recording = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        var position = Microphone.GetPosition(null);
        Microphone.End(null);
        var samples = new float[position * clip.channels];
        clip.GetData(samples, 0);
        bytes = EncodeAsWAV(samples, clip.frequency, clip.channels);
        //recording = false;
        SendRecording();
        clip = Microphone.Start(null, false, 10, 44100);
    }

    private void SendRecording() {
        HuggingFaceAPI.AutomaticSpeechRecognition(bytes, response => {
            Debug.Log(response);
            if (response == " Left.") {
                //Debug.Log("entra left");
                MoveLeft();
            } 
            else if (response == " Right.") {
              //Debug.Log("entra right");
               MoveRight(); 
            } 
        }, error => {
            Debug.Log(error);
        });
    }

    private byte[] EncodeAsWAV(float[] samples, int frequency, int channels) {
        using (var memoryStream = new MemoryStream(44 + samples.Length * 2)) {
            using (var writer = new BinaryWriter(memoryStream)) {
                writer.Write("RIFF".ToCharArray());
                writer.Write(36 + samples.Length * 2);
                writer.Write("WAVE".ToCharArray());
                writer.Write("fmt ".ToCharArray());
                writer.Write(16);
                writer.Write((ushort)1);
                writer.Write((ushort)channels);
                writer.Write(frequency);
                writer.Write(frequency * channels * 2);
                writer.Write((ushort)(channels * 2));
                writer.Write((ushort)16);
                writer.Write("data".ToCharArray());
                writer.Write(samples.Length * 2);

                foreach (var sample in samples) {
                    writer.Write((short)(sample * short.MaxValue));
                }
            }
            return memoryStream.ToArray();
        }
    }

    private void MoveLeft() {
        transform.Translate(-10f, 0, 0);
    }

    private void MoveRight() {
        transform.Translate(10f, 0, 0);
    }
}
}
