using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public string playerState;
    public TextMeshProUGUI textState;
    public TextMeshProUGUI textVerticalSpeedState;
    public TextMeshProUGUI textHorizontalSpeedState;
	private Rigidbody2D rb;//YOk ARTIIK RIGIDBODY!!! @Han
    private PlayerCombat pCombat;
    private PlayerMovement pMovement;
    //Daha Yumusak Gitmesini Sagliyo bu deger. @Han
    private float _MaxCoyoteTimeValue = 0.25f;
    [SerializeField] private LayerMask _GroundLayers; //Ground Layerlari
    private Transform m_GroundCheck;
    public Transform m_GroundCheck_2;
    public bool _CoyoteTime; //coyote time 
    public float coyoteTimeValue;
    private float fallMultiplier = 1.2f; 
    private float fallFastMultiplier = 0.8f; 
    private float peakMultiplier = 0.5f; 

    #region Dash Values
    
    private bool isDashing;

    #endregion
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pCombat = GetComponent<PlayerCombat>();
        pMovement = GetComponent<PlayerMovement>();
        m_GroundCheck = transform.GetChild(0);
        textState = transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        
    }

    private void Update()
	{
        StateCheck();
        StateController();
        SetGravity();
        textState.text = playerState;
        textVerticalSpeedState.text = rb.velocity.y.ToString();
        //textHorizontalSpeedState.text = rb.velocity.x.ToString();
    }

     public void StateController(){

        if(pCombat.isAttacking){

            playerState = "Attacking";
        }
        else if(pMovement.isDashing){

            playerState = "Dashing";
        }
        else if(IsGrounded())
        {

            Vector2 velocity =  rb.velocity;
            float magnitude = velocity.x;

            if(magnitude > 0.1f || magnitude < -0.1f){
                
                playerState = "Run";
            }
            else if(magnitude < 0.1f && magnitude > -0.1f){

                playerState = "Idle";
            }
        }
        else if(!IsGrounded())
        {

            Vector2 velocity =  rb.velocity;
            float magnitude = velocity.y;

            if(magnitude < -0.1f && IsNearGround()){

                playerState = "NearGroundOnAir";
            }
            else if(magnitude <= 1f && magnitude > 0f){

                pMovement.airSpeed = 9f;
                playerState = "Peak";
            }else if(magnitude <= 0f && magnitude >= -1f){
                pMovement.airSpeed = 9f;
                playerState = "PeakLow";
            }
             else if(magnitude > 1f){

                pMovement.airSpeed = 8f; 
                playerState = "Ascend";
            }else if(magnitude < -1f){

                pMovement.airSpeed = 8f;
                playerState = "Des_Fast";
            }
            else if(magnitude < -3.5f){

                pMovement.airSpeed = 7f;
                playerState = "Descend";
            }       
        }
    }

    public void SetGravity(){
        if(playerState == "Attacking"){
            if(Mathf.Abs(rb.velocity.y) > 10 && (Mathf.Abs(rb.velocity.x) < 4)){
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallFastMultiplier * 8 - 1) * Time.deltaTime;
            }
        }else if(playerState == "AttackingSlow"){
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallFastMultiplier * 2 - 1) * Time.deltaTime;
        }
        else if(playerState == "Peak" || playerState == "PeakLow"){

            rb.velocity += Vector2.up * Physics2D.gravity.y * (peakMultiplier - 1) * Time.deltaTime;

        }
        else if (playerState == "descendFast"){

            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallFastMultiplier - 1) * Time.deltaTime;

        }
        else if(playerState == "Descend"){

            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;

        }
    }
      private void StateCheck(){


        if(IsGrounded()){

            coyoteTimeValue = _MaxCoyoteTimeValue;
        }else{
            coyoteTimeValue -= Time.deltaTime; 
        }
        
        if (coyoteTimeValue > 0f) 
        {
            _CoyoteTime = true;
        }
        else{
            _CoyoteTime = false;
        }
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(m_GroundCheck.position, 0.2f, _GroundLayers);
    }

    public bool IsNearGround()
    {
        // Perform linecast from player position to target position
        RaycastHit2D hit = Physics2D.Linecast(this.transform.position, m_GroundCheck_2.position, _GroundLayers);

        // Check if there was a collision along the line
        if (hit.collider != null)
        {
            return true;
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        // Draw a line from player to target in Scene view
        if (this.transform != null && this.transform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(this.transform.position, m_GroundCheck_2.position);
        }
    }

}
