using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine.UI;

public class BombTutorialManager : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] float scaleSize;

    [Header("Component References")]
    [SerializeField] Transform bombIcon;
    [SerializeField] Vector3 bombIconOriginalPos;
    [SerializeField] GameObject tutorial;


    [SerializeField] Image lockIcon;
    [SerializeField] Image unlockIcon;
    [SerializeField] Transform radial;
    [SerializeField] Transform bombInfo;

    //[SerializeField] Transform bombInfo;





    // Start is called before the first frame update
    async void Start()
    {
        await Task.Delay(1500);
        bombIconOriginalPos = bombIcon.position;
        await BombMove();
        await unlockIcon.transform.DOPunchScale(new Vector3(0.1f,0.1f,0.1f), 0.5f).AsyncWaitForCompletion();
        lockIcon.gameObject.SetActive(false);

        unlockIcon.gameObject.SetActive(true);

        await unlockIcon.transform.DOPunchScale(unlockIcon.transform.localScale * 1.5f, 0.5f).AsyncWaitForCompletion();

        await Task.Delay(500);
        bombIcon.GetComponent<Button>().interactable = true;

        unlockIcon.gameObject.SetActive(false);

        await Task.Delay(750);
        bombIcon.DOScale(bombIcon.localScale / 2, 0.7f);

        await bombIcon.DOMove(bombIconOriginalPos, 0.7f).AsyncWaitForCompletion();


        bombInfo.gameObject.SetActive(true);
        bombInfo.localScale = Vector3.zero;
        await bombInfo.DOScale(Vector3.one, 0.7f).AsyncWaitForCompletion();

        radial.DORotate(new Vector3(0, 0, 180), 2f).SetLoops(-1,LoopType.Yoyo);
        tutorial.SetActive(true);
        //GridManager.Instance.EnableBombGrid();
    }

    public async Task BombMove()
    {
       await bombIcon.DOLocalMove(Vector3.zero, 0.5f).AsyncWaitForCompletion();

       await bombIcon.DOScale(bombIcon.localScale*2f, 0.5f).AsyncWaitForCompletion();

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
