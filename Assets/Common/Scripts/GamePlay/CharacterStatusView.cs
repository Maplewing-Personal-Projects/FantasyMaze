using UnityEngine;

namespace Maplewing.FantansyMaze
{
    public class CharacterStatusView : MonoBehaviour
    {
        public static readonly Vector2 CAMERA_OFFSET = new Vector2(-175f, -280f);

        private static readonly string[] IDLE_ANIMATION_STATE = new string[]
        {
            "Idle",
            "Idle2"
        };


        [SerializeField]
        private GameObject _characterModel;

        public void SetPositionByCamera(Camera camera)
        {
            var cameraPosition = camera.transform.localPosition;

            _characterModel.transform.localPosition = new Vector3(
                cameraPosition.x + CAMERA_OFFSET.x,
                cameraPosition.y + CAMERA_OFFSET.y,
                _characterModel.transform.localPosition.z);
        }
        
        public void PlayHitAnimation()
        {
            var animator = _characterModel.GetComponent<Animator>();
            animator.Play("Hit");
        }

        public void PlayHappyAnimation()
        {
            var animator = _characterModel.GetComponent<Animator>();
            animator.Play("Happy");
        }

        private void Update()
        {
            var animator = _characterModel.GetComponent<Animator>();
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("End"))
            {
                animator.Play(IDLE_ANIMATION_STATE[Random.Range(0, 2)]);
            }
        }
    }
}