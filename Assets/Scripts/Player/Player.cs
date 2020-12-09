using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class Player : MonoBehaviour
    {
        #region Atributos

        public GameObject Camera { get; private set; }

        private Vector3 _moveDirection;

        public CharacterController Controller { get; private set; }

        public float MoveSpeed { get; private set; }

        public float JumpForce { get; private set; }

        public float CameraSpeed { get; private set; }

        public float CameraRotationX { get; private set; }

        public float CameraRotationY { get; private set; }

        #endregion

        #region getter and setter

        public Vector3 MoveDirection
        {
            get => _moveDirection;
            set => _moveDirection = value;
        }

        #endregion

        private void Awake()
        {
            MoveSpeed = 5;
            CameraSpeed = 5f;
            JumpForce = 10f;
            MoveDirection = Vector3.zero;
            Controller = GetComponent<CharacterController>();
        }

        void Start()
        {
            transform.tag = "Player";
            Camera = GetComponentInChildren(typeof(Camera)).transform.gameObject;
            Camera.transform.localRotation = Quaternion.identity;
            Camera.transform.localPosition = new Vector3(0, 0.9f, 0);
        }

        void Update()
        {
            MovimentaPlayer();
            CameraController();
        }

        #region Movimentação

        private void MovimentaPlayer()
        {
            Vector3 cameraDirectionFront = MovimentaHorizontal();
            cameraDirectionFront.Normalize();
            Vector3 cameraDirectionRight = MovimentaVertical();
            cameraDirectionRight.Normalize();
            if (Controller.isGrounded)
            {
                MovimentacaoFinal(cameraDirectionFront, cameraDirectionRight);
                Jump();
            }

            PutGravityInY();
            Controller.Move(_moveDirection * Time.deltaTime);
        }

        private void PutGravityInY()
        {
            _moveDirection.y += Physics.gravity.y * Time.deltaTime;
        }

        private void Jump()
        {
            if (Input.GetButtonDown("Jump"))
            {
                _moveDirection.y = JumpForce;
            }
        }

        private void MovimentacaoFinal(Vector3 cameraDirectionFront, Vector3 cameraDirectionRight)
        {
            Vector3 cameraDirectFinal = cameraDirectionFront + cameraDirectionRight;
            MoveDirection = new Vector3(cameraDirectFinal.x, 0, cameraDirectFinal.z) * MoveSpeed;
        }

        private Vector3 MovimentaVertical()
        {
            Vector3 cameraForward = this.Camera.transform.forward;
            Vector3 cameraDirectionFront = new Vector3(cameraForward.x, 0, cameraForward.z);
            cameraDirectionFront *= Input.GetAxis("Vertical");
            return cameraDirectionFront;
        }

        private Vector3 MovimentaHorizontal()
        {
            Vector3 cameraRight = Camera.transform.right;
            Vector3 cameraDirectionRight = new Vector3(cameraRight.x, 0, cameraRight.z);
            cameraDirectionRight *= Input.GetAxis("Horizontal");
            return cameraDirectionRight;
        }

        #endregion

        #region Camera

        private void CameraController()
        {
            CameraRotationX += Input.GetAxis("Mouse X") * CameraSpeed;
            CameraRotationX = ClampAngle(CameraRotationX, -360, 360);
            CameraRotationY += Input.GetAxis("Mouse Y") * CameraSpeed;
            CameraRotationY = ClampAngle(CameraRotationY, -80, 80);
            RotarionaPlayer();
        }

        private void RotarionaPlayer()
        {
            Quaternion cameraQuaternionX = Quaternion.AngleAxis(CameraRotationX, Vector3.up);
            Quaternion cameraQuaternionY = Quaternion.AngleAxis(CameraRotationY, -Vector3.right);
            Quaternion cameraQuaternionFinal = Quaternion.identity * cameraQuaternionX * cameraQuaternionY;
            Camera.transform.localRotation = Quaternion.Lerp(Camera.transform.localRotation, cameraQuaternionFinal,
                CameraSpeed * Time.deltaTime);
        }

        private float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360)
            {
                angle += 360;
            }

            if (angle > 360)
            {
                angle -= 360;
            }

            return Mathf.Clamp(angle, min, max);
        }

        #endregion
    }
}