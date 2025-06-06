using UnityEngine;
using System.Collections;
//using UnityStandardAssets.ImageEffects;

public enum GunStyles
{
    nonautomatic, automatic
}
public class GunScript : MonoBehaviour
{
    [Tooltip("Selects type of weapon to shoot rapidly or one bullet per click.")]
    public GunStyles currentStyle;
    [HideInInspector]
    public MouseLookScript mls;

    [Header("Player movement properties")]
    [Tooltip("Speed is determined via gun because not every gun has same properties or weights so you MUST set up your speeds here")]
    public int walkingSpeed = 3;
    [Tooltip("Speed is determined via gun because not every gun has same properties or weights so you MUST set up your speeds here")]
    public int runningSpeed = 5;


    [Header("Bullet properties")]
    [Tooltip("Preset value to tell with how many bullets will our weapon spawn aside.")]
    public float bulletsIHave = 20;
    [Tooltip("Preset value to tell with how much bullets will our weapon spawn inside rifle.")]
    public float bulletsInTheGun = 5;
    [Tooltip("Preset value to tell how much bullets can one magazine carry.")]
    public float amountOfBulletsPerLoad = 5;
    public string ammo_type = "9mm";
    public float bullet_impact_force = 1000;
    [Tooltip("Gun damage")]
    public float damage_force = 60;

    [Header("Shells settings")]
    public ObjectsPool shells_pool;
    public GameObject shells_spawn_point;
    public float shells_shoot_time = 0.1f;
    public float shells_push_force = 2f;
    public string shells_layer;

    [Header("Droping magazines settings")]
    public ObjectsPool magazines_pool;
    public GameObject magazines_spawn_point;
    public float magazines_spawn_time = 0.1f;
    public string magazine_layer;

    [Header("Other")]
    public float min_obstacle_distance = 0;

    private Transform player;
    private Camera cameraComponent;
    private Transform gunPlaceHolder;

    private PlayerMovementScript pmS;

    /*
	 * Collection the variables upon awake that we need.
	 */
    void Awake()
    {


        mls = GameObject.FindGameObjectWithTag("Player").GetComponent<MouseLookScript>();
        player = mls.transform;
        mainCamera = mls.myCamera;
        secondCamera = GameObject.FindGameObjectWithTag("SecondCamera").GetComponent<Camera>();
        cameraComponent = mainCamera.GetComponent<Camera>();
        pmS = player.GetComponent<PlayerMovementScript>();

        //update max speed after gun switch
        if(pmS.isRunning)
        {
            pmS.maxSpeed = runningSpeed;
        }
        else
        {
            pmS.maxSpeed = walkingSpeed;
        }

        //bulletSpawnPlace = GameObject.FindGameObjectWithTag("BulletSpawn");
        hitMarker = transform.Find("hitMarkerSound").GetComponent<AudioSource>();

        startLook = mouseSensitvity_notAiming;
        startAim = mouseSensitvity_aiming;
        startRun = mouseSensitvity_running;

        rotationLastY = mls.currentYRotation;
        rotationLastX = mls.currentCameraXRotation;

        pickup_source.Play();

    }


    [HideInInspector]
    public Vector3 currentGunPosition;
    [Header("Gun Positioning")]
    [Tooltip("Vector 3 position from player SETUP for NON AIMING values")]
    public Vector3 restPlacePosition;
    [Tooltip("Vector 3 position from player SETUP for AIMING values")]
    public Vector3 aimPlacePosition;
    [Tooltip("Time that takes for gun to get into aiming stance.")]
    public float gunAimTime = 0.1f;

    [HideInInspector]
    public bool reloading;

    private Vector3 gunPosVelocity;
    private float cameraZoomVelocity;
    private float secondCameraZoomVelocity;

    private Vector2 gunFollowTimeVelocity;

    /*
	Update loop calling for methods that are descriped below where they are initiated.
	*/
    void Update()
    {

        Animations();

        GiveCameraScriptMySensitvity();

        PositionGun();

        Shooting(); 
                    //MeeleAttack();
        LockCameraWhileMelee();

        Sprint(); //if we have the gun you sprint from here, if we are gunless then its called from movement script

        CrossHairExpansionWhenWalking();

        CheckWeaponBlock();
    }

    void CheckWeaponBlock()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            handsAnimator.SetBool("weaponBlocked", !handsAnimator.GetBool("weaponBlocked"));
        }
    }

    public void SetBlockedStatus(bool status)
    {
        handsAnimator.SetBool("weaponBlocked", status);
    }

    /*
	*Update loop calling for methods that are descriped below where they are initiated.
	*+
	*Calculation of weapon position when aiming or not aiming.
	*/
    void FixedUpdate()
    {
        if(is_usable)
        {
            RotationGun();

            MeeleAnimationsStates();

            /*
             * Changing some values if we are aiming, like sensitity, zoom racion and position of the waepon.
             */
            //if aiming
            if (Input.GetAxis("Fire2") != 0 && !reloading && !meeleAttack)
            {
                gunPrecision = gunPrecision_aiming;
                recoilAmount_x = recoilAmount_x_;
                recoilAmount_y = recoilAmount_y_;
                recoilAmount_z = recoilAmount_z_;
                currentGunPosition = Vector3.SmoothDamp(currentGunPosition, aimPlacePosition, ref gunPosVelocity, gunAimTime);
                cameraComponent.fieldOfView = Mathf.SmoothDamp(cameraComponent.fieldOfView, cameraZoomRatio_aiming, ref cameraZoomVelocity, gunAimTime);
                secondCamera.fieldOfView = Mathf.SmoothDamp(secondCamera.fieldOfView, secondCameraZoomRatio_aiming, ref secondCameraZoomVelocity, gunAimTime);
            }
            //if not aiming
            else
            {
                gunPrecision = gunPrecision_notAiming;
                recoilAmount_x = recoilAmount_x_non;
                recoilAmount_y = recoilAmount_y_non;
                recoilAmount_z = recoilAmount_z_non;
                currentGunPosition = Vector3.SmoothDamp(currentGunPosition, restPlacePosition, ref gunPosVelocity, gunAimTime);
                cameraComponent.fieldOfView = Mathf.SmoothDamp(cameraComponent.fieldOfView, cameraZoomRatio_notAiming, ref cameraZoomVelocity, gunAimTime);
                secondCamera.fieldOfView = Mathf.SmoothDamp(secondCamera.fieldOfView, secondCameraZoomRatio_notAiming, ref secondCameraZoomVelocity, gunAimTime);
            }
        }

    }

    [Header("Sensitvity of the gun")]
    [Tooltip("Sensitvity of this gun while not aiming.")]
    public float mouseSensitvity_notAiming = 10;
    //[HideInInspector]
    [Tooltip("Sensitvity of this gun while aiming.")]
    public float mouseSensitvity_aiming = 5;
    //[HideInInspector]
    [Tooltip("Sensitvity of this gun while running.")]
    public float mouseSensitvity_running = 4;
    /*
	 * Used to give our main camera different sensivity options for each gun.
	 */
    void GiveCameraScriptMySensitvity()
    {
        mls.mouseSensitvity_notAiming = mouseSensitvity_notAiming;
        mls.mouseSensitvity_aiming = mouseSensitvity_aiming;
    }

    /*
	 * Used to expand position of the crosshair or make it dissapear when running
	 */
    void CrossHairExpansionWhenWalking()
    {

        if (player.GetComponent<Rigidbody>().velocity.magnitude > 1 && Input.GetAxis("Fire1") == 0)
        {//ifnot shooting

            expandValues_crosshair += new Vector2(20, 40) * Time.deltaTime;
            if (player.GetComponent<PlayerMovementScript>().maxSpeed < runningSpeed)
            { //not running
                expandValues_crosshair = new Vector2(Mathf.Clamp(expandValues_crosshair.x, 0, 10), Mathf.Clamp(expandValues_crosshair.y, 0, 20));
                fadeout_value = Mathf.Lerp(fadeout_value, 1, Time.deltaTime * 2);
            }
            else
            {//running
                fadeout_value = Mathf.Lerp(fadeout_value, 0, Time.deltaTime * 10);
                expandValues_crosshair = new Vector2(Mathf.Clamp(expandValues_crosshair.x, 0, 20), Mathf.Clamp(expandValues_crosshair.y, 0, 40));
            }
        }
        else
        {//if shooting
            expandValues_crosshair = Vector2.Lerp(expandValues_crosshair, Vector2.zero, Time.deltaTime * 5);
            expandValues_crosshair = new Vector2(Mathf.Clamp(expandValues_crosshair.x, 0, 10), Mathf.Clamp(expandValues_crosshair.y, 0, 20));
            fadeout_value = Mathf.Lerp(fadeout_value, 1, Time.deltaTime * 2);

        }

    }

    /* 
	 * Changes the max speed that player is allowed to go.
	 * Also max speed is connected to the animator which will trigger the run animation.
	 */
    void Sprint()
    {// Running();  so i can find it with CTRL + F
        if (Input.GetAxis("Vertical") > 0 && Input.GetAxisRaw("Fire2") == 0 && meeleAttack == false && Input.GetAxisRaw("Fire1") == 0)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                if (pmS.maxSpeed == walkingSpeed)
                {
                    pmS.maxSpeed = runningSpeed;//sets player movement speed to max
                    pmS.isRunning = true;
                }
                else
                {
                    pmS.maxSpeed = walkingSpeed;
                    pmS.isRunning = false;
                }
            }
        }
        else
        {
            pmS.maxSpeed = walkingSpeed;
            pmS.isRunning = false;
        }

    }

    [HideInInspector]
    public bool meeleAttack;
    [HideInInspector]
    public bool aiming;
    /*
	 * Checking if meeleAttack is already running.
	 * If we are not reloading we can trigger the MeeleAttack animation from the IENumerator.
	 */
    void MeeleAnimationsStates()
    {
        if (handsAnimator)
        {
            meeleAttack = handsAnimator.GetCurrentAnimatorStateInfo(0).IsName(meeleAnimationName);
            aiming = handsAnimator.GetCurrentAnimatorStateInfo(0).IsName(aimingAnimationName);
        }
    }
    /*
	* User inputs meele attack with Q in keyboard start the coroutine for animation and damage attack.
	*/
    void MeeleAttack()
    {

        if (Input.GetKeyDown(KeyCode.Q) && !meeleAttack)
        {
            StartCoroutine("AnimationMeeleAttack");
        }
    }
    /*
	* Sets meele animation to play.
	*/
    IEnumerator AnimationMeeleAttack()
    {
        handsAnimator.SetBool("meeleAttack", true);
        //yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.1f);
        handsAnimator.SetBool("meeleAttack", false);
    }

    private float startLook, startAim, startRun;
    /*
	* Setting the mouse sensitvity lower when meele attack and waits till it ends.
	*/
    void LockCameraWhileMelee()
    {
        if (meeleAttack)
        {
            mouseSensitvity_notAiming = 2;
            mouseSensitvity_aiming = 1.6f;
            mouseSensitvity_running = 1;
        }
        else
        {
            mouseSensitvity_notAiming = startLook;
            mouseSensitvity_aiming = startAim;
            mouseSensitvity_running = startRun;
        }
    }


    private Vector3 velV;
    [HideInInspector]
    public Transform mainCamera;
    private Camera secondCamera;
    /*
	 * Calculatin the weapon position accordingly to the player position and rotation.
	 * After calculation the recoil amount are decreased to 0.
	 */
    void PositionGun()
    {
        transform.position = Vector3.SmoothDamp(transform.position,
            mainCamera.transform.position -
            (mainCamera.transform.right * (currentGunPosition.x + currentRecoilXPos)) +
            (mainCamera.transform.up * (currentGunPosition.y + currentRecoilYPos)) +
            (mainCamera.transform.forward * (currentGunPosition.z + currentRecoilZPos)), ref velV, 0);



        pmS.cameraPosition = new Vector3(currentRecoilXPos, currentRecoilYPos, 0);

        currentRecoilZPos = Mathf.SmoothDamp(currentRecoilZPos, 0, ref velocity_z_recoil, recoilOverTime_z);
        currentRecoilXPos = Mathf.SmoothDamp(currentRecoilXPos, 0, ref velocity_x_recoil, recoilOverTime_x);
        currentRecoilYPos = Mathf.SmoothDamp(currentRecoilYPos, 0, ref velocity_y_recoil, recoilOverTime_y);

    }


    [Header("Rotation")]
    private Vector2 velocityGunRotate;
    private float gunWeightX, gunWeightY;
    [Tooltip("The time waepon will lag behind the camera view best set to '0'.")]
    public float rotationLagTime = 0f;
    private float rotationLastY;
    private float rotationDeltaY;
    private float angularVelocityY;
    private float rotationLastX;
    private float rotationDeltaX;
    private float angularVelocityX;
    [Tooltip("Value of forward rotation multiplier.")]
    public Vector2 forwardRotationAmount = Vector2.one;
    /*
	* Rotatin the weapon according to mouse look rotation.
	* Calculating the forawrd rotation like in Call Of Duty weapon weight
	*/
    void RotationGun()
    {

        rotationDeltaY = mls.currentYRotation - rotationLastY;
        rotationDeltaX = mls.currentCameraXRotation - rotationLastX;

        rotationLastY = mls.currentYRotation;
        rotationLastX = mls.currentCameraXRotation;

        angularVelocityY = Mathf.Lerp(angularVelocityY, rotationDeltaY, Time.deltaTime * 5);
        angularVelocityX = Mathf.Lerp(angularVelocityX, rotationDeltaX, Time.deltaTime * 5);

        gunWeightX = Mathf.SmoothDamp(gunWeightX, mls.currentCameraXRotation, ref velocityGunRotate.x, rotationLagTime);
        gunWeightY = Mathf.SmoothDamp(gunWeightY, mls.currentYRotation, ref velocityGunRotate.y, rotationLagTime);

        transform.rotation = Quaternion.Euler(gunWeightX + (angularVelocityX * forwardRotationAmount.x), gunWeightY + (angularVelocityY * forwardRotationAmount.y), 0);
    }

    private float currentRecoilZPos;
    private float currentRecoilXPos;
    private float currentRecoilYPos;
    /*
	 * Called from ShootMethod();, upon shooting the recoil amount will increase.
	 */
    public void RecoilMath()
    {
        currentRecoilZPos -= recoilAmount_z;
        currentRecoilXPos -= (Random.value - 0.5f) * recoilAmount_x;
        currentRecoilYPos -= (Random.value - 0.5f) * recoilAmount_y;
        mls.wantedCameraXRotation -= Mathf.Abs(currentRecoilYPos * gunPrecision);
        mls.wantedYRotation -= (currentRecoilXPos * gunPrecision);

        expandValues_crosshair += new Vector2(6, 12);

    }

    [Header("Shooting setup - MUSTDO")]
    public GameObject restBulletSpawnPlace;
    public GameObject aimBulletSpawnPlace;
    [Tooltip("Bullet prefab that this waepon will shoot.")]
    public GameObject bullet;
    [Tooltip("Rounds per second if weapon is set to automatic rafal.")]
    public float roundsPerSecond;
    private float waitTillNextFire;

    private bool isShooting = false;
    /*
	 * Checking if the gun is automatic or nonautomatic and accordingly runs the ShootMethod();.
	 */
    void Shooting()
    {
        if(!handsAnimator.GetBool("weaponBlocked") && !handsAnimator.GetBool("sceneInteraction"))
        {
            if (currentStyle == GunStyles.nonautomatic)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    ShootMethod();
                }
            }
            if (currentStyle == GunStyles.automatic && !isReloadingAmmo)
            {
                if (Input.GetButton("Fire1"))
                {
                    if (!isShooting)
                    {
                        isShooting = true;
                        StartCoroutine("StartAutoShooting");
                    }

                    ShootMethod();
                }
                else if (Input.GetButtonUp("Fire1"))
                {
                    EndShootMethod();
                    isShooting = false;
                }
            }
            waitTillNextFire -= roundsPerSecond * Time.deltaTime;
        }
    }


    [HideInInspector] public float recoilAmount_z = 0.5f;
    [HideInInspector] public float recoilAmount_x = 0.5f;
    [HideInInspector] public float recoilAmount_y = 0.5f;
    [Header("Recoil Not Aiming")]
    [Tooltip("Recoil amount on that AXIS while NOT aiming")]
    public float recoilAmount_z_non = 0.5f;
    [Tooltip("Recoil amount on that AXIS while NOT aiming")]
    public float recoilAmount_x_non = 0.5f;
    [Tooltip("Recoil amount on that AXIS while NOT aiming")]
    public float recoilAmount_y_non = 0.5f;
    [Header("Recoil Aiming")]
    [Tooltip("Recoil amount on that AXIS while aiming")]
    public float recoilAmount_z_ = 0.5f;
    [Tooltip("Recoil amount on that AXIS while aiming")]
    public float recoilAmount_x_ = 0.5f;
    [Tooltip("Recoil amount on that AXIS while aiming")]
    public float recoilAmount_y_ = 0.5f;
    [HideInInspector] public float velocity_z_recoil, velocity_x_recoil, velocity_y_recoil;
    [Header("")]
    [Tooltip("The time that takes weapon to get back on its original axis after recoil.(The smaller number the faster it gets back to original position)")]
    public float recoilOverTime_z = 0.5f;
    [Tooltip("The time that takes weapon to get back on its original axis after recoil.(The smaller number the faster it gets back to original position)")]
    public float recoilOverTime_x = 0.5f;
    [Tooltip("The time that takes weapon to get back on its original axis after recoil.(The smaller number the faster it gets back to original position)")]
    public float recoilOverTime_y = 0.5f;

    [Header("Gun Precision")]
    [Tooltip("Gun rate precision when player is not aiming. THis is calculated with recoil.")]
    public float gunPrecision_notAiming = 200.0f;
    [Tooltip("Gun rate precision when player is aiming. THis is calculated with recoil.")]
    public float gunPrecision_aiming = 100.0f;
    [Tooltip("FOV of first camera when NOT aiming(ONLY SECOND CAMERA RENDERS WEAPONS")]
    public float cameraZoomRatio_notAiming = 60;
    [Tooltip("FOV of first camera when aiming(ONLY SECOND CAMERA RENDERS WEAPONS")]
    public float cameraZoomRatio_aiming = 40;
    [Tooltip("FOV of second camera when NOT aiming(ONLY SECOND CAMERA RENDERS WEAPONS")]
    public float secondCameraZoomRatio_notAiming = 60;
    [Tooltip("FOV of second camera when aiming(ONLY SECOND CAMERA RENDERS WEAPONS")]
    public float secondCameraZoomRatio_aiming = 40;
    [HideInInspector]
    public float gunPrecision;

    [Tooltip("Audios for shootingSound, reloading, pick-up")]
    public AudioSource shoot_sound_source, reloadSound_source, reloadWithTriggerSound_source, pickup_source;
    public SoundEffect shooting_sound_effect;
    [Tooltip("Sound that plays after successful attack bullet hit.")]
    public static AudioSource hitMarker;

    /*
	* Sounds that is called upon hitting the target.
	*/
    public static void HitMarkerSound()
    {
        hitMarker.Play();
    }

    [Tooltip("Array of muzzel flashes, randmly one will appear after each bullet.")]
    public GameObject[] muzzelFlash;
    [Tooltip("Place on the gun where muzzel flash will appear.")]
    public GameObject muzzelSpawn;
    private GameObject holdFlash;
    private GameObject holdSmoke;
    /*
	 * Called from Shooting();
	 * Creates bullets and muzzle flashes and calls for Recoil.
	 */
    private void ShootMethod()
    {
        if(is_usable)
        {
            if (waitTillNextFire <= 0 && !reloading && pmS.maxSpeed < 5)
            {
                if (bulletsInTheGun > 0)
                {
                    StartCoroutine("Shoot_Animation");
                }

                else
                {
                    //if(!aiming)
                    StartCoroutine("Reload_Animation");
                    //if(emptyClip_sound_source)
                    //	emptyClip_sound_source.Play();
                }

            }
        }

    }

    private void EndShootMethod()
    {
        handsAnimator.Play(endShootingAnimationName);
    }

    IEnumerator StartAutoShooting()
    {
        handsAnimator.Play(startShootingAnimationName);
        yield return new WaitForSeconds(startShootingTime);
    }

    IEnumerator Shoot_Animation()
    {
        handsAnimator.Play(shootAnimationName);
        if (currentStyle == GunStyles.nonautomatic)
        {
            handsAnimator.Play(shootAnimationName);
            yield return new WaitForSeconds(shells_shoot_time);
            SpawnShell();
            yield return new WaitForSeconds(shootAnimationTime - shells_shoot_time);
        }
        else
        {
            handsAnimator.Play(shootAnimationName);
            //yield return new WaitForSeconds(shells_shoot_time);
            SpawnShell();
        }

        int randomNumberForMuzzelFlash = Random.Range(0, muzzelFlash.Length);
        if (bullet)
        {
            bullet.GetComponent<BulletScript>().impactForce = bullet_impact_force;
            bullet.GetComponent<BulletScript>().damage = damage_force;

            GameObject bulletSpawnPlace = aiming ? aimBulletSpawnPlace : restBulletSpawnPlace;

            Quaternion bullet_rotation = bulletSpawnPlace.transform.rotation;
            //bullet_rotation.ro
            GameObject new_bullet = Instantiate(bullet, bulletSpawnPlace.transform.position, bullet_rotation);
            //new_bullet.transform.parent = bulletSpawnPlace.transform;
        }
        else
            print("Missing the bullet prefab");
        holdFlash = Instantiate(muzzelFlash[randomNumberForMuzzelFlash], muzzelSpawn.transform.position /*- muzzelPosition*/, muzzelSpawn.transform.rotation * Quaternion.Euler(0, 0, 90)) as GameObject;
        holdFlash.transform.parent = muzzelSpawn.transform;

        //sound effect
        if(shooting_sound_effect)
        {
            GameObject new_sound_source = Instantiate(shooting_sound_effect.gameObject, transform);
            new_sound_source.GetComponent<SoundEffect>().PlayEffect();
        }
        else
        {
            if (shoot_sound_source)
            {
                shoot_sound_source.Play();
            }
            else
            {
                print("Missing 'Shoot Sound Source'.");
            }
        }

        RecoilMath();

        waitTillNextFire = 1;
        bulletsInTheGun -= 1;
    }

    private void SpawnShell()
    {
        GameObject new_shell = shells_pool.getNext();
        if(new_shell != null)
        {
            new_shell.transform.position = shells_spawn_point.transform.position;
            new_shell.transform.GetComponent<Rigidbody>().AddForce(transform.right * shells_push_force);
            new_shell.transform.GetComponent<Rigidbody>().AddForce(transform.up * shells_push_force);

            //change shell layer
            int new_layer = LayerMask.NameToLayer(shells_layer);
            new_shell.layer = new_layer;
            Transform[] parts = new_shell.GetComponentsInChildren<Transform>();
            foreach(Transform part in parts)
            {
                part.gameObject.layer = new_layer;
            }
        }
    }

    private void spawnMagazine()
    {
        GameObject new_magazine = magazines_pool.getNext();
        if (new_magazine != null)
        {
            new_magazine.transform.position = magazines_spawn_point.transform.position;
            new_magazine.transform.Rotate(new Vector3(90, 90, 0));

            //change shell layer
            int new_layer = LayerMask.NameToLayer(magazine_layer);
            new_magazine.layer = new_layer;
            Transform[] parts = new_magazine.GetComponentsInChildren<Transform>();
            foreach (Transform part in parts)
            {
                part.gameObject.layer = new_layer;
            }
        }
    }

    public void Hide()
    {
        this.handsAnimator.SetBool("changingWeapon", true);
    }



    /*
	* Reloading, setting the reloading to animator,
	* Waiting for 2 seconds and then seeting the reloaded clip.
	*/
    [Header("reload time after anima")]
    [Tooltip("Time that passes after reloading. Depends on your reload animation length, because reloading can be interrupted via meele attack or running. So any action before this finishes will interrupt reloading.")]
    public float reloadChangeBulletsTime;
    private bool isReloadingAmmo = false;
    IEnumerator Reload_Animation()
    {
        float available_bullets = player.GetComponent<GunInventory>().getAmmoCount(ammo_type);
        
        if (available_bullets > 0 && bulletsInTheGun < amountOfBulletsPerLoad && !reloading/* && !aiming*/ && !isReloadingAmmo)
        {
            isReloadingAmmo = true;
            bool reloading_without_trigger = bulletsInTheGun > 0;

            //sound
            if (!reloading_without_trigger && reloadWithTriggerSound_source != null && reloadWithTriggerSound_source.isPlaying == false) reloadWithTriggerSound_source.Play();
            else if(reloading_without_trigger && reloadSound_source != null && reloadSound_source.isPlaying == false) reloadSound_source.Play();

            //start animation
            handsAnimator.SetBool(reloading_without_trigger ? "reloading" : "reloading_with_trigger", true);
            yield return new WaitForSeconds(0.5f);
            handsAnimator.SetBool(reloading_without_trigger ? "reloading" : "reloading_with_trigger", false);

            //spawn magazine
            yield return new WaitForSeconds(magazines_spawn_time - 0.5f);
            this.spawnMagazine();


            yield return new WaitForSeconds(reloadChangeBulletsTime - magazines_spawn_time  - 0.5f);//minus ovo vrijeme cekanja na yield

            float initial_bullets_in_the_gun = bulletsInTheGun;
            if (/*meeleAttack == false && */pmS.maxSpeed != runningSpeed)
            {

                if (available_bullets - amountOfBulletsPerLoad >= 0)
                {
                    available_bullets -= amountOfBulletsPerLoad - bulletsInTheGun;
                    bulletsInTheGun = amountOfBulletsPerLoad;
                }
                else if (available_bullets - amountOfBulletsPerLoad < 0)
                {
                    float valueForBoth = amountOfBulletsPerLoad - bulletsInTheGun;
                    if (available_bullets - valueForBoth < 0)
                    {
                        bulletsInTheGun += available_bullets;
                        available_bullets = 0;
                    }
                    else
                    {
                        available_bullets -= valueForBoth;
                        bulletsInTheGun += valueForBoth;
                    }
                }
            }
            /*else
            {
                reloadSound_source.Stop();

                print("Reload interrupted via meele attack");
            }*/

            player.GetComponent<GunInventory>().setAmmoCount(ammo_type, (int)available_bullets);
            isReloadingAmmo = false;


        }
    }

    /*
	 * Setting the number of bullets to the hud UI gameobject if there is one.
	 * And drawing CrossHair from here.
	 */
    void OnGUI()
    {
        GunInventory player_gun_inventory = player.GetComponent<GunInventory>();
        if (mls && player_gun_inventory)
        {
            //amountOfBulletsPerLoad.ToString() && bulletsIHave.ToString()
            float available_bullets = player_gun_inventory.getAmmoCount(ammo_type);
            player_gun_inventory.gunAmmoText.text = bulletsInTheGun.ToString() + "/" + amountOfBulletsPerLoad.ToString();
            player_gun_inventory.totalAmmoText.text = available_bullets.ToString() + "   " + "30"; //1
        }


        //DrawCrosshair();
    }

    [Header("Crosshair properties")]
    public Texture horizontal_crosshair, vertical_crosshair;
    public Vector2 top_pos_crosshair, bottom_pos_crosshair, left_pos_crosshair, right_pos_crosshair;
    public Vector2 size_crosshair_vertical = new Vector2(1, 1), size_crosshair_horizontal = new Vector2(1, 1);
    [HideInInspector]
    public Vector2 expandValues_crosshair;
    private float fadeout_value = 1;
    /*
	 * Drawing the crossHair.
	 */
    void DrawCrosshair()
    {
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, fadeout_value);
        if (Input.GetAxis("Fire2") == 0)
        {//if not aiming draw
            GUI.DrawTexture(new Rect(vec2(left_pos_crosshair).x + position_x(-expandValues_crosshair.x) + Screen.width / 2, Screen.height / 2 + vec2(left_pos_crosshair).y, vec2(size_crosshair_horizontal).x, vec2(size_crosshair_horizontal).y), vertical_crosshair);//left
            GUI.DrawTexture(new Rect(vec2(right_pos_crosshair).x + position_x(expandValues_crosshair.x) + Screen.width / 2, Screen.height / 2 + vec2(right_pos_crosshair).y, vec2(size_crosshair_horizontal).x, vec2(size_crosshair_horizontal).y), vertical_crosshair);//right

            GUI.DrawTexture(new Rect(vec2(top_pos_crosshair).x + Screen.width / 2, Screen.height / 2 + vec2(top_pos_crosshair).y + position_y(-expandValues_crosshair.y), vec2(size_crosshair_vertical).x, vec2(size_crosshair_vertical).y), horizontal_crosshair);//top
            GUI.DrawTexture(new Rect(vec2(bottom_pos_crosshair).x + Screen.width / 2, Screen.height / 2 + vec2(bottom_pos_crosshair).y + position_y(expandValues_crosshair.y), vec2(size_crosshair_vertical).x, vec2(size_crosshair_vertical).y), horizontal_crosshair);//bottom
        }

    }

    //#####		RETURN THE SIZE AND POSITION for GUI images ##################
    private float position_x(float var)
    {
        return Screen.width * var / 100;
    }
    private float position_y(float var)
    {
        return Screen.height * var / 100;
    }
    private float size_x(float var)
    {
        return Screen.width * var / 100;
    }
    private float size_y(float var)
    {
        return Screen.height * var / 100;
    }
    private Vector2 vec2(Vector2 _vec2)
    {
        return new Vector2(Screen.width * _vec2.x / 100, Screen.height * _vec2.y / 100);
    }
    //#

    public Animator handsAnimator;
    /*
	* Fetching if any current animation is running.
	* Setting the reload animation upon pressing R.
	*/
    void Animations()
    {

        if (handsAnimator)
        {
            reloading = handsAnimator.GetCurrentAnimatorStateInfo(0).IsName(bulletsInTheGun > 0 ? reloadAnimationName : reloadWithTriggerAnimationName);

            handsAnimator.SetFloat("walkSpeed", pmS.currentSpeed);
            handsAnimator.SetBool("aiming", Input.GetButton("Fire2"));
            handsAnimator.SetInteger("maxSpeed", pmS.maxSpeed);
            if (Input.GetKeyDown(KeyCode.R) && pmS.maxSpeed < 5 && !isReloadingAmmo && !meeleAttack && !handsAnimator.GetBool("sceneInteraction")/* && !aiming*/)
            {
                StartCoroutine("Reload_Animation");
            }
        }

    }

    [Header("Animation names")]
    public string reloadAnimationName = "Player_Reload";
    public string reloadWithTriggerAnimationName = "Player_Reload";
    //public string reloadWithoutTiggerAnimationName = "Player_Reload";
    public string aimingAnimationName = "Player_AImpose";
    public string meeleAnimationName = "Character_Malee";
    public string shootAnimationName = "Player_Shoot";
    public float shootAnimationTime = 0.25f;
    public float takeOutAnimationTime = 1f;
    public float takeDownAnimationTime = 1f;
    public float reloadAnimationTime = 1f;

    [Header("Auto-guns animations")]
    public string startShootingAnimationName = "Player_Shoot";
    public string endShootingAnimationName = "Player_Shoot";
    public float startShootingTime = 0.25f;

    
    private bool is_usable = true;

    public void SetUsable(bool usable)
    {
        is_usable = usable;
    }
}