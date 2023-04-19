using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    [Header("Effects")]
    [SerializeField] GameObject m_RunStopDust;
    [SerializeField] GameObject m_JumpDust;
    [SerializeField] GameObject m_LandingDust;

    [Header("Variables")]
    [SerializeField] float     velocidad       = 5f;
    [SerializeField] float     fuerza_Salto    = 9f;
    [SerializeField] int       saltos_Maximos  = 1;
    
    public LayerMask capaSuelo;
  
    private Rigidbody2D    rigidbody;
    private BoxCollider2D  boxCollider;
    private Animator       animator;
    private AudioSource    m_audioSource;
    private AudioManager   audio_Manager;

    private float               direccion_MovimientoX;
    private float               saltos_Disponibles;
    private int                 m_facingDirection = 1;
    private void Start() {
        rigidbody   = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator    = GetComponent<Animator>();

        audio_Manager      = AudioManager.Instance;
        saltos_Disponibles = saltos_Maximos;
    }
    // Update is called once per frame
    void Update() {
        ProcesarMovimiento();
        ProcesarSalto();
    }

    void ProcesarMovimiento() { 
        direccion_MovimientoX = Input.GetAxis("Horizontal");

        if( direccion_MovimientoX != 0f ) animator.SetBool("Running", true);
        else animator.SetBool("Running", false);
        
        rigidbody.velocity = new Vector2( direccion_MovimientoX * velocidad, rigidbody.velocity.y );
        animator.SetFloat("AirSpeedY", rigidbody.velocity.y);
        GirarOrientacion( direccion_MovimientoX );
    }

    void GirarOrientacion( float direccion_MovimientoX ) {
        if ( direccion_MovimientoX > 0 ) GetComponent<SpriteRenderer>().flipX = false;
        else if ( direccion_MovimientoX < 0) GetComponent<SpriteRenderer>().flipX = true;
    }

    void ProcesarSalto() { 
        if( Suelo() ) {
            saltos_Disponibles = saltos_Maximos;
            animator.SetBool("Grounded", true );
        } else animator.SetBool("Grounded", false);

        if( Input.GetKeyDown( KeyCode.Space ) && saltos_Disponibles > 0 ){
            saltos_Disponibles-- ;
            rigidbody.velocity = new Vector2( rigidbody.velocity.y, 0f );
            rigidbody.AddForce(Vector2.up * fuerza_Salto, ForceMode2D.Impulse);
            
            animator.SetTrigger("Jump");
        }
    }
    bool Suelo() { 
        Debug.DrawRay( transform.position, Vector3.down * 0.1f, Color.white );
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, new Vector2( boxCollider.bounds.size.x, boxCollider.bounds.size.y ), 0f, Vector2.down, 0.1f, capaSuelo );
        return raycastHit.collider != null;
    }

     void AE_Detenerse() {
        audio_Manager.ReproducirSonido("RunStop");
        float dustXOffset = 0.6f;
        SpawnDustEffect(m_RunStopDust, dustXOffset);
    }

    void AE_Caminar() {
        audio_Manager.ReproducirSonido("Footstep");
    }

    void AE_Saltar() {
        audio_Manager.ReproducirSonido("Jump");
        SpawnDustEffect(m_JumpDust);
    }

    void AE_Landing() {
        audio_Manager.ReproducirSonido("Landing");
        SpawnDustEffect(m_LandingDust);
    }

    void SpawnDustEffect(GameObject dust, float dustXOffset = 0)
    {
        if (dust != null)
        {
            // Set dust spawn position
            Vector3 dustSpawnPosition = transform.position + new Vector3(dustXOffset * m_facingDirection, 0.0f, 0.0f);
            GameObject newDust = Instantiate(dust, dustSpawnPosition, Quaternion.identity) as GameObject;
            // Turn dust in correct X direction
            newDust.transform.localScale = newDust.transform.localScale.x * new Vector3(m_facingDirection, 1, 1);
        }
    }
}