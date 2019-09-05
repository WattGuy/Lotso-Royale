using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private LeftJoystick leftJoystick;
    private RightJoystick rightJoystick;
    public float moveSpeed = 0.5f;
    private Vector3 leftJoystickInput;
    private Vector3 rightJoystickInput;
    private Vector3 position;
    private float angle = 0f;
    private System.Random r = new System.Random();
    public Gun gun = Gun.ARMS;
    public int bullets = 0;
    public GameObject bullet;
    public static Items items = null;
    public ushort health = 100;
    public ushort helmet = 0;

    private GameObject selected;
    private GameObject selectedHelmet;
    private string l = null;
    public Detector d = null;
    public bool controllable = true;
    private Vector3 v;
    private Transform spawn = null;

    public Vector3 lastPosition;
    public Quaternion lastRotation;

    public Vector3? toPosition = null;
    public Quaternion? toRotation = null;

    private float moveDistance = 0.05f;

    private GameObject leftArm;
    private GameObject rightArm;

    //public Client client;
    public int id;

    public GameObject blood;
    public GameObject cartridge;

    public void Start() {
        health = 100;
        System.Random r = new System.Random();

        if (controllable)
        {
            GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
            cam.transform.position = this.transform.position;
            cam.GetComponent<CameraFollow>().target = transform;
        }

        leftJoystick = GameObject.FindGameObjectWithTag("LeftJoystick").GetComponent<LeftJoystick>();
        rightJoystick = GameObject.FindGameObjectWithTag("RightJoystick").GetComponent<RightJoystick>();

        if (items == null)
        {

            items = new Items();

        }

        SetGun(gun);

        foreach (Transform t in transform) {

            if (t.name == "Trigger") {

                d = t.GetComponent<Detector>();

            }

        }

        leftArm = getChild(selected.transform, "Left");
        rightArm = getChild(selected.transform, "Right");

        if (controllable) StartCoroutine("ticker");
    }

    void Awake() {
        if (!controllable) return;

        lastPosition = transform.position;
        lastRotation = transform.rotation;

    }

    public void setHealth(ushort s) {

        health = s;
        if (!controllable) return;
        updateHealth();

    }

    private void updateHealth()
    {
        float f = health / 100f;
        if (f > 1f) f = 1f;

        GameObject.FindGameObjectWithTag("HealthBar").GetComponent<Image>().fillAmount = f;

    }

    public void SetHelmet(ushort h)
    {
        if (h > 2) return;
        Transform last = GetObject(this.helmet);
        if (last != null) last.gameObject.SetActive(false);

        helmet = h;

        Transform n = GetObject(this.helmet);
        if (n != null)
        {
            selectedHelmet = n.gameObject;
            selectedHelmet.SetActive(true);
        }

    }

    private Transform GetObject(ushort h)
    {

        string s = "";
        if (h == 1)
        {

            s = "Helmet1";

        }
        else if (h == 2)
        {

            s = "Helmet2";

        }

        if (s == "") return null;

        foreach (Transform t in transform)
        {

            if (t.name == s)
            {

                return t;

            }

        }

        return null;

    }

    public void attack(GameObject go) {
        if (!controllable) return;

        if (go == null) return;
        Destructible dest = go.GetComponent<Destructible>();
        if (dest != null) return;

        //client.Send(new DestructibleShotMessage((ushort) dest.id), 0, ENet.PacketFlags.Reliable);

    }

    public void AddItem(Item i) {
        Gun? g = IsItGun(i);

        if (g != null)
        {

            SetGun((Gun)g);
            return;

        }

        Helmet? h = IsItHelmet(i);
        if (h != null)
        {

            SetHelmet(ushort.Parse(h.ToString().Replace("HELMET", "")));
            return;

        }

        if (i == Item.BULLETS) {

            bullets += 20;

        }

    }

    public Helmet? IsItHelmet(Item i)
    {

        foreach (Helmet h in Enum.GetValues(typeof(Helmet)).Cast<Helmet>())
        {

            if (h.ToString() == i.ToString())
            {

                return h;

            }

        }

        return null;

    }

    Gun? IsItGun(Item i)
    {

        foreach (Gun g in Enum.GetValues(typeof(Gun)).Cast<Gun>())
        {

            if (g.ToString() == i.ToString())
            {

                return g;

            }

        }

        return null;

    }

    private GameObject getChild(Transform t, string name) {

        foreach (Transform mb in t) {

            if (mb.name == name) return mb.gameObject;

        }

        return null;

    }

    public void playGun() {
        playGun(false, 0);
    }

    public void playGun(bool bullet, ushort id)
    {
        bool was = false;

        if (gun == Gun.ARMS)
        {
            Animation left = leftArm.GetComponent<Animation>();
            Animation right = rightArm.GetComponent<Animation>();

            if (left == null || right == null) return;

            if (!left.IsPlaying("LeftArm") && !right.IsPlaying("RightArm"))
            {
                int i = r.Next(2);

                if (i == 0)
                {

                    left.Play("LeftArm");
                    was = true;

                }
                else
                {

                    right.Play("RightArm");
                    was = true;

                }

            }

        }
        else if (gun == Gun.M92) {
            Animation a = selected.GetComponent<Animation>();
            if (bullet) SpawnBullet();
            if (a == null || a.IsPlaying("Pistol")) return;

            a.Play();
            was = true;

        } else if (gun == Gun.AK47)
        {
            Animation a = selected.GetComponent<Animation>();
            if (bullet) SpawnBullet();
            if (a == null || a.IsPlaying("AR")) return;

            a.Play();
            was = true;

        }

        if (!controllable) return;
        if (!was) return;

        if (!bullet)
        {
            if (gun != Gun.ARMS) {

                SpawnBullet();
                new PlayerShotPacket(this.id, ObjectType.NONE, 0, transform.rotation.eulerAngles.z + 90f).Write();

            } else {

                if (!(d != null && d.col != null && d.col.gameObject != null)) return;

                PlayerController pt = d.col.GetComponent<PlayerController>();
                Destructible dt = d.col.GetComponent<Destructible>();

                if (pt != null)
                {

                    new PlayerShotPacket(this.id, ObjectType.PLAYER, pt.id, 0).Write();

                }
                else if (dt != null)
                {

                    new PlayerShotPacket(this.id, ObjectType.DESTRUCTIBLE, dt.id, 0).Write();

                }
                else {

                    new PlayerShotPacket(this.id, ObjectType.NONE, 0, 0).Write();

                }

            }
        }

    }

    private void SpawnBullet()
    {
        GameObject.Instantiate(bullet, new Vector3(spawn.position.x, spawn.position.y, spawn.position.z), transform.rotation);
        SpawnCartridge();
    }

    private float time = 0f;

    public float getGunTime() {

        if (gun == Gun.AK47) {

            return 0.3f;

        } else if (gun == Gun.M92)
        {

            return 0.4f;

        }

        return -1f;

    }

    void Update() {

        if (toPosition != null)
        {

            transform.position = Vector3.Lerp(transform.position, (Vector3) toPosition, 0.1f);

        }

        if (toRotation != null) {

            transform.rotation = Quaternion.Lerp(transform.rotation, (Quaternion) toRotation, 0.15f);

        }

    }

    IEnumerator ticker() {

        while (true)
        {

            if ((Vector3.Distance(lastPosition, transform.position) > moveDistance || lastRotation != transform.rotation) && Client.instance.id != null)
            {

                Client.instance.Send(new PlayerMovePacket((int) Client.instance.id, transform.position.x, transform.position.y, transform.rotation.z, transform.rotation.w));

                lastPosition = transform.position;
                lastRotation = transform.rotation;

            }

            yield return new WaitForSeconds((float) 1 / 20);
        }

    }

    void FixedUpdate()
    {
        if (!controllable) return;

        bool shot = false;
        float gunTime = getGunTime();
        if (gunTime > -1f) {

            time += Time.deltaTime;
            if (time >= gunTime)
            {

                time = 0f;
                shot = true;

            }

        } else time = 0f;

        leftJoystickInput = leftJoystick.GetInputDirection();
        rightJoystickInput = rightJoystick.GetInputDirection();

        Vector3 v = new Vector3(leftJoystickInput.x, leftJoystickInput.y, 0f);

        transform.position +=  v * Time.deltaTime * moveSpeed;

        float a;
        if (rightJoystickInput.x == 0 && rightJoystickInput.y == 0)
        {

            a = Mathf.Atan2(leftJoystickInput.x, leftJoystickInput.y) * Mathf.Rad2Deg;

        }
        else {

            a = Mathf.Atan2(rightJoystickInput.x, rightJoystickInput.y) * Mathf.Rad2Deg;

            playGun();

        }

        if (a == 0) a = angle;
        else angle = a;

        if (!(rightJoystickInput.x == 0 && rightJoystickInput.y == 0) && spawn != null && shot && bullets > 0)
        {
            playGun();

            if (bullets == 0 && gun != Gun.ARMS) {

                SetGun(Gun.ARMS);

            }

        }

        toRotation = Quaternion.Euler(new Vector3(0, 0, -a));
        GameObject.FindGameObjectWithTag("BulletsCounter").GetComponent<Text>().text = bullets.ToString();
    }

    public void SetGun(Gun g) {

        Transform last = GetObject(this.gun);
        if (last != null) last.gameObject.SetActive(false);


        gun = g;
        Transform n = GetObject(this.gun);
        if (n != null) {
            selected = n.gameObject;
            selected.SetActive(true);
            SetBulletSpawn();
        }

    }

    public void SpawnCartridge() {
        if (selected == null) return;

        foreach (Transform t in selected.transform)
        {

            if (t.name == "ar" || t.name == "pistol")
            {

                GameObject.Instantiate(cartridge, t.position, transform.rotation);

            }

        }

    }

    private void SetBulletSpawn() {

        foreach (Transform t in selected.transform)
        {

            if (t.name == "BulletSpawn")
            {

                spawn = t;
                return;

            }

        }

        spawn = null;

    }

    private Transform GetObject(Gun g) {

        string s;
        if (g == Gun.M92) {

            s = "Pistol";

        }
        else if (g == Gun.AK47)
        {

            s = "AR";

        }
        else {

            s = "Arms";

        }

        foreach (Transform t in transform) {

            if (t.name == s) {

                return t;

            }

        }

        return null;

    }

}