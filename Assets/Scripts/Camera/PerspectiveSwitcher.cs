using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System.Collections;

public class PerspectiveSwitcher : Interactable
{
    new public Camera camera;
    Transform camTransform;

    public float fov = 60f,
                        near = .3f,
                        far = 1000f,
                        orthographicSize = 50f;
    private float aspect;
    private bool orthoOn;
    private int triggers = 0;


    //0: persective
    //1: ortho
    float blend = 0.9f;
    float velocity = 0;
    float target = 1;
    public float longTime = 4;
    public AudioSource longfx;
    public float shortTime = 1;
    public AudioSource shortfx;

    public int postProcessingLayer;
    PostProcessVolume volume;
    ChromaticAberration aberration;

    public FPController p_Player;
    public Transform p_root;
    private Matrix4x4 p_matrix;

    public CubePlayer o_Player;
    public Transform o_root;
    private Matrix4x4 o_matrix;

    protected override void Start()
    {
        base.Start();

        aspect = Screen.width / (float)Screen.height;
        o_matrix = Matrix4x4.Ortho(-orthographicSize * aspect, orthographicSize * aspect, -orthographicSize, orthographicSize, near, far);
        p_matrix = Matrix4x4.Perspective(fov, aspect, near, far);
        camera.projectionMatrix = o_matrix;
        camTransform = camera.transform;
        orthoOn = true;
        p_Player.c.Disable();
        o_Player.c.Player.Reify.performed += Interaction;

        aberration = ScriptableObject.CreateInstance<ChromaticAberration>();
        aberration.enabled.Override(true);
        aberration.intensity.Override(1f);

        volume = PostProcessManager.instance.QuickVolume(postProcessingLayer, 100f, aberration);
        
        volume.weight = 1f;
        Update();
    }

    public override void Interaction(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        orthoOn = !orthoOn;
        target = orthoOn ? 1 : 0;
        if (orthoOn)
        {
            o_Player.c.Enable();
            p_Player.c.Disable();
        }
        else
        {
            o_Player.c.Disable();
            p_Player.c.Enable();
        }

        if (triggers++ < 1)
        {
            longfx.Play();
        }
        else
        {
            shortfx.Play();
        }

        print("Switching: " + orthoOn.ToString());
    }

    void Update()
    {
        blend = Mathf.SmoothDamp(blend, target, ref velocity, (triggers < 2)? longTime : shortTime);
        camera.projectionMatrix = Helper.Lerp(p_matrix, o_matrix, blend);
        camTransform.position = Vector3.Lerp(p_root.position, o_root.position, blend);
        camTransform.rotation = Quaternion.Slerp(p_root.rotation, o_root.rotation, blend);
        aberration.intensity.value = (-4 * blend * blend) + (4 * blend);
        shortfx.spatialBlend = longfx.spatialBlend = 1 - blend;
    }

    private void OnDestroy()
    {
        RuntimeUtilities.DestroyVolume(volume, true);
    }
}
