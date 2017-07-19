using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System.IO;
using System.Diagnostics;
using Windows.Storage;
using System.Runtime.Serialization;
using System.Xml;
using Microsoft.ProjectOxford.Common.Contract;

namespace DSFace
{
    public class FaceRec
    {
        private readonly IFaceServiceClient _faceServiceClient;
        public List<DPerson> Persons { get; }

        private string NameId = "wl";
        private bool isInit = false;

        public FaceRec()
        {
            _faceServiceClient = new FaceServiceClient(Constraints.FaceKey);
            Persons = new List<DPerson>();
        }

        public async Task InitListAsync()
        {
            await CreateWhiteListPersonGroupAsync();

            StorageFolder appInstalledFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFolder assets = await appInstalledFolder.GetFolderAsync("Assets");
            var files = await assets.GetFoldersAsync();

            foreach (var file in files)
            {
                string name = file.Name;
                DPerson per = new DPerson(name);
                var f = await file.GetFilesAsync();
                if (f.Count != 0)
                {
                    per.Id = await AddPersonAsync(name);
                }
                foreach (var t in f)
                {
                    using (Stream s = await t.OpenStreamForReadAsync())
                    {
                        await AddPersonFaceAsync(per.Id, s);
                    }
                }
                Persons.Add(per);
            }

            await TrainAsync();
            isInit = true;
            Debug.WriteLine("Initalize Completed");
        }

        private bool LoadList()
        {
            try {
                Console.WriteLine("Deserializing an instance of the object.");
                FileStream fs = new FileStream("Person.xml", FileMode.Open);
                XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
                DataContractSerializer ser = new DataContractSerializer(typeof(List<DPerson>));

                List<DPerson> deserializedPerson = (List<DPerson>)ser.ReadObject(reader, true);
                reader.Dispose();
                fs.Dispose();

                return true;
            }
            catch (FileNotFoundException e)
            {
                Debug.WriteLine("File Not Exists!");
                return false;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Could not load list");
                return false;
            }
        }

        private bool SaveList()
        {
            try
            {
                FileStream writer = new FileStream("Person.xml", FileMode.Create);
                DataContractSerializer ser = new DataContractSerializer(typeof(List<DPerson>));
                ser.WriteObject(writer, Persons);
                writer.Dispose();

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Could not save list");
                return false;
            }
        }

        public async Task<Face[]> UploadAndDetectFaceAsync(Stream stream)
        {
            try
            {
                List<FaceAttributeType> type = new List<FaceAttributeType>();
                type.Add(FaceAttributeType.Emotion);
                
                var faces = await _faceServiceClient.DetectAsync(stream, true, false, type.AsEnumerable());
                return faces;
            }
            catch (FaceAPIException e)
            {
                Debug.WriteLine("Could not find any faces.");
                return new Face[0];
            }
        }

        public async Task<Dictionary<Guid, DSEmotion.Emotion>> GetEmotionByGuidAsync(Stream stream = null, Face[] faces = null)
        {
            try
            {
                if(faces == null && stream != null)
                {
                    faces = await UploadAndDetectFaceAsync(stream);
                }
                if (faces.Length == 0 || stream == null)
                {
                    return new Dictionary<Guid, DSEmotion.Emotion>();
                }

                var faceIds = faces.Select(face => face.FaceId);
                var faceAttributes = faces.Select(face => face.FaceAttributes);
                Dictionary<Guid, DSEmotion.Emotion> emotions = new Dictionary<Guid, DSEmotion.Emotion>();

                for(int i = 0; i < faceIds.ToArray().Length; i++)
                {
                    EmotionScores sc = faceAttributes.ElementAt(i).Emotion;
                    var rankedList = sc.ToRankedList();
                    emotions.Add(faceIds.ElementAt(i), DSEmotion.EmotionUtil.GetEmotionByString(rankedList.ElementAt(0).Key));
                }
                return emotions;
            }
            catch (FaceAPIException e)
            {
                Debug.WriteLine("UploadAndGetEmotionByGuidAsync - " + e.Message);
                return new Dictionary<Guid, DSEmotion.Emotion>();
            }
        }

        public async Task<FaceRectangle[]> UploadAndGetFaceRectangleAsync(Stream stream)
        {
            try
            {
                var faces = await UploadAndDetectFaceAsync(stream);

                if (faces.Length == 0)
                {
                    return new FaceRectangle[0];
                }

                var faceRects = faces.Select(face => face.FaceRectangle);
                return faceRects.ToArray();
            }
            catch (FaceAPIException e)
            {
                Debug.WriteLine("UploadAndGetFaceRectangleAsync - " + e.Message);
                return new FaceRectangle[0];
            }
        }

        public async Task<Guid[]> UploadAndGetFaceIdAsync(Stream stream)
        {
            try
            {
                var faces = await UploadAndDetectFaceAsync(stream);

                if (faces.Length == 0)
                {
                    return new Guid[0];
                }

                var faceId = faces.Select(face => face.FaceId);
                return faceId.ToArray();
            }
            catch (FaceAPIException e)
            {
                Debug.WriteLine("UploadAndGetFaceIdAsync - " + e.Message);
                return new Guid[0];
            }
        }

        public async Task CreateWhiteListPersonGroupAsync()
        {
            try
            {
                await _faceServiceClient.GetPersonGroupAsync(NameId);
                await _faceServiceClient.DeletePersonGroupAsync(NameId);
            }
            catch (Exception e) { }

            string name = "WhiteList";
            await _faceServiceClient.CreatePersonGroupAsync(NameId, name);
        }

        public async Task<Guid> AddPersonAsync(string name)
        {
            CreatePersonResult res = await _faceServiceClient.CreatePersonAsync(NameId, name);
            return res.PersonId;
        }

        public async Task<Person> GetPersonAsync(Guid id)
        {
            try
            {
                return await _faceServiceClient.GetPersonAsync(NameId, id);
            }
            catch(Exception e)
            {
                Debug.WriteLine("GetPersonAsync - " + e.Message);
                return null;
            }
        }

        public async Task AddPersonFaceAsync(Guid guid, Stream stream)
        {
            try
            {
                await _faceServiceClient.AddPersonFaceAsync(NameId, guid, stream);
            }
            catch(Exception e)
            {
                Debug.WriteLine("Fail to Add Person Face.");
            }
        }

        public async Task<IdentifyResult[]> IdentifyAsync(Stream stream = null, Face[] faces = null)
        {
            if (!isInit || (stream == null && faces == null))
            {
                return new IdentifyResult[0];
            }

            if (faces == null)
            {
                faces = await UploadAndDetectFaceAsync(stream);
            }
            var faceIds = faces.Select(face => face.FaceId).ToArray();

            try
            {
                IdentifyResult[] results = await _faceServiceClient.IdentifyAsync(NameId, faceIds);
                return results;
            }
            catch(Exception e)
            {
                Debug.WriteLine("IdentifyAsync - " + e.Message);
                return new IdentifyResult[0];
            }
        }

        public async Task<string[]> GetIdentifiedNameAsync(Stream stream = null, Face[] faces = null)
        {
            if (!isInit || (stream == null && faces == null))
            {
                return new string[0];
            }
            IdentifyResult[] results = null;
            if (faces == null)
            {
                results = await IdentifyAsync(stream);
            } else if(stream == null)
            {
                results = await IdentifyAsync(null, faces);
            }
            
            List<string> name = new List<string>();
            foreach (IdentifyResult identifyResult in results)
            {
                if (identifyResult.Candidates.Length == 0)
                {
                    Debug.WriteLine("No one identified");
                    name.Add("외부인");
                }
                else
                {
                    if(identifyResult.Candidates[0].Confidence > 0.6)
                    {
                        var candidateId = identifyResult.Candidates[0].PersonId;
                        var person = DPerson.GetDPersonByGuidFromList(Persons, candidateId);
                        if(person != null)
                        {
                            name.Add(person.Name);
                        }
                    }
                }
            }
            return name.ToArray();
        }

        public async Task TrainAsync()
        {
            try
            {
                await _faceServiceClient.TrainPersonGroupAsync(NameId);
            }
            catch(Exception e)
            {
                Debug.WriteLine("TrainAsync - " + e.Message);
            }
        }

        public bool IsInit()
        {
            return isInit;
        }
    }
}
