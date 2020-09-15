using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fluids._2D
{
    public class Fluid : MonoBehaviour
    {
        [Header("bounds")]
        public Vector4 bounds;

        public int count;
        public ComputeBuffer particleBuffer;


        [Header("Material")]
        public Color color;
        public float alphaCutoff;
        public float smoothFactor;

        public float radious;

        public float solidRadious;
        public float maxRadious;

        [Header("Texture")]
        public float dpi;


        public Vector2Int texSize
        {
            get { return new Vector2Int((int)(bounds.z * dpi), (int)(bounds.w * dpi)); }
        }

        [Header("Renderer")]
        public SpriteRenderer spriteRenderer;

        [Header("Compute Shaders")]
        public ComputeShader fluidParticlesCompute;
        public ComputeShader fluidBaseCompute;
        public ComputeShader fluidMaterialCompute;

        [Header("Temporary")]
        public GameObject particlePrefab;
        private Transform[] ts;

        private void Start()
        {
            ts = new Transform[20];
            for (int i = 0; i < ts.Length; i++)
                ts[i] = Instantiate(particlePrefab).transform;
            SetComputeData();
            UpdateTexture();

        }
        private void Update()
        {
            Particle[] ps = new Particle[ts.Length];
            for (int i = 0; i < ps.Length; i++)
                ps[i] = new Particle(ts[i].position.x, ts[i].position.y);
            ResetParticleBuffer();
            AddParticles(ps);
            UpdateTexture();
        }

        private void OnDestroy()
        {
            if (particleBuffer != null)
            {
                particleBuffer.Release();
            }
        }
        private void OnApplicationQuit()
        {
            if (particleBuffer != null)
            {
                particleBuffer.Release();
            }
        }


        void ResetParticleBuffer()
        {
            count = 0;
            if (particleBuffer != null)
            {
                particleBuffer.Release();
                particleBuffer = null;
            }
        }
        void AddParticles(Particle[] newParticles)
        {
            if (count > 0)
            {
                Particle[] oldParticles = new Particle[count];
                Particle[] allParticles = new Particle[count + newParticles.Length];

                particleBuffer.GetData(oldParticles);

                int index = 0;
                for (int i = 0; i < count; i++)
                {
                    allParticles[index] = oldParticles[i];
                    index++;
                }
                for (int i = 0; i < newParticles.Length; i++)
                {
                    allParticles[index] = newParticles[i];
                    index++;
                }

                particleBuffer = new ComputeBuffer(allParticles.Length, Particle.stride);
                particleBuffer.SetData(allParticles);
                count = allParticles.Length;
            }
            else
            {
                particleBuffer = new ComputeBuffer(newParticles.Length, Particle.stride);
                particleBuffer.SetData(newParticles);
                count = newParticles.Length;
            }
        }

        void SetComputeData()
        {
            // fluidParticleCompute

            // fluidBaseCompute
            fluidBaseCompute.SetVector("bounds", bounds);

            fluidBaseCompute.SetFloat("solidRadious", solidRadious);
            fluidBaseCompute.SetFloat("maxRadious", maxRadious);

            fluidBaseCompute.SetVector("color", new Vector4(color.r, color.g, color.b, color.a));

            fluidBaseCompute.SetFloat("dpi", dpi);

            fluidBaseCompute.SetVector("texSize", (Vector2)texSize);

            //fluidMaterialCompute
            fluidMaterialCompute.SetFloat("alphaCutoff", alphaCutoff);
            fluidMaterialCompute.SetFloat("smoothFactor", smoothFactor);

            fluidMaterialCompute.SetVector("color", new Vector4(color.r, color.g, color.b, color.a));

            fluidMaterialCompute.SetVector("texSize", (Vector2)texSize);
        }

        void UpdateTexture()
        {
            if (count > 0)
            {
                fluidBaseCompute.SetInt("count", count);

                fluidBaseCompute.SetBuffer(0, "particles", particleBuffer);

                RenderTexture rt = new RenderTexture(texSize.x, texSize.y, 32);
                rt.enableRandomWrite = true;
                rt.Create();
                fluidBaseCompute.SetTexture(0, "tex", rt);

                int numthreadX = Mathf.CeilToInt(texSize.x / 8f);
                int numthreadY = Mathf.CeilToInt(texSize.y / 8f);

                fluidBaseCompute.Dispatch(0, numthreadX, numthreadY, 1);

                fluidMaterialCompute.SetTexture(0, "tex", rt);

                fluidMaterialCompute.Dispatch(0, numthreadX, numthreadY, 1);

                RenderTexture active = RenderTexture.active;
                RenderTexture.active = rt;

                Texture2D tex = new Texture2D(rt.width, rt.height);
                tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
                tex.Apply();

                spriteRenderer.sprite = Sprite.Create(tex, new Rect(0, 0, rt.width, rt.height), new Vector2(0, 0));

                RenderTexture.active = active;


            }
            else
            {
                
            }
        }

    }
}
