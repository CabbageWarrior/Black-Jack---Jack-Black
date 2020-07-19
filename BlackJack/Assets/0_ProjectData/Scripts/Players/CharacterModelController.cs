using UnityEngine;
using DG.Tweening;

namespace CabbageSoft.BlackJack.Characters
{
    public class CharacterModelController : MonoBehaviour
    {
        public enum ECharacterAction
        {
            AskCard,
            Stop,
            Busted,
            Win,
            Lose
        }

        private static int AnimProperty_AskCard = Animator.StringToHash("Ask Card");
        private static int AnimProperty_Stop = Animator.StringToHash("Stop");
        private static int AnimProperty_Busted = Animator.StringToHash("Busted");
        private static int AnimProperty_Win = Animator.StringToHash("Win");
        private static int AnimProperty_Lose = Animator.StringToHash("Lose");

        /// <summary>
        /// Reference to the CharacterAnimator.
        /// </summary>
        public Animator characterAnimator = default;
        /// <summary>
        /// Reference to the CharacterFace.
        /// </summary>
        public GameObject characterFace = default;

        /// <summary>
        /// Initial scale of the CharacterFace.
        /// </summary>
        private Vector3 characterFaceInitialScale = default;

        private SpriteRenderer frontFaceSpriteRenderer = default;

        private Sequence faceSequence = default;

        private void Awake()
        {
            frontFaceSpriteRenderer = characterFace.GetComponentInChildren<SpriteRenderer>();
        }

        private void Start()
        {
            characterFaceInitialScale = characterFace.transform.localScale;
        }

        public void SetTriggerAction(ECharacterAction characterAction)
        {
            switch (characterAction)
            {
                case ECharacterAction.AskCard:
                    characterAnimator.SetTrigger(AnimProperty_AskCard);
                    break;
                case ECharacterAction.Stop:
                    characterAnimator.SetTrigger(AnimProperty_Stop);
                    break;
                case ECharacterAction.Busted:
                    characterAnimator.SetTrigger(AnimProperty_Busted);
                    break;
                case ECharacterAction.Win:
                    characterAnimator.SetTrigger(AnimProperty_Win);
                    break;
                case ECharacterAction.Lose:
                    characterAnimator.SetTrigger(AnimProperty_Lose);
                    break;
                default:
                    break;
            }
        }

        public void SetFaceSprite(Sprite sprite)
        {
            if (frontFaceSpriteRenderer && sprite)
            {
                frontFaceSpriteRenderer.sprite = sprite;
            }
        }

        public void SetFaceScale(bool bigger)
        {
            if (faceSequence != null) faceSequence.Kill();

            if (bigger)
            {
                // Scales the face to a greater value in order to declare who is the current player.
                faceSequence = DOTween.Sequence();
                faceSequence.Append(characterFace.transform.DOScale(characterFaceInitialScale + Vector3.one * .25f, .3f));
            }
            else
            {
                faceSequence = DOTween.Sequence();
                faceSequence.Append(characterFace.transform.DOScale(characterFaceInitialScale, .2f));
            }
        }
    }
}
