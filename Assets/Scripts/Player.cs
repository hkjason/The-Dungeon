using UnityEngine;

public class Player : Character
{
    ////////////////////////////////
    /// For Input //////////////////
    private float _horizontalSpeed;
    private float _verticalSpeed;
    private float _running;
    private bool _rolling;
    private bool _attacking;
    /// For Input //////////////////
    ////////////////////////////////
    public bool _isRolling = false;
    public bool _isAttacking = false;
    private float _turnSmoothTime = 0.1f;
    private float _turnSmoothVelocity;

    [Header("GameObjects")]
    public CharacterController characterController;

    private void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        animator.SetInteger("WeaponState", (int)currentWeapon.weaponType);
    }

    private void FixedUpdate()
    {
        AnimationControl();
    }

    private void Update()
    {
        InputChecking();
    }

    private void InputChecking()
    {
        if (_isDead)
            return;
        _horizontalSpeed = Input.GetAxis("Horizontal");
        _verticalSpeed = Input.GetAxis("Vertical");
        _running = Input.GetAxis("Run");
        _rolling = Input.GetKeyDown(KeyCode.Space);
        _attacking = Input.GetKeyDown(KeyCode.Mouse0);
    }

    private void AnimationControl()
    {
        Vector3 direction;
        float magnitude;

        animator.SetBool("Roll", _rolling);
        animator.SetBool("Attack", _attacking);

        if (_isRolling)
        {
            direction = new Vector3(0, 0, _verticalSpeed * 0.5f);
            magnitude = Mathf.Abs(_verticalSpeed * 0.5f);
        }
        else if (_isAttacking)
        {
            direction = new Vector3(_horizontalSpeed, 0, _verticalSpeed);
            magnitude = Mathf.Abs(_verticalSpeed * 0.3f);
        }
        else
        {
            direction = new Vector3(_horizontalSpeed, 0, _verticalSpeed);
            magnitude = Mathf.Max(Mathf.Abs(_horizontalSpeed), Mathf.Abs(_verticalSpeed));
        }

        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _turnSmoothTime);
        if (!_isAttacking && !_isDead)
            transform.rotation = Quaternion.Euler(0, angle, 0);

        if (direction.magnitude >= 0.1f && !_isDead)
        {
            Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            characterController.Move(moveDirection.normalized * (GetMovementSpeed() * (magnitude * (1 + _running))) * Time.deltaTime);
        }
        characterController.Move(Physics.gravity * Time.deltaTime);

        if (!_isAttacking)
            animator.SetFloat("Speed", (magnitude + _running));
        else
            animator.SetFloat("Speed", (magnitude * 2));
    }

    public void SetRolling(int i)
    {
        _isRolling = (i == 1);
    }
    public void SetAttacking(int i)
    {
        _isAttacking = (i == 1);
        currentWeapon.SetColliderActive((i == 1));
    }

    protected override void DeadTrigger()
    {
        animator.SetBool("Dead", _isDead);
    }
}