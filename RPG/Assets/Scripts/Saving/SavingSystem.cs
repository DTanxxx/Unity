using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace RPG.Saving
{
    public class SavingSystem : MonoBehaviour
    {
        public IEnumerator LoadLastScene(string saveFile)
        {
            Dictionary<string, object> state = LoadFile(saveFile);
            int buildIndex = SceneManager.GetActiveScene().buildIndex;

            if (state.ContainsKey("lastSceneBuildIndex"))
            {
                buildIndex = (int)state["lastSceneBuildIndex"];
            }

            yield return SceneManager.LoadSceneAsync(buildIndex);  // yield returning LoadSceneAsync here so that RestoreState is called after Awake and before Start (to avoid race condition)
            RestoreState(state);
        }

        public void Save(string saveFile)
        {
            Dictionary<string, object> state = LoadFile(saveFile);
            CaptureState(state);
            SaveFile(saveFile, state);
        }

        public void Load(string saveFile)
        {
            RestoreState(LoadFile(saveFile));
        }

        public void Delete(string saveFile)
        {
            File.Delete(GetPathFromSaveFile(saveFile));
        }

        private void SaveFile(string saveFile, object state)
        {
            string path = GetPathFromSaveFile(saveFile);
            Debug.Log("Saving to " + path);
            using (FileStream stream = File.Open(path, FileMode.Create))
            {
                //Transform playerTransform = GetPlayerTransform();
                //byte[] buffer = SerializeVector(playerTransform.position);
                //stream.Write(buffer, 0, buffer.Length);

                //SerializableVector3 position = new SerializableVector3(playerTransform.position);

                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, state);
            }
        }

        private Dictionary<string, object> LoadFile(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);            

            if (!File.Exists(path))
            {
                return new Dictionary<string, object>();
            }

            using (FileStream stream = File.Open(path, FileMode.Open))
            {
                //byte[] buffer = new byte[stream.Length];
                //stream.Read(buffer, 0, buffer.Length);
                //playerTransform.position = DeserializeVector(buffer);           

                //Transform playerTransform = GetPlayerTransform();
                //playerTransform.position = position.ToVector();

                BinaryFormatter formatter = new BinaryFormatter();
                return (Dictionary<string, object>)formatter.Deserialize(stream);
            }
        }

        private void CaptureState(Dictionary<string, object> state)
        {
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                state[saveable.GetUniqueIdentifier()] = saveable.CaptureState();
            }

            state["lastSceneBuildIndex"] = SceneManager.GetActiveScene().buildIndex;
        }

        private void RestoreState(Dictionary<string, object> state)
        {            
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                string id = saveable.GetUniqueIdentifier();
                if (state.ContainsKey(id))
                {
                    saveable.RestoreState(state[id]);
                }                
            }
        }

        private string GetPathFromSaveFile(string saveFile)
        {
            return Path.Combine(Application.persistentDataPath, saveFile + ".sav");
        }

        //private byte[] SerializeVector(Vector3 vector)
        //{
        //    byte[] vectorBytes = new byte[12];            
        //    BitConverter.GetBytes(vector.x).CopyTo(vectorBytes, 0);
        //    BitConverter.GetBytes(vector.y).CopyTo(vectorBytes, 4);
        //    BitConverter.GetBytes(vector.z).CopyTo(vectorBytes, 8);
        //    return vectorBytes;
        //}

        //private Vector3 DeserializeVector(byte[] buffer)
        //{
        //    Vector3 result = new Vector3();
        //    result.x = BitConverter.ToSingle(buffer, 0);
        //    result.y = BitConverter.ToSingle(buffer, 4);
        //    result.z = BitConverter.ToSingle(buffer, 8);
        //    return result;
        //}
    }
}
