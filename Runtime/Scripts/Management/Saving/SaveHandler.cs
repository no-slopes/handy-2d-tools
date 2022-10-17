using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using H2DT.Cripto;
using H2DT.Generics.Threading;
using H2DT.NaughtyAttributes;
using H2DT.Serializing;
using UnityEngine;
using UnityEngine.Events;
using H2DT.Utils;
using H2DT.Management.Booting;

namespace H2DT.Management.Saving
{
    public abstract class SaveHandler<T0> : ScriptableObject, IBootable where T0 : SaveSlot
    {
        #region Static

        public static string SlotFileExtension = ".slot";
        public static string DataFileExtension = ".save";

        #endregion

        #region Inspector

        [Header("Events")]
        [Space]
        [SerializeField]
        public UnityEvent<bool> SavingProcessUpdate;

        #endregion

        #region Fields

        [Foldout("Debug")]
        [ReadOnly]
        [SerializeField]
        protected T0 _currentSlot;

        // Actions
        private UnityAction<T0> OnSlotSelectedAction;
        private UnityAction<T0> OnSlotReleasedAction;
        private UnityAction<T0> OnSlotCreatedAction;
        private UnityAction<T0> OnSlotDeletedAction;

        protected string _saveDirectory;
        private TaskQueue _tasksQueue;
        private bool _currentlySaving;

        #endregion

        #region Properties 

        protected string SlotsPath => _saveDirectory + "/Slots";
        protected string DataPath => _saveDirectory + "/Data";

        #endregion

        #region Getters

        public bool HasSelectedSlot => _currentSlot != null;
        public bool currentlySaving => _currentlySaving;

        #endregion

        #region Mono

        public virtual async Task BootableBoot()
        {
            _saveDirectory = Application.persistentDataPath + "/Saves";
            _tasksQueue = new TaskQueue();

            await Task.Run(LoadActions);

            CreateDirectoryIfNotExists(SlotsPath);
            CreateDirectoryIfNotExists(DataPath);
        }

        public virtual async Task BootableDismiss()
        {
            if (!HasSelectedSlot) return;

            await ReleaseCurrentSlot();
        }

        #endregion

        #region Loading 

        /// <summary>
        /// Loading actions for children.
        /// </summary>
        protected virtual void LoadActions()
        {
            System.Type stateType = this.GetType();
            MethodInfo mi;

            mi = stateType.GetMethod("OnSlotSelected", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (mi != null)
                OnSlotSelectedAction = Delegate.CreateDelegate(typeof(UnityAction<T0>), this, mi) as UnityAction<T0>;

            mi = stateType.GetMethod("OnSlotReleased", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (mi != null)
                OnSlotReleasedAction = Delegate.CreateDelegate(typeof(UnityAction<T0>), this, mi) as UnityAction<T0>;

            mi = stateType.GetMethod("OnSlotCreated", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (mi != null)
                OnSlotCreatedAction = Delegate.CreateDelegate(typeof(UnityAction<T0>), this, mi) as UnityAction<T0>;

            mi = stateType.GetMethod("OnSlotDeleted", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (mi != null)
                OnSlotDeletedAction = Delegate.CreateDelegate(typeof(UnityAction<T0>), this, mi) as UnityAction<T0>;
        }

        #endregion

        #region Slots

        /// <summary>
        /// Defines current save data slot
        /// </summary>
        /// <param name="slot"></param>
        public void SelectSlot(T0 slot)
        {
            _currentSlot = slot;
            OnSlotSelectedAction?.Invoke(slot);
        }

        /// <summary>
        /// Saves the current Slot and releases it.
        /// </summary>
        /// <returns></returns>
        public async Task ReleaseCurrentSlot()
        {
            if (_currentSlot == null) return;

            await SaveCurrentSlot();
            OnSlotReleasedAction?.Invoke(_currentSlot);
            _currentSlot = null;
        }

        /// <summary>
        /// Creates a Slot.
        /// </summary>
        /// <param name="slot"></param>
        public async void CreateSlotFile(T0 slot)
        {
            if (!await SaveSlot(slot)) return;

            string slotDataDirectoryPath = GetSlotDataDirectoryPath(slot);

            CreateDirectoryIfNotExists(DataPath + "/" + slot.id);

            OnSlotCreatedAction?.Invoke(slot);
        }

        /// <summary>
        /// Persists the current slot
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SaveCurrentSlot()
        {
            if (_currentSlot == null) return true;

            return await SaveSlot(_currentSlot);
        }

        /// <summary>
        /// Saves a Slot.
        /// </summary>
        /// <param name="slot"></param>
        public async Task<bool> SaveSlot(T0 slot)
        {
            string slotFileName = $"{slot.id}{SlotFileExtension}";

            return await _tasksQueue.Enqueue<bool>(() => SaveFile(SlotsPath, slotFileName, slot));
        }

        /// <summary>
        /// Deletes a slot completly removing its data directory.
        /// </summary>
        /// <param name="slot"></param>
        public void DeleteSlot(T0 slot)
        {
            string slotFilePath = GetSlotFilePath(slot);

            if (File.Exists(slotFilePath))
            {
                File.Delete(slotFilePath);
            }

            string savesDirectoryPath = GetSlotDataDirectoryPath(slot);

            if (Directory.Exists(savesDirectoryPath))
            {
                Directory.Delete(savesDirectoryPath);
            }

            OnSlotDeletedAction?.Invoke(slot);
        }

        /// <summary>
        /// Gets all available slots
        /// </summary>
        /// <returns></returns>
        public List<T0> RetrieveAllSlots()
        {
            List<T0> slotList = new List<T0>();

            string[] files = Directory.GetFiles(SlotsPath);

            foreach (string file in files)
            {
                T0 slot = LoadFile<T0>(file);
                slotList.Add(slot);
            }

            return slotList;
        }

        /// <summary>
        /// Returns an specific slog based on its filepath.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public T0 RetrieveSlot(string filePath)
        {
            return LoadFile<T0>(filePath);
        }

        /// <summary>
        /// Gets the slot file path.
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public string GetSlotFilePath(T0 slot)
        {
            return SlotsPath + "/" + slot.id + SlotFileExtension;
        }

        /// <summary>
        /// Gets the slot file path.
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public string GetSlotFilePath(string slotId)
        {
            return SlotsPath + "/" + slotId + SlotFileExtension;
        }

        /// <summary>
        /// Gets the slot data directory path. This is the path in wich any data for that 
        /// specific slot will be stored.
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        protected string GetSlotDataDirectoryPath(T0 slot)
        {
            return DataPath + "/" + slot.id;
        }

        /// <summary>
        /// True if there is a currentSlot properly selected and its file is 
        /// present Slots path.
        /// </summary>
        /// <returns></returns>
        protected bool ValidateCurrentSlot()
        {
            if (_currentSlot == null)
            {
                Debug.LogError($"Trying to operate data but no slot is selected.");
                return false;
            }

            string slotPath = GetSlotFilePath(_currentSlot);

            if (!File.Exists(slotPath))
            {
                Debug.LogError($"Trying to operate data but current selected slot has no file or been corrupted.");
                return false;
            }

            return true;
        }

        #endregion

        #region Handling Data

        /// <summary>
        /// Stores an [System.Serializable] data into the current slot
        /// </summary>
        /// <param name="savable"></param>
        /// <param name="data"></param>
        public async Task<bool> SaveData(ISavable savable)
        {
            if (!ValidateCurrentSlot()) return false;
            if (!ValidateSavable(savable)) return false;
            if (savable.saveableData == null) return false;

            string directoryPath = GetSlotDataDirectoryPath(_currentSlot) + $"/{savable.savableContainer}";
            string fileName = $"{savable.savableId}{DataFileExtension}";

            return await _tasksQueue.Enqueue<bool>(() => SaveFile(directoryPath, fileName, savable.saveableData));
        }

        /// <summary>
        /// Loads some data from the current slot.
        /// </summary>
        /// <param name="savable"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T LoadData<T>(ISavable savable)
        {
            if (!ValidateCurrentSlot()) return default(T); // If there is no current slot selected.
            if (!ValidateSavable(savable)) return default(T); // If Savable structure is not well set

            string directoryPath = GetSlotDataDirectoryPath(_currentSlot) + $"/{savable.savableContainer}";
            string filePath = $"{directoryPath}/{savable.savableId}{DataFileExtension}";

            return LoadFile<T>(filePath);
        }

        /// <summary>
        /// Detects 
        /// </summary>
        /// <param name="savable"></param>
        /// <returns></returns>
        protected bool ValidateSavable(ISavable savable)
        {
            if (string.IsNullOrEmpty(savable.savableId))
            {
                Debug.LogError($"ISavable invalid: empty or null ID");
                return false;
            }

            if (string.IsNullOrEmpty(savable.savableContainer))
            {
                Debug.LogError($"ISavable invalid: empty or null Container name");
                return false;
            }

            return true;
        }

        #endregion

        #region Save Files

        /// <summary>
        /// Stores data into a Binary Formated file.
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="fileName"></param>
        /// <param name="saveData"></param>
        /// <returns></returns>
        protected async Task<bool> SaveFile(string directoryPath, string fileName, object saveData)
        {

            CreateDirectoryIfNotExists(directoryPath);
            string filePath = $"{directoryPath}/{fileName}";

            _currentlySaving = true;
            SavingProcessUpdate.Invoke(true);

            bool result = await Task.Run<bool>(() =>
            {
                try
                {
                    FileStream file = File.Create(filePath);
                    BinaryFormatter formatter = Serializer.GetBinaryFormatter();
                    formatter.Serialize(file, saveData);

                    file.Close();

                    return true;
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                    Debug.LogError($"Failed to save serialized file at {filePath}");
                    return false;
                }
            });

            _currentlySaving = false;
            SavingProcessUpdate.Invoke(false);

            return result;
        }

        /// <summary>
        /// Loads data from a binary formatted file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T LoadFile<T>(string filePath)
        {
            if (!File.Exists(filePath)) return default(T);

            try
            {
                T obj = Retry.Do<T>(() =>
                {
                    BinaryFormatter formatter = Serializer.GetBinaryFormatter();
                    FileStream fileStream = File.Open(filePath, FileMode.Open);

                    T obj = (T)formatter.Deserialize(fileStream);

                    fileStream.Close();

                    return obj;
                }, TimeSpan.FromSeconds(1), 5);

                return obj;
            }
            catch
            {
                Debug.LogError($"Failed to load file at {filePath}");
                return default(T);
            }
        }

        protected async void SaveJSONFileAsync(string directoryPath, string fileName, object saveData)
        {
            CreateDirectoryIfNotExists(directoryPath);

            string filePath = $"{directoryPath}/{fileName}";

            try
            {
                string jsonString = JsonUtility.ToJson(saveData);
                string encryptedData = AESEncrypter.EncryptAES(jsonString);
                await File.WriteAllTextAsync(filePath, encryptedData);
            }
            catch
            {
                Debug.LogError($"Failed to save JSON file at {filePath}");
            }
        }

        protected T LoadJSONFile<T>(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return default(T);
            }

            string encryptedData = File.ReadAllText(filePath);
            string decryptedString = AESEncrypter.DecryptAES(encryptedData);
            Debug.Log(decryptedString);

            try
            {
                T obj = JsonUtility.FromJson<T>(decryptedString);
                return obj;
            }
            catch
            {
                Debug.LogError($"Failed to load JSON file at {filePath}");
                return default(T);
            }
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Come on... this one is pretty self explanatory.
        /// </summary>
        /// <param name="path"></param>
        protected void CreateDirectoryIfNotExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        #endregion
    }
}
