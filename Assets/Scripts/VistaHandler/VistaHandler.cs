using System.Collections.Generic;
using UnityEngine;

namespace VistaHandlerScripts
{
    [System.Serializable]
    public class VistaLayer
    {
        public Transform layerTransform;
        public Transform[] loopableLayers = new Transform[3];
        [Range(0f, 1f)] public float parallaxFactor = 0.5f;

        private float _tileWidth;

        public void Initialize()
        {
            SpriteRenderer sr = loopableLayers[0].GetComponent<SpriteRenderer>();

            if (sr != null)
            {
                _tileWidth = sr.bounds.size.x * 1f;
            }
        }

        public void Move(float cameraX)
        {
            Vector3 pos = layerTransform.position;
            pos.x = cameraX * parallaxFactor;
            layerTransform.position = pos;

            CheckLoop(cameraX);
        }

        private void CheckLoop(float cameraX)
        {
            float distance = cameraX - loopableLayers[1].position.x;

            if (distance >= _tileWidth)
            {
                SwitchRight();
            }
            else if (distance <= -_tileWidth)
            {
                SwitchLeft();
            }
        }

        public void SwitchRight()
        {
            Transform left = loopableLayers[0];

            loopableLayers[0] = loopableLayers[1];
            loopableLayers[1] = loopableLayers[2];
            loopableLayers[2] = left;

            left.position = loopableLayers[1].position + Vector3.right * _tileWidth;
        }

        public void SwitchLeft()
        {
            Transform right = loopableLayers[2];

            loopableLayers[2] = loopableLayers[1];
            loopableLayers[1] = loopableLayers[0];
            loopableLayers[0] = right;

            right.position = loopableLayers[1].position + Vector3.left * _tileWidth;
        }
    }

    public class VistaHandler : MonoBehaviour
    {
        public List<VistaLayer> vistaLayers = new List<VistaLayer>();

        private Transform _cameraTransform;

        private void Start()
        {
            _cameraTransform = Camera.main.transform;

            foreach (VistaLayer layer in vistaLayers)
            {
                layer.Initialize();
            }
        }

        private void LateUpdate()
        {
            float cameraX = _cameraTransform.position.x;

            foreach (VistaLayer layer in vistaLayers)
            {
                layer.Move(cameraX);
            }
        }
    }
}