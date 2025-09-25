using UnityEditor.UIElements;
using UnityEngine;

public class VoxelPlayerController : MonoBehaviour
{
    // rigidBody
    [Header("rigidBody")]
    public Rigidbody rb;

    // mouvement X, Z
    [Header("mouvement X, Z")]
    public bool oldOrNew;
    public float zMovementInput;
    public float xMovementInput;
    public float playerMaxMovementSpeed;
    public Vector3 directionMovement;

    // Saut et gravite
    [Header("Saut et gravite")]
    public float puissanceSaut;
    public float vitesseDeChuteMax;
    public float vitesseVerticaleActuelle;
    public float gravityAcceleration;
    bool jumped;
    public Vector3 jumpV3;

    // detection du sol
    [Header("detection du sol")]
    public bool spherecastOuBricolage;
    public float sphereCastDistance;
    public float sphereCastRadius;
    public bool onGround;
    public float collisionY;
    bool justHitGround;

    // mouvement camera
    [Header("mouvement camera")]
    public GameObject playerFPSCamera;
    public float mouseSensitivity;
    float cameraVerticalRotation;
    bool lockedCursor = true;

    [Header("Barre d'item")]
    public int currentIndexItemScrollBar;

    // pointeur
    [Header("pointeur")]
    public float pointerHitRange;
    public Vector3 playerPointerDirection; // la direction dans laquelle la camera pointe
    public Vector3 clickHitWorldPosition;
    public Vector3 roundedClickHitWorldPosition;
    public Vector3 realBlockClickedPosition;
    public Vector3 realLeftClickBlockPosition;

    [Header("Chunk")]
    public GameObject testChunk;
    // tirer
    [Header("tirer")]
    public GameObject projectile;
    public GameObject targetManager;

    [Header("Tests et modes")]
    public bool enableTestArrow;
    public GameObject testArrow;
    public bool newPlayerClickOrOld;
    public bool FaceOrientationLog;
    

    // visualiser Vector3
    //public GameObject vecteurPointeur;

    void Start()
    {
        //transform.position += transform.forward * vitesse;
        Cursor.visible = false; // Rendre la sourirs invisible
        Cursor.lockState = CursorLockMode.Locked;  // caller la souris au centre de l'�cran
    }
    void Update()
    {
        //vecteurPointeur.transform.rotation = Quaternion.Euler(directionMovement);
        //PlayerJump();        
        PlayerCameraMovement();
        MouseWheelScroll();
        if (Input.GetMouseButton(1))
        {
            //Shoot();
        }
        //if (Input.GetKeyDown(KeyCode.Space))
        //    {
        ////    vitesseVerticaleActuelle = puissanceSaut;
        //        vitesseVerticaleActuelle = 0;
        //        if (!spherecastOuBricolage)
        //        {
        //            transform.position = new Vector3(transform.position.x, 1, transform.position.z); // giga bricolage partie 1
        //        }
        //     }
        if (!onGround) // bricolage pour simuler la collision du sol
        {
            Gravity();
            justHitGround = false;
        }
        else
        {
            if (!justHitGround)
            {
                justHitGround = true;
                vitesseVerticaleActuelle = 0;
            }
        }
        GroundDetection();
        PlayerJump();
        if (oldOrNew)
        {
            PlayerXZMovement();
        }
        else
        {
            PlayerXZMovementNew();
        }
        Toolbar();

        //rb.position = transform.position + new Vector3(directionMovement.normalized.x * playerMaxMovementSpeed, (transform.up.y * vitesseVerticaleActuelle), directionMovement.normalized.z * playerMaxMovementSpeed) * Time.deltaTime;
        //rb.position += new Vector3(directionMovement.normalized.x * playerMaxMovementSpeed, vitesseVerticaleActuelle, directionMovement.normalized.z * playerMaxMovementSpeed)* Time.deltaTime;
        //Debug.Log(new Vector3(directionMovement.normalized.x * playerMaxMovementSpeed, vitesseVerticaleActuelle, directionMovement.normalized.z * playerMaxMovementSpeed) * Time.deltaTime);

    }

    void FixedUpdate()
    {
        //rb.MovePosition(rb.position + new Vector3(directionMovement.normalized.x * playerMaxMovementSpeed, vitesseVerticaleActuelle, directionMovement.normalized.z * playerMaxMovementSpeed) * Time.fixedDeltaTime);
        rb.AddForce(new Vector3(directionMovement.normalized.x * playerMaxMovementSpeed, vitesseVerticaleActuelle, directionMovement.normalized.z * playerMaxMovementSpeed) * Time.fixedDeltaTime);
    }

    void PlayerXZMovement()
    {
        zMovementInput = Input.GetAxisRaw("Vertical"); // Input.GetAxis() donne un float adoucis entre 1 et -1
        xMovementInput = Input.GetAxisRaw("Horizontal"); // Input.GetAxisRaw() donne 1, 0 ou -1


        directionMovement.z = zMovementInput;
        directionMovement.x = xMovementInput;

        if (zMovementInput != 0)
        {
            directionMovement.z = transform.forward.z * zMovementInput; // transform.forward permet d'aligner un vector3 avec l'axe z du porteur du script
            directionMovement += transform.forward * zMovementInput;
        }
        if (xMovementInput != 0)
        {
            directionMovement.x = transform.right.x * xMovementInput; // transform.right = axe x
            directionMovement += transform.right * xMovementInput;
        }

        if (xMovementInput != 0 || zMovementInput != 0)
        {
            rb.position = transform.position + directionMovement.normalized * playerMaxMovementSpeed * Time.fixedDeltaTime; // /!\/!\/!\/!\/!\/!\  retirer transform.position pour �viter la d�rive angulaire    
            //transform.position += directionMovement.normalized * playerMaxMovementSpeed * Time.fixedDeltaTime; // Vector3.normalized limite le r�sultat � 1 ou 0 pour �viter de bouger plus vite que normal en diagonale

            //rb.MovePosition(rb.position + (directionMovement.normalized * playerMaxMovementSpeed * Time.deltaTime));
        }
    }

    void PlayerXZMovementNew()
    {
        directionMovement = (transform.forward * Input.GetAxisRaw("Vertical")) + (transform.right * Input.GetAxisRaw("Horizontal")); //+ jumpV3; // Input.GetAxis() donne un float adoucis entre 1 et -1
        //directionMovement = Input.GetAxisRaw("Horizontal"); // Input.GetAxisRaw() donne 1, 0 ou -1        

        //rb.position = transform.position + directionMovement.normalized * playerMaxMovementSpeed * Time.fixedDeltaTime;
        //rb.MovePosition(directionMovement.normalized * playerMaxMovementSpeed * Time.fixedDeltaTime);
    }

    void PlayerJump() // on fait juste monter la vitesse verticale et la gravite fait le reste
    {
        jumpV3.y = vitesseVerticaleActuelle;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            vitesseVerticaleActuelle = puissanceSaut;
            //directionMovement += new Vector3(transform.position, vitesseVerticaleActuelle, transform.position);

            //transform.position += transform.up * vitesseVerticaleActuelle * Time.deltaTime; // fait monter la vitesse verticale d'un coup
            //rb.position += transform.up * vitesseVerticaleActuelle * Time.deltaTime;
        }
    }

    void Gravity() // on fait juste baisser la vitesse verticale
    {
        if (vitesseVerticaleActuelle > -vitesseDeChuteMax)
        {
            vitesseVerticaleActuelle -= gravityAcceleration * Time.deltaTime; // la gravit� fait baisser la vitesse verticale � chaque frame
        }
        //transform.position += transform.up * vitesseVerticaleActuelle * Time.deltaTime;
        //rb.position += transform.up * vitesseVerticaleActuelle * Time.deltaTime;
    }

    void GroundDetection() // le sphereCast � un rayon plus petit que le rigidBody pour �viter de confondre les murs comme des sols;
    {
        if (spherecastOuBricolage)
        {
            LayerMask coucheGround = LayerMask.GetMask("Ground"); // on cr�e une variable couche
            RaycastHit gHit; // on d�clare une variable RaycastHit qui permet d'avoir des informations sur se qu'a touch� un Raycast
            if (Physics.SphereCast(transform.position, sphereCastRadius, -transform.up, out gHit, sphereCastDistance, coucheGround)) // le SphereCast ne voit que la couche "Ground" et ignore le reste
            {
                onGround = true;
                collisionY = gHit.transform.position.y; //+ (sphereCastDistance + (sphereCastRadius * 2));
                                                        //if (transform.position.y > collisionY)
                                                        //{
                                                        //transform.position = new Vector3(transform.position.x, collisionY, transform.position.z);
                                                        //}
            }
            else
            {
                onGround = false;
            }
        }
        else
        {
            if (transform.position.y > 1) // giga bricolage partie 2
            {
                onGround = false;
            }
            else
            {
                onGround = true;
            }
        }
    }

    void OnDrawGizmos() // se lance tout seul dans l'�diteur, sert uniquement � voir les trucs invisibles 
    {
        Gizmos.color = Color.green; // la couleur du gizmo
        Gizmos.DrawSphere(transform.position - transform.up * sphereCastDistance, sphereCastRadius); // on cr�e une representation visuelle du spherecast !!! la syntaxe est differente du vrai SphereCast !!!
    }

    void PlayerCameraMovement() // on cr�e une fonction
    {
        float inputX = Input.GetAxis("Mouse X") * mouseSensitivity; // on r�cup�re l'axe x de la souris et on la met dans la variable "inputX"
        float inputY = Input.GetAxis("Mouse Y") * mouseSensitivity; // on r�cup�re l'axe y de la souris et on la met dans la variable "inputY"
        cameraVerticalRotation -= inputY;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f); // on force la rotation verticale � rester entre -90 et 90
        playerFPSCamera.transform.localEulerAngles = Vector3.right * cameraVerticalRotation;
        transform.Rotate(Vector3.up * inputX);
        //rb.MoveRotation(rb.rotation * Vector3.up * inputX);

        playerPointerDirection = playerFPSCamera.transform.eulerAngles; // le pointeur = la rotation de la camera
    }

    void MouseWheelScroll()
    {
        var scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput > 0f)
        {
            if (currentIndexItemScrollBar == 8)
            {
                currentIndexItemScrollBar = 0;
            }
            else
            {
                currentIndexItemScrollBar++;
            }
        }

        if (scrollInput < 0f)
        {
            if (currentIndexItemScrollBar == 0)
            {
                currentIndexItemScrollBar = 8;
            }
            else
            {
                currentIndexItemScrollBar--;
            }
        }
        //Debug.Log(currentIndexItemScrollBar);
    }

    void Toolbar()
    {
        // bricolage
        if (currentIndexItemScrollBar == 0)
        {
            PlayerClickInWorld();
        }
        else
        {
            ShootMode();
        }
    }

    void PlayerClickInWorld()
    {
        // Apr�s la generation du chunk ou on genere un mesh / instantie des cubes de sorte que les sommets soient des ints aligner avec la grille Vector3 du monde
        // V V V V V V V V V V V V V V

        //1// On envoie un raycast qui recupere les coordone Vector3 du clic sur une surface quelconque avec un collider

        //2// Si une des coordon�es x,y,z est un Int, �a veut dire qu'on a bien cliquer sur un bloc aligner avec la grille Vector3 du monde
        // si c'est x qui est un Int sa veut dire qu'on a cliquer sur une face aligner dans l'axe x donc c'est soit la face "Est" ou "Ouest"
        // si c'est y qui est un Int sa veut dire qu'on a cliquer sur une face aligner dans l'axe y donc c'est soit la face "Haut" ou "Bas"
        // si c'est z qui est un Int sa veut dire qu'on a cliquer sur une face aligner dans l'axe z donc c'est soit la face "Nord" ou "Sud"

        //3// on utilise l'orientation du pointeur du joueur pour determiner laquelle des 2 faces est la bonne
        // Comme les blocs ont des angles de 90� qui sont alignes sur avec les axes cardinaux du monde:
        // Si la face est sur l'axe x et que le pointeur est a un angle(euler) x au dessus de 180� alors la bonne face est "Est" SINON sinon la face est "Ouest"
        // Si on regarde en haut, la bonne face est "Bas" sinon "Haut"
        // Si on regarde en face, la bonne face est "Sud" sinon "Nord"

        //4// On arrondit les donn�es du Vector3 au Int le plus bas
        // on soustrait ou ajoute 1 a l'axe corespondant (x,y ou z) car les coordone Vector3 d'un bloc correspondent � un sommet qui est "en bas arriere a gauche"
        // pour l'envoyer au chunk pour que le chunk puisse traduire se Vector3 en Index
        // !!!!!!!! ces operations ne fonctionnent que si x,y et z sont positif !!!!!!!!
        // !!!!!!!! pour gerer x,y ou z negatif il faut dupliquer et modifier les blocs de code pour chaques conditions donc multiplier tout par 8 mais j'ai retirer cette option pour faciliter le devellopement !!!!!!!!


        if (Input.GetMouseButtonDown(0)) // clic gauche casser bloc
        {
            RaycastHit hit1;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit1, pointerHitRange)) // On envoie un raycast qui recupere les coordone Vector3 du clic sur une surface quelconque avec un collider
            {
                clickHitWorldPosition = hit1.point;

                roundedClickHitWorldPosition = new Vector3(Mathf.FloorToInt(clickHitWorldPosition.x), Mathf.FloorToInt(clickHitWorldPosition.y), Mathf.FloorToInt(clickHitWorldPosition.z));

                //X
                if (clickHitWorldPosition.x == Mathf.Ceil(clickHitWorldPosition.x)) // si on tape une face qui est en coordon�e int // informe aussi sur quel axe cardinal est la face qu'on touche xyz // Mathf.Ceil arondit en haut
                {
                    if (playerPointerDirection.y > 180) // d�termine quelle face x on regarde celon l'orientation du pointeur
                                                        // car les 2 faces de 2 blocks adjacents ont les meme coordon�es
                                                        // plus tard il y aura des blocs qui agirons differament selon la face qu'on touche
                    {
                        realLeftClickBlockPosition = new Vector3(roundedClickHitWorldPosition.x - 1, roundedClickHitWorldPosition.y, roundedClickHitWorldPosition.z); // on enl�ve 1 � la coordon�e x
                        if (FaceOrientationLog) // 
                        {
                            Debug.Log("East face");
                        }
                        SendHitBlockData();
                        if (enableTestArrow)
                        {
                            Instantiate(testArrow, clickHitWorldPosition, Quaternion.Euler(0, 90, 0)); // test
                        }
                    }
                    else
                    {
                        realLeftClickBlockPosition = roundedClickHitWorldPosition;
                        if (FaceOrientationLog)
                        {
                            Debug.Log("West face");
                        }
                        SendHitBlockData();
                        if (enableTestArrow)
                        {
                            Instantiate(testArrow, clickHitWorldPosition, Quaternion.Euler(0, 270, 0)); // test
                        }
                    }
                }

                //Z
                if (clickHitWorldPosition.z == Mathf.Ceil(clickHitWorldPosition.z))
                {
                    if (playerPointerDirection.y > 270 || playerPointerDirection.y < 90) // si angle est entre 270 et 90 en passant par 0
                    {
                        realLeftClickBlockPosition = roundedClickHitWorldPosition;
                        if (FaceOrientationLog)
                        {
                            Debug.Log("South face");
                        }
                        SendHitBlockData();
                        if (enableTestArrow)
                        {
                            Instantiate(testArrow, clickHitWorldPosition, Quaternion.Euler(180, 0, 0)); // test
                        }
                    }
                    else
                    {
                        realLeftClickBlockPosition = new Vector3(roundedClickHitWorldPosition.x, roundedClickHitWorldPosition.y, roundedClickHitWorldPosition.z - 1);
                        if (FaceOrientationLog)
                        {
                            Debug.Log("North face");
                        }
                        SendHitBlockData();
                        if (enableTestArrow)
                        {
                            Instantiate(testArrow, clickHitWorldPosition, Quaternion.Euler(0, 0, 0)); // test
                        }
                    }
                }

                //Y    
                if (clickHitWorldPosition.y == Mathf.Ceil(clickHitWorldPosition.y))
                {
                    if (playerPointerDirection.x > 270)
                    {
                        realLeftClickBlockPosition = roundedClickHitWorldPosition;
                        if (FaceOrientationLog)
                        {
                            Debug.Log("Bottom face");
                        }
                        SendHitBlockData();
                        if (enableTestArrow)
                        {
                            Instantiate(testArrow, clickHitWorldPosition, Quaternion.Euler(90, 0, 0)); // test
                        }
                    }
                    else
                    {
                        realLeftClickBlockPosition = new Vector3(roundedClickHitWorldPosition.x, roundedClickHitWorldPosition.y - 1, roundedClickHitWorldPosition.z);
                        if (FaceOrientationLog)
                        {
                            Debug.Log("Up face");
                        }
                        SendHitBlockData();
                        if (enableTestArrow)
                        {
                            Instantiate(testArrow, clickHitWorldPosition, Quaternion.Euler(270, 0, 0)); // test
                        }
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1)) // clic droit placer bloc
        {

            RaycastHit hit2;
            var ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray1, out hit2, pointerHitRange))
            {
                clickHitWorldPosition = hit2.point;


                roundedClickHitWorldPosition = new Vector3(Mathf.FloorToInt(clickHitWorldPosition.x), Mathf.FloorToInt(clickHitWorldPosition.y), Mathf.FloorToInt(clickHitWorldPosition.z));

                //X
                if (clickHitWorldPosition.x == Mathf.Ceil(clickHitWorldPosition.x)) // si on tape une face qui est en coordon�e int // informe aussi sur quel axe cardinal est la face qu'on touche xyz // Mathf.Ceil arondit en haut
                {
                    if (playerPointerDirection.y > 180) // d�termine quelle face x on regarde car les 2 faces de 2 blocks adjacents ont les meme coordon�es
                                                        // permet de savoir ou poser un bloc par rapport a la face qu'on touche
                    {
                        realBlockClickedPosition = roundedClickHitWorldPosition;
                        if (FaceOrientationLog)
                        {
                            Debug.Log("East face");
                        }
                        //Instantiate(testArrow, clickHitWorldPosition, Quaternion.Euler(0, 90, 0)); // test
                        SendPlacedBlockData();
                    }
                    else
                    {
                        realBlockClickedPosition = new Vector3(roundedClickHitWorldPosition.x - 1, roundedClickHitWorldPosition.y, roundedClickHitWorldPosition.z); // on enl�ve 1 � la coordon�e x
                        if (FaceOrientationLog)
                        {
                            Debug.Log("West face");
                        }
                        //Instantiate(testArrow, clickHitWorldPosition, Quaternion.Euler(0, 270, 0)); // test
                        SendPlacedBlockData();
                    }
                }

                //Z
                if (clickHitWorldPosition.z == Mathf.Ceil(clickHitWorldPosition.z))
                {
                    if (playerPointerDirection.y > 270 || playerPointerDirection.y < 90) // si angle est entre 270 et 90 en passant par 0
                    {
                        realBlockClickedPosition = new Vector3(roundedClickHitWorldPosition.x, roundedClickHitWorldPosition.y, roundedClickHitWorldPosition.z - 1);
                        if (FaceOrientationLog)
                        {
                            Debug.Log("South face");
                        }
                        //Instantiate(testArrow, clickHitWorldPosition, Quaternion.Euler(180, 0, 0)); // test
                        SendPlacedBlockData();
                    }
                    else
                    {
                        realBlockClickedPosition = roundedClickHitWorldPosition;
                        if (FaceOrientationLog)
                        {
                            Debug.Log("North face");
                        }
                        //Instantiate(testArrow, clickHitWorldPosition, Quaternion.Euler(0, 0, 0)); // test
                        SendPlacedBlockData();
                    }
                }

                //Y
                if (clickHitWorldPosition.y == Mathf.Ceil(clickHitWorldPosition.y))
                {
                    if (playerPointerDirection.x > 270)
                    {
                        realBlockClickedPosition = new Vector3(roundedClickHitWorldPosition.x, roundedClickHitWorldPosition.y - 1, roundedClickHitWorldPosition.z);
                        if (FaceOrientationLog)
                        {
                            Debug.Log("Bottom face");
                        }
                        //Instantiate(testArrow, clickHitWorldPosition, Quaternion.Euler(90, 0, 0)); // test
                        SendPlacedBlockData();
                    }
                    else
                    {
                        realBlockClickedPosition = roundedClickHitWorldPosition;
                        if (FaceOrientationLog)
                        {
                            Debug.Log("Up face");
                        }
                        //Instantiate(testArrow, clickHitWorldPosition, Quaternion.Euler(270, 0, 0)); // test
                        SendPlacedBlockData();
                    }
                }
            }
        }

        void SendHitBlockData() // on envoie le Vector3 de l'emplacement de bloc qu'on veut enlever
        {
            testChunk.GetComponent<ChunkScript>().BreakingBlocks(realLeftClickBlockPosition);
        }

        void SendPlacedBlockData() // on envoie le Vector3 de l'emplacement de bloc qu'on veut ajouter
        {
            testChunk.GetComponent<ChunkScript>().PlacingBlocks(realBlockClickedPosition);
        }
    }
    void ShootMode() // on instancie l'object projectile dans la rotation du pointeur
    {
        if (Input.GetMouseButtonDown(0)) // clic gauche
        {
            RaycastHit shotHit;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out shotHit))
            {
                if (shotHit.transform.gameObject.GetComponent<TargetScript>())
                {
                    Destroy(shotHit.transform.gameObject);
                    targetManager.GetComponent<TargetManagerScript>().InstantiateNewTarget();
                    Debug.Log("joueur HIT");
                }
                //var instantiated = Instantiate(projectile, transform.position, Quaternion.Euler(playerPointerDirection));
                //instantiated.GetComponent<ProjectileScript>().instantiator = gameObject;
            }
        }
    }
}
