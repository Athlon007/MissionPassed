using HutongGames.PlayMaker;
using MSCLoader;
using UnityEngine;

namespace MissionPassed
{
    public class MissionPassed : Mod
    {
        public override string ID => "MissionPassed"; //Your mod ID (unique)
        public override string Name => "MissionPassed"; //You mod name
        public override string Author => "Athlon"; //Your Username
        public override string Version => "1.0"; //Version

        // Set this to true if you will be load custom assets from Assets folder.
        // This will create subfolder in Assets folder for your mod.
        public override bool UseAssetsFolder => true;

        Settings playOnLessMoney = new Settings("playOnMoneyLost", "Mission Passed when loosing money", false);

        AudioSource source;
        TextMesh mainText;

        bool showMissionPassed;
        float timer;

        float timerMoneyCheck;
        float lastMoneyValue;
        FsmFloat money;

        public override void OnLoad()
        {
            // Called once, when mod is loading after game is fully loaded
            AssetBundle ab = LoadAssets.LoadBundle(this, "missionpassed.unity3d");
            GameObject missionPassedSourcePrefab = ab.LoadAsset<GameObject>("MissionPassedPlayer.prefab");
            GameObject missionPassedSource = GameObject.Instantiate(missionPassedSourcePrefab);
            missionPassedSource.transform.parent = GameObject.Find("PLAYER").transform;
            missionPassedSource.transform.localPosition = Vector3.zero;
            source = missionPassedSource.GetComponent<AudioSource>();

            GameObject newMissionPassed = GameObject.Instantiate(GameObject.Find("GUI").transform.Find("Indicators/Subtitles").gameObject);
            Object.Destroy(newMissionPassed.GetComponent<PlayMakerFSM>());
            newMissionPassed.transform.parent = GameObject.Find("GUI").transform.Find("Indicators");
            newMissionPassed.transform.localPosition = new Vector3(0, 7.7f, 0);
            newMissionPassed.name = "MisionPassed";
            mainText = newMissionPassed.GetComponent<TextMesh>();
            mainText.anchor = TextAnchor.MiddleCenter;
            mainText.alignment = TextAlignment.Center;
            mainText.characterSize = 0.2f;
            ab.Unload(false);
            ResetText();

            money = PlayMakerGlobals.Instance.Variables.GetFsmFloat("PlayerMoney");
            lastMoneyValue = money.Value;
        }

        public override void ModSettings()
        {
            Settings.AddCheckBox(this, playOnLessMoney);
        }

        public override void Update()
        {
            timerMoneyCheck += Time.deltaTime;

            if (timerMoneyCheck > 5)
            {
                timerMoneyCheck = 0;

                if ((bool)playOnLessMoney.GetValue())
                {
                    if (money.Value - lastMoneyValue != 0)
                    {
                        int moneyEarned = Mathf.RoundToInt(money.Value - lastMoneyValue);
                        PlayMissionPassed(moneyEarned);
                    }
                }
                else
                {
                    if (money.Value > lastMoneyValue)
                    {
                        int moneyEarned = Mathf.RoundToInt(money.Value - lastMoneyValue);
                        PlayMissionPassed(moneyEarned);
                    }
                }

                lastMoneyValue = money.Value;
            }

            // Update is called once per frame
            if (showMissionPassed)
            {
                timer += Time.deltaTime;
                if (timer > 10)
                {
                    showMissionPassed = false;
                    timer = 0;
                    ResetText();
                }
            }
        }

        void PlayMissionPassed(int value)
        {
            source.Play();
            string text = value <= 0 ? "RESPECT+" : $"${value}";
            mainText.text = $"<color=#ffaa00>MISSION PASSED</color>\n<color=white>{text}</color>";
            timer = 0;
            showMissionPassed = true;
        }

        void ResetText()
        {
            mainText.text = $"";
        }
    }
}
