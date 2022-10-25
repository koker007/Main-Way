using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamLeaderboard : MonoBehaviour
{
    public static SteamLeaderboard main;

    static float timeOldTryGet; //Время последней попытки получить данные таблицы из стима

    public class Leaderboard
    {
        string key = "LeaderBoardKey";
        public string Key { get { return key; } }

        float timeWaitReDownload = 99999; //время ожидания до новой загрузки этой таблицы
        int MyScore = 0;
        int[] MyDetails;
        public List<LeaderData> ListUsers = new List<LeaderData>(); //Список позиций таблицы лидеров

        public bool uploadScoreNeed = false; //Нужно ли обновить свои данные таблицы
        public bool reWriteTopNeed = false; //Требуется ли перезапись

        bool isCanCreate = false;
        public Leaderboard(string KeyLeaderBoardFunc, bool CanCreate, float timeWaitReDownloadFunc)
        {
            key = KeyLeaderBoardFunc;
            isCanCreate = CanCreate;
            timeWaitReDownload = timeWaitReDownloadFunc;

            //Результат поиска таблицы
            m_LeaderboardFindResult = CallResult<LeaderboardFindResult_t>.Create(IniSteamLeaderboard);
            //Загруженная таблица
            m_LeaderboardScoresDownloaded = CallResult<LeaderboardScoresDownloaded_t>.Create(IniSteamLeaderboardScoresDownloaded);
            //Загрузить свой результат в таблицу
            m_LeaderboardScoreUploaded_t = CallResult<LeaderboardScoreUploaded_t>.Create(IniLeaderboardScoreUploaded);


        }

        public void ReDownLoad()
        {
            getterLeaderboardDownloadTOP = false;
        }
        public void ReUploadMyScore(bool reWrite, int score, int[] details)
        {
            isUploadedScore = false;
            uploadScoreNeed = true;

            MyScore = score;
            MyDetails = details;

            reWriteTopNeed = reWrite;
        }

        bool getterLeaderboard = false;
        float timeLastTryResult = -99999;
        LeaderboardFindResult_t Steamleaderboard;
        private CallResult<LeaderboardFindResult_t> m_LeaderboardFindResult;
        private void IniSteamLeaderboard(LeaderboardFindResult_t pCallback, bool bIOFailure)
        {
            timeLastTryResult = Time.unscaledTime;
            if (pCallback.m_hSteamLeaderboard.m_SteamLeaderboard == 0 || bIOFailure)
            {
                Debug.Log("SteamLeaderboard_t Error");
            }
            else
            {
                Debug.Log("SteamLeaderboard_t OK");
                Steamleaderboard = pCallback;
                getterLeaderboard = true;
            }
        }

        //Загруженная таблица с лидерами на карте
        public bool getterLeaderboardDownloadTOP = false;
        public LeaderboardScoresDownloaded_t SteamleaderboardScoresDownloaded;
        private CallResult<LeaderboardScoresDownloaded_t> m_LeaderboardScoresDownloaded;
        float timeOldDownload = 0;
        float TimeOldDownload { get { return timeOldDownload; } }
        private void IniSteamLeaderboardScoresDownloaded(LeaderboardScoresDownloaded_t pCallback, bool bIOFailure)
        {
            if (pCallback.m_hSteamLeaderboard.m_SteamLeaderboard == 0 || bIOFailure)
            {
                Debug.Log("SteamLeaderboard_t Error");
            }
            else
            {
                Debug.Log("SteamLeaderboard_t OK");
                SteamleaderboardScoresDownloaded = pCallback;
                getterLeaderboardDownloadTOP = true;
                timeOldDownload = Time.unscaledTime;

                //Перебираем все результаты
                ListUsers = new List<LeaderData>();
                for (int num = 0; num < SteamleaderboardScoresDownloaded.m_cEntryCount; num++)
                {
                    //вытаскиваем лидера
                    LeaderData leaderData = new LeaderData();
                    int[] getDetails = new int[Constants.k_cLeaderboardDetailsMax];
                    bool ok = SteamUserStats.GetDownloadedLeaderboardEntry(SteamleaderboardScoresDownloaded.m_hSteamLeaderboardEntries, num, out leaderData.data, getDetails, getDetails.Length);
                    leaderData.details = new int[leaderData.data.m_cDetails];
                    for (int detailnum = 0; detailnum < leaderData.details.Length; detailnum++)
                    {
                        leaderData.details[detailnum] = getDetails[detailnum];
                    }
                    if (leaderData.data.m_steamIDUser.m_SteamID != 0)
                        ListUsers.Add(leaderData);
                }
            }
        }

        //Загружены ли очки в таблицу лидеров - загрузка очков
        bool isUploadedScore = false;
        public bool IsUploadedScore { get { return isUploadedScore; } }

        public LeaderboardScoreUploaded_t SteamLeaderboardScoreUploaded;
        private CallResult<LeaderboardScoreUploaded_t> m_LeaderboardScoreUploaded_t;
        private void IniLeaderboardScoreUploaded(LeaderboardScoreUploaded_t pCallback, bool bIOFailure)
        {
            if (pCallback.m_bSuccess == 0 || bIOFailure)
            {
                Debug.Log("SteamLeaderboardScoreUploaded Error");
            }
            else
            {
                Debug.Log("SteamLeaderboardScoreUploaded OK");
                SteamLeaderboardScoreUploaded = pCallback;
                isUploadedScore = true;
                reWriteTopNeed = false;
            }
        }

        public void UpdateLeadersBoard()
        {
            if (SteamManager.Initialized && main != null && Time.unscaledTime - SteamLeaderboard.timeOldTryGet > 1)
            {
                //Сперва пытаемся загрузить таблицу
                if (!getterLeaderboard && (Time.unscaledTime - timeLastTryResult) > 60)
                {
                    Debug.Log("Geting leaderboard key:" + key);
                    SteamAPICall_t hedder;
                    if (isCanCreate)
                    {
                        hedder = SteamUserStats.FindOrCreateLeaderboard(key, ELeaderboardSortMethod.k_ELeaderboardSortMethodDescending, ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNumeric);
                    }
                    else
                    {
                        hedder = SteamUserStats.FindLeaderboard(key);
                    }

                    m_LeaderboardFindResult.Set(hedder);

                    SteamLeaderboard.timeOldTryGet = Time.unscaledTime;
                }
                //Пытаемся загрузить свой результат
                else if (getterLeaderboard && uploadScoreNeed && !isUploadedScore)
                {
                    Debug.Log("Setting My Score leaderboard key:" + key);
                    SteamLeaderboard_t steamLeaderboard_T;
                    steamLeaderboard_T.m_SteamLeaderboard = Steamleaderboard.m_hSteamLeaderboard.m_SteamLeaderboard;
                    int[] testArray = new int[0];
                    if (MyDetails.Length > 0)
                        testArray = MyDetails;

                    SteamAPICall_t handle;
                    if (!reWriteTopNeed)
                    {
                        handle = SteamUserStats.UploadLeaderboardScore(steamLeaderboard_T, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest, MyScore, MyDetails, MyDetails.Length);
                    }
                    else
                    {
                        handle = SteamUserStats.UploadLeaderboardScore(steamLeaderboard_T, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate, MyScore, MyDetails, MyDetails.Length);
                    }
                    m_LeaderboardScoreUploaded_t.Set(handle);

                    SteamLeaderboard.timeOldTryGet = Time.unscaledTime;
                }
                //Теперь, если таблица загружена то грузим список лидеров
                else if (getterLeaderboard && !getterLeaderboardDownloadTOP)
                {
                    Debug.Log("Getting leaderboard List key:" + key);
                    SteamLeaderboard_t steamLeaderboard_T;
                    steamLeaderboard_T.m_SteamLeaderboard = Steamleaderboard.m_hSteamLeaderboard.m_SteamLeaderboard;

                    SteamAPICall_t handle = SteamUserStats.DownloadLeaderboardEntries(steamLeaderboard_T, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal, 0, 1000);
                    m_LeaderboardScoresDownloaded.Set(handle);

                    SteamLeaderboard.timeOldTryGet = Time.unscaledTime;
                }
            }

            if (Time.unscaledTime - timeOldDownload > timeWaitReDownload)
            {
                getterLeaderboardDownloadTOP = false;
            }
        }
    }

    public class LeaderData
    {
        public LeaderboardEntry_t data;
        public int[] details = new int[0];
    }


    Leaderboard timePlayMax = new Leaderboard("TimePlayMax", true, 30);

    // Start is called before the first frame update
    void Start()
    {
        main = this;

        int[] detals = new int[0];
        timePlayMax.ReUploadMyScore(true, (int)Time.time, detals);
    }
    private void Update()
    {
        timePlayMax.UpdateLeadersBoard();
    }
}
