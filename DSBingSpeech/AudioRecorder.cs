using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Audio;
using Windows.Media.Capture;
using Windows.Media.Devices;
using Windows.Media.MediaProperties;
using Windows.Media.Render;
using Windows.Storage;

namespace DSBingSpeech
{
    public class AudioRecorder
    {
        private const uint CHANNEL = 1;
        private const uint BITS_PER_SAMPLE = 16;
        private const uint SAMPLE_RATE = 16000;
        private string _responseText;

        private AudioFileOutputNode _audioFileOutputNode;
        private AudioGraph _audioGraph;
        private string _outputFilename;
        private StorageFile _storageFile;
        string recordingFilename = "test.wav";
        public AudioRecorder()
        {

        }
        public bool GetStatues()
        {
            if (_audioGraph == null)
                return true;
            else
                return false;
        }
        public async void StartRecord()
        {
            string strTick = DateTime.Now.Ticks.ToString();
            //TODO: Shared Access Exception 처리 보완
            _outputFilename = recordingFilename;
            _storageFile = await ApplicationData.Current.LocalFolder
                .CreateFileAsync(_outputFilename, CreationCollisionOption.ReplaceExisting);

            await InitialiseAudioGraph();
            await InitialiseAudioFileOutputNode();
            await InitialiseAudioFeed();

            _audioGraph.Start();
        }

        public async Task<string> StopRecord()
        {
            
            _audioGraph.Stop();
            await _audioFileOutputNode.FinalizeAsync();

            _audioGraph.Dispose();
            _audioGraph = null;
            BingSpeechHelper bs = new BingSpeechHelper();
            // var result = await Singleton<BingSpeechHelper>.Instance.GetTextResultAsync("test.wav");

            var result = await bs.GetTextResultAsync("test.wav");
            if (result == null) return "실패";
            _responseText = result;
            return _responseText;
        }

        private async Task InitialiseAudioGraph()
        {
            // Prompt the user for permission to access the microphone. This request will only happen
            // once, it will not re-prompt if the user rejects the permission.
            var permissionGained = await AudioCapturePermission.RequestMicrophonePermission();
            if (permissionGained == false)
            {
                Debug.WriteLine("마이크 사용 권한을 얻지 못했습니다 .");
                return;
            }

            var audioGraphSettings = new AudioGraphSettings(AudioRenderCategory.Media);
            var audioGraphResult = await AudioGraph.CreateAsync(audioGraphSettings);

            if (audioGraphResult.Status != AudioGraphCreationStatus.Success)
                throw new InvalidOperationException("AudioGraph creation error !");

            _audioGraph = audioGraphResult.Graph;
        }

        private async Task InitialiseAudioFileOutputNode()
        {
            var outputProfile = MediaEncodingProfile.CreateWav(AudioEncodingQuality.Low);
            outputProfile.Audio = AudioEncodingProperties.CreatePcm(SAMPLE_RATE, CHANNEL, BITS_PER_SAMPLE);

            var outputResult = await _audioGraph.CreateFileOutputNodeAsync(_storageFile, outputProfile);

            if (outputResult.Status != AudioFileNodeCreationStatus.Success)
                throw new InvalidOperationException("AudioFileNode creation error !");

            _audioFileOutputNode = outputResult.FileOutputNode;
        }

        private async Task InitialiseAudioFeed()
        {
            var defaultAudioCaptureId = MediaDevice.GetDefaultAudioCaptureId(AudioDeviceRole.Default);
            var microphone = await DeviceInformation.CreateFromIdAsync(defaultAudioCaptureId);

            var inputProfile = MediaEncodingProfile.CreateWav(AudioEncodingQuality.High);
            var inputResult =
                await _audioGraph.CreateDeviceInputNodeAsync(MediaCategory.Media, inputProfile.Audio, microphone);

            if (inputResult.Status != AudioDeviceNodeCreationStatus.Success)
                throw new InvalidOperationException("AudioDeviceNode creation error !");

            inputResult.DeviceInputNode.AddOutgoingConnection(_audioFileOutputNode);
        }
    }
    
}
