using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum MenuStyle
{
    horizontal, vertical
}

public class GunInventory : MonoBehaviour
{
    [Tooltip("Current weapon gameObject.")]
    public GameObject currentGun;
    public GameObject primaryGun;
    public GameObject secondaryGun;
    public bool isPrimaryGun = true;
    private Animator currentHAndsAnimator;
    private int currentGunCounter = 0;

    [Tooltip("Put Strings of weapon objects from Resources Folder.")]
    public List<string> gunsIHave = new List<string>();
    [Tooltip("Icons from weapons.(Fetched when you run the game)*MUST HAVE ICONS WITH CORRESPONDING NAMES IN RESOUCES FOLDER*")]
    public Texture[] icons;
    [Tooltip("Name of ammo type and amount of the ammo")]
    public List<string> ammo_names = new List<string>();
    //public List<int> ammo_count = new List<int>();

    [HideInInspector]
    public float switchWeaponCooldown;

    [Header("Ammo ui")]
    public Text gunAmmoText;
    public Text totalAmmoText;

    public int primary_gun_ammo = 0;
    public int secondary_gun_ammo = 0;
    [Space]

    [Header("Other")]
    [SerializeField] private RaycastElement raycast;
    [SerializeField] private List<string> avoid_tags_on_block_check;

    /*
	 * Calling the method that will update the icons of our guns if we carry any upon start.
	 * Also will spawn a weapon upon start.
	 */
    void Awake()
    {
        StartCoroutine("UpdateIconsFromResources");

        StartCoroutine("SpawnWeaponUponStart");//to start with a gun

        /*if (gunsIHave.Count == 0)
			print ("No guns in the inventory");*/

        if (primaryGun != null) primary_gun_ammo = (int) primaryGun.GetComponent<GunScript>().bulletsInTheGun;
        if (secondaryGun != null) secondary_gun_ammo = (int)secondaryGun.GetComponent<GunScript>().bulletsInTheGun;
    }

    /*
	*Waits some time then calls for a waepon spawn
	*/
    IEnumerator SpawnWeaponUponStart()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine("Spawn", 0);
    }

    /* 
	 * Calculation switchWeaponCoolDown so it does not allow us to change weapons millions of times per second,
	 * and at some point we will change the switchWeaponCoolDown to a negative value so we have to wait until it
	 * overcomes 0.0f. 
	 */
    void Update()
    {
        this.CheckWeaponBlock();
       /* if (Input.GetKeyDown(KeyCode.P))
        {
            Time.timeScale = 0f;
        }*/

        switchWeaponCooldown += 1 * Time.deltaTime;
        if (switchWeaponCooldown > 1.2f && Input.GetKey(KeyCode.LeftShift) == false)
        {
            Create_Weapon();
        }

    }

    public int getAmmoCount(string ammo_type_name)
    {
        Inventory player_inventory = GetComponent<Inventory>();
        return player_inventory.getGunAmmo(ammo_type_name);
    }

    public void setAmmoCount(string ammo_type_name, int value)
    {
        Inventory player_inventory = GetComponent<Inventory>();
        player_inventory.setGunAmmo(ammo_type_name, value);
    }

    public Animator getCurrentHandsAnimator()
    {
        return currentHAndsAnimator;
    }

    public GameObject getCurrentGun()
    {
        return currentGun;
    }

    private void CheckWeaponBlock()
    {
        if (currentGun != null)
        {
            bool gun_collides = false;

            GunScript gun = currentGun.GetComponent<GunScript>();
            RaycastHit hit = raycast.getRayCastElement();

            if (hit.transform != null && !avoid_tags_on_block_check.Contains(hit.transform.gameObject.transform.tag))
            {
                gun_collides = hit.distance <= gun.min_obstacle_distance;
                //Debug.Log(hit.transform.gameObject.transform.name + "; Distance: " + Vector3.Distance(transform.position, hit.transform.gameObject.transform.position));
                //Debug.Log("Raycast distance: " + hit.distance);
            }

            gun.SetBlockedStatus(gun_collides);
        }
    }

    /*
	 * Grabing the icons from the Resources/Weapo_Icons/ -> gun name of the image.
	 * (!!!!!!!1!READ IMPORTANT) 
	 * the weapon image to respond the weapon must have the same name as the WEAPON  with the extension _img.
	 * So if the gun prefab is called "Sniper_Piper" the corresponding image must be located in the location form previous,
	 * with the name "Sniper_Piper_img".
	 */
    IEnumerator UpdateIconsFromResources()
    {
        yield return new WaitForEndOfFrame();

        /*icons = new Texture[gunsIHave.Count];
		for(int i = 0; i < gunsIHave.Count; i++){
			icons[i] = (Texture) Resources.Load("Weap_Icons/" + gunsIHave[i].ToString() + "_img");
		}*/

    }

    /*
	 * If used scroll mousewheel or arrows up and down the player will change weapon.
	 * GunPlaceSpawner is child of Player gameObject, where the gun is going to spawn and transition to our
	 * gun properties value.
	 */
    void Create_Weapon()
    {

        /*
		 * Scrolling wheel weapons changing
		 */
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            switchWeaponCooldown = 0;

            /*currentGunCounter++;
			if(currentGunCounter > gunsIHave.Count-1){
				currentGunCounter = 0;
			}*/

            isPrimaryGun = !isPrimaryGun;

            StartCoroutine("Spawn", currentGunCounter);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            switchWeaponCooldown = 0;

            /*currentGunCounter--;
			if(currentGunCounter < 0){
				currentGunCounter = gunsIHave.Count-1;
			}*/

            isPrimaryGun = !isPrimaryGun;
            StartCoroutine("Spawn", currentGunCounter);
        }

        /*
		 * Keypad numbers
		 */
        if (Input.GetKeyDown(KeyCode.Alpha1) && !isPrimaryGun)
        {
            /*switchWeaponCooldown = 0;
			currentGunCounter = 0;*/

            isPrimaryGun = true;

            StartCoroutine("Spawn", currentGunCounter);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && isPrimaryGun)
        {
            /*switchWeaponCooldown = 0;
			currentGunCounter = 1;*/

            isPrimaryGun = false;

            StartCoroutine("Spawn", currentGunCounter);
        }

    }

    /*
	 * This method is called from Create_Weapon() upon pressing arrow up/down or scrolling the mouse wheel,
	 * It will check if we carry a gun and destroy it, and its then going to load a gun prefab
	 */
    IEnumerator Spawn(int _redniBroj = 0)
    {
        if (currentGun)
        {
            currentHAndsAnimator.SetBool("changingWeapon", true);
            yield return new WaitForSeconds(currentGun.GetComponent<GunScript>().takeDownAnimationTime);

            //save current gun magazine state for future usage
            int current_gun_ammo = (int)currentGun.GetComponent<GunScript>().bulletsInTheGun;
            if (isPrimaryGun) secondary_gun_ammo = current_gun_ammo;
            else primary_gun_ammo = current_gun_ammo;

            Destroy(currentGun);

            //GameObject resource = (GameObject) Resources.Load(gunsIHave[_redniBroj].ToString());

            GameObject new_gun = isPrimaryGun ? primaryGun : secondaryGun;
            currentGun = (GameObject)Instantiate(new_gun, transform.position, /*gameObject.transform.rotation*/Quaternion.identity);
            AssignHandsAnimator(currentGun);

            //}
            //else if(currentGun.name.Contains("Sword")){
            //	currentHAndsAnimator.SetBool("changingWeapon", true);
            //	yield return new WaitForSeconds(0.25f);//0.5f

            //	currentHAndsAnimator.SetBool("changingWeapon", false);

            //	yield return new WaitForSeconds(0.6f);//1
            //	Destroy(currentGun);

            //	GameObject resource = (GameObject) Resources.Load(gunsIHave[_redniBroj].ToString());
            //	currentGun = (GameObject) Instantiate(resource, transform.position, /*gameObject.transform.rotation*/Quaternion.identity);
            //	AssignHandsAnimator(currentGun);
            //}
        }
        else
        {
            GameObject resource = isPrimaryGun ? primaryGun : secondaryGun;
            currentGun = (GameObject)Instantiate(resource, transform.position, /*gameObject.transform.rotation*/Quaternion.identity);

            AssignHandsAnimator(currentGun);
        }

        currentGun.GetComponent<GunScript>().bulletsInTheGun = isPrimaryGun ? primary_gun_ammo : secondary_gun_ammo;
    }


    /*
	* Assigns Animator to the script so we can use it in other scripts of a current gun.
	*/
    void AssignHandsAnimator(GameObject _currentGun)
    {
        //if(_currentGun.name.Contains("Gun")){
        currentHAndsAnimator = currentGun.GetComponent<GunScript>().handsAnimator;
        //}
    }

    /*
	 * Unity buil-in method to draw GUI.
	 * From here I am listing thourhg guns I have and drawing corresponding images on the sceen.
	 */
    void OnGUI()
    {

        if (currentGun)
        {
            /*for(int i = 0; i < gunsIHave.Count; i++){
				DrawCorrespondingImage(i);
			}*/
        }

    }

    [Header("GUI Gun preview variables")]
    [Tooltip("Weapon icons style to pick.")]
    public MenuStyle menuStyle = MenuStyle.horizontal;
    [Tooltip("Spacing between icons.")]
    public int spacing = 10;
    [Tooltip("Begin position in percetanges of screen.")]
    public Vector2 beginPosition;
    [Tooltip("Size of icon in percetanges of screen.")]
    public Vector2 size;
    /*
	 * Passing the image number and gun list have the same sort,
	 * so it will fitthe gun image to our current gun or guns we have.
	 * The curent gun selected image has their image slightly enlared for some value.
	 */
    void DrawCorrespondingImage(int _number)
    {

        string deleteCloneFromName = currentGun.name.Substring(0, currentGun.name.Length - 7);

        if (menuStyle == MenuStyle.horizontal)
        {
            if (deleteCloneFromName == gunsIHave[_number])
            {
                GUI.DrawTexture(new Rect(vec2(beginPosition).x + (_number * position_x(spacing)), vec2(beginPosition).y,//position variables
                    vec2(size).x, vec2(size).y),//size
                    icons[_number]);
            }
            else
            {
                GUI.DrawTexture(new Rect(vec2(beginPosition).x + (_number * position_x(spacing) + 10), vec2(beginPosition).y + 10,//position variables
                    vec2(size).x - 20, vec2(size).y - 20),//size
                    icons[_number]);
            }
        }
        else if (menuStyle == MenuStyle.vertical)
        {
            if (deleteCloneFromName == gunsIHave[_number])
            {
                GUI.DrawTexture(new Rect(vec2(beginPosition).x, vec2(beginPosition).y + (_number * position_y(spacing)),//position variables
                    vec2(size).x, vec2(size).y),//size
                    icons[_number]);
            }
            else
            {
                GUI.DrawTexture(new Rect(vec2(beginPosition).x, vec2(beginPosition).y + 10 + (_number * position_y(spacing)),//position variables
                    vec2(size).x - 20, vec2(size).y - 20),//size
                    icons[_number]);
            }
        }



    }

    /*
	 * Call this method when player dies.
	 */
    public void DeadMethod()
    {
        Destroy(currentGun);
        Destroy(this);
    }


    //#####		RETURN THE SIZE AND POSITION for GUI images
    //(we pass in the percentage and it returns some number to appear in that percentage on the sceen) ##################
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
    //######################################################

    /*
	 * Sounds
	 */
    [Header("Sounds")]
    [Tooltip("Sound of weapon changing.")]
    public AudioSource weaponChanging;


    public float TemporarlyHideGun(float hide_time)
    {
        float weapon_hiding_time = this.getCurrentGun().GetComponent<GunScript>().takeDownAnimationTime;
        StartCoroutine(TemporarlyHideCurrentGun(hide_time));

        return weapon_hiding_time;
    }

    public IEnumerator TemporarlyHideCurrentGun(float hide_time)
    {
        this.getCurrentHandsAnimator().SetBool("changingWeapon", true);
        yield return new WaitForSeconds(hide_time);

        yield return new WaitForSeconds(hide_time);

        this.getCurrentHandsAnimator().SetBool("changingWeapon", false);
        this.getCurrentHandsAnimator().SetBool("pickingUp", true);
        yield return new WaitForSeconds(this.getCurrentGun().GetComponent<GunScript>().takeOutAnimationTime);
        this.getCurrentHandsAnimator().SetBool("pickingUp", false);
    }

    public int[] GetGunsAmmoState()
    {
        int[] result = new int[] { 0, 0 };
        
        int current_ammo = currentGun != null 
            ? (int) currentGun.GetComponent<GunScript>().bulletsInTheGun 
            : isPrimaryGun ? primary_gun_ammo : secondary_gun_ammo; // if gun is not equiped yet, return current gun ammo

        //first element - primary gun ammo, second element - secondary gun ammo
        result = isPrimaryGun
            ? new int[] { current_ammo, secondary_gun_ammo }
            : new int[] { primary_gun_ammo, current_ammo };

        return result;
    }

    public void SetGunsAmmoState(int primary_ammo, int secondary_ammo)
    {
        primary_gun_ammo = primary_ammo;
        secondary_gun_ammo = secondary_ammo;

        if(currentGun != null)
        {
            currentGun.GetComponent<GunScript>().bulletsInTheGun = isPrimaryGun ? primary_gun_ammo : secondary_gun_ammo;
        }
    }

    public void ToggleGunUsable(bool usable)
    {
        if (currentGun != null)
        {
            currentGun.GetComponent<GunScript>().SetUsable(usable);
        }
    }
}