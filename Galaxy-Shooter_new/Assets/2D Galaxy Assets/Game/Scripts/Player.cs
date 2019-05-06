using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update

    public bool canTripleShot = false;

    public bool isSpeedBoostActive = false;
    public bool shieldsActive = false;
    public int lives = 3;

    [SerializeField]
    private GameObject _explosionPrefab;



    [SerializeField]
    private GameObject _laserPrefab;

    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private GameObject _shieldGameObject;

    [SerializeField]
    private GameObject[] _engine;

    [SerializeField]
    private float _fireRate = 0.25f;
    

    private float _canFire = 0.0f;




    [SerializeField]
    
    private float _speed = 5.0f;

    private UIManager _uiManager;
    private GameManager _gameManager;
    private SpawnManager _spawnManager;

    private AudioSource _audioSource;
    private int hitCount = 0;

    private void Start()
    {
        transform.position = new Vector3(0,0,0);
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if(_uiManager != null)
        {
            _uiManager.UpdateLives(lives);
        }
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if(_spawnManager != null)
        {
            _spawnManager.StartSpawnRoutines();
        }
        _audioSource = GetComponent<AudioSource>();

        hitCount = 0;

           
        
        
    }

    // Update is called once per frame
    private void Update()
    {
        Movement();

        if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            Shoot();                       
        }
        
        
    }
    private void Shoot()
    {

        if(Time.time > _canFire)
        {
            _audioSource.Play();
            if(canTripleShot == true)
            {
                Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);




            }
            else
            {
                Instantiate(_laserPrefab, transform.position + new Vector3(0, 0.88f, 0), Quaternion.identity);
            }
            
            _canFire = Time.time + _fireRate;
                
        }
        

    }

    private void Movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        //if speed boost enabled
        //move 1.5x the normal speed
        //else 
        //move normal speed
        if(isSpeedBoostActive)
        {
            transform.Translate(Vector3.right  * _speed * 2.0f * horizontalInput * Time.deltaTime);
            transform.Translate(Vector3.up  * _speed * 2.0f * verticalInput * Time.deltaTime);
            
        }
        else
        {
            transform.Translate(Vector3.right  * _speed * horizontalInput * Time.deltaTime);
            transform.Translate(Vector3.up  * _speed * verticalInput * Time.deltaTime);

        }


        
        if(transform.position.y > 5)
        {
            transform.position = new Vector3(transform.position.x, 5,0);
        }
        else if(transform.position.y < -4.2f)
        {
            transform.position = new Vector3(transform.position.x, -4.2f,0);
        }
        
        if(transform.position.x < -9.0f)
        {
            transform.position = new Vector3(9.0f, transform.position.y, 0);
        }
        else if(transform.position.x > 9.0f)
        {
            transform.position = new Vector3(-9.0f, transform.position.y, 0);
        }

    }

    public void Damage()
    {
        if(shieldsActive == true)
        {
            shieldsActive =false;
            _shieldGameObject.SetActive(false);
            return;
        }

        hitCount ++;
        if(hitCount == 1)
        {
            //turn left engine_failure on
            _engine[0].SetActive(true);
        }

        else if(hitCount  == 2)
        {
            //turn right engine_failure on
            _engine[1].SetActive(true);

        }

        
        lives--;
        _uiManager.UpdateLives(lives);
        
        if(lives < 1)
        {
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            _gameManager.gameOver = true;
            _uiManager.ShowTitleScreen();
            Destroy(this.gameObject);
        }

    }

    public void TripleShotPowerupOn()
    {
        canTripleShot = true;
        StartCoroutine(TripleShotPowerDownRoutine());

    }

    //method to enable to powerup
    public void SpeedBoostPowerupOn()
    {
        isSpeedBoostActive = true;
        StartCoroutine(SpeedBooseDownRoutine());

    }
    public void EnableShields()
    {
        shieldsActive = true;
        _shieldGameObject.SetActive(true);


    }


    //coroutine method (ienumerator) to powerdown the speed
    public IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        canTripleShot = false;
    }

    public IEnumerator SpeedBooseDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        isSpeedBoostActive = false;

    }
}

