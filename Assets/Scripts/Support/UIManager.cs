using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Threading.Tasks;
[System.Serializable]
public class PowerupButton
{
    public PowerupType pt;
    public Button b;
    public Text t;

}
public class UIManager : MonoBehaviour
{
    #region Properties
    public static UIManager Instance = null;

    [Header("Components Reference")]

    [SerializeField] private GameObject PointText;
    [SerializeField] private GameObject AwesomeText;
    [SerializeField] private GameObject JoyStick;

    [SerializeField] private List<PowerupButton> powerups;
    [SerializeField] private Image fillAmtGrey;
    [SerializeField] private Text fillAmttext;
    [SerializeField] private float fillAmt;
    [SerializeField] private Transform radial;



    [SerializeField] private Image unlocked;




    [Header("UI Panel")]
    [SerializeField] private GameObject mainMenuUIPanel = null;
    [SerializeField] private GameObject gameplayUIPanel = null;
    [SerializeField] private GameObject gameOverWinUIPanel = null;
    [SerializeField] private GameObject gameOverLoseUIPanel = null;
    [SerializeField] private TextMeshProUGUI scoreText = null;
    [SerializeField] private Text mainLevelText = null;
    [SerializeField] private Text inGameLevelText = null;
    [SerializeField] private Text winLevelText = null;
    [SerializeField] private Text loseLevelText = null;
    [SerializeField] private Text debugText = null;
    [SerializeField] private GameObject powerupInfo = null;



    [Header("Reward/Coins")]
    [SerializeField] List<Text> allCurrentCoins = null;

    [Header("Post Level")]
    [SerializeField] Button multiplyReward;
    [SerializeField] Text multiplyText;
    [SerializeField] Text levelReward;


    [Header("Daily")]
    [SerializeField] Button dailyReward;
    [SerializeField] Text dailyText;



    [Header("Objective Boxes")]
    [SerializeField] Image burgerFill;
    [SerializeField] Text burgerFillText;

    [SerializeField] Transform burgerPos;


    [SerializeField] Image pizzaFill;
    [SerializeField] Text pizzaFillText;

    [SerializeField] Transform pizzaPos;

    [SerializeField] Image donutFill;
    [SerializeField] Transform donutPos;


    Sequence goalSequence;

    #endregion

    #region MonoBehaviour Functions
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

    private void Start()
    {
        //SwitchControls(Controls.Touch);
        goalSequence = DOTween.Sequence();
    }
    #endregion

    #region Getter And Setter

    #endregion

    #region Public Core Functions
    public void SwitchUIPanel(UIPanelState state)
    {
        switch (state)
        {
            case UIPanelState.MainMenu:
                mainMenuUIPanel.SetActive(true);
                gameplayUIPanel.SetActive(false);
                gameOverWinUIPanel.SetActive(false);
                gameOverLoseUIPanel.SetActive(false);
                break;
            case UIPanelState.Gameplay:
                mainMenuUIPanel.SetActive(false);
                gameplayUIPanel.SetActive(true);
                gameOverWinUIPanel.SetActive(false);
                gameOverLoseUIPanel.SetActive(false);
                break;
            case UIPanelState.GameWin:
                mainMenuUIPanel.SetActive(false);
                gameplayUIPanel.SetActive(false);
                gameOverWinUIPanel.SetActive(true);
                gameOverLoseUIPanel.SetActive(false);
                _ = UpdateFillImage();
                break;
            case UIPanelState.GameLose:
                mainMenuUIPanel.SetActive(false);
                gameplayUIPanel.SetActive(false);
                gameOverWinUIPanel.SetActive(false);
                gameOverLoseUIPanel.SetActive(true);
                break;
        }
    }

    public void UpdateScore(int value)
    {
        scoreText.text = "" + value;
    }

    public void UpdateDebugText(string s)
    {
        debugText.text = s;
    }
    public void UpdateLevel(int level)
    {
        mainLevelText.text = "LEVEL " + level;
        inGameLevelText.text = "LEVEL " + level;
        winLevelText.text = "LEVEL " + level;
        loseLevelText.text = "LEVEL " + level;
    }

    public async Task UpdateFillImage()
    {
        fillAmtGrey.fillAmount = 0;
        fillAmttext.text = "0%";

        await Task.Delay(500);

        fillAmtGrey.DOFillAmount(fillAmt / 100, 2f).OnStepComplete(() => {
            if (fillAmt == 100)
            {
                fillAmtGrey.transform.DOScale(Vector3.zero, 0.4f).OnComplete(() =>
                {
                    fillAmtGrey.gameObject.SetActive(false);
                    unlocked.gameObject.SetActive(true);
                    unlocked.transform.DOScale(new Vector3(1, 1, 1), 0.4f);
                    radial.gameObject.SetActive(true);
                    radial.DOScale(new Vector3(1, 1, 1), 1f);
                    radial.DORotate(new Vector3(0, 0, 180), 2f).SetLoops(-1,LoopType.Yoyo);
                });
            }
        }) ;
        fillAmttext.text = "" + fillAmt + "%";

       
    }
    public void UpdateCurrentCoins(int v)
    {
        foreach(Text t in allCurrentCoins)
        {
            t.text = v + "";
        }
        UpdatePowerupButtons(v);

    }
    public void UpdatePowerupButtons(int currentCoin)
    {
        foreach(PowerupButton pb in powerups)
        {
            if(PowerupManager.Instance.GetPowerupCost(pb.pt) <= currentCoin)
            {
                pb.b.interactable = true;
                if(pb.pt == PowerupType.Bomb)
                {
                    if (PlayerPrefs.GetInt("bomb", 0) == 0)
                    {
                        EnablePowerupInfo();
                        PlayerPrefs.SetInt("bomb", 1);
                    }
                }
            }
            else
            {
                pb.b.interactable = false;
            }
            pb.t.text = "" + PowerupManager.Instance.GetPowerupCost(pb.pt);
        }
    }
    public void UpdateLevelReward(int v)
    {
        levelReward.text ="+"+ v + "";
    }
    public void EnablePowerupInfo()
    {
        powerupInfo.SetActive(true);
    }
    public void UpdateObjective(HexType ht, float fillAmt, float max)
    {
        switch (ht)
        {
            case HexType.A:
                pizzaFill.DOFillAmount(fillAmt/max, 2f);
                pizzaFillText.text = fillAmt + "/" + max;
                break;

            case HexType.B:
                burgerFill.DOFillAmount(fillAmt/max, 2f);
                break;

            case HexType.C:
                donutFill.DOFillAmount(fillAmt/max, 2f);
                break;
        }
    }

    public Vector3 GetItemPos(HexType ht)
    {
        Vector3 p = Vector3.zero;
        switch (ht)
        {
            case HexType.A:
                p=  pizzaPos.position;
                break;

            case HexType.B:
                p = burgerPos.position;
                break;

            case HexType.C:
                p = donutPos.position;
                break;

        }
        return p;
    }
    #region Give Rewards

    #endregion

    #region OnClickUIButtons    

    public void OnClickPlayButton()
    {
        GameManager.Instance.StartLevel();
    }

    public void OnClickChangeButton()
    {
        GameManager.Instance.ChangeLevel();
    }

    public void OnClickPowerupButton()
    {
        GridManager.Instance.EnableBombGrid();
    }

    public void OnClickMove()
    {
        GameManager.Instance.AddMove(1);
    }

    public void OnClickWin()
    {
        GameManager.Instance.WinLevel();
    }

    #endregion


    public void SpawnPointText(Vector3 point)
    {
        Instantiate(PointText, point, Quaternion.identity);
    }

    public void SpawnAwesomeText(Vector3 point, string s)
    {
        GameObject g = Instantiate(AwesomeText, new Vector3(point.x, 2, point.z), Quaternion.identity);
        g.GetComponentInChildren<TextMeshPro>().text = s;
    }


    #endregion


    #region Button Click events


    #endregion
}




