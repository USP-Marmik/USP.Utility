using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace USP.MiniGame.recognitionPatterns
{
    /// <summary>
    /// Script by Vishal Lakhani
    /// Email: usp.vishal@gmail.com
    /// Description:
    /// </summary>
    public class AudioLibrary : MonoBehaviour
    {
        #region Variables

        public static AudioLibrary instance;
        public List<AudioClipsC> audioLib;
        [System.Serializable]
        public class AudioClipsC
        {
            public string name;
            public AudioClip female, male;
        }

        public List<AudioClip> femaleAudio;
        public List<AudioClip> maleAudio;

        bool isMale;

        #endregion

        #region Unity Methods

        void Awake()
        {
            instance = this;
        }

        void OnDestroy()
        {
            instance = null;
        }

        public AudioClip GetAudioByClip(AudioClip clip)
        {
            var audioC = audioLib.Find(x => x.name == clip.name);
            return isMale ? audioC.male : audioC.female;
        }

        public AudioClip GetAudioByName(string namee)
        {
            var audioC = audioLib.Find(x => x.name == namee);
            return isMale ? audioC.male : audioC.female;
        }

        public AudioClip GetAudioByEnum(AudioID id)
        {
            Debug.Log("Audio Library Length " + audioLib.Count);
            if (id == 0)
            {
                Debug.LogError("ID was " + id);
                return null;
            }
            if (audioLib == null || audioLib.Count == 0)
            {
                Debug.LogError("AudioLibrary is null or Empty");
                return null;
            }
            var audioC = audioLib[(int)id - 1];
            Debug.Log("Selected ID " + id + " filename female " + audioC.female.name);

            return isMale ? audioC.male : audioC.female;
        }

        public AudioClip GetAudioByIndex(int index)
        {
            var audioC = audioLib[index];
            return isMale ? audioC.male : audioC.female;
        }


        #endregion

        #region Public Methods



        #endregion

        #region Private Methods
#if UNITY_EDITOR
        void OnValidate()
        {

            /* GenerateAudioEnumsLocal();*/

        }
#endif

        [ContextMenu("Find Duplicates")]
        private void FindDuplicates()
        {
            HashSet<AudioClip> uniqueClips = new HashSet<AudioClip>();
            HashSet<AudioClip> duplicateClips = new HashSet<AudioClip>();

            foreach (AudioClip clip in femaleAudio)
            {
                if (clip == null)
                    continue;

                if (!uniqueClips.Add(clip))
                {
                    duplicateClips.Add(clip);
                }
            }

            foreach (AudioClip clip in maleAudio)
            {
                if (clip == null)
                    continue;

                if (!uniqueClips.Add(clip))
                {
                    duplicateClips.Add(clip);
                }
            }

            if (duplicateClips.Count == 0)
            {
                Debug.Log("No duplicate audio clips found.");
                return;
            }

            Debug.Log("Duplicate audio clips found:");

            foreach (AudioClip duplicate in duplicateClips)
            {
                Debug.Log("Duplicate: " + duplicate.name, duplicate);
            }
        }

        [ContextMenu("Remove Duplicates")]
        private void RemoveDuplicates()
        {
            HashSet<AudioClip> uniqueClips = new HashSet<AudioClip>();

            for (int i = femaleAudio.Count - 1; i >= 0; i--)
            {
                AudioClip clip = femaleAudio[i];

                if (clip == null)
                    continue;

                if (uniqueClips.Contains(clip))
                {
                    Debug.Log("Removed duplicate: " + clip.name, clip);
                    femaleAudio.RemoveAt(i);
                }
                else
                {
                    uniqueClips.Add(clip);
                }
            }

            Debug.Log("Duplicate cleanup completed. Final count: " + femaleAudio.Count);

            for (int i = maleAudio.Count - 1; i >= 0; i--)
            {
                AudioClip clip = maleAudio[i];

                if (clip == null)
                    continue;

                if (uniqueClips.Contains(clip))
                {
                    Debug.Log("Removed duplicate: " + clip.name, clip);
                    maleAudio.RemoveAt(i);
                }
                else
                {
                    uniqueClips.Add(clip);
                }
            }
            Debug.Log("Duplicate cleanup completed. Final count: " + maleAudio.Count);
        }

        [ContextMenu("Update Enums here")]
        void GenerateAudioEnumsLocal()
        {

            foreach (var x in femaleAudio)
            {
                var filename = System.Text.RegularExpressions.Regex.Replace(x.name, @"[^\w\s]", "");
                string useName = filename.ToUpper().Replace("FEMALE_", "");
                // var y = audioLib.Find(y => useName == y.name);

                var y = audioLib.Find(y => NormalizeAudioName(useName) == NormalizeAudioName(y.name));
                if (y != null)
                {
                    if (y.female == null)
                    {
                        y.female = x;
                    }
                    else
                    {
                        Debug.Log("Name matched, but female clip already exists: " + y.name);
                    }
                }
                else
                {
                    audioLib.Add(new AudioClipsC() { name = useName, female = x });
                }
            }

            foreach (var x in maleAudio)
            {
                var filename = System.Text.RegularExpressions.Regex.Replace(x.name, @"[^\w\s]", "");
                string useName = filename.ToUpper().Replace("MALE_", "");
                var y = audioLib.Find(y => NormalizeAudioName(useName) == NormalizeAudioName(y.name));
                //var y = audioLib.Find(y => GetStringMatchRate(useName, y.name) > .9f);
                if (y == null)
                {
                    Debug.Log("DIDNT MATCHED " + useName);
                }
                else
                {
                    float rate = GetStringMatchRate(useName, y.name);

                    Debug.Log("Found match | useName: [" + useName + "]" + " | libName: [" + y.name + "]" + " | rate: " + rate + " | female already exists: " + (y.female != null));
                }
                if (y != null)
                {
                    if (y.male == null)
                    {
                        y.male = x;
                    }
                    else
                    {
                        Debug.Log("Name matched, but male clip already exists: " + y.name);
                    }
                }
                else
                {
                    audioLib.Add(new AudioClipsC() { name = useName, male = x });
                }
            }
            //------------------------------------

            foreach (var x in audioLib)
            {
                if (string.IsNullOrEmpty(x.name))
                {
                    if (x.female != null)
                    {
                        x.name = x.female.name;
                    }
                    else
                        if (x.male != null)
                        {
                            x.name = x.male.name;
                        }
                }
            }
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }


        public float GetStringMatchRate(string a, string b)
        {
            if (string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b))
                return 1f;

            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
                return 0f;

            a = a.ToLower();
            b = b.ToLower();

            int distance = LevenshteinDistance(a, b);
            int maxLength = Mathf.Max(a.Length, b.Length);

            return 1f - ((float)distance / maxLength);
        }

        private int LevenshteinDistance(string a, string b)
        {
            int[,] dp = new int[a.Length + 1, b.Length + 1];

            for (int i = 0; i <= a.Length; i++)
                dp[i, 0] = i;

            for (int j = 0; j <= b.Length; j++)
                dp[0, j] = j;

            for (int i = 1; i <= a.Length; i++)
            {
                for (int j = 1; j <= b.Length; j++)
                {
                    int cost = a[i - 1] == b[j - 1] ? 0 : 1;

                    dp[i, j] = Mathf.Min(
                        Mathf.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1),
                        dp[i - 1, j - 1] + cost
                    );
                }
            }

            return dp[a.Length, b.Length];
        }

        private string NormalizeAudioName(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            value = value.ToUpper().Trim();

            // Remove punctuation but keep letters, numbers, underscore and spaces
            value = System.Text.RegularExpressions.Regex.Replace(value, @"[^\w\s]", "");

            // Convert multiple spaces into single space
            value = System.Text.RegularExpressions.Regex.Replace(value, @"\s+", " ");

            return value;
        }

        [ContextMenu("Print Names here")]
        void printAllAudioFiles()
        {
            string audionames = "Audio Files: ";
            foreach (var x in femaleAudio)
            {
                audionames += x.name + ", ";
            }
            Debug.Log(audionames);
        }


        #endregion
    }
}