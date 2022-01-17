using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSphere : MonoBehaviour
{
    private const float yPositionConst = 2.05f;

    [SerializeField] private float animationSpeed;
    [SerializeField] private float transparencySpeed;

    public GameObject destroyEffectPrefab;

    private Transform sphereTransform;
    private Transform wallTransform;
    private MeshRenderer sphereRenderer;
    private MeshRenderer wallRenderer;
    private ParticleSystem destroyEffect;
    private Wall wallScript;
    private Vector3 newDestination;
    private Vector3 newWallScale;
    private float sphereAnimateProgress;
    private float wallScalingProgress;
    private bool sphereIsMoving;

    private Color colorOrange;
    private Color colorGreen;
    private Color colorTransparent;

    private List<Transform> currentWallsList;

    private void Start()
    {
        animationSpeed = 0.0015f;
        transparencySpeed = 0.06f;
        sphereAnimateProgress = 0;
        wallScalingProgress = 0;
        sphereIsMoving = false;
        sphereTransform = transform;
        wallTransform = null;

        colorOrange = new Color(1, 0.6f, 0, 0.84f);
        colorGreen = new Color(0.18f, 1, 0, 0.84f);
        colorTransparent = new Color(1, 1, 1, 0);

        sphereRenderer = GetComponent<MeshRenderer>();
        newDestination = sphereTransform.position;
        currentWallsList = new List<Transform>();
    }
    private void FixedUpdate()
    {
        Move();
        AnimateDestruction();
    }
    private void Move()
    {
        if (sphereTransform.position != newDestination) {
            sphereTransform.position = Vector3.Lerp(sphereTransform.position, newDestination, sphereAnimateProgress);
            sphereAnimateProgress += animationSpeed;
        }
        else
        {
            sphereIsMoving = false;
            ToggleColor();
        }
    }
    public void TryToMove(string direction)
    {
        if(!sphereIsMoving)
        {
            sphereIsMoving = true;
            Transform currentWall = null;

            float directionX = sphereTransform.position.x;
            float directionZ = sphereTransform.position.z;

            float roundingPosX = Mathf.Round(sphereTransform.position.x * 100) / 100;
            float roundingPosZ = Mathf.Round(sphereTransform.position.z * 100) / 100;

            foreach (var wall in currentWallsList)
            {
                if (wall.position.x > roundingPosX)
                {
                    if (direction == "right")
                    {
                        currentWall = wall;
                        directionX = roundingPosX + wall.localScale.x + 1;
                        break;
                    }
                }
                if (wall.position.x < roundingPosX)
                {
                    if (direction == "left")
                    {
                        currentWall = wall;
                        directionX = roundingPosX - (wall.localScale.x + 1);
                        break;
                    }
                }
                if (wall.position.z > roundingPosZ)
                {
                    if (direction == "up")
                    {
                        currentWall = wall;
                        directionZ = roundingPosZ + wall.localScale.x + 1;
                        break;
                    }
                }
                if (wall.position.z < roundingPosZ)
                {
                    if (direction == "down")
                    {
                        currentWall = wall;
                        directionZ = roundingPosZ - (wall.localScale.x + 1);
                        break;
                    }
                }
            }
            if (currentWall != null)
            {
                SelectWall(currentWall);
                SetNewDestination(directionX, directionZ);
                ToggleColor();
                if (currentWallsList.Count == 0)
                    Invoke("EndCheck", 2f);
            }
        }
    }
    private void SetNewDestination(float directionX, float directionZ)
    {
        wallScalingProgress = 0;
        sphereAnimateProgress = 0;
        currentWallsList.Clear();
        newDestination = new Vector3(directionX, yPositionConst, directionZ);
    }
    private void OnTriggerEnter(Collider collider)
    {
        if(collider.tag == "AddWall")
        {
            currentWallsList.Add(collider.transform);
        }
    }
    private void SelectWall(Transform currentWall)
    {
        wallTransform = currentWall;
        newWallScale = wallTransform.localScale * 1.3f;
        wallRenderer = wallTransform.GetComponent<MeshRenderer>();
        wallScript = wallTransform.GetComponent<Wall>();
        wallRenderer.material = Resources.Load<Material>("Materials/AddWallFade");
    }
    private void ToggleColor()
    {
        if(sphereIsMoving)
        {
            sphereRenderer.material.SetColor("_Color", colorOrange);
        }
        else
        {
            sphereRenderer.material.SetColor("_Color", colorGreen);
        }
    }
    private void AnimateDestruction()
    {
        if (wallTransform != null)
        {
            wallTransform.localScale = Vector3.Lerp(wallTransform.localScale, newWallScale, wallScalingProgress);
            wallRenderer.material.color = Color.Lerp(wallRenderer.material.color, colorTransparent, transparencySpeed);

            wallScalingProgress += animationSpeed * 2;

            if (wallRenderer.material.color.a <= 0.01f)
            {
                wallScript.DestroyWall();
                StickmansController.Instance.DelayBeforeBuildPaths();
            }
            if (wallRenderer.material.color.a <= 0.6f)
                CreateDestroyEffect();
        }
    }
    private void CreateDestroyEffect()
    {
        if (destroyEffect == null)
        {
            Vector3 rotation = destroyEffectPrefab.transform.rotation.eulerAngles;
            rotation = new Vector3(rotation.x, rotation.y, wallTransform.rotation.eulerAngles.y + 90);

            destroyEffect = Instantiate(destroyEffectPrefab, wallTransform.position, Quaternion.Euler(rotation)).GetComponent<ParticleSystem>();

            var shape = destroyEffect.shape;
            shape.scale = new Vector3(1, wallTransform.localScale.x, 1);

            Destroy(destroyEffect.gameObject, destroyEffect.main.startLifetime.constant);
        }
    }
    private void EndCheck()
    {
        if (currentWallsList.Count == 0)
            StickmansController.Instance.StartCheckingPaths();
    }
}