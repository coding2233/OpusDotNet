using System.Collections;
using System.Collections.Generic;
using POpusCodec;
using UnityEngine;

public class OpusSample : MonoBehaviour
{
    [SerializeField]
    private AudioClip _clip;

    private List<float> _receiveData=new List<float>();

    // Start is called before the first frame update
    IEnumerator Start()
    {
        AudioClip clip=AudioClip.Create("test002",48000,2,_clip.frequency,true,OnAudioRead,OnAudioSetPosition);
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.clip=clip;
        audioSource.Play();

        yield return null;


        OpusEncoder encoder = new OpusEncoder(POpusCodec.Enums.SamplingRate.Sampling48000, POpusCodec.Enums.Channels.Stereo);
        OpusDecoder decoder=new OpusDecoder(POpusCodec.Enums.SamplingRate.Sampling48000, POpusCodec.Enums.Channels.Stereo);
        float[] datas= new float[_clip.samples];
        List<float> sendData=new List<float>();
        if (_clip.GetData(datas,0))
        {
            sendData.AddRange(datas);
            
            
           
           // AudioClip clip=AudioClip.Create("test-002",newFloatData.Length,2,960);
        }
        else
        {
            Debug.Log($"Unity AudioClip 获取数据失败");
        }
        
        float[] floatData=new float[2000];
        while(sendData.Count>0)
        {
            if (sendData.Count<floatData.Length)
            {
                floatData=new float[sendData.Count];
            }

            floatData=sendData.GetRange(0,floatData.Length).ToArray();
            sendData.RemoveRange(0,floatData.Length);
            Debug.Log($"@@@@@@@@@@@@@@@@@@@@--转码前float的数据:{floatData.Length}  剩余数量:{sendData.Count}");
            byte[] newData=encoder.Encode(floatData);
            Debug.Log($"####################--转码过后byte的数据:{newData.Length}");
            float[] newFloatData = decoder.DecodePacketFloat(newData);
            _receiveData.AddRange(newFloatData);
            Debug.Log($"$$$$$$$$$$$$$$$$$$$$$--解码过后float的数据:{newFloatData.Length}");
            
            yield return null;

            yield return new WaitForSeconds(0.1f);
        }

        Debug.Log("--------------------------------------------------!!");

    }
    
    // Update is called once per frame
    void Update()
    {
        
    }


     // this is used by the second audio source, to read data from playData and play it back
    // OnAudioRead requires the AudioSource to be on the same GameObject as this script
    void OnAudioRead(float[] data) {
       // Debug.LogWarning("Opustest.OnAudioRead: " + data.Length);

        int pullSize = Mathf.Min(data.Length, _receiveData.Count);
        float[] dataBuf = _receiveData.GetRange(0, pullSize).ToArray();
        dataBuf.CopyTo(data,0);
        _receiveData.RemoveRange(0, pullSize);

        // clear rest of data
        for (int i=pullSize; i<data.Length; i++) {
            data[i] = 0;
        }
    }
    void OnAudioSetPosition(int newPosition) {
        // we dont need the audio position at the moment
    }   



}
