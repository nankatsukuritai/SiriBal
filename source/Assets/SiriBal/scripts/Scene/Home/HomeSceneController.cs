﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Generic;
using Generic.Manager;

public class HomeSceneController : MonoBehaviour
{
    // COMMON
    private GameSceneManager _gameSceneMng;

    [SerializeField]
    Fade FadeObject;

    // STAGE DATA (Ref for scriptable objects)
    // NOTE: easy,normal,hard は現在使用していません
    // 
    // public StageData easyStage;
    // public StageData normalStage;
    // public StageData hardStage;
    public StageData yarikomiStage_rank1;

    //---------

    // RANK UI
    private ScoreManager _scoreManager;
    private bool rankUpdateFlag;
    //---------

    // WEAPON UI
    [SerializeField]
    Sprite stone_on;
    [SerializeField]
    Sprite stone_off;
    [SerializeField]
    Sprite hammer_on;
    [SerializeField]
    Sprite hammer_off;
    [SerializeField]
    GameObject WeaponPropertyDialog;
    [SerializeField]
    GameObject WeaponInformationHolder;
    private bool weaponLoadFlag;
    //--------


    // OPTIONS UI
    [SerializeField]
    GameObject VibrationToggleObject;

    //--------

    // GAME UI

    // NOTE: 現在使用していません
    // private enum StageIndices
    // {
    //     easy = 1,
    //     normal,
    //     hard,
    // }
    // NOTE: DescriptiopnUIはeasy, normal, hardの説明用だったため、一旦不要
    // [SerializeField]
    // GameObject DescriptionUI;
    [SerializeField]
    GameObject bestScoreValueText;
    // NOTE: 画面動作制御用フラグ ( 現在使用していません )
    // bool IsSwipeOutPlayMode = false;
    // bool IsSwipeInStages = false;
    //------------

#region Start-Update
    // MAIN
    public void Start()
    {
        // マネージャー系の委譲
        _gameSceneMng = new GameSceneManager();
        _scoreManager = new ScoreManager(DataManager.service);

        // Top画面は回転したくない
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;

        // 情報更新フラグ
        rankUpdateFlag = true;
        weaponLoadFlag = true;
        // ベストスコアを表示
        InitialzeBestScoreUI();
        // オプション画面の設定をキャッシュから取得して初期化する
        InitializeOptionsUI();
        // ランキング情報をサーバーから取得しておく（非同期）
        _scoreManager.GetAllRecordsFromDatabase();
    }
    public void Update()
    {
        if(GameObject.Find("RankToggle").GetComponent<Toggle>().isOn && rankUpdateFlag == true)
        {
            // ランキング画面の更新はUpdate処理で実施する。更新完了時は内部でフラグを立てて処理はスキップさせる。
            // NOTE: 1回きりのコールにしないのは、サーバーからの再取得処理を実装するとき、フラグの切り替えだけで完結するから。
            UpdateRanks(_scoreManager.GetRecords());
        }
        else if (GameObject.Find("WeaponToggle").GetComponent<Toggle>().isOn && weaponLoadFlag == true)
        {
            LoadWeaponResultFromCache();
        }
        // UIの更新(SWIPE): やりこみモード以外は一旦隠しているので、スワイプの動作も不要
        // if(IsSwipeOutPlayMode) SwipeOutPlayModeUI();
        // if(IsSwipeInStages) SwipeInStageUI();
    }
    //-----------
#endregion

#region GAME-UI

    // NOTE: お一人様モード用の処理なので、一旦コメントアウト
    // public void GameStartButtonClicked()
    // {
    //     var sceneChangeFlag = false;
    //     switch (GetActiveStageIndex())
    //     {
    //         case StageIndices.easy:
    //             DataManager.currentStage = new Stage(easyStage);
    //             sceneChangeFlag = true;
    //             break;
    //         case StageIndices.normal:
    //             DataManager.currentStage = new Stage(normalStage);
    //             sceneChangeFlag = true;
    //             break;
    //         case StageIndices.hard:
    //             DataManager.currentStage = new Stage(hardStage);
    //             sceneChangeFlag = true;
    //             break;
    //         default:
    //             break;
    //     }
        
    //     if(sceneChangeFlag)
    //     {
    //         GameStart();
    //     }
    // }

    // NOTE: お一人様モードの難易度チェックですが、現在使用していません
    // private StageIndices GetActiveStageIndex()
    // {
    //     // Stages の toggle の状態を取得する
    //     var stg_easy = GameObject.Find("Easy").GetComponent<Toggle>();
    //     var stg_normal = GameObject.Find("Normal").GetComponent<Toggle>();
    //     var stg_hard = GameObject.Find("Hard").GetComponent<Toggle>();
        
    //     // なんもとれないとき
    //     var value = StageIndices.easy;

    //     if (stg_easy.isOn)
    //     {
    //         value = StageIndices.easy;
    //     }
    //     else if (stg_normal.isOn)
    //     {
    //         value = StageIndices.normal;
    //     }
    //     else if (stg_hard.isOn)
    //     {
    //         value = StageIndices.hard;
    //     }
    //     return value;
    // }

    // NOTE: おひとり様モード（かんたん・ふつう・むずかしい）は現在使用していません
    // public void SingleModeButtonClicked()
    // {
    //     // PlayModeのUIを消し、StageのUIを出す
    //     IsSwipeOutPlayMode = true;
    //     IsSwipeInStages = true;
    // }

    // NOTE: おふたりさまモードも現在使用していません
    // public void PairModeButtonClicked()
    // {
    //     var stage = new Stage(normalStage);
    //     stage.BalloonArrangementMode = Stage.ArrangementMode.Manual;
    //     DataManager.currentStage = stage;
    //     // 他の画面は回転してもOK
    //     GameStart();
    // }

    public void YarikomiModeButtonClicked()
    {
        var stage = new Stage(yarikomiStage_rank1);
        DataManager.currentStage = stage;
        GameStart();
    }

    // NOTE: スワイプアウトはおひとりさまモード用で、現在は使用していません
    // private void SwipeOutPlayModeUI()
    // {
    //     var playModeUI = GameObject.Find("PlayModes").GetComponent<RectTransform>();
    //     var left = playModeUI.offsetMin.x - 100;
    //     var right = playModeUI.offsetMax.x - 100;
    //     playModeUI.offsetMin = new Vector2(left, playModeUI.offsetMin.y);
    //     playModeUI.offsetMax = new Vector2(right, playModeUI.offsetMax.y);

    //     if (playModeUI.offsetMin.x < -1500 )
    //     {
    //         IsSwipeOutPlayMode = false;
    //     }
    // }

    // NOTE: スワイプインはおひとりさまモード用で、現在は使用していません
    // private void SwipeInStageUI()
    // {
    //     var stageUI = GameObject.Find("Stages").GetComponent<RectTransform>();
    //     var left = stageUI.offsetMin.x - 100 < 0? 0 : stageUI.offsetMin.x - 100;
    //     var right = stageUI.offsetMax.x - 100;

    //     stageUI.offsetMin = new Vector2(left, stageUI.offsetMin.y);
    //     stageUI.offsetMax = new Vector2(right, stageUI.offsetMax.y);

    //     if (stageUI.offsetMin.x == 0 )
    //     {
    //         IsSwipeInStages = false;
    //         DescriptionUI.SetActive(true);
    //     }
    // }


    // NOTE: スワイプ動作は現在使用していません
    // Swipe動作で移動した画面をもとに戻す
    // public void InitializeSwipedUIs()
    // {
    //     var playModeObj = GameObject.Find("PlayModes");
    //     if(playModeObj == null) return;
    //     var playModeUI = playModeObj.GetComponent<RectTransform>();
    //     playModeUI.offsetMin = new Vector2(0, playModeUI.offsetMin.y);
    //     playModeUI.offsetMax = new Vector2(0, playModeUI.offsetMax.y);
    //     IsSwipeOutPlayMode = false;

    //     var stageObj = GameObject.Find("Stages").GetComponent<RectTransform>();
    //     if (stageObj == null) return;
    //     var stageUI = stageObj.GetComponent<RectTransform>();
    //     stageUI.offsetMin = new Vector2(1500, stageUI.offsetMin.y);
    //     stageUI.offsetMax = new Vector2(1500, stageUI.offsetMax.y);
    //     IsSwipeInStages = false;

    //     DescriptionUI.SetActive(false);
    // }

    private void GameStart()
    {
        UpdateCurrentVibrationOption();
        // 他の画面は回転してもOK
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        FadeWithChangeScene();
    }

    private void FadeWithChangeScene()
    {
		FadeObject.FadeIn (1, () => {
			Invoke("GameSceneStart",0.1f);
		});
	}

    // NOTE: 参照先が無い関数に見えますが、FadeWIthChangeSceneから時差呼び出しされています。
	public void GameSceneStart()
    {
		_gameSceneMng.ChangeScene(GameScenes.SeriousBalloon);
    }

    private void InitialzeBestScoreUI()
    {
        if(bestScoreValueText != null)
        {
            var bestScore = _scoreManager.LoadYarikomiBestFromLocal();
            bestScoreValueText.GetComponent<Text>().text = bestScore.ToString();
        }
    }
#endregion 

#region RANK-UI
    private void UpdateRanks(List<Record> records)
    {
        if (records == null)
        {
            return ;
        }
        if (records.Count > 0)
        {
            // 基本、固定で8個なのでベタ書きする
            var objName = "";
            for( var count = 0; count < records.Count && count < 8; ++count)
            {
                objName = "Score"+ (count+1);
                // 0個目:Label, 1個目:Score, 2個目:Name　の順にScoreにぶら下がっている。（これ崩さないでね・・・）
                GameObject.Find(objName).transform.GetChild(0).gameObject.GetComponent<Text>().text = (count+1).ToString();
                GameObject.Find(objName).transform.GetChild(1).gameObject.GetComponent<Text>().text = records[count].TotalScore.ToString();
                GameObject.Find(objName).transform.GetChild(2).gameObject.GetComponent<Text>().text = records[count].UserName;
            }
            
            // 8個に満たない場合、使っていないランクを非表示にする
            if (records.Count < 8)
            {
                for (var i = records.Count; i < 8; ++i)
                {
                    objName = "Score" + (i+1);
                    GameObject.Find(objName).SetActive(false);
                }
            }
            rankUpdateFlag = false; // 更新完了
        }
    }

#endregion

#region WEAPON-UI
    private void LoadWeaponResultFromCache()
    {
        // キャッシュからロード
        //foreach(var weaponValue in Enum.GetValues(typeof(Weapons)))
        foreach(var weaponlist in WeaponData2.Entity.HeroWeaponList)
        {
            //このキャッシュの仕組みは必要。あとで改良方法を検討。
            //string weaponName = Enum.GetName(typeof(Weapons), weaponValue);
            //var cache = PlayerPrefs.GetInt(weaponName, 0);

            var cache = 0; //暫定。キャッシュの仕組みを復元したら削除。

            int weaponValue = WeaponData2.Entity.HeroWeaponList.IndexOf(weaponlist);
            //var objName = "Weapon" + (int)weaponValue;
            var objName = "Weapon" + (weaponValue+1);
            var obj = GameObject.Find(objName);
            if (obj != null)
            {
                //対応変更必要？後で検討する
            //    var weaponData = WeaponInformationHolder.GetComponent<WeaponInformationHolder>().GetWeaponDataFromKey((Weapons)weaponValue);
            //    if (weaponData != null)
                if (weaponlist != null)
                {
                    // TODO #58
                    // ウェポン獲得の実装をしていないので、とりあえず全部見れるようにしている。
                    // 本来はcache == 1 で image_on を表示し、 cache == 0 の場合は追加で、
                    // ボタンを無効にするか、未開放のウェポンとして、ウェポンの説明画面を表示する
                    //このキャッシュの仕組みは必要。あとで復元する。
                    //obj.GetComponent<Image>().sprite = (cache == 0)? weaponData.image_on: weaponData.image_off;
                    obj.GetComponent<Image>().sprite = (cache == 0)? weaponlist.ImageOn: weaponlist.ImageOff;
                }
            }
        }
        // var stone = PlayerPrefs.GetInt(Weapons.Stone.ToString(), 0);
        // if (stone == 1)
        // {
        //     GameObject.Find("Weapon1").GetComponent<Image>().sprite = stone_on;
        //     // Stone Button Be Active
        // }
        // var hammer = PlayerPrefs.GetInt(Weapons.Hammer.ToString(), 0);
        // if (hammer == 1)
        // {
        //     GameObject.Find("Weapon2").GetComponent<Image>().sprite = hammer_on;
        //     // Hammer Button Be Active
        // }

        // 読み込み完了
        weaponLoadFlag = false;
    }

    // Weapon の詳細を表示
    public void ShowWeaponPropertyDialog(int weaponKey)
    {
        WeaponPropertyDialog.SetActive(true);
        //var weapon = (Weapons)Enum.ToObject(typeof(Weapons), weaponKey);
        //ShowCurrentWeaponInformation(weapon);
        //weaponKeyは使用しなくなったので、名称を変えた方がわかりやすいかも？
        ShowCurrentWeaponInformation(weaponKey-1);
    }

    //private void ShowCurrentWeaponInformation(Weapons weapon)
    private void ShowCurrentWeaponInformation(int weaponKey)
    {
        // ウェポン名
        var weaponName = GameObject.Find("WeaponName");
        if (weaponName != null)
        {
            weaponName.GetComponent<Text>().text = WeaponData2.Entity.HeroWeaponList[weaponKey].Name;
            //weaponName.GetComponent<Text>().text = weapon.ToString();
        }
        // ウェポン画像
        var weaponImage = GameObject.Find("WeaponImage");
        if (weaponImage != null)
        {
            weaponImage.GetComponent<Image>().sprite = WeaponData2.Entity.HeroWeaponList[weaponKey].ImageOn;
            //var weaponData = WeaponInformationHolder.GetComponent<WeaponInformationHolder>().GetWeaponDataFromKey(weapon);
            /*
            if (weaponData != null)
            {
                weaponImage.GetComponent<Image>().sprite = weaponData.image_on;
            }
            */
        }
        // ウェポンの説明
        var weaponExplanation = GameObject.Find("WeaponExplanation");
        if (weaponExplanation != null)
        {
            weaponExplanation.GetComponent<Text>().text = WeaponData2.Entity.HeroWeaponList[weaponKey].Explanation;
            //var weaponData = WeaponInformationHolder.GetComponent<WeaponInformationHolder>().GetWeaponDataFromKey(weapon);
            //if (weaponData != null)
            //{
            //    weaponExplanation.GetComponent<Text>().text = weaponData.explanation;
            //}
        }
        // レーダーチャート
        var radarPoly = GameObject.Find("RadarPoly");
        if (radarPoly != null)
        {
            var radar = radarPoly.GetComponent<RadarChartController>();
            if (radar != null)
            {
                var props = WeaponData2.Entity.HeroWeaponList[weaponKey];
                radar.Volumes = new float[]{
                                            (float)props.Attack/5,
                                            (float)props.Scale/5,
                                            (float)props.Distance/5,
                                            (float)props.Penetrate/5,
                                            (float)props.Rapidfire/5,
                                            };
                /*
                var weaponData = WeaponInformationHolder.GetComponent<WeaponInformationHolder>().GetWeaponDataFromKey(weapon);
                if (weaponData != null)
                {
                    radar.Volumes = new float[]{
                                                (float)weaponData.attack/5,
                                                (float)weaponData.size/5,
                                                (float)weaponData.distance/5,
                                                (float)weaponData.penetrate/5,
                                                (float)weaponData.rapidfire/5
                                                };
                }
                */
            }
        }
    }

    public void CloseWeaponPropertyButtonClicked()
    {
        Invoke("CloseWeaponPropertyDialog",0.1f);
    }
    private void CloseWeaponPropertyDialog()
    {
        WeaponPropertyDialog.SetActive(false);
    }
#endregion

#region OPTION-UI
    private void InitializeOptionsUI()
    {
        var VibrationOnToggle = VibrationToggleObject.GetComponent<Toggle>();
        var VibrationCache = PlayerPrefs.GetInt("Vibration", 1); // 0:OFF , 1:ON
        if (VibrationCache == 0) // OFF
        {
            VibrationOnToggle.isOn = false;
        }
        else
        {
            VibrationOnToggle.isOn = true;
        }
    }

    public void UpdateCurrentVibrationOption()
    {
        var VibrationOnToggle = VibrationToggleObject.GetComponent<Toggle>();
        if (VibrationOnToggle != null)
        {
            if (DataManager.options == null)
            {
                DataManager.options = new GameOptions();
            }
            DataManager.options.IsVibration = VibrationOnToggle.isOn;

            if (DataManager.options.IsVibration)
            {
                PlayerPrefs.SetInt("Vibration",1);
            }
            else
            {
                PlayerPrefs.SetInt("Vibration",0);
            }
            PlayerPrefs.Save();
        }
    }


#endregion

}