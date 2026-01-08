using DG.Tweening;
using MH.GameScene.Core.Entites;
using MH.GameScene.Runtime;
using MH.GameScene.Runtime.Entities;
using MH.GameScene.Runtime.Utilities;
using MH.GameScene.Runtime.Views;
using R3;
using R3.Triggers;
using SgFramework.UI;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [UIConfig(UILayer.Default, "Assets/GameRes/Prefabs/UI/UIPlough.prefab")]
    public class UIPlough : UIForm
    {
        [SerializeField] private Button cropBtn;
        [SerializeField] private Button sickleBtn;

        private Vector2 sickleBtnOrigin;
        private RectTransform sickleRectTf;

        private Vector2 cropBtnOrigin;
        private RectTransform cropRectTf;

        private void Start()
        {
            sickleRectTf = sickleBtn.transform as RectTransform;
            sickleBtnOrigin = sickleRectTf.anchoredPosition;
            RectTransform rootRectTf = transform as RectTransform;
            sickleBtn.OnBeginDragAsObservable().Subscribe(b => sickleBtn.interactable = false);
            sickleBtn.OnDragAsObservable().Subscribe(async b =>
            {
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rootRectTf, b.position, null, out var lp))
                {
                    sickleRectTf.anchoredPosition = lp;

                    Vector3 pos = Camera.main.ScreenToWorldPoint(b.position);
                    Vector2Int index = MathUtility.GamePosToIndex(pos);
                    IMapScene mapScene = World.Current.FindEntity<IMapScene>();
                    IGridEntity grid = mapScene.GetGrid(index);
                    if (grid != null)
                    {
                        IItemEntity item = grid.GetItem(GameConst.SURFACEDECORATE_LAYER);
                        if (item != null)
                        {
                            PloughItem ploughItem = item as PloughItem;
                            if (ploughItem != null && ploughItem.Crop != null)
                            {
                                CommonCropView view = ploughItem.Crop.GetCom<CommonCropView>();
                                if (view != null && view.CanCrop)
                                {
                                    await view.ToRemoveEffect().Waiter();
                                    ploughItem.GainCrop();
                                }
                            }
                        }
                    }
                }
            });
            sickleBtn.OnEndDragAsObservable().Subscribe(b =>
            {
                sickleBtn.interactable = true;
                sickleRectTf.DOAnchorPos(sickleBtnOrigin, 0.2f);
            });

            cropRectTf = cropBtn.transform as RectTransform;
            cropBtnOrigin = cropRectTf.anchoredPosition;
            cropBtn.OnBeginDragAsObservable().Subscribe(b => cropBtn.interactable = false);
            cropBtn.OnDragAsObservable().Subscribe(b =>
            {
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rootRectTf, b.position, null, out var lp))
                {
                    cropRectTf.anchoredPosition = lp;

                    Vector3 pos = Camera.main.ScreenToWorldPoint(b.position);
                    Vector2Int index = MathUtility.GamePosToIndex(pos);
                    IMapScene mapScene = World.Current.FindEntity<IMapScene>();
                    IGridEntity grid = mapScene.GetGrid(index);
                    if (grid != null)
                    {
                        IItemEntity item = grid.GetItem(GameConst.SURFACEDECORATE_LAYER);
                        if (item != null)
                        {
                            PloughItem ploughItem = item as PloughItem;
                            if (ploughItem != null)
                            {
                                ploughItem.SetCrop(Random.Range(300014, 300016));
                            }
                        }
                    }
                }
            });
            cropBtn.OnEndDragAsObservable().Subscribe(b =>
            {
                cropBtn.interactable = true;
                cropRectTf.DOAnchorPos(cropBtnOrigin, 0.2f);
            });
        }
    }
}
